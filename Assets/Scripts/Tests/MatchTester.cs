using System.Text;
using Core.Solver;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{

    public class MatchTester
    {
        [Test]
        public void TestRandomMatch()
        {
            var state = new MatchState
            {
                Width = 4,
                Height = 4
            };

            state.TemplateIds = new[] { 2, 3, 3, 3, 2, 1, -1, 2, 2, 2, -1, 1, 1, 0, -1, 3 };

            var hasMatch = BoardSolver.CheckMatches(in state, out var matches);

            if (hasMatch)
            {
                for (var i = 0; i < matches.Count; i++)
                {
                    Debug.Log($"Match {i}:");

                    var stringBuilder = new StringBuilder();
                    
                    for (int j = 0; j < matches[i].Indices.Length; j++)
                    {
                        stringBuilder.Append(matches[i].Indices[j]);

                        if (j != matches[i].Indices.Length - 1)
                        {
                            stringBuilder.Append("-");
                        }
                    }

                    Debug.Log(stringBuilder.ToString());

                    stringBuilder.Clear();
                    
                }
                
                Assert.True(hasMatch);
            }
        }

        [Test]
        public void TestBoardGeneration()
        {
            const int WIDTH = 8;
            const int HEIGHT = 8;

            var templateIds = new int[] { 0, 1, 2, 3 };

            var boardState = BoardSolver.CreateBoardConfiguration(WIDTH, HEIGHT, in templateIds);

            for (int i = 0; i < HEIGHT; i++)
            {
                var strBuilder = new StringBuilder();
                for (int j = 0; j < WIDTH; j++)
                {
                    strBuilder.Append(boardState.TemplateIds[j + i * WIDTH]);

                    if (j != WIDTH - 1)
                    {
                        strBuilder.Append(" ");
                    }
                }

                Debug.Log(strBuilder.ToString());

                strBuilder.Clear();
            }
        }
    }

}