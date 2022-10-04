using Assets.Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Enemy : Character
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private List<Collider2D> _targetCols;
        private EnemyConfig _config;
        private Player _target;

        public override CharacterConfig CurrentConfig => _config;

        public void Init(EnemyConfig config)
        {
            if (!config)
                throw new ArgumentException($"Tried to initalise enemy \"{name}\" with invalid config");

            if (curAbilities != null)
            {
                foreach (Ability ability in curAbilities)
                {
                    ability.Disable();
                    ScriptableObject.Destroy(ability);
                }

                curAbilities.Clear();
            }
            else
            {
                curAbilities = new List<Ability>();
            }

            _config = config;
            Animator.runtimeAnimatorController = _config.animController;

            IsDead = false;
            Animator.SetTrigger(AnimVars.Spawn);

            Health = _config.health;

            if (CurrentConfig.spawnSound)
                AudioSource.PlayOneShot(CurrentConfig.spawnSound);

            foreach (Ability ability in CurrentConfig.abilities)
            {
                if (!ability)
                    continue;

                curAbilities.Add(Instantiate(ability));
                curAbilities[curAbilities.Count - 1].Init(this);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Player player;
            if (!IsDead && !_target && other && other.enabled && (player = other.gameObject.GetComponent<Player>()) && !player.IsDead)
            {
                _target = player;
                Debug.Log($"New target of enemy {name} is {_target.name}");
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!IsDead && _target && other && other.enabled && other.gameObject == _target.gameObject)
            {
                _target = null;
                Debug.Log($"Enemy {name} lost it's target");
            }
        }

        private void FixedUpdate()
        {
            if (!IsDead && !HudManager.IsPaused)
            {
                HandleMovement();
                HandleAnimations();
                UpdateTarget();
            }
        }

        private void LateUpdate()
        {
            if (_target && (IsDead || _target.IsDead))
            {
                _target = null;
                Debug.Log($"Enemy {name} lost it's target");
            }
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
                direction = Vector2.zero;
            }
            else
            {
                 float distToTarget = Vector2.Distance(transform.position, _target.transform.position);
                
                if (distToTarget < _config.minDistToTarget)
                    direction = transform.position - _target.transform.position;
                else
                    direction = _target.transform.position - transform.position;
                
                if (distToTarget >= _config.minDistToTarget && distToTarget <= _config.maxDistToTarget)
                {
                    inRange = true;
                    lastDirection = (_target.transform.position - transform.position).normalized;
                    Debug.Log($"Enemy {name} is in range of it's target");
                }
            }

            if (_config)
                foreach (Ability ability in curAbilities)
                    if (ability.isPassive || (!ability.isPassive && inRange))
                        ability.Use();

            base.HandleMovement();
        }

        private void UpdateTarget()
        {
            if (!_config)
                return;

            if (_target && Vector2.Distance(_target.transform.position, transform.position) > _config.maxDistToTarget)
            {
                _target = null;
                return;
            }

            if (_targetCols == null)
                _targetCols = new List<Collider2D>();

            int colCount = Physics2D.OverlapCircle(transform.position, _config.maxDistToTarget, _config.targetContactFilter, _targetCols);

            Collider2D col;
            Player target;

            for (int i = 0; i < colCount; i++)
            {
                col = _targetCols[i];

                if (!col || !col.enabled || col.isTrigger || col.gameObject == gameObject || !(target = col.GetComponent<Player>()))
                    continue;

                _target = target;
                break;
            }
        }

        protected override IEnumerator Die()
        {
            yield return base.Die();

            if (CurrentConfig.deathSound)
                yield return new WaitForSeconds(Mathf.Max(Animator.GetCurrentAnimatorStateInfo(0).length, CurrentConfig.deathSound.length));
            else
                yield return new WaitForSeconds(Animator.GetCurrentAnimatorStateInfo(0).length);

            yield return Common.waitFrame;

            Destroy(gameObject);
        }
    }
}
