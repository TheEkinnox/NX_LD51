using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.DunGen;

namespace Assets.Scripts.UI
{
    public class HudManager : MonoBehaviour
    {
        [SerializeField] private Dungeon _dungeon;

        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _roomTimeText;
        [SerializeField] private TMP_Text _ageText;
        [SerializeField] private TMP_Text _agingTimerText;
        [SerializeField] private TMP_Text _healthText;

        [SerializeField] private GameObject _pauseGroup;

        private static bool _isPaused;
        //private float _previousTimeScale;

        public static bool IsPaused => _isPaused;

        private void Awake()
        {
            _isPaused = false;
            _pauseGroup.SetActive(false);

            _levelText.text = string.Empty;
            _roomTimeText.text = string.Empty;
            _ageText.text = string.Empty;
            _agingTimerText.text = string.Empty;
            _healthText.text = string.Empty;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Pause();
        }

        private void FixedUpdate()
        {
            if (!_dungeon || !_dungeon.Player)
                return;

            _levelText.text = $"Room {_dungeon.CurLevel}";
            _roomTimeText.text = $"{_dungeon.CurRoomTime:00.0}s";

            if (_dungeon.Player.IsDead)
            {
                _ageText.text = "Dead";
                _agingTimerText.text = "Dead";
                _healthText.text = "Dead";
            }
            else if (_dungeon.Player.IsAging)
            {
                _ageText.text = $"{_dungeon.Player.DisplayedAge}";
                _agingTimerText.text = "Aging";
                _healthText.text = "Aging";
            }
            else
            {
                _ageText.text = $"{_dungeon.Player.DisplayedAge}";
                _agingTimerText.text = $"Aging in {_dungeon.Player.TimeBeforeAging:00.0}s";
                _healthText.text = $"{_dungeon.Player.Health} HP";
            }
        }

        public void Pause()
        {
            _isPaused = !_isPaused;

            if (_isPaused)
            {
                //_previousTimeScale = Time.timeScale;
                Time.timeScale = 0;
            }
            else
            {
                //Time.timeScale = _previousTimeScale;
                Time.timeScale = 1;
            }

            _pauseGroup.SetActive(_isPaused);
        }

        public void Restart()
        {
            SceneManager.LoadSceneAsync(Scenes.Dungeon);
        }

        public void BackToMenu()
        {
            SceneManager.LoadSceneAsync(Scenes.Title);
        }

        public void Exit()
        {
            Common.ExitGame();
        }
    }
}
