using System;
using System.Runtime.InteropServices;
using Core.Runtime.Graphics;

namespace Core.Runtime.Slots
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct Slot : IEquatable<Slot>
    {
        public readonly int Id;
        public readonly GraphicHandle GraphicHandle;

        public Slot(int id)
        {
            Id = id;
            GraphicHandle = default;
        }
        
        public Slot(int id, in GraphicHandle handle)
        {
            Id = id;
            GraphicHandle = handle;
        }

        public bool Equals(Slot other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is Slot other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }

}