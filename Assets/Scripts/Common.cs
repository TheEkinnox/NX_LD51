using UnityEngine;

namespace Assets.Scripts
{
    internal class Common
    {
        internal static WaitForEndOfFrame waitFrame;

        static Common()
        {
            waitFrame = new WaitForEndOfFrame();
        }
    }
}
