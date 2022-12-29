namespace cst_back.Helpers
{
    public class CryptoHelper : ICryptoHelper
    {
        public CryptoHelper(){ }

        public string Hash(string str)
        {
            return BCrypt.Net.BCrypt.HashPassword(str);
        }

        public bool Verify(string str, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(str, hash);
        }
    }
}
