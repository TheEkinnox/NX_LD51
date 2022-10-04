using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public static class Common
    {
        internal static WaitForEndOfFrame waitFrame;

        private static Image _fadeScreen;
        private static Image FadeScreen
        {
            get
            {
                if (!_fadeScreen)
                {
                    _fadeScreen = GameObject.FindGameObjectWithTag(Tags.FadeScreen).GetComponent<Image>();

                    if (!_fadeScreen)
                        throw new MissingComponentException($"Unable to find a game object with the tag {Tags.FadeScreen} and an Image component");
                }

                return _fadeScreen;
            }
        }

        static Common()
        {
            waitFrame = new WaitForEndOfFrame();
        }

        public static bool IsPrefab(this GameObject a_Object)
        {
            return a_Object.scene.rootCount == 0;
        }

        public static IEnumerator FadeIn(float duration)
        {
            duration = Mathf.Abs(duration);

            float originalTimeScale = Time.timeScale;

            if (originalTimeScale != 0)
                Time.timeScale = 0;

            float timer = duration;

            while (timer > 0)
            {
                FadeScreen.color = new Color(0, 0, 0, Mathf.Clamp(timer / duration, 0, 1));

                yield return Common.waitFrame;

                timer -= Time.unscaledDeltaTime;
            }

            if (originalTimeScale != 0)
                Time.timeScale = originalTimeScale;
        }

        public static IEnumerator FadeOut(float duration)
        {
            duration = Mathf.Abs(duration);

            float originalTimeScale = Time.timeScale;
            
            if (originalTimeScale != 0)
                Time.timeScale = 0;

            float timer = 0;

            while (timer < duration)
            {
                FadeScreen.color = new Color(0, 0, 0, Mathf.Clamp(timer / duration, 0, 1));

                yield return Common.waitFrame;

                timer += Time.unscaledDeltaTime;
            }

            if (originalTimeScale != 0)
                Time.timeScale = originalTimeScale;
        }

        internal static void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}
