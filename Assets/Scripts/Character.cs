using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public abstract class Character : MonoBehaviour, IDamageable
    {
        [SerializeField] private Animator animator;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Rigidbody2D rb2d;
        [SerializeField] private Transform projectileSpawn;

        protected Vector2 direction;
        protected Vector2 lastDirection;

        private int _health = 1;
        private bool _isDead = false;
        private float _freezeEndTime;

        public bool IsDead
        {
            get => _isDead;
            protected set
            {
                _isDead = value;
                animator.SetBool(AnimVars.IsDead, _isDead);
            }
        }

        public int Health
        {
            get => _health;
            protected set
            {
                _health = Mathf.Max(value, 0);

                if (_health == 0)
                    StartCoroutine(Die());
            }
        }

        public bool IsFrozen => Time.time < _freezeEndTime;

        public Animator Animator => animator;
        public AudioSource AudioSource => audioSource;
        public Rigidbody2D Rigidbody => rb2d;
        public Transform ProjectileSpawn => projectileSpawn;
        public Vector2 LastDirection => lastDirection;
        public abstract CharacterConfig CurrentConfig { get; }

        protected virtual IEnumerator Die()
        {
            IsDead = true;

            Debug.Log("Dying");

            if (CurrentConfig.deathSound)
                AudioSource.PlayOneShot(CurrentConfig.deathSound);

            if (CurrentConfig.deathFx)
            {
                GameObject particle = Instantiate(CurrentConfig.deathFx, transform.position, transform.rotation);
                ParticleSystem[] ps = particle.GetComponentsInChildren<ParticleSystem>();

                Destroy(particle, ps.Max(p => p.main.duration));
            }

            yield return null;
        }

        protected virtual void OnEnable()
        {
            if (!animator)
                throw new MissingComponentException("Missing player animator");

            if (!audioSource)
                throw new MissingComponentException("Missing player audio source");

            if (!rb2d)
                throw new MissingComponentException("Missing player rigidbody");

            if (!projectileSpawn)
                throw new MissingComponentException("Missing player projectie spawn point");

            IsDead = false;
            _freezeEndTime = Time.time;

            GetComponent<SpriteRenderer>().color = Color.white;

            Debug.Log("Awake");
        }
        
        public virtual void Damage(int damage)
        {
            if (!IsDead)
                Health -= Mathf.Min(damage, Health);
        }
        
        public virtual void Heal(int health)
        {
            if (!IsDead)
                Health += Mathf.Min(health, CurrentConfig.health - Health);
        }

        public void Freeze(float duration)
        {
            if (duration <= 0)
                return;

            _freezeEndTime = Time.time + duration;
        }

        public void Unfreeze()
        {
            _freezeEndTime = Time.time;
        }

        protected virtual void HandleMovement()
        {
            direction.Normalize();

            if (direction != Vector2.zero)
                lastDirection = direction;

            rb2d.velocity = direction * CurrentConfig.speed;
        }

        protected void HandleAnimations()
        {
            animator.SetBool(AnimVars.IsMoving, Mathf.Abs(direction.x) > 0 || Mathf.Abs(direction.y) > 0);
            animator.SetBool(AnimVars.IsDead, IsDead);
            animator.SetFloat(AnimVars.VelX, direction.x);
            animator.SetFloat(AnimVars.VelY, direction.y);
            animator.SetFloat(AnimVars.LastX, lastDirection.x);
            animator.SetFloat(AnimVars.LastY, lastDirection.y);
        }
    }
}
