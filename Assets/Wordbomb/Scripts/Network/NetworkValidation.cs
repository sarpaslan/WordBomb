namespace WordBomb.Network
{
    public class NetworkValidation
    {
        public static bool IsValidName(string name)
        {
            if (name.Length > 20 || name.Length < 2)
                return false;
            return true;
        }
        public static string EnsureValidName(string name, string newName)
        {
            if (!IsValidName(newName))
                return name;
            return newName;
        }
    }
}