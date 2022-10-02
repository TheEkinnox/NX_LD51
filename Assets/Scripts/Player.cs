using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour, IDamageable
    {
        private float _nextAgeStepTime = 0;

        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private Animator animator;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Rigidbody2D rb2d;
        [SerializeField] private Transform projectileSpawn;

        private int _configIndex = 0;
        private Vector2 _input;
        private Vector2 _lastInput;
        private int _health = 1;
        private bool _isDead = false;

        public bool IsAging
        {
            get;
            private set;
        }
        public bool IsDead
        {
            get => _isDead;
            private set
            {
                _isDead = value;
                animator.SetBool(AnimVars.IsDead, _isDead);
            }
        }

        public int Health
        {
            get => _health;
            private set
            {
                _health = Mathf.Max(value, 0);

                if (_health == 0)
                    StartCoroutine(Die());
            }
        }

        public PlayerConfig CurrentConfig => gameConfig.playerConfigs[_configIndex];
        public Animator Animator => animator;
        public AudioSource AudioSource => audioSource;
        public Rigidbody2D Rigidbody => rb2d;
        public Transform ProjectileSpawn => projectileSpawn;
        public Vector2 LastInput => _lastInput;

        void OnEnable()
        {
            if (!animator)
                throw new MissingComponentException("Missing player animator");

            if (!audioSource)
                throw new MissingComponentException("Missing player audio source");

            if (!rb2d)
                throw new MissingComponentException("Missing player rigidbody");

            if (!projectileSpawn)
                throw new MissingComponentException("Missing player projectie spawn point");

            IsAging = false;
            IsDead = false;

            GetComponent<SpriteRenderer>().color = Color.white;

            Debug.Log("Awake");

            StartCoroutine(SetConfig(0));
        }

        void Update()
        {
            _input = Vector2.zero;

            if (IsAging || IsDead)
            {
                rb2d.velocity = Vector2.zero;
                return;
            }

            if (Time.time >= _nextAgeStepTime)
                StartCoroutine(SetConfig(_configIndex + 1));

            if (!IsAging && !IsDead)
            {
                HandleInputs();
                HandleAnimations();
            }
        }

        void HandleInputs()
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                _input.y = 1;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                _input.x = -1;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                _input.y = -1;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                _input.x = 1;

            bool spacePressed = Input.GetKeyDown(KeyCode.Space);

            foreach (Ability ability in CurrentConfig.abilities)
                if (ability.isPassive || (!ability.isPassive && spacePressed))
                    ability.Use();

#if DEBUG || UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.K))
                if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                    Health--;
                else
                    Health = 0;

            if (Input.GetKeyDown(KeyCode.G))
                StartCoroutine(SetConfig(_configIndex + 1));

            if (Input.GetKeyDown(KeyCode.R))
                Rejuvenate();
#endif

            if (_input != Vector2.zero)
                _lastInput = _input;

            rb2d.velocity = _input.normalized * CurrentConfig.speed;
        }

        void HandleAnimations()
        {
            animator.SetBool(AnimVars.IsMoving, _input.x != 0 || _input.y != 0);
            animator.SetBool(AnimVars.IsDead, IsDead);
            animator.SetFloat(AnimVars.VelX, _input.x);
            animator.SetFloat(AnimVars.VelY, _input.y);
            animator.SetFloat(AnimVars.LastX, _lastInput.x);
            animator.SetFloat(AnimVars.LastY, _lastInput.y);
        }

        public void Damage(int damage)
        {
            if (IsDead || IsAging)
                return;

            Health -= Mathf.Min(damage, Health);
        }

        public void Heal(int health)
        {
            if (IsDead || IsAging)
                return;

            Health += Mathf.Min(health, CurrentConfig.health - Health);
        }

        public void Rejuvenate()
        {
            if (IsDead || IsAging)
                return;

            StartCoroutine(SetConfig(_configIndex - gameConfig.rejuvenateCount));
        }

        IEnumerator Die()
        {
            IsDead = true;
            Debug.Log("Dying");

            if (CurrentConfig.deathSound)
                audioSource.PlayOneShot(CurrentConfig.deathSound);

            if (CurrentConfig.deathFx)
            {
                GameObject particle = Instantiate(CurrentConfig.deathFx, transform.position, transform.rotation);
                ParticleSystem[] ps = particle.GetComponentsInChildren<ParticleSystem>();

                Destroy(particle, ps.Max(p => p.main.duration));
            }

            if (!IsAging)
                if (CurrentConfig.deathSound)
                    yield return new WaitForSeconds(Mathf.Max(animator.GetCurrentAnimatorStateInfo(0).length, CurrentConfig.deathSound.length));
                else
                    yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            else
                yield return new WaitUntil(() => !IsAging);

            yield return Common.waitFrame;

            enabled = false;
        }

        IEnumerator SetConfig(int index)
        {
            if (index < 0)
                index = 0;

            if (index != 0 && index == _configIndex)
                yield break;

            IsAging = true;
            _lastInput.y = -1;

            int displayedAge = _configIndex * gameConfig.ageStep;
            int targetAge = index * gameConfig.ageStep;

            if (index < gameConfig.playerConfigs.Length)
            {
                foreach (Ability ability in CurrentConfig.abilities)
                    ability.Disable();

                _configIndex = index;
                Health = CurrentConfig.health;
                animator.runtimeAnimatorController = CurrentConfig.animController;

                animator.SetTrigger(AnimVars.Spawn);

                if (CurrentConfig.spawnSound)
                    audioSource.PlayOneShot(CurrentConfig.spawnSound);

                foreach (Ability ability in CurrentConfig.abilities)
                    ability.Init(this);
            }
            else
                StartCoroutine(Die());

            float animDuration = animator.GetCurrentAnimatorStateInfo(0).length;

            AudioClip curSound = IsDead ? CurrentConfig.deathSound : CurrentConfig.spawnSound;

            if (audioSource.isPlaying && curSound)
                animDuration = Mathf.Max(animDuration, curSound.length);

            float progress = displayedAge != targetAge ? 0 : animDuration;

            while (progress < animDuration)
            {
                Debug.Log($"Cur. Age : {displayedAge} y/o");
                displayedAge = (int) Mathf.Lerp(displayedAge, targetAge, progress);

                yield return Common.waitFrame;

                progress += Time.deltaTime;
            }

            Debug.Log($"New config : {CurrentConfig.displayName}");

            if (!IsDead)
                _nextAgeStepTime = Time.time + gameConfig.secPerPlayerConf;

            IsAging = false;
        }
    }
}
