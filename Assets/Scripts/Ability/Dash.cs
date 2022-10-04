using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "Dash", menuName = "Custom/Ability/Dash", order = 1)]
    public class Dash : Ability
    {
        public float speed = 5;
        public float duration = .2f;
        public int collisionDamage = 0;
        public ContactFilter2D contactFilter;

        private List<Collider2D> _colliders;

        public override bool Use()
        {
            if (!base.Use())
                return false;

            if (_colliders == null)
                _colliders = new List<Collider2D>();

            character.StartCoroutine(DashCoroutine());
            return true;

        }

        private IEnumerator DashCoroutine()
        {
            float endTime = Time.time + duration;
            Vector2 dashVel = character.LastDirection.normalized * speed;

            while (Time.time < endTime)
            {
                if (!isPassive)
                    character.Rigidbody.velocity = dashVel;

                if (collisionDamage != 0 && (Mathf.Abs(character.Rigidbody.velocity.x) > 0 || Mathf.Abs(character.Rigidbody.velocity.y) > 0))
                {
                    int colCount = character.Rigidbody.OverlapCollider(contactFilter, _colliders);

                    for (int i = 0; i < colCount; i++)
                        if (_colliders[i] && _colliders[i].enabled && _colliders[i].gameObject != character.gameObject &&
                    !(character is Enemy && _colliders[i].GetComponent<Enemy>()) &&
                    !(character is Player && _colliders[i].GetComponent<Player>()))
                            _colliders[i].GetComponent<IDamageable>()?.Damage(collisionDamage);
                }

                yield return Common.waitFrame;
            }
        }
    }
}
