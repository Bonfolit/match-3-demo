using BonLib.DependencyInjection;
using UnityEngine;

namespace Core.Config
{

    [CreateAssetMenu(fileName = "SlotConfig", menuName = "Config/Slot Config", order = 0)]
    public class SlotConfig : ScriptableObject
    {
        public Sprite Sprite;
        public Vector3 Scale;
        public Vector2 Offset;
    }

}