using Assets.Scripts.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Player : Character
    {
        private float _nextAgeStepTime = 0;

        [SerializeField] private GameConfig gameConfig;

        private int _configIndex = 0;

        public bool IsAging
        {
            get;
            private set;
        }

        public string DisplayedAge
        {
            get;
            private set;
        }

        public float TimeBeforeAging => IsDead || IsAging ? 0 : _nextAgeStepTime - Time.time;

        public override CharacterConfig CurrentConfig => gameConfig.playerConfigs[_configIndex];

        protected override void OnEnable()
        {
            base.OnEnable();

            IsAging = false;

            StartCoroutine(SetConfig(0));
        }

        void Update()
        {
            direction = Vector2.zero;

            if (IsAging || IsDead)
            {
                Rigidbody.velocity = Vector2.zero;
                return;
            }

            if (Time.time >= _nextAgeStepTime)
                StartCoroutine(SetConfig(_configIndex + 1));

            if (!IsAging && !IsDead && !HudManager.IsPaused)
            {
                HandleMovement();
                HandleAnimations();
            }
        }

        protected override void HandleMovement()
        {
            if (IsFrozen)
            {
                Rigidbody.velocity = Vector2.zero;
                return;
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                direction.y = 1;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                direction.x = -1;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                direction.y = -1;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                direction.x = 1;

            bool spacePressed = Input.GetKeyDown(KeyCode.Space);

            foreach (Ability ability in curAbilities)
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

            base.HandleMovement();
        }

        public override void Damage(int damage)
        {
            if (!IsAging)
                base.Damage(damage);
        }

        public override void Heal(int health)
        {
            if (!IsAging)
                base.Heal(health);
        }

        public void Rejuvenate()
        {
            if (IsDead || IsAging)
                return;

            StartCoroutine(SetConfig(_configIndex - gameConfig.rejuvenateCount));
        }

        protected override IEnumerator Die()
        {
            yield return base.Die();

            if (!IsAging)
                if (CurrentConfig.deathSound)
                    yield return new WaitForSeconds(Mathf.Max(Animator.GetCurrentAnimatorStateInfo(0).length, CurrentConfig.deathSound.length));
                else
                    yield return new WaitForSeconds(Animator.GetCurrentAnimatorStateInfo(0).length);
            else
                yield return new WaitUntil(() => !IsAging);

            yield return Common.waitFrame;

            enabled = false;

            yield return Common.FadeOut(1);

            SceneManager.LoadSceneAsync(Scenes.Dungeon);

            yield return Common.FadeIn(1);
        }

        IEnumerator SetConfig(int index)
        {
            if (index < 0)
                index = 0;

            if (index != 0 && index == _configIndex)
                yield break;

            IsAging = true;
            
            lastDirection.x = 0;
            lastDirection.y = -1;

            int displayedAge = _configIndex * gameConfig.ageStep;
            int targetAge = index * gameConfig.ageStep;

            if (index < gameConfig.playerConfigs.Length)
            {
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
                    curAbilities = new System.Collections.Generic.List<Ability>();
                }

                _configIndex = index;
                Health = CurrentConfig.health;
                Animator.runtimeAnimatorController = CurrentConfig.animController;

                Animator.SetTrigger(AnimVars.Spawn);

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
            else
                StartCoroutine(Die());

            float animDuration = Animator.GetCurrentAnimatorStateInfo(0).length;

            AudioClip curSound = IsDead ? CurrentConfig.deathSound : CurrentConfig.spawnSound;

            if (AudioSource.isPlaying && curSound)
                animDuration = Mathf.Max(animDuration, curSound.length);

            float progress = displayedAge != targetAge ? 0 : animDuration;

            while (progress < animDuration)
            {
                Debug.Log($"Cur. Age : {displayedAge} y/o");

                DisplayedAge = $"{displayedAge} y/o";
                displayedAge = (int) Mathf.Lerp(displayedAge, targetAge, progress);

                yield return Common.waitFrame;

                progress += Time.deltaTime;
            }

            Debug.Log($"New config : {CurrentConfig.displayName}");
            DisplayedAge = targetAge == 0 ? CurrentConfig.displayName : $"{CurrentConfig.displayName} ({targetAge} y/o)";

            if (!IsDead)
                _nextAgeStepTime = Time.time + gameConfig.secPerPlayerConf;

            IsAging = false;
        }
    }
}
