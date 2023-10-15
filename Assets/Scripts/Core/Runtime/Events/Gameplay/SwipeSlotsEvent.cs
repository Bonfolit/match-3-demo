using BonLib.Events;
using Core.Runtime.Slots;

namespace Core.Runtime.Events.Gameplay
{

    public struct SwipeSlotsEvent : IEvent
    {
        public bool IsConsumed { get; set; }

        public Slot FromSlot;
        public Slot ToSlot;

        public SwipeSlotsEvent(Slot fromSlot, Slot toSlot) : this()
        {
            FromSlot = fromSlot;
            ToSlot = toSlot;
        }
    }

}