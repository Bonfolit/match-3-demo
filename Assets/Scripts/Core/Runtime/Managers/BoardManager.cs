using System.Collections.Generic;
using BonLib.DependencyInjection;
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

    public class BoardManager : Manager<BoardManager>
    {
        private BoardConfig m_config;
        public BoardConfig Config => m_config ??= Resources.Load<BoardConfig>("Config/BoardConfig");
        public Vector2 PlacementOffset => Config.Offset;
        
        private Dictionary<int, Item> m_addressMap;


        private ItemManager m_itemManager;

        public override void ResolveDependencies()
        {
            base.ResolveDependencies();

            m_itemManager = DI.Resolve<ItemManager>();
        }

        public override void Initialize()
        {
            base.Initialize();

            m_addressMap = new Dictionary<int, Item>(Config.Dimensions.x * Config.Dimensions.y);

            var initializeEvt = new InitializeBoardEvent(Config.Dimensions, Config.Offset);
            EventManager.SendEvent(ref initializeEvt);
        }

        public MatchState GenerateNewMatchState(in int[] templateIds)
        {
            var matchState = BoardSolver.Solve(Config.Dimensions.x, Config.Dimensions.y, in templateIds);
            
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
            var pos = new Vector3(
                -((float)(Config.Dimensions.x - 1) / 2f) * PlacementOffset.x, 
                -((float)(Config.Dimensions.y - 1) / 2f) * PlacementOffset.y, 
                0f);

            var coords = index.GetCoordinates(Config.Dimensions.x);

            pos.x += coords.x * PlacementOffset.x;
            pos.y += coords.y * PlacementOffset.y;

            return pos;
        }

        public void SetAddress(ref Item item, in Slot slot)
        {
            m_addressMap[slot.Id] = item;
            m_itemManager.SetAddress(ref item, in slot);
        }

        public Item GetItem(in Slot slot)
        {
            return m_addressMap[slot.Id];
        }
        
        public Item GetItem(int index)
        {
            return m_addressMap[index];
        }
    }

}