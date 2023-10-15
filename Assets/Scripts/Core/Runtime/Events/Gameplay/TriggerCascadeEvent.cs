using BonLib.Events;

namespace Core.Runtime.Events.Gameplay
{

    public struct TriggerCascadeEvent : IEvent
    {
        public bool IsConsumed { get; set; }
    }

}