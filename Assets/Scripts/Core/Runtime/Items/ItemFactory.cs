using System.Collections.Generic;
using UnityEngine;

namespace Core.Runtime.Items
{

    public class ItemFactory
    {
        private Dictionary<int, ItemTemplate> m_templates;

        private int m_counter;

        public ItemFactory(int capacity)
        {
            m_templates = new Dictionary<int, ItemTemplate>(capacity);

            m_counter = 0;
        }


        public void RegisterTemplate(ItemTemplate template)
        {
            m_templates.Add(template.GetInstanceID(), template);
        }

        public Item Create(int templateId)
        {
            // var template = m_templates[templateId];

            var item = new Item(GenerateId(), templateId);
            Debug.Log($"Create Item {item.Id}");

            return item;
        }

        private int GenerateId()
        {
            return m_counter++;
        }
    }

}