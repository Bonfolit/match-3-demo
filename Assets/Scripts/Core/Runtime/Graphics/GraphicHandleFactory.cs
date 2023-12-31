﻿using System.Collections.Generic;
using UnityEngine;

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
            var handle = new GraphicHandle(GenerateId());
            return handle;
        }

        private int GenerateId()
        {
            return m_counter++;
        }
    }

}