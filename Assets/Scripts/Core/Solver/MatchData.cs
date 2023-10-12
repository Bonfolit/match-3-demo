namespace Core.Solver
{

    public struct MatchData
    {
        public int TemplateId;
        public int[] Indices;

        public MatchData(int templateId, int count)
        {
            TemplateId = templateId;
            Indices = new int[count];
        }
    }

}