using BonLib.Pooling;
using UnityEngine;

namespace Core.Config
{
    [CreateAssetMenu(fileName = "ItemGraphicConfig", menuName = "Config/Item Graphic Config", order = 0)]
    public class ItemGraphicConfig : ScriptableObject
    {
        public Vector3 Scale;
        public Vector2 Offset;
    }

}