using System.Collections.Generic;
using BonLib.DependencyInjection;
using BonLib.Events;
using BonLib.Managers;
using BonLib.Pooling;
using Core.Config;
using Core.Runtime.Events.Gameplay;
using Core.Runtime.Graphics;
using Core.Runtime.Items;
using UnityEngine;

namespace Core.Runtime.Managers
{

    public class GraphicManager : Manager<GraphicManager>
    {
        private ItemGraphicConfig m_itemGraphicConfig;

        public ItemGraphicConfig ItemGraphicConfig =>
            m_itemGraphicConfig ??= Resources.Load<ItemGraphicConfig>("Config/ItemGraphicConfig");
        
        private Dictionary<int, Graphic> m_graphicMap;

        private GraphicHandleFactory m_handleFactory;

        private ItemManager m_itemManager;

        public override void ResolveDependencies()
        {
            base.ResolveDependencies();

            m_itemManager = DI.Resolve<ItemManager>();
        }

        public override void PreInitialize()
        {
            base.PreInitialize();

            m_graphicMap = new Dictionary<int, Graphic>(512);

            m_handleFactory = new GraphicHandleFactory();
        }

        public Graphic GetGraphic(in GraphicHandle handle)
        {
            return m_graphicMap[handle.Id];
        }

        public Graphic GetItemGraphic(in Item item)
        {
            return m_graphicMap[item.GraphicHandle.Id];
        }

        public GraphicHandle CreateHandle(object target)
        {
            var graphic = new Graphic(target);

            var handle = m_handleFactory.Create();
            
            m_graphicMap.Add(handle.Id, graphic);

            return handle;
        }

        public void DisposeHandle(in GraphicHandle handle)
        {
            m_graphicMap.Remove(handle.Id);
        }

        public GraphicHandle CreateItemGraphic(ItemTemplate template, PoolObject graphicTarget)
        {
            var rend = ((SpriteRenderer)graphicTarget.CustomReference);
            rend.sprite = template.Sprite;
            var handle = CreateHandle(graphicTarget);
            
            return handle;
        }
    }

}