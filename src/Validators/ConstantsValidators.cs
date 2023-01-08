namespace cst_back.Validators
{
    public static class ConstantsValidators
    {
        public const int USERNAME_MIN_LENGTH = 3;
        public const int USERNAME_MAX_LENGTH = 16;
        public const int PASSWORD_MIN_LENGTH = 8;
        public static readonly List<string> INSTANCES_FILTER = new() { string.Empty, "Goal", "Map", "Civilization" };
    }
}
