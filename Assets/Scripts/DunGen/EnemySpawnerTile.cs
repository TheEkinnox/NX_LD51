using UnityEngine;

namespace Assets.Scripts.DunGen
{
    public class EnemySpawnerTile : SpawnerTile
    {
        public Enemy Spawn(EnemyConfig config)
        {
            prefab = config.prefab;
            GameObject instance = Spawn();

            if (!instance)
                return null;

            Enemy enemy = instance.GetComponent<Enemy>();

            if (!enemy)
                return null;

            enemy.Init(config);

            return enemy;
        }
    }
}
