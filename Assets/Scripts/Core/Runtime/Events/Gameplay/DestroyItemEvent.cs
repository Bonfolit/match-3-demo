using BonLib.Events;
using Core.Runtime.Items;
using Core.Runtime.Slots;

namespace Core.Runtime.Events.Gameplay
{

    public struct DestroyItemEvent : IEvent
    {
        public bool IsConsumed { get; set; }

        public Item Item;
        public Slot Slot;

        public DestroyItemEvent(Item item, Slot slot) : this()
        {
            Item = item;
            Slot = slot;
        }
    }

}