using Core.Config;
using UnityEngine;

namespace Core.Solver
{

    public static class BoardSolver
    {
        private static SolverConfig m_config;
        public static SolverConfig Config => m_config ??= Resources.Load<SolverConfig>("Config/SolverConfig");

        public static BoardState Solve(int width, int height, in int[] templateIds)
        {
            var boardState = new BoardState
            {
                Width = width,
                Height = height
            };
            
            var count = boardState.Height * boardState.Width;

            boardState.Ids = new int[count];

            for (int i = 0; i < count; i++)
            {
                boardState.Ids[i] = templateIds[Random.Range(0, templateIds.Length)];
            }

            return boardState;
        }
    }

}