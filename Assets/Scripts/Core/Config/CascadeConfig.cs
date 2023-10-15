using DG.Tweening;
using UnityEngine;

namespace Core.Config
{
    [CreateAssetMenu(fileName = "CascadeConfig", menuName = "Config/Cascade Config", order = 0)]
    public class CascadeConfig : ScriptableObject
    {
        public float DestroyDuration;
        public Ease DestroyEase;
    }

}