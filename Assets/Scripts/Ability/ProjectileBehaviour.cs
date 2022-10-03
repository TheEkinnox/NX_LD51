using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class ProjectileBehaviour : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb2d;
        [SerializeField] private new SpriteRenderer renderer;

        private Projectile _ability;
        private Character _caster;

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

            Vector3 force = _caster.LastDirection * _ability.throwSpeed + _caster.Rigidbody.velocity;

            renderer.sortingOrder = _caster.LastDirection.y > 0 ? -1 : 1;
            rb2d.AddForce(force, ForceMode2D.Impulse);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_ability && _caster && other && other.enabled && other.gameObject != _caster.gameObject)
            {
                IDamageable target = other.gameObject.GetComponent<IDamageable>();

                if (target != null)
                    target.Damage(_ability.damage);

                if (_ability.breakOnContact)
                    Destroy(gameObject);

                Debug.Log("Collision");
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            OnTriggerEnter2D(collision.otherCollider);
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
