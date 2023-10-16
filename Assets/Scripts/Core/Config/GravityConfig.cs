using DG.Tweening;
using UnityEngine;

namespace Core.Config
{
    [CreateAssetMenu(fileName = "GravityConfig", menuName = "Config/Gravity Config", order = 0)]
    public class GravityConfig : ScriptableObject
    {
        public float Acceleration;
        public float DelayPerRow;
        public Ease DropEase;
    }

}