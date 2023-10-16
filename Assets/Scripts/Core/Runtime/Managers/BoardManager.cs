using System.Collections.Generic;
using System.Threading.Tasks;
using BonLib.DependencyInjection;
using BonLib.Events;
using BonLib.Managers;
using BonLib.Pooling;
using Core.Config;
using Core.Runtime.Events.Gameplay;
using Core.Runtime.Helpers;
using Core.Runtime.Items;
using Core.Runtime.Slots;
using Core.Solver;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Core.Runtime.Managers
{

    public class BoardManager : Manager<BoardManager>,
        IEventHandler<DestroyItemEvent>
    {
        private BoardConfig m_config;
        public BoardConfig Config => m_config ??= Resources.Load<BoardConfig>("Config/BoardConfig");
        public Vector2 PlacementOffset => Config.Offset;
        
        private Dictionary<int, Item> m_slotContentMap;
        private Dictionary<int, Slot> m_addressMap;

        private ItemManager m_itemManager;
        private GraphicManager m_graphicManager;

        private Vector3 m_bottomLeftPos;
        
        public override void ResolveDependencies()
        {
            base.ResolveDependencies();

            m_itemManager = DI.Resolve<ItemManager>();
            m_graphicManager = DI.Resolve<GraphicManager>();
        }

        public override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            
            EventManager.AddListener<DestroyItemEvent>(this);
        }

        public override void PreInitialize()
        {
            base.PreInitialize();
            
            m_bottomLeftPos = new Vector3(
                -((float)(Config.Dimensions.x - 1) / 2f) * PlacementOffset.x, 
                -((float)(Config.Dimensions.y - 1) / 2f) * PlacementOffset.y, 
                0f);
        }

        public override void Initialize()
        {
            base.Initialize();

            m_slotContentMap = new Dictionary<int, Item>(Config.Dimensions.x * Config.Dimensions.y * 2);
            m_addressMap = new Dictionary<int, Slot>(Config.Dimensions.x * Config.Dimensions.y * 2);

            var initializeEvt = new InitializeBoardEvent(Config.Dimensions, Config.Offset);
            EventManager.SendEvent(ref initializeEvt);
        }

        public void OnEventReceived(ref DestroyItemEvent evt)
        {
            ClearSlot(evt.Slot);
        }

        public MatchState GenerateNewMatchState(in int[] templateIds)
        {
            var matchState = BoardSolver.CreateBoardConfiguration(Config.Dimensions.x, Config.Dimensions.y, in templateIds);
            
            return matchState;
        }

        public MatchState GetCurrentMatchState()
        {
            var matchState = new MatchState
            {
                Width = Config.Dimensions.x,
                Height = Config.Dimensions.y
            };
            var count = matchState.Width * matchState.Height;
            
            matchState.TemplateIds = new int[count];
            matchState.TemplateIds.Populate(-1);

            foreach (var kv in m_slotContentMap)
            {
                if (kv.Key >= count)
                    continue;
                
                var item = kv.Value;
                if (item.IsValid)
                {
                    matchState.TemplateIds[kv.Key] = item.TemplateId;
                }
                else
                {
                    matchState.TemplateIds[kv.Key] = -1;
                }
            }

            return matchState;
        }

        public BoardState GetCurrentBoardState()
        {
            var boardState = new BoardState
            {
                Width = Config.Dimensions.x,
                Height = Config.Dimensions.y
            };
            
            var count = boardState.Width * boardState.Height;

            boardState.Items = new Item[count];

            foreach (var kv in m_slotContentMap)
            {
                if (kv.Key >= count)
                    continue;
                
                boardState.Items[kv.Key] = kv.Value;
            }

            return boardState;
        }

        public Vector3 GetWorldPosition(int index)
        {
            var pos = m_bottomLeftPos;

            var coords = index.GetCoordinates(Config.Dimensions.x);

            pos.x += coords.x * PlacementOffset.x;
            pos.y += coords.y * PlacementOffset.y;

            return pos;
        }

        public int GetIndexFromWorldPos(Vector3 pos)
        {
            var refPos = (Vector2)m_bottomLeftPos - PlacementOffset * .5f;

            var x = (int)((pos.x - refPos.x) / PlacementOffset.x);
            var y = (int)((pos.y - refPos.y) / PlacementOffset.y);
                
            return (x, y).GetIndex(Config.Dimensions.x);
        }

        public bool IsSpawnColumn(int rowIndex)
        {
            return Config.SpawnColumns[rowIndex];
        }

        public void SetAddress(ref Item item, in Slot slot)
        {
            // Debug.LogError($"Set address {slot.Id.GetCoordinates(Config.Dimensions.x)}");
            Debug.LogError($"Set address {slot.Id}");
            if (m_slotContentMap.ContainsKey(slot.Id) && m_slotContentMap[slot.Id].IsValid)
            {
                Debug.LogError($"Overriding address on a valid item!");
            }
            m_slotContentMap[slot.Id] = item;
            m_addressMap[item.Id] = slot;
            m_itemManager.SetAddress(ref item, in slot);
        }

        public Item GetItemAtSlot(in Slot slot)
        {
            return m_slotContentMap[slot.Id];
        }
        
        public Item GetItemAtSlot(int slotIndex)
        {
            return m_slotContentMap[slotIndex];
        }

        public int GetItemAddressAsIndex(in Item item)
        {
            return m_addressMap[item.Id].Id;
        }
        
        public Slot GetItemAddressAsSlot(in Item item)
        {
            return m_addressMap[item.Id];
        }
        
        public ItemIterator GetItemIterator()
        {
            return new ItemIterator(m_slotContentMap);
        }

        public void ClearSlot(in Slot slot)
        {
            // Debug.LogError($"Clear address {slot.Id.GetCoordinates(Config.Dimensions.x)}");
            Debug.LogError($"Clear address {slot.Id}");
            m_slotContentMap[slot.Id] = default;
        }

        public void ClearAddress(in Item item)
        {
            if (item.IsValid && m_addressMap.ContainsKey(item.Id))
            {
                m_addressMap.Remove(item.Id);
            }
        }

        [Button]
        public void DebugAddresses()
        {
            foreach (var kv in m_addressMap)
            {
                Debug.Log($"{kv.Key} - {kv.Value.Id}");
            }
        }
        
        [Button]
        public void DebugSlotContents()
        {
            foreach (var kv in m_slotContentMap)
            {
                Debug.Log($"{kv.Key} - {kv.Value.Id} : {kv.Value.TemplateId}");
            }
        }
    }

}