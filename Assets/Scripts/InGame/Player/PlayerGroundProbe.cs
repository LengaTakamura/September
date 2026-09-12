using UnityEngine;

namespace InGame.Player
{
    /// <summary> 直立した移動カプセルの接地面と安全な吸着距離を調べる。 </summary>
    internal static class PlayerGroundProbe
    {
        public static bool TryProbe(CapsuleCollider capsule, LayerMask groundLayer,
            float slopeThreshold, float probeDistance, float probeOffset, float snapTolerance,
            out Vector3 normal, out float gap)
        {
            normal = Vector3.up;
            gap = 0f;
            if (!capsule.enabled || !capsule.gameObject.activeInHierarchy) return false;

            // Collider.bounds の物理同期を待たず、予測で復元された Transform から起点を作る。
            // PlayerBase の移動カプセルは Y 軸方向で、回転は yaw のみ。
            Transform capsuleTransform = capsule.transform;
            Vector3 scale = capsuleTransform.lossyScale;
            float radius = capsule.radius * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z));
            if (radius <= Mathf.Epsilon) return false;
            float halfHeight = Mathf.Max(radius, capsule.height * Mathf.Abs(scale.y) * 0.5f);
            Vector3 center = capsuleTransform.TransformPoint(capsule.center);
            Vector3 bottom = center - Vector3.up * halfHeight;
            Vector3 rayOrigin = bottom + Vector3.up * probeOffset;
            Vector3 sphereOrigin = rayOrigin + Vector3.up * radius;

            bool hasRayGround = Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit rayHit,
                probeDistance, groundLayer, QueryTriggerInteraction.Ignore)
                && IsWalkable(rayHit.normal, slopeThreshold);
            // 球の距離が不明な場合でも、足元の面へ既に接していることだけは確認できる。
            // この代替判定の距離を吸着量として使うと、坂と平地の境界へ食い込む。
            float rayClearance = hasRayGround
                ? radius * (1f / Mathf.Max(rayHit.normal.y, Mathf.Epsilon) - 1f)
                : 0f;
            bool hasRayContact = hasRayGround
                && bottom.y - rayHit.point.y - rayClearance <= snapTolerance;

            // 開始球が重なっている場合、Cast の距離では安全な下降量を求められない。
            // 中心レイが接触距離内の場合だけ接地とし、吸着はしない。
            if (Physics.CheckSphere(sphereOrigin, radius, groundLayer, QueryTriggerInteraction.Ignore))
            {
                if (hasRayContact) normal = rayHit.normal;
                return hasRayContact;
            }

            bool hasSphereHit = Physics.SphereCast(sphereOrigin, radius, Vector3.down,
                out RaycastHit sphereHit, probeDistance, groundLayer, QueryTriggerInteraction.Ignore);
            bool hasSphereGround = hasSphereHit && sphereHit.distance > 0f
                && IsWalkable(sphereHit.normal, slopeThreshold);

            if (hasRayGround)
            {
                // 法線は中心レイを優先し、頂上の平地を先取りして登れなくなるのを防ぐ。
                // 距離はカプセル下半球の実接触から求める。平地と坂を跨いでいても、
                // 中心レイの平地へ押し下げたり、斜面を無限平面と見なして浮かせたりしない。
                normal = rayHit.normal;
                // 遠い面にレイだけが当たった場合、未知の球の距離を gap=0 と見なさない。
                if (!hasSphereGround) return hasRayContact;
                gap = Mathf.Max(0f, sphereHit.distance - probeOffset);
                return true;
            }

            if (!hasSphereGround) return false;

            float expectedContactHeight = bottom.y + radius * (1f - sphereHit.normal.y);
            // レイを失った際の補完でも、頂上の縁を先取りする法線は採用しない。
            if (sphereHit.point.y > expectedContactHeight + snapTolerance) return false;

            normal = sphereHit.normal;
            gap = Mathf.Max(0f, sphereHit.distance - probeOffset);
            return true;
        }

        private static bool IsWalkable(Vector3 normal, float slopeThreshold)
        {
            return Vector3.Angle(Vector3.up, normal) <= slopeThreshold;
        }
    }
}
