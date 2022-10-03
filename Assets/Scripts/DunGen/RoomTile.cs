using UnityEngine;

namespace Assets.Scripts.DunGen
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class RoomTile : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected Sprite[] sprites;

        public bool IsExit => this is ExitTile;

        public virtual void Randomize()
        {
            if (sprites == null || sprites.Length == 0)
                return;

            spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        }

        public virtual void Update() { }
    }
}
