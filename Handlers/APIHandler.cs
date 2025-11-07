using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnackToSixPack.Handlers
{
    public class APIHandler
    {
        public static string readAPIUN()
        {

            // Läs samma nyckel i alla miljöer

            //If environment == dev (kör det som är nedan) annars använd GH vars i YAML-filen
            
            var mailAuthUN = Environment.GetEnvironmentVariable("MAIL_AUTH_UN");

            if (string.IsNullOrWhiteSpace(mailAuthUN))
            {
                Console.Error.WriteLine("Saknar API-nyckel till mail auth. Sätt den som miljövariabel.");
                Environment.Exit(1);
            }
            else
            {
                return mailAuthUN;
            }
            return "";
        }
        public static string readAPIPW()
        {
            // Läs samma nyckel i alla miljöer
            var mailAuthPW = Environment.GetEnvironmentVariable("MAIL_AUTH_PW");

            if (string.IsNullOrWhiteSpace(mailAuthPW))
            {
                Console.Error.WriteLine("Saknar API-nyckel till mail auth. Sätt den som miljövariabel.");
                Environment.Exit(1);
            }
            else
            {
                return mailAuthPW;
            }
            return "" ;
        }
    }
}
