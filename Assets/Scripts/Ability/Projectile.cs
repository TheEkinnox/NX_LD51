using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "Projectile", menuName = "Custom/Ability/Projectile", order = 2)]
    public class Projectile : Ability
    {
        public int damage = 1;
        public float range = 5;
        public float throwSpeed = 5;
        public GameObject projectilePrefab;
        public bool breakOnContact;
        public GameObject breakFx;

        public override bool Use()
        {
            if (projectilePrefab && base.Use())
            {
                float angle = Mathf.Atan2(character.LastDirection.x, -character.LastDirection.y) * Mathf.Rad2Deg;

                GameObject instance = Instantiate(projectilePrefab, character.ProjectileSpawn.position, Quaternion.Euler(new Vector3(0, 0, angle)));

                ProjectileBehaviour proj = instance.GetComponent<ProjectileBehaviour>();

                if (!proj)
                {
                    Destroy(instance);
                    return false;
                }

                proj.Init(this, character);
                Destroy(instance, range / throwSpeed);

                return true;
            }

            return false;
        }
    }
}
