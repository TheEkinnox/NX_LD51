using System.Collections;
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

        private Collider2D[] _colliders;

        public override bool Use()
        {
            if (base.Use())
            {
                character.StartCoroutine(DashCoroutine());
                return true;
            }

            return false;
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
                        if (_colliders[i] && _colliders[i].enabled && _colliders[i].gameObject != character.gameObject)
                            _colliders[i].GetComponent<IDamageable>()?.Damage(collisionDamage);
                }

                yield return Common.waitFrame;
            }
        }
    }
}
