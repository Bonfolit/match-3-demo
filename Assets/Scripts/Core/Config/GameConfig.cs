using NaughtyAttributes;
using UnityEngine;

namespace Core.Config
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Config/Game Config", order = 0)]
    public class GameConfig : ScriptableObject
    {
        public GameObject UIPrefab;
        
        [Scene]
        public int LoadingScene;
        [Scene]
        public int MainScene;
    }
}