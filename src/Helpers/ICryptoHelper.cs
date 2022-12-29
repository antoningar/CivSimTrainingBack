namespace cst_back.Helpers
{
    public interface ICryptoHelper
    {
        string Hash(string str);
        bool Verify(string str, string hash);
    }
}
