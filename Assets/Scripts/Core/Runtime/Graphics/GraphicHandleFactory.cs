using System.Collections.Generic;

namespace Core.Runtime.Graphics
{

    public class GraphicHandleFactory
    {
        private int m_counter;

        public GraphicHandleFactory()
        {
            m_counter = 0;
        }

        public GraphicHandle Create()
        {
            return new GraphicHandle(GenerateId());
        }

        private int GenerateId()
        {
            return m_counter++;
        }
    }

}