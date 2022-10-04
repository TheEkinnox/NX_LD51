using UnityEngine;

namespace Assets.Scripts.DunGen
{
    [CreateAssetMenu(fileName = "DungeonConfig", menuName = "Custom/DungeonConfig", order = 2)]
    public class DungeonConfig : ScriptableObject
    {
        public int roomCount = int.MaxValue;
        public int minEnemiesCount = 1;
        public int maxEnemiesCount = 10;
        public AnimationCurve enemiesCountCurve;
        public GameObject playerPrefab;
        public float fadeDuration = 1;

        // Note: The first room in the array will always be generated on spawn but is excluded from the rotation
        public Room[] rooms;
    }
}
