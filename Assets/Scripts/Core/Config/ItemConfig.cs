using BonLib.Pooling;
using UnityEngine;

namespace Core.Config
{
    [CreateAssetMenu(fileName = "ItemConfig", menuName = "Config/Item Config", order = 0)]
    public class ItemConfig : ScriptableObject
    {
        public Vector3 Scale;
        public Vector2 Offset;
    }

}