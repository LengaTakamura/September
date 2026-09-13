using UnityEngine;

namespace InGame.Player
{
    /// <summary> 同期された軌道の進捗から乗り越え位置を再計算する。 </summary>
    internal static class PlayerVaultMotion
    {
        public static Vector3 Evaluate(Vector3 start, Vector3 top, Vector3 end,
            float progress, AnimationCurve curve)
        {
            // Tick 境界で所要時間を超えても終点より先へ進めない。
            if (progress <= 0f) return start;
            if (progress >= 1f) return end;

            Vector3 position = Vector3.Lerp(start, end, progress);
            bool descending = progress >= 0.5f;
            float heightProgress = descending ? 2f * (1f - progress) : 2f * progress;
            position.y = Mathf.Lerp(descending ? end.y : start.y, top.y, curve.Evaluate(heightProgress));
            return position;
        }
    }
}
