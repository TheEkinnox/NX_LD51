using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.TextCore.Text;

namespace Assets.Scripts
{
    public class ProjectileBehaviour : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb2d;
        [SerializeField] private new SpriteRenderer renderer;

        private Projectile _ability;
        private Character _caster;

        private List<Collider2D> _colliders;

        private void Awake()
        {
            if (!rb2d)
                throw new MissingComponentException($"Missing rigidbody for projectile behaviour \"{name}\"");

            if (!renderer)
                throw new MissingComponentException($"Missing sprite renderer for projectile behaviour \"{name}\"");
        }

        internal void Init(Projectile ability, Character caster)
        {
            if (!ability)
                throw new ArgumentNullException($"Tried to initialize projectile behaviour \"{name}\" with null ability");

            if (!caster)
                throw new ArgumentNullException($"Tried to initialize projectile behaviour \"{name}\" with invalid caster");

            _ability = ability;
            _caster = caster;

            float throwSpeed = _ability.throwSpeed + _caster.Rigidbody.velocity.magnitude;
            Vector3 force = _caster.LastDirection * throwSpeed;

            renderer.sortingOrder = _caster.LastDirection.y > 0 ? 0 : 1;
            rb2d.AddForce(force, ForceMode2D.Impulse);

            Destroy(gameObject, _ability.range / throwSpeed);
        }

        private void FixedUpdate()
        {
            if (!_caster || !_ability)
                return;

            if (_colliders == null)
                _colliders = new List<Collider2D>();

            int colCount = rb2d.OverlapCollider(_ability.contactFilter, _colliders);

            Collider2D col;
            IDamageable target;
            bool collided = false;

            for (int i = 0; i < colCount; i++)
            {
                col = _colliders[i];

                if (!col || !col.enabled || col.isTrigger || col.gameObject == _caster.gameObject ||
                    (_caster is Enemy && col.GetComponent<Enemy>()) ||
                    (_caster is Player && col.GetComponent<Player>()))
                    continue;

                collided = true;

                if (Mathf.Abs(_ability.damage) > 0 && (target = col.GetComponent<IDamageable>()) != null)
                    target.Damage(_ability.damage);
            }

            if (collided && _ability.breakOnContact)
                Destroy(gameObject);
        }

        private void OnDestroy()
        {
            Break();
        }

        private void Break()
        {
            if (_ability && _ability.breakFx)
            {
                GameObject particle = Instantiate(_ability.breakFx, transform.position, transform.rotation);
                ParticleSystem[] ps = particle.GetComponentsInChildren<ParticleSystem>();

                Destroy(particle, ps.Max(p => p.main.duration));
            }
        }
    }
}
