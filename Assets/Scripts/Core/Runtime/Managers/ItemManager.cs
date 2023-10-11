using System.Collections.Generic;
using System.Linq;
using BonLib.DependencyInjection;
using BonLib.Events;
using BonLib.Managers;
using BonLib.Pooling;
using Core.Runtime.Events.Gameplay;
using Core.Runtime.Items;
using UnityEngine;

namespace Core.Runtime.Managers
{

    public class ItemManager : Manager<ItemManager>
    {
        private Dictionary<int, ItemTemplate> m_itemTemplateMap;

        private Dictionary<int, Item> m_itemMap;

        private ItemFactory m_factory;

        private GraphicManager m_graphicManager;

        public override void ResolveDependencies()
        {
            base.ResolveDependencies();

            m_graphicManager = DI.Resolve<GraphicManager>();
        }

        public override void PreInitialize()
        {
            base.PreInitialize();

            m_itemMap = new Dictionary<int, Item>(512);

            RegisterTemplates();
        }

        private void RegisterTemplates()
        {
            var itemTemplates = Resources.LoadAll<ItemTemplate>("Data/Item");
            m_itemTemplateMap = new Dictionary<int, ItemTemplate>(128);
            
            var templateCount = itemTemplates.Length;
            m_factory = new ItemFactory(templateCount);
            
            for (var i = 0; i < templateCount; i++)
            {
                m_itemTemplateMap.Add(i, itemTemplates[i]);
                m_factory.RegisterTemplate(itemTemplates[i]);

                m_itemTemplateMap[i].Id = i;
            }
        }

        public int[] GetAllTemplateIDs()
        {
            var count = m_itemTemplateMap.Count;
            
            var templateIds = new int[count];
            
            var mapArray = m_itemTemplateMap.ToArray();

            for (int i = 0; i < count; i++)
            {
                templateIds[i] = mapArray[i].Key;
            }

            return templateIds;
        }

        public Item CreateItem(int templateId)
        {
            var item = m_factory.Create(templateId);
            m_itemMap.Add(item.Id, item);

            var template = GetItemTemplate(templateId);

            var rentedPoolObj = PrefabPool.Rent(template.RendererPoolObject);

            var handle = m_graphicManager.CreateItemGraphic(template, rentedPoolObj);
            item.SetGraphicHandle(handle);

            return item;
        }

        public ItemTemplate GetItemTemplate(int templateId)
        {
            return m_itemTemplateMap[templateId];
        }

    }

}