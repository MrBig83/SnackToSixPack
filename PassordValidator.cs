public class PasswordValidator
{
    public static bool IsPassWordStrong(string password)
    {
        if (password.Length < 6)
        {
            Console.WriteLine("Password must be at least 6 characters long.");
            return false;
        }

        if (!password.Any(char.IsUpper))
        {
            Console.WriteLine("Password must contain at least one uppercase letter.");
            return false;
        }

        if (!password.Any(char.IsLower))
        {
            Console.WriteLine("Password must contain at least one lowercase letter.");
            return false;
        }

        if (!password.Any(char.IsDigit))
        {
            Console.WriteLine("Password must contain at least one number.");
            return false;
        }

        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            Console.WriteLine("Password must contain at least one special character.");
            return false;
        }

        return true;
    }
}