using UnityEngine;

namespace Assets.Scripts
{
    internal class AnimVars
    {
        internal static readonly int Spawn = Animator.StringToHash("spawn");
        internal static readonly int Ability = Animator.StringToHash("ability");
        internal static readonly int Exit = Animator.StringToHash("exit");
        internal static readonly int IsMoving = Animator.StringToHash("isMoving");
        internal static readonly int IsDead = Animator.StringToHash("isDead");
        internal static readonly int VelX = Animator.StringToHash("velX");
        internal static readonly int VelY = Animator.StringToHash("velY");
        internal static readonly int LastX = Animator.StringToHash("lastX");
        internal static readonly int LastY = Animator.StringToHash("lastY");
    }
}
