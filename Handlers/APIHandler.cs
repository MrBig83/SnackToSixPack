namespace SnackToSixPack.Handlers
{
    public class APIHandler
    {
        public static string readAPIUN()
        {
            // Hämta aktuell miljö (standard: Development)
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";
            var isGitHub = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"));

            // Läs samma nyckel i alla miljöer
            var mailAuthUN = Environment.GetEnvironmentVariable("MAIL_AUTH_UN");

            if (string.IsNullOrWhiteSpace(mailAuthUN))
            {
                var source = isGitHub ? "GitHub Secrets" : "lokala miljövariabler";
                Console.Error.WriteLine(
                    $"[{environment}] Saknar MAIL_AUTH_UN. " +
                    $"Kontrollera att värdet är satt i {source}.");
                Environment.Exit(1);
            }

            return mailAuthUN!;
        }

        public static string readAPIPW()
        {
            // Hämta aktuell miljö (standard: Development)
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";
            var isGitHub = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"));

            // Läs samma nyckel i alla miljöer
            var mailAuthPW = Environment.GetEnvironmentVariable("MAIL_AUTH_PW");

            if (string.IsNullOrWhiteSpace(mailAuthPW))
            {
                var source = isGitHub ? "GitHub Secrets" : "lokala miljövariabler";
                Console.Error.WriteLine(
                    $"[{environment}] Saknar MAIL_AUTH_PW. " +
                    $"Kontrollera att värdet är satt i {source}.");
                Environment.Exit(1);
            }

            return mailAuthPW!;
        }
    }
}
