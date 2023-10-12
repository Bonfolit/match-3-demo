using System.Collections.Generic;
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

            const int ITERATION_LIMIT = 1000;

            var iter = 0;
            while (iter < ITERATION_LIMIT && CheckMatches(in boardState, out var matches))
            {
                iter++;
                
                var match = matches[0];
                
                var newTemplate = match.TemplateId;

                while (newTemplate == match.TemplateId)
                {
                    newTemplate = templateIds[Random.Range(0, templateIds.Length)];
                }

                var replaceIndex = match.Indices[Random.Range(0, match.Indices.Length)];

                boardState.Ids[replaceIndex] = newTemplate;
            }

            if (iter == ITERATION_LIMIT)
            {
                Debug.LogError("ITERATION REACHED LIMIT!");
            }

            Debug.Log($"Iteration reached: {iter}");

            return boardState;
        }

        public static bool CheckMatches(in BoardState state, out List<MatchData> matches)
        {
            matches = new List<MatchData>();

            int scanId;
            int count;
            
            for (int i = 0; i < state.Width; i++)
            {
                scanId = -1;
                count = 0;
                
                for (int j = 0; j < state.Height; j++)
                {
                    var index = i + state.Width * j;
                    var currentId = state.Ids[index];

                    if (scanId != currentId)
                    {
                        if (count >= 3)
                        {
                            var match = new MatchData(scanId, count);

                            for (int k = 0; k < count; k++)
                            {
                                var fillIndex = index - state.Width * (k + 1);
                                match.Indices[k] = fillIndex;
                            }

                            matches.Add(match);
                        }
                        
                        count = 1;
                        scanId = currentId;
                    }
                    else
                    {
                        count++;
                    }
                }

                if (count >= 3)
                {
                    var match = new MatchData(scanId, count);

                    for (int j = 0; j < count; j++)
                    {
                        var fillIndex = i + (state.Height - 1 - j) * state.Width;
                        match.Indices[j] = fillIndex;
                    }
                    
                    matches.Add(match);
                }
            }
            
            for (int i = 0; i < state.Height; i++)
            {
                scanId = -1;
                count = 0;
                
                for (int j = 0; j < state.Width; j++)
                {
                    var index = j + state.Width * i;
                    var currentId = state.Ids[index];

                    if (scanId != currentId)
                    {
                        if (count >= 3)
                        {
                            var match = new MatchData(scanId, count);

                            for (int k = 0; k < count; k++)
                            {
                                var fillIndex = index - (k + 1);
                                match.Indices[k] = fillIndex;
                            }

                            matches.Add(match);
                        }
                        
                        count = 1;
                        scanId = currentId;
                    }
                    else
                    {
                        count++;
                    }
                }

                if (count >= 3)
                {
                    var match = new MatchData(scanId, count);

                    for (int j = 0; j < count; j++)
                    {
                        // var fillIndex = i + (state.Height - 1 - j) * state.Width;
                        var fillIndex = i * state.Width + (state.Width - 1 - j);
                        match.Indices[j] = fillIndex;
                    }
                    
                    matches.Add(match);
                }
            }

            return matches.Count > 0;
        }
    }

}