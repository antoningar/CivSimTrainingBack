namespace cst_back.Settings
{
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string AccountCollectionName { get; set; } = null!;
        public string CounterCollectionName { get; set; } = null!;
        public string CounterAccountId { get; set; } = null!;
        public string InstanceCollectionName { get; set; } = null!;
    }
}
