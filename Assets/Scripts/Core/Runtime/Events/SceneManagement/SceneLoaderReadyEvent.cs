using BonLib.Events;

namespace Core.Runtime.Events.SceneManagement
{

    public struct SceneLoaderReadyEvent : IEvent
    {
        public bool IsConsumed { get; set; }
    }

}