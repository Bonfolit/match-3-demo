﻿using System.Collections.Generic;
using BonLib.DependencyInjection;
using BonLib.Events;
using BonLib.Managers;
using Core.Config;
using Core.Runtime.Events.Gameplay;
using Core.Runtime.Helpers;
using Core.Runtime.Items;
using Core.Runtime.Slots;
using Core.Solver;
using UnityEngine;

namespace Core.Runtime.Managers
{

    public class BoardManager : Manager<BoardManager>,
        IEventHandler<DestroyItemEvent>,
        IEventHandler<SwipeSlotsEvent>
    {
        private BoardConfig m_config;
        public BoardConfig Config => m_config ??= Resources.Load<BoardConfig>("Config/BoardConfig");
        public Vector2 PlacementOffset => Config.Offset;
        
        private Dictionary<int, Item> m_addressMap;

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
            EventManager.AddListener<SwipeSlotsEvent>(this);
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

            m_addressMap = new Dictionary<int, Item>(Config.Dimensions.x * Config.Dimensions.y);

            var initializeEvt = new InitializeBoardEvent(Config.Dimensions, Config.Offset);
            EventManager.SendEvent(ref initializeEvt);
        }

        public void OnEventReceived(ref DestroyItemEvent evt)
        {
            ClearAddress(evt.Slot);
        }

        public void OnEventReceived(ref SwipeSlotsEvent evt)
        {
            var fromItem = GetItemAtSlot(in evt.FromSlot);
            var toItem = GetItemAtSlot(in evt.ToSlot);

            var fromPos = GetWorldPosition(evt.FromSlot.Id);
            var toPos = GetWorldPosition(evt.ToSlot.Id);
            
            m_graphicManager.SetPosition(in fromItem, toPos);
            m_graphicManager.SetPosition(in toItem, fromPos);
            
            SetAddress(ref fromItem, evt.ToSlot);
            SetAddress(ref toItem, evt.FromSlot);

            var cascadeEvt = new TriggerCascadeEvent();
            EventManager.SendEvent(ref cascadeEvt);
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

            foreach (var item in m_itemManager.GetItemIterator())
            {
                matchState.TemplateIds[item.Address.Slot.Id] = item.TemplateId;
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
            
            foreach (var item in m_itemManager.GetItemIterator())
            {
                boardState.Items[item.Address.Slot.Id] = item;
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

        public void SetAddress(ref Item item, in Slot slot)
        {
            m_addressMap[slot.Id] = item;
            m_itemManager.SetAddress(ref item, in slot);
        }

        public Item GetItemAtSlot(in Slot slot)
        {
            return m_addressMap[slot.Id];
        }
        
        public Item GetItemAtSlot(int slotIndex)
        {
            return m_addressMap[slotIndex];
        }

        public void ClearAddress(in Slot slot)
        {
            m_addressMap[slot.Id] = default;
        }
    }

}