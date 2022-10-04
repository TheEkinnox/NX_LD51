using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class MainMenu : MonoBehaviour
    {
        private VisualElement _root;

        // Buttons, Groups & Labels
        private Button _playButton;
        private Button _helpButton;
        private Button _creditsButton;
        private Button _exitButton;
        private Label _versionText;

        // Sub menus
        private VisualElement _titleScreen;
        private VisualElement _helpScreen;
        private VisualElement _creditsScreen;

        // Menu managers
        private CreditsManager _creditsManager;

        private void OnEnable()
        {
            Init(GetComponent<UIDocument>().rootVisualElement);
        }

        private void Init(VisualElement root)
        {
            _root = root;

            // Load Managers
            _creditsManager = GetComponent<CreditsManager>();

            if (!_creditsManager)
                _creditsManager = gameObject.AddComponent<CreditsManager>();

            // Load elements
            _playButton = _root.Q<Button>("btn_play");
            _helpButton = _root.Q<Button>("btn_help");
            _creditsButton = _root.Q<Button>("btn_credits");
            _exitButton = _root.Q<Button>("btn_exit");
            _versionText = _root.Q<Label>("txt_version");

            // Load sub-menus;
            _titleScreen = _root.Q<VisualElement>("screen_title");
            _helpScreen = _root.Q<TemplateContainer>("screen_help");
            _creditsScreen = _root.Q<TemplateContainer>("screen_credits");

            // Setup buttons
            _playButton.clicked += Play;
            _helpButton.clicked += ShowHelp;
            _creditsButton.clicked += ShowCredits;
            _exitButton.clicked += ExitGame;

            _versionText.text = $"ver. {Application.version}";

            // Focus the play button by default
            _playButton.Blur();
            _playButton.Focus();
        }

        private void OnDisable()
        {
            // TODO ?
        }

        private void Play()
        {
            SceneManager.LoadSceneAsync(Scenes.Dungeon);
        }

        private void ShowHelp()
        {
            Debug.Log("Todo : integrate help menu");
            _titleScreen.style.display = DisplayStyle.None;
            _versionText.style.display = DisplayStyle.None;
            _helpScreen.style.display = DisplayStyle.Flex;
            //_helpManager.Init(_helpScreen);
            //_helpManager.hide += HideHelp;
        }

        private void HideHelp()
        {
            _titleScreen.style.display = DisplayStyle.Flex;
            _versionText.style.display = DisplayStyle.Flex;
            _helpScreen.style.display = DisplayStyle.None;
        }

        private void ShowCredits()
        {
            _titleScreen.style.display = DisplayStyle.None;
            _versionText.style.display = DisplayStyle.None;
            _creditsScreen.style.display = DisplayStyle.Flex;
            _creditsManager.Init(_creditsScreen);
            _creditsManager.onComplete += OnCreditsComplete;
        }

        private void OnCreditsComplete()
        {
            _titleScreen.style.display = DisplayStyle.Flex;
            _versionText.style.display = DisplayStyle.Flex;
            _creditsManager.onComplete -= OnCreditsComplete;
            _playButton.Focus();
        }

        private void ExitGame()
        {
            Common.ExitGame();
        }
    }
}
