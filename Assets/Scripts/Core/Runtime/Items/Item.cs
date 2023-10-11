using System;
using System.Runtime.InteropServices;
using BonLib.Pooling;
using Core.Runtime.Graphics;

namespace Core.Runtime.Items
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Item : IEquatable<Item>
    {
        private int m_id;
        private int m_templateId;
        private GraphicHandle m_graphicHandle;
        
        public int Id => m_id;
        public int TemplateId => m_templateId;

        public GraphicHandle GraphicHandle => m_graphicHandle;

        public Item(int id, int templateId)
        {
            m_id = id;
            m_templateId = templateId;

            m_graphicHandle = default;
        }

        public void SetGraphicHandle(GraphicHandle handle)
        {
            m_graphicHandle = handle;
        }

        public bool Equals(Item other)
        {
            return m_id == other.m_id;
        }

        public override bool Equals(object obj)
        {
            return obj is Item other && Equals(other);
        }

        public override int GetHashCode()
        {
            return m_id;
        }
    }

}