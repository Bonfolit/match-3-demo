using BonLib.Events;
using Core.Runtime.Slots;

namespace Core.Runtime.Events.Gameplay
{

    public struct SwapSlotsEvent : IEvent
    {
        public bool IsConsumed { get; set; }

        public Slot FromSlot;
        public Slot ToSlot;
        public float Duration;

        public SwapSlotsEvent(Slot fromSlot, Slot toSlot, float duration) : this()
        {
            FromSlot = fromSlot;
            ToSlot = toSlot;
            Duration = duration;
        }
    }

}