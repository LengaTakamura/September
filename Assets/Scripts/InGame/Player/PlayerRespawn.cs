using System;
using Cysharp.Threading.Tasks;
using Fusion;
using September.InGame.Fields;
using September.InGame.UI;
using UnityEngine;

namespace InGame.Player
{
    public class PlayerRespawn : NetworkBehaviour
    {
        [SerializeField] private float _coolTime;
        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private GameObject _splashEffectPrefab;

        [Networked, OnChangedRender(nameof(OnOutFieldStateChanged))]
        private bool IsOutField { get; set; }

        public event Action OnOutFieldEvent;
        public event Action OnRevivalFieldEvent;

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority) return;

            if (IsOutField) return;

            if (OutOfFieldArea.I == null) return;

            IsOutField = OutOfFieldArea.I.IsOutOfField(transform.position);
        }

        private void OnOutFieldStateChanged()
        {
            if (HasInputAuthority)
            {
                UIController.I.ShowOutFieldUI(IsOutField);
            }

            if (IsOutField)
            {
                PlaySplashEffect();
                OnOutFieldEvent?.Invoke();
                RespawnAsync().Forget();
            }
        }

        private void PlaySplashEffect()
        {
            if (_splashEffectPrefab == null) return;

            Vector3 splashPosition = transform.position;

            GameObject splashEffect = Instantiate(_splashEffectPrefab, splashPosition, Quaternion.identity);
            ParticleSystem rootParticleSystem = splashEffect.GetComponent<ParticleSystem>();
            if (rootParticleSystem == null)
            {
                Debug.LogWarning("[PlayerRespawn] 水しぶきエフェクトのルートにParticleSystemがありません。", splashEffect);
                Destroy(splashEffect);
                return;
            }

            var main = rootParticleSystem.main;
            main.stopAction = ParticleSystemStopAction.Destroy;
        }

        private async UniTaskVoid RespawnAsync()
        {
            _playerManager.RPC_SetInvisible(true);
            await UniTask.WaitForSeconds(_coolTime);
            OnRevivalFieldEvent?.Invoke();
            IsOutField = false;
            _playerManager.RPC_SetInvisible(false);
        }
    }
}
