using System.Collections.Generic;
using System.Linq;
using BonLib.DependencyInjection;
using BonLib.Events;
using BonLib.Managers;
using BonLib.Pooling;
using Core.Config;
using Core.Runtime.Events.Gameplay;
using Core.Runtime.Items;
using Core.Runtime.Slots;
using NaughtyAttributes;
using UnityEngine;

namespace Core.Runtime.Managers
{

    public class ItemManager : Manager<ItemManager>
    {
        private static int MAIN_TEX_ID = Shader.PropertyToID("_MainTex");
        private static int STENCIL_REF_ID = Shader.PropertyToID("_StencilRef");
        
        private ItemConfig m_config;
        public ItemConfig Config =>
            m_config ??= Resources.Load<ItemConfig>("Config/ItemConfig");
        
        private Dictionary<int, ItemTemplate> m_itemTemplateMap;

        private Dictionary<int, Item> m_itemMap;

        private ItemFactory m_factory;

        private GraphicManager m_graphicManager;
        private BoardManager m_boardManager;
        private SlotManager m_slotManager;

        public override void ResolveDependencies()
        {
            base.ResolveDependencies();

            m_graphicManager = DI.Resolve<GraphicManager>();
            m_boardManager = DI.Resolve<BoardManager>();
            m_slotManager = DI.Resolve<SlotManager>();
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

            var template = GetItemTemplate(templateId);

            var rentedPoolObj = PrefabPool.Rent(template.RendererPoolObject);

            var handle = m_graphicManager.CreateItemGraphic(template, rentedPoolObj);
            item.SetGraphicHandle(handle);
            m_itemMap.Add(item.Id, item);

            return item;
        }

        public void DestroyItem(in Item item)
        {
            m_graphicManager.DestroyItemGraphic(in item);
            m_itemMap.Remove(item.Id);
        }

        public Item GetItem(int id)
        {
            return m_itemMap[id];
        }

        public void SetAddress(ref Item item, in Slot slot)
        {
            item.m_address.Slot = slot;
            m_itemMap[item.Id] = item;
        }

        public ItemTemplate GetItemTemplate(int templateId)
        {
            return m_itemTemplateMap[templateId];
        }

        public void InitializeItems()
        {
            var templateIds = GetAllTemplateIDs();

            var boardState = m_boardManager.GenerateNewBoardState(in templateIds);
            
            var unitOffset = m_boardManager.PlacementOffset;
            var drawOffset = new Vector3(
                -((float)(boardState.Width - 1) / 2f) * unitOffset.x, 
                -((float)(boardState.Height - 1) / 2f) * unitOffset.y, 
                0f);
            
            Vector3 pos;

            for (int i = 0; i < boardState.Height; i++)
            {
                for (int j = 0; j < boardState.Width; j++)
                {
                    var index = i * boardState.Width + j;
                    
                    pos = m_boardManager.GetWorldPosition(index);

                    var item = CreateItem(boardState.Ids[index]);
                    var slot = m_slotManager.GetSlot(index);
                    
                    SetAddress(ref item, in slot);
                    
                    // item.SetAddress(in slot);

                    var graphic = m_graphicManager.GetItemGraphic(in item);
                    var poolObj = ((PoolObject)graphic.Target);
                    var poolObjTransform = poolObj.transform;
                    poolObjTransform.position = pos;
                    poolObjTransform.localScale = Config.Scale;
                }
            }
        }

        public ItemIterator GetItemIterator()
        {
            return new ItemIterator(m_itemMap);
        }

        [Button]
        public void TestDestroy()
        {
            var boardState = m_boardManager.GetCurrentBoardState();
            
            for (int i = 0; i < boardState.Width; i++)
            {
                var item = GetItem(i + boardState.Height);
                DestroyItem(in item);
            }
        }

    }

}