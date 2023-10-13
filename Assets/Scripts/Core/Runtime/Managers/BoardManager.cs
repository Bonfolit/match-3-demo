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

        public override void Initialize()
        {
            base.Initialize();

            var initializeEvt = new InitializeBoardEvent(Config.Dimensions, Config.Offset);
            EventManager.SendEvent(ref initializeEvt);
        }

        public BoardState GenerateNewBoardState(in int[] templateIds)
        {
            var boardState = BoardSolver.Solve(Config.Dimensions.x, Config.Dimensions.y, in templateIds);
            
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
    }

}