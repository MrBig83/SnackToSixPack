using SnackToSixPack.Handlers;

namespace SnackToSixPack.Classes
{
    public static class Session
    {
        public static User? CurrentUser { get; set; }

        public static void SetCurrentUser(User user)
        {
            CurrentUser = user;
        }

        public static void CurrentUserLogout()
        {
            CurrentUser = null;
        }
    }
}