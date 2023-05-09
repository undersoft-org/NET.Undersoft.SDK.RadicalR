namespace RadicalR
{
    public static class StoreRoutes
    {
        public static string EntryStore { get; set; } = "entryData";
        public static string ReportStore { get; set; } = "reportData";
        public static string EventStore { get; set; } = "event";
        public static string DataStore { get; set; } = "data";
        public static string OpenEventStore { get; set; } = "openStore";
        public static string OpenDataStore { get; set; } = "openData";
        public static string StreamEventStore { get; set; } = "streamStore/Events";
        public static string StreamDataStore { get; set; } = "streamData";
        public static string CrudEventStore { get; set; } = "crudStore";
        public static string CrudDataStore { get; set; } = "crudData/";

        public static class Constant
        {
            public const string EntryStore = "entryData";
            public const string ReportStore = "reportData";
            public const string EventStore = "event";
            public const string DataStore = "data";
            public const string OpenEventStore = "openStore";
            public const string OpenDataStore = "openData";
            public const string StreamEventStore = "streamStore/Events";
            public const string StreamDataStore = "streamData/";
            public const string CrudEventStore = "crudStore";
            public const string CrudDataStore = "crudData";
        }
    }
}
