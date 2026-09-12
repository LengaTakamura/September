using UnityEngine;

namespace InGame.Player
{
    [CreateAssetMenu(fileName = "EvasionData", menuName = "Scriptable Objects/EvasionData")]
    public class EvasionData : ScriptableObject
    {
        [SerializeField] private float _rollDistance = 4f;
        [SerializeField] private float _rollDuration = 0.95f;
        [SerializeField,Range(0,180)] private float _inputAngle = 0f;
        [SerializeField] private float _maxTurnDuration = 1f;// 最大角度時のターン所要時間
        [SerializeField] private float _startInvincibleTime = 0.10f;
        [SerializeField] private float _invincibleTime = 0.25f;
        [SerializeField] private float _cooldown = 0;
        [Header("回避スタミナ")]
        [SerializeField, Min(0.01f), Tooltip("スタミナが1回復するまでの秒数")]
        private float _staminaRecoveryInterval = 3f;
        [Header("重量補正")]
        [SerializeField, Min(0f), Tooltip("宝石1個ごとの回避距離減衰率。0.02なら1個ごとに距離が2%短くなる")]
        private float _weightDistanceDecay = 0.02f;
        [SerializeField, Min(0f), Tooltip("宝石1個ごとの回避速度減衰率。0なら重量にかかわらず等速")]
        private float _weightSpeedDecay = 0f;
        [SerializeField] private AnimationCurve _rollSpeedCurve;
        [SerializeField] private AnimationCurve _turnSpeedCurve;

        public float RollDistance => _rollDistance;
        public float RollDuration => _rollDuration;
        public float InputAngle => _inputAngle;
        public float MaxTurnDuration => _maxTurnDuration;
        public float StartInvincibleTime => _startInvincibleTime;
        public float InvincibleTime => _invincibleTime;
        public float Cooldown => _cooldown;
        public float StaminaRecoveryInterval => Mathf.Max(0.01f, _staminaRecoveryInterval);
        public float WeightDistanceDecay => _weightDistanceDecay;
        public float WeightSpeedDecay => _weightSpeedDecay;
        public AnimationCurve RollSpeedCurve => _rollSpeedCurve;
        public AnimationCurve TurnSpeedCurve => _turnSpeedCurve;
    }
}
