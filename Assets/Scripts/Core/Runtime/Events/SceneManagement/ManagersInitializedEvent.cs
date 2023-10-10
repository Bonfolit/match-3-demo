using BonLib.Events;

namespace Core.Runtime.Events.SceneManagement
{

    public struct ManagersInitializedEvent : IEvent
    {
        public bool IsConsumed { get; set; }
    }

}