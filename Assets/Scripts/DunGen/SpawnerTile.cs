using UnityEngine;

namespace Assets.Scripts.DunGen
{
    public class SpawnerTile : RoomTile
    {
        [SerializeField] protected GameObject prefab;
        [SerializeField] private float _spawnInterval;
        [SerializeField] private int _maxSpawnedCount = int.MaxValue;

        private bool _autoSpawn = false;
        private float _nextSpawnTime;
        private int _spawnableCount;

        public override void Update()
        {
            if (_autoSpawn && Time.time >= _nextSpawnTime)
                Spawn();
        }

        private void OnEnable()
        {
            _spawnableCount = _maxSpawnedCount;
        }

        public GameObject Spawn()
        {
            if (_spawnableCount <= 0)
                return null;

            GameObject instance = Instantiate(prefab, transform.position, Quaternion.identity);
            
            _spawnableCount--;

            if (_autoSpawn)
                _nextSpawnTime = Time.time + _spawnInterval;

            return instance;
        }

        public void StartSpawn()
        {
            _nextSpawnTime = Time.time + _spawnInterval;
            _autoSpawn = true;
        }

        public void EndSpawn()
        {
            _autoSpawn = false;
        }
    }
}
