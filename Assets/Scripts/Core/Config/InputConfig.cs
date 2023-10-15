using UnityEngine;

namespace Core.Config
{

    [CreateAssetMenu(fileName = "InputConfig", menuName = "Config/Input Config", order = 0)]
    public class InputConfig : ScriptableObject
    {
        public float SwipeSensitivity;
        public float SwipeDuration;
    }

}