using BonLib.Events;

namespace Core.Runtime.Events.SceneManagement
{

    public struct SceneLoadedEvent : IEvent
    {
        public bool IsConsumed { get; set; }

        public int Scene;

        public SceneLoadedEvent(int scene) : this()
        {
            Scene = scene;
        }
    }

}