using BonLib.Events;
using Core.Runtime.Items;

namespace Core.Runtime.Events.Gameplay
{

    public struct ItemCreatedEvent : IEvent
    {
        public bool IsConsumed { get; set; }

        public Item Item;

        public ItemCreatedEvent(Item item) : this()
        {
            Item = item;
        }
    }

}