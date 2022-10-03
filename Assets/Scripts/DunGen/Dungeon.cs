using System.Collections;
using UnityEngine;

namespace Assets.Scripts.DunGen
{
    public class Dungeon : MonoBehaviour
    {
        [SerializeField] private DungeonConfig _config;

        private Room _curRoom;
        private int _curLevel;
        private Player _player;
        private bool _isLoading;

        private void Start()
        {
            if (!_config)
                throw new MissingReferenceException($"Missing reference to dungeon config for dungeon \"{name}\"");

            if (!_config.playerPrefab)
                throw new MissingReferenceException($"Missing reference to player prefab on Dungeon config \"{_config.name}\"");

            if (_config.rooms.Length == 0)
                throw new MissingReferenceException($"No rooms have been added to Dungeon config \"{_config.name}\"");

            StartCoroutine(Generate());
        }

        private void Update()
        {
            if (!_isLoading && (!_curRoom || _curRoom.IsComplete))
                StartCoroutine(Generate());
        }

        private IEnumerator Generate()
        {
            if (!_config || _curLevel >= _config.roomCount || _config.rooms == null || _config.rooms.Length == 0)
                yield break;

            _isLoading = true;

            yield return Common.FadeOut(_config.fadeDuration);

            if (_curRoom)
                _curRoom.Close();

            if (_curLevel == 0 || _config.rooms.Length == 1)
                _curRoom = _config.rooms[0];
            else
                _curRoom = _config.rooms[Random.Range(1, _config.rooms.Length)];

            GameObject instance = Instantiate(_curRoom.gameObject, Vector3.zero, Quaternion.identity);

            _curRoom = instance.GetComponent<Room>();

            _curRoom.Generate((int)_config.enemiesCountCurve.Evaluate(_curLevel));

            if (!_player)
            {
                instance = Instantiate(_config.playerPrefab, Vector2.zero, Quaternion.identity);
                _player = instance.GetComponent<Player>();
            }
            else
            {
                _player.transform.position = Vector2.zero;
                _player.transform.rotation = Quaternion.identity;
                _player.Rejuvenate();
            }

            yield return Common.FadeIn(_config.fadeDuration);

            _curLevel++;
            _isLoading = false;
        }
    }
}
