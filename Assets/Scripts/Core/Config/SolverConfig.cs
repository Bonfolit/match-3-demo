using Core.Solvers;
using UnityEngine;

namespace Core.Config
{
    [CreateAssetMenu(fileName = "SolverConfig", menuName = "Config/Solver Config", order = 0)]
    public class SolverConfig : ScriptableObject
    {
        public SolverRule[] Rules;
    }

}