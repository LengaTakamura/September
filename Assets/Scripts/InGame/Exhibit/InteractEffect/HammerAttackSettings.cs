using Result;
using UnityEngine;

namespace InGame.Exhibit.InteractEffect
{
    [CreateAssetMenu(menuName = "ScriptableObjects/HammerAttackSettings", fileName = "HammerAttackSettings", order = 0)]
    public class HammerAttackSettings : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<ExhibitType, float> _disableDuration;

        public bool TryGetDisableDuration(ExhibitType exhibitType, out float duration)
        {
            if (_disableDuration.Dictionary.TryGetValue(exhibitType, out duration))
            {
                return true;
            }

            Debug.LogWarning($"[HammerAttackSettings] ExhibitType:{exhibitType} の破壊秒数が設定されていません。", this);
            return false;
        }
    }
}
