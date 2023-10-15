using System.Collections.Generic;
using Core.Config;
using UnityEngine;

namespace Core.Solver
{

    public static class BoardSolver
    {
        private static SolverConfig m_config;
        public static SolverConfig Config => m_config ??= Resources.Load<SolverConfig>("Config/SolverConfig");

        public static MatchState CreateBoardConfiguration(int width, int height, in int[] templateIds)
        {
            var matchState = new MatchState
            {
                Width = width,
                Height = height
            };
            
            var count = matchState.Height * matchState.Width;

            matchState.TemplateIds = new int[count];

            for (int i = 0; i < count; i++)
            {
                matchState.TemplateIds[i] = templateIds[Random.Range(0, templateIds.Length)];
            }

            const int ITERATION_LIMIT = 1000;

            var iter = 0;
            while (iter < ITERATION_LIMIT && CheckMatches(in matchState, out var matches))
            {
                iter++;
                
                var match = matches[0];
                
                var newTemplate = match.TemplateId;

                while (newTemplate == match.TemplateId)
                {
                    newTemplate = templateIds[Random.Range(0, templateIds.Length)];
                }

                var replaceIndex = match.Indices[Random.Range(0, match.Indices.Length)];

                matchState.TemplateIds[replaceIndex] = newTemplate;
            }

            if (iter == ITERATION_LIMIT)
            {
                Debug.LogError("ITERATION REACHED LIMIT!");
            }

            Debug.Log($"Iteration reached: {iter}");

            return matchState;
        }

        public static bool CheckMatches(in MatchState state, out List<MatchData> matches)
        {
            matches = new List<MatchData>();

            int scanId;
            int count;
            
            for (int i = 0; i < state.Width; i++)
            {
                scanId = int.MinValue;
                count = 0;
                
                for (int j = 0; j < state.Height; j++)
                {
                    var index = i + state.Width * j;
                    var currentId = state.TemplateIds[index];

                    if (scanId != currentId)
                    {
                        if (count >= 3 && scanId != -1)
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

                if (count >= 3 && scanId != -1)
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
                scanId =  int.MinValue;
                count = 0;
                
                for (int j = 0; j < state.Width; j++)
                {
                    var index = j + state.Width * i;
                    var currentId = state.TemplateIds[index];

                    if (scanId != currentId)
                    {
                        if (count >= 3 && scanId != -1)
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

                if (count >= 3 && scanId != -1)
                {
                    var match = new MatchData(scanId, count);

                    for (int j = 0; j < count; j++)
                    {
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