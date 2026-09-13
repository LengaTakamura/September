using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace September.InGame.Kraken
{
    /// <summary>各クライアントで魔法陣の Start → Loop → End を再生する。</summary>
    public sealed class KrakenMagicCircle : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _startParticle;
        [SerializeField] private ParticleSystem _loopParticle;
        [SerializeField] private ParticleSystem _endParticle;
        [SerializeField] private ParticleSystem _continuousParticle;

        private ParticleSystem _currentParticle;

        private Phase _phase;

        private enum Phase
        {
            Start,
            Loop,
            End,
        }

        public void Play()
        {
            PlayAsync().Forget();
        }

        private async UniTask PlayAsync()
        {
            _continuousParticle.gameObject.SetActive(true);
            StartPhase(Phase.Start, _startParticle);

            try
            {
                await UniTask.WaitUntil(_startParticle, p => !p.IsAlive(true), cancellationToken: destroyCancellationToken);
                StartPhase(Phase.Loop, _loopParticle);
            }
            catch (OperationCanceledException) { }
        }

        public void End()
        {
            if (_phase == Phase.End) return;
            EndAsync().Forget();
        }

        private async UniTask EndAsync()
        {
            // 本体が先に Despawn されても終了演出は最後まで再生する。
            transform.SetParent(null, true);
            _continuousParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            StartPhase(Phase.End, _endParticle);

            try
            {
                await UniTask.WaitUntil(_endParticle, p => !p.IsAlive(true), cancellationToken: destroyCancellationToken);
                Destroy(gameObject);
            }
            catch (OperationCanceledException) { }
        }

        private void StartPhase(Phase phase, ParticleSystem particle)
        {
            if (_currentParticle)
            {
                _currentParticle.gameObject.SetActive(false);
                _currentParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            _currentParticle = particle;
            _currentParticle.gameObject.SetActive(true);
            _currentParticle.Play(true);

            _phase = phase;
        }
    }
}
