﻿using System;
using System.Runtime.InteropServices;

namespace Core.Runtime.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct GraphicHandle : IEquatable<GraphicHandle>
    {
        public readonly int Id;

        public GraphicHandle(int id)
        {
            Id = id;
        }

        public bool Equals(GraphicHandle other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is GraphicHandle other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }

}