using BonLib.DependencyInjection;
using BonLib.Managers;
using Core.Config;
using Core.Runtime.Board;
using Core.Runtime.Events.Gameplay;
using Core.Runtime.Items;
using Core.Solvers;
using UnityEngine;

namespace Core.Runtime.Managers
{

    public class BoardManager : Manager<BoardManager>
    {
        private BoardConfig m_config;
        public BoardConfig Config => m_config ??= Resources.Load<BoardConfig>("Config/BoardConfig");

        public override void Initialize()
        {
            base.Initialize();

            var initializeEvt = new InitializeBoardEvent(Config.Dimensions);
            EventManager.SendEvent(ref initializeEvt);
        }

        public BoardState GenerateNewBoardState(in int[] templateIds)
        {
            var boardState = BoardSolver.Solve(Config.Dimensions.x, Config.Dimensions.y, in templateIds);
            
            return boardState;
        }
    }

}