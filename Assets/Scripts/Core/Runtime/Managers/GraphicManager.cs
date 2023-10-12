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

    public class GraphicManager : Manager<GraphicManager>,
        IEventHandler<SetPlayableAreaEvent>
    {
        private static int MAIN_TEX_ID = Shader.PropertyToID("_MainTex");
        private static int STENCIL_REF_ID = Shader.PropertyToID("_StencilRef");

        private ItemGraphicConfig m_itemGraphicConfig;
        public ItemGraphicConfig ItemGraphicConfig =>
            m_itemGraphicConfig ??= Resources.Load<ItemGraphicConfig>("Config/ItemGraphicConfig");
        
        private Dictionary<int, Graphic> m_graphicMap;

        private GraphicHandleFactory m_handleFactory;

        private ItemManager m_itemManager;

        [SerializeField]
        private Transform m_maskTransform;
        
        private MaterialPropertyBlock m_mpb;

        public override void ResolveDependencies()
        {
            base.ResolveDependencies();

            m_itemManager = DI.Resolve<ItemManager>();
        }

        public override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            
            EventManager.AddListener<SetPlayableAreaEvent>(this);
        }

        public override void PreInitialize()
        {
            base.PreInitialize();

            m_graphicMap = new Dictionary<int, Graphic>(512);

            m_handleFactory = new GraphicHandleFactory();

            m_mpb = new MaterialPropertyBlock();

        }

        public void OnEventReceived(ref SetPlayableAreaEvent evt)
        {
            m_maskTransform.localScale = evt.Dimensions;
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
            var rend = ((MeshRenderer)graphicTarget.CustomReference);
            m_mpb.SetTexture(MAIN_TEX_ID, template.Texture);

            rend.SetPropertyBlock(m_mpb);
            var handle = CreateHandle(graphicTarget);
            
            return handle;
        }
    }

}