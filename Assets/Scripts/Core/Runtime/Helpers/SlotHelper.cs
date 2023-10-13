﻿namespace Core.Runtime.Helpers
{

    public static class SlotHelper
    {
        public static (int x, int y) GetCoordinates(this int index, int width)
        {
            return (index % width, index / width);
        }
        
        public static void Populate<T>(this T[] arr, T value ) {
            for ( int i = 0; i < arr.Length;i++ ) {
                arr[i] = value;
            }
        }
    }

}