using BonLib.Events;
using Core.Runtime.Items;

namespace Core.Runtime.Events.Gameplay
{

    public struct CreateItemGraphicEvent : IEvent
    {
        public bool IsConsumed { get; set; }

        public Item Item;

        public CreateItemGraphicEvent(in Item item) : this()
        {
            Item = item;
        }
    }

}