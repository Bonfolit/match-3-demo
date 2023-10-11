using System;

namespace Core.Runtime.Graphics
{

    public struct GraphicHandle : IEquatable<GraphicHandle>
    {
        public int Id;

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