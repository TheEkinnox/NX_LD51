using UnityEngine;

namespace Assets.Scripts
{
    internal class CamAspect : MonoBehaviour
    {
        [SerializeField] private Camera _cam;
        [SerializeField] private float _targetAspect = 16/9f;

        // Code from http://gamedesigntheory.blogspot.com/2010/09/controlling-aspect-ratio-in-unity.html
        private void Update()
        {
            // determine the game window's current aspect ratio
            float screenAspect = Screen.width / (float)Screen.height;

            // current viewport height should be scaled by this amount
            float heightScale = screenAspect / _targetAspect;

            // if scaled height is less than current height, add letterbox
            if (heightScale < 1.0f)
            {
                Rect rect = _cam.rect;

                rect.width = 1.0f;
                rect.height = heightScale;
                rect.x = 0;
                rect.y = (1.0f - heightScale) / 2.0f;

                _cam.rect = rect;
            }
            else // add pillarbox
            {
                float widthScale = 1.0f / heightScale;

                Rect rect = _cam.rect;

                rect.width = widthScale;
                rect.height = 1.0f;
                rect.x = (1.0f - widthScale) / 2.0f;
                rect.y = 0;

                _cam.rect = rect;
            }
        }
    }
}
