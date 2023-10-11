namespace Core.Solvers
{
    [System.Serializable]
    public struct SolverRule
    {
        public readonly Axis Axis;
        
        
    }

    public enum Axis
    {
        NULL,
        Horizontal,
        Vertical
    }

}