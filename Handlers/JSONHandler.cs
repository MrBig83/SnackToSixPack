using SnackToSixPack.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SnackToSixPack.Handlers
{
    public static class JSONHelper
    {

        public static List<User> LoadUsers()
        {
            string userFilePath = Path.Combine("Data", "Users.json");
            try
            {
                if (!File.Exists(userFilePath))
                    return new List<User>();

                string json = File.ReadAllText(userFilePath);

                var options = new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter() }
                };

                return JsonSerializer.Deserialize<List<User>>(json, options) ?? new List<User>();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] Failed to load users: {ex.Message}");
                Console.ResetColor();
                return new List<User>();
            }
        }



        public static void SaveUsers(List<User> users)
        {
            string userFilePath = Path.Combine("Data", "Users.json");
            try
            {

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };

            string json = JsonSerializer.Serialize(users, options);
            File.WriteAllText(userFilePath, json);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[SUCCESS] Users saved successfully!");
            Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] Failed to save users: {ex.Message}");
                Console.ResetColor();
            }
        }

        public static void ReadWP()
        {
            //OBS! =================== NEDAN ÄR INTE FÄRDIGT!! ================================== OBS!
            //// Ladda en eller flera planer från JSON-fil:
            string WPFilepath = Path.Combine($"Data/Users/{Session.CurrentUser.Id}", "workoutplans.json");
            string WPjson = File.ReadAllText(WPFilepath);
            WorkoutPlan plans = JsonSerializer.Deserialize<WorkoutPlan>(WPjson);

            
            
                Console.WriteLine(plans.PlanName);
            string nyttPlanName = Console.ReadLine();
            plans.PlanName = nyttPlanName;
            
            Console.WriteLine("Nytt plan name: " + plans.PlanName);
            SaveWP(plans);

        }

        public static void SaveWP(WorkoutPlan plans)
        {
            //OBS! =================== NEDAN ÄR INTE FÄRDIGT!! ================================== OBS!
            string WPFilepath = Path.Combine($"Data/Users/{Session.CurrentUser.Id}", "workoutplans.json");
            string outputJson = JsonSerializer.Serialize(plans, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(WPFilepath, outputJson);
        }


    }
}
