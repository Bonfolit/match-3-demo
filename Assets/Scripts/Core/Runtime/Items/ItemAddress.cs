using Core.Runtime.Slots;

namespace Core.Runtime.Items
{

    public struct ItemAddress
    {
        public Slot Slot;

        public ItemAddress(Slot slot)
        {
            Slot = slot;
        }
    }

}