using System;
using UnityEngine;

namespace Assets.Scripts
{

    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Custom/PlayerConfig", order = 2)]
    public class CharacterConfig : ScriptableObject
    {
        public GameObject prefab;
        public string displayName;
        public float speed;
        public int health;
        public AudioClip spawnSound;
        public AudioClip deathSound;
        public GameObject deathFx;
        public RuntimeAnimatorController animController;
        public Ability[] abilities;
    }
}