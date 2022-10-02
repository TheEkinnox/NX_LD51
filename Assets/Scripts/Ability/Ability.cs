using System;
using UnityEngine;

namespace Assets.Scripts
{
    public abstract class Ability : ScriptableObject
    {
        public AudioClip sfx;
        public float coolDown = 0;
        public bool isPassive = false;

        protected Player player;
        private float _nextUseTime;

        public void Init(Player player)
        {
            if (!player)
                throw new ArgumentException($"Tried to initialise ability \"{name}\" with invalid player");

            Debug.Log($"Initialised ability \"{name}\" for player \"{player.name}\"");

            this.player = player;
            _nextUseTime = 0;
        }

        public void Disable()
        {
            player = null;
        }

        public virtual bool Use()
        {
            if (!player || player.IsDead || player.IsAging || Time.time < _nextUseTime)
                return false;

            if (!isPassive)
                player.Animator.SetTrigger(AnimVars.Ability);

            if (sfx)
                player.AudioSource.PlayOneShot(sfx);

            _nextUseTime = Time.time + coolDown;

            Debug.Log($"Player \"{player.name}\" used ability \"{name}\"");
            return true;
        }

        public virtual void LateUpdate()
        {
            if (!player)
                return;

            if (isPassive || (!isPassive && Input.GetKeyDown(KeyCode.Space)))
                Use();
        }
    }
}