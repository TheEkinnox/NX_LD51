using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "Regen", menuName = "Custom/Ability/Regen", order = 3)]
    public class Regen : Ability
    {
        public int health = 5;

        public override bool Use()
        {
            if (base.Use())
            {
                player.Heal(health);
                return true;
            }

            return false;
        }
    }
}
