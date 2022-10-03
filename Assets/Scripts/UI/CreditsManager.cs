using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.UI
{
    public class CreditsManager : MonoBehaviour
    {
        private VisualElement _root;

        private VisualElement _scrollArea;

        internal System.Action onComplete;

        private float _currentExitHold = 0;
        private float _scrollProgress = 0;
        private float _endDelayProgress = 0;
        private float _fadeProgress = 0;

        public float startPos = 100;
        public float endPos = -100;
        public float exitHoldDuration = 1;
        public float scrollSpeed = .025f;
        public float endDelay = 1;
        public float fadeDuration = .5f;

        public void Init(VisualElement root)
        {
            _root = root;

            _scrollProgress = 0;
            _currentExitHold = 0;
            _endDelayProgress = 0;
            _fadeProgress = 0;

            _scrollArea = _root.Q<VisualElement>("scroll_area");
            _scrollArea.style.translate = new Translate(0, new Length(startPos, LengthUnit.Percent), 0);
            _scrollArea.style.opacity = 1;
        }

        private void Hide()
        {
            _root.style.display = DisplayStyle.None;
            onComplete?.Invoke();
        }

        private void Update()
        {
            if (_root == null || !_root.visible) return;

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButton("Cancel"))
                Hide();
            else if (Input.anyKey)
            {
                if (_currentExitHold >= exitHoldDuration)
                    Hide();
                else
                    _currentExitHold += Time.unscaledDeltaTime;
            }
            else
            {
                _currentExitHold = 0;
            }

            float uDeltaTime = Time.unscaledDeltaTime;

            if (_scrollProgress < 1)
            {
                _scrollProgress += uDeltaTime * scrollSpeed;
                float val = Mathf.Lerp(startPos, endPos, _scrollProgress);
                _scrollArea.style.translate = new Translate(0, new Length(val, LengthUnit.Percent), 0);
            }
            else
            {
                if (_endDelayProgress < endDelay)
                {
                    _endDelayProgress += Time.unscaledDeltaTime;
                }
                else if (_fadeProgress < fadeDuration)
                {
                    _fadeProgress += Time.unscaledDeltaTime;
                    _scrollArea.style.opacity = 1 - (_fadeProgress / fadeDuration);
                }
                else
                {
                    Hide();
                }
            }
        }
    }
}
