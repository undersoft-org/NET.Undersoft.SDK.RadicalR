namespace RadicalR
{
    [Serializable]
    public class QueryItems
    {
        public List<FilterItem> Filter { get; set; } = new();

        public List<SortItem> Sort { get; set; } = new();

    }
}
