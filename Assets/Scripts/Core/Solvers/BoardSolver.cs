using Core.Config;
using Core.Runtime.Board;
using UnityEngine;

namespace Core.Solvers
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
        public static void Solve(this BoardState state, in int[] templateIds)
        {
            var count = state.Height * state.Width;

            state.Ids = new int[count];

            for (int i = 0; i < count; i++)
            {
                state.Ids[i] = templateIds[Random.Range(0, templateIds.Length)];
            }
        }
    }

}