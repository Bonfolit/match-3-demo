using System;
using System.Runtime.InteropServices;
using Core.Runtime.Graphics;
using Core.Runtime.Slots;

namespace Core.Runtime.Items
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Item : IEquatable<Item>
    {
        private ItemAddress m_address;
        private GraphicHandle m_graphicHandle;
        
        public int Id { get; }
        public int TemplateId { get; }
        public ItemAddress Address => m_address;
        public GraphicHandle GraphicHandle => m_graphicHandle;

        public Item(int id, int templateId)
        {
            Id = id;
            TemplateId = templateId;
            m_address = new ItemAddress
            {
                Slot = new Slot(-1, default)
            };

            m_graphicHandle = default;
        }

        public void SetAddress(in Slot slot)
        {
            m_address.Slot = slot;
        }

        public void SetGraphicHandle(GraphicHandle handle)
        {
            m_graphicHandle = handle;
        }

        public bool Equals(Item other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is Item other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }

}