using Fusion;
using UnityEngine;

namespace InGame.Player.Okubo
{
    public class SwordNetworkController : NetworkBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _swordObject;
        [SerializeField] private Transform _swordSocket;
        [SerializeField] private HumanBodyBones _swordHandBone;

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_DrawSword()
        {
            var parent = _animator.GetBoneTransform(_swordHandBone);

            _swordObject.transform.SetParent(parent);
            _swordObject.transform.localRotation = Quaternion.identity;
            _swordObject.transform.localPosition = Vector3.zero;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_SheathSword()
        {
            _swordObject.transform.SetParent(_swordSocket);
            _swordObject.transform.localRotation = Quaternion.identity;
            _swordObject.transform.localPosition = Vector3.zero;
        }
    }
}
