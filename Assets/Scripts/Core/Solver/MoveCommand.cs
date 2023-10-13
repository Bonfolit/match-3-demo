using Core.Runtime.Items;

namespace Core.Solver
{

    public struct MoveCommand
    {
        public int FromIndex;
        public int ToIndex;
        public Item Item;

        public MoveCommand(int fromIndex, int toIndex, Item item)
        {
            FromIndex = fromIndex;
            ToIndex = toIndex;
            Item = item;
        }
    }

}