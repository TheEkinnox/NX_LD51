using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class Enemy : Character
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private EnemyConfig _config;
        private Player _target;

        public override CharacterConfig CurrentConfig => _config;

        public void Init(EnemyConfig config)
        {
            if (!config)
                throw new ArgumentException($"Tried to initalise enemy \"{name}\" with invalid config");

            _config = config;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Player player;
            if (!IsDead && other && other.enabled && (player = other.gameObject.GetComponent<Player>()) && !player.IsDead)
                _target = player;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!IsDead && _target && other && other.enabled && other.gameObject == _target.gameObject)
                _target = null;
        }

        private void LateUpdate()
        {
            if (_target && (IsDead || _target.IsDead))
                _target = null;
        }

        protected override void HandleMovement()
        {
            if (IsFrozen)
            {
                Rigidbody.velocity = Vector2.zero;
                return;
            }

            bool inRange = false;

            if (!_target)
            {
                // TODO: Roam();
            }
            else
            {
                 float distToTarget = Vector3.Distance(transform.position, _target.transform.position);
                
                if (distToTarget < _config.minDistToTarget)
                {
                    direction = (transform.position - _target.transform.position);
                }
                else if (distToTarget > _config.maxDistToTarget)
                {
                    direction = (_target.transform.position - transform.position);
                }
                else
                {
                    inRange = true;
                    direction = Vector2.zero;
                    lastDirection = (_target.transform.position - transform.position);
                }
            }

            foreach (Ability ability in CurrentConfig.abilities)
                if (ability.isPassive || (!ability.isPassive && inRange))
                    ability.Use();

            base.HandleMovement();
        }

        protected override IEnumerator Die()
        {
            yield return base.Die();

            if (CurrentConfig.deathSound)
                yield return new WaitForSeconds(Mathf.Max(Animator.GetCurrentAnimatorStateInfo(0).length, CurrentConfig.deathSound.length));
            else
                yield return new WaitForSeconds(Animator.GetCurrentAnimatorStateInfo(0).length);

            yield return Common.waitFrame;

            enabled = false;
        }
    }
}
