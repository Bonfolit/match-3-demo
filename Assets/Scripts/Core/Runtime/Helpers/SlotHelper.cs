namespace Core.Runtime.Helpers
{

    public static class SlotHelper
    {
        public static (int x, int y) GetCoordinates(this int index, int width)
        {
            return (index % width, index / width);
        }
    }

}