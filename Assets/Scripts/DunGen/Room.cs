using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scripts.DunGen
{
    public class Room : MonoBehaviour
    {
        public List<ExitTile> ExitTiles
        {
            get;
            private set;
        }

        public bool IsComplete { get; private set; }

        [SerializeField] private EnemyConfig[] _enemyConfigs;

        private RoomTile[] _tiles;
        private List<EnemySpawnerTile> _enemySpawnPoints;
        private Enemy _keyHolder;

        private void Awake()
        {
            _tiles = GetComponentsInChildren<RoomTile>();
            _enemySpawnPoints = new List<EnemySpawnerTile>();


            if (ExitTiles == null)
                ExitTiles = new List<ExitTile>();
            else
                ExitTiles.Clear();

            for (int i = 0; i < _tiles.Length; i++)
            {
                if (_tiles[i].IsExit)
                {
                    ExitTiles.Add((ExitTile)_tiles[i]);
                    ExitTiles[ExitTiles.Count - 1].OnExit += () => {
                        IsComplete = true;
                        Debug.Log($"Exiting room {name}");
                    };
                }

                if (_tiles[i] is EnemySpawnerTile est && est)
                    _enemySpawnPoints.Add(est);
            }
        }

        private void FixedUpdate()
        {
            if (_keyHolder && _keyHolder.IsDead)
                RoomComplete();
        }

        public void Generate(int enemyCount)
        {
            _keyHolder = null;

            foreach (RoomTile tile in _tiles)
            {
                tile.Randomize();

                if (tile is not EnemySpawnerTile && tile is SpawnerTile st)
                    st.StartSpawn();
            }

            if (enemyCount == 0 || _enemyConfigs.Length == 0 || _enemySpawnPoints.Count == 0)
            {
                foreach (ExitTile exit in ExitTiles)
                    exit.SetLocked(false);

                return;
            }

            int keyHolderIndex = Random.Range(0, enemyCount);

            EnemyConfig config;
            Enemy enemy;

            for (int i = 0; i < enemyCount; i++)
            {
                config = _enemyConfigs[Random.Range(0, _enemyConfigs.Length)];
                enemy = _enemySpawnPoints[Random.Range(0, _enemySpawnPoints.Count)].Spawn(config);

                if (i == keyHolderIndex)
                    _keyHolder = enemy;
            }
        }

        public void RoomComplete()
        {
            foreach (RoomTile tile in _tiles)
                if (tile is SpawnerTile st)
                    st.EndSpawn();

            foreach (ExitTile exit in ExitTiles)
                exit.SetLocked(false);
        }

        public void Close()
        {
            if (!gameObject.IsPrefab())
                Destroy(gameObject);
        }
    }
}
