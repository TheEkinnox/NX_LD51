using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "Hit", menuName = "Custom/Ability/Hit", order = 1)]
    public class Hit : Ability
    {
        public int damage = 1;
        public float range = 1;
        public float knockBackForce = 2;
        public float knockBackDuration = .1f;
        [Range(0, 360)]
        public float maxDegAngle = 360;
        public ContactFilter2D contactFilter;

        private List<Collider2D> _colliders;

        public override bool Use()
        {
            if (base.Use())
            {
                if (_colliders == null)
                    _colliders = new List<Collider2D>();

                Vector2 charPos = character.transform.position;
                int colCount = Physics2D.OverlapCircle((Vector2)character.AbilitySpawn.position, range, contactFilter, _colliders);
                float maxAngle = maxDegAngle % 360;

                Collider2D col;
                IDamageable target;
                Vector2 dirToCol;
                Rigidbody2D rb;

                for (int i = 0; i < colCount; i++)
                {
                    col = _colliders[i];
                    dirToCol = ((Vector2)col.transform.position - charPos).normalized;

                    if (!col || !col.enabled || col.gameObject == character.gameObject || (maxAngle > 0 && Vector2.Angle(character.LastDirection.normalized, dirToCol) > maxAngle) ||
                        (character is Enemy && col.GetComponent<Enemy>()) ||
                        (character is Player && col.GetComponent<Player>()))
                        continue;

                    if (Mathf.Abs(damage) > 0 && (target = col.GetComponent<IDamageable>()) != null)
                        target.Damage(damage);

                    if (knockBackForce > 0 && (rb = col.GetComponent<Rigidbody2D>()))
                        character.StartCoroutine(KnockBackCoroutine(rb, dirToCol));
                }

                return true;
            }

            return false;
        }

        private IEnumerator KnockBackCoroutine(Rigidbody2D target, Vector2 direction)
        {
            float endTime = Time.time + knockBackDuration;

            while (Time.time < endTime)
            {
                target.velocity = direction * knockBackForce;
                yield return Common.waitFrame;
            }
        }
    }
}
