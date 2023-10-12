using BonLib.Pooling;
using UnityEngine;

namespace Core.Runtime.Items
{
    [CreateAssetMenu(fileName = "ItemTemplate", menuName = "Data/Item/Template", order = 0)]
    public class ItemTemplate : ScriptableObject
    {
        [HideInInspector]
        public int Id;
        public Texture Texture;
        public PoolObject RendererPoolObject;
    }

}