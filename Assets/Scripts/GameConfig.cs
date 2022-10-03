using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Custom/GameConfig", order = 1)]
    public class GameConfig : ScriptableObject
    {
        public float secPerPlayerConf = 10;
        public int rejuvenateCount = 3;
        public int ageStep = 10;
        public CharacterConfig[] playerConfigs;
    }
}
