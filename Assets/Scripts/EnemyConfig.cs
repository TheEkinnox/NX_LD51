using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Custom/EnemyConfig", order = 3)]
    public class EnemyConfig : CharacterConfig
    {
        public float minDistToTarget = 1;
        public float maxDistToTarget = 5;
        public ContactFilter2D targetContactFilter;
    }
}
