namespace cst_back.Settings
{
    public class AccountDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string AccountCollectionName { get; set; } = null!;
    }
}
