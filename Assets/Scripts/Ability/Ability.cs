using System;
using UnityEngine;

namespace Assets.Scripts
{
    public abstract class Ability : ScriptableObject
    {
        public AudioClip sfx;
        public float coolDown = 0;
        public float freezeDuration = 0;
        public bool isPassive = false;

        protected Character character;
        private float _nextUseTime;

        public void Init(Character character)
        {
            if (!character)
                throw new ArgumentException($"Tried to initialise ability \"{name}\" with invalid player");

            Debug.Log($"Initialised ability \"{name}\" for player \"{character.name}\"");

            this.character = character;
            _nextUseTime = 0;
        }

        public void Disable()
        {
            character = null;
        }

        public virtual bool Use()
        {
            if (!character || character.IsDead || Time.time < _nextUseTime || (character is Player player && player.IsAging))
                return false;

            if (!isPassive)
                character.Animator.SetTrigger(AnimVars.Ability);

            if (sfx)
                character.AudioSource.PlayOneShot(sfx);

            _nextUseTime = Time.time + coolDown;

            if (freezeDuration > 0)
                character.Freeze(freezeDuration);

            Debug.Log($"Player \"{character.name}\" used ability \"{name}\"");
            return true;
        }
    }
}