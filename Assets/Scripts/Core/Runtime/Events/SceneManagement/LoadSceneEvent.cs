using BonLib.Events;

namespace Core.Runtime.Events.SceneManagement
{

    public struct LoadSceneEvent : IEvent
    {
        public bool IsConsumed { get; set; }

        public int SceneIndex;

        public LoadSceneEvent(int sceneIndex) : this()
        {
            SceneIndex = sceneIndex;
        }
    }

}