using UnityEngine;

namespace Assets.Scripts.DunGen
{
    public class ExitTile : RoomTile
    {
        [SerializeField] protected Sprite[] lockedSprites;
        [SerializeField] protected Collider2D collider2d;

        private int _spriteIndex;
        private bool _isLocked;
        internal System.Action OnExit;

        public override void Randomize()
        {
            if (sprites == null || sprites.Length == 0)
                return;

            SetLocked(true);
        }

        public void SetLocked(bool locked)
        {
            _isLocked = locked;
            collider2d.isTrigger = !_isLocked;

            if (_isLocked && lockedSprites != null && lockedSprites.Length != 0)
                spriteRenderer.sprite = lockedSprites[Random.Range(0, lockedSprites.Length)];
            else if (!_isLocked && sprites != null && sprites.Length != 0)
                spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
            else
                spriteRenderer.sprite = null;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Player player;
            if (!_isLocked && other && other.enabled && (player = other.gameObject.GetComponent<Player>()) && !player.IsDead)
                OnExit?.Invoke();
        }
    }
}
