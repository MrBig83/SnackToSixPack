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

        public static WorkoutPlan ReadWP()
        {
            //// Ladda en eller flera planer från JSON-fil:
            string WPFilepath = Path.Combine($"Data/Users/{Session.CurrentUser.Id}", "workoutplans.json");
            string WPjson = File.ReadAllText(WPFilepath);
            WorkoutPlan plans = JsonSerializer.Deserialize<WorkoutPlan>(WPjson);

            Console.WriteLine(plans.PlanName);
            string nyttPlanName = Console.ReadLine();
            plans.PlanName = nyttPlanName;

            Console.WriteLine("Nytt plan name: " + plans.PlanName);
            SaveWP(plans);
            return JsonSerializer.Deserialize<WorkoutPlan>(WPjson);

        }

        public static void SaveWP(WorkoutPlan plans)
        {
            string WPFilepath = Path.Combine($"Data/Users/{Session.CurrentUser.Id}", "workoutplans.json");
            string outputJson = JsonSerializer.Serialize(plans, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(WPFilepath, outputJson);
        }

        public static void SaveProfile(Profile profile)
        {
            // Bygg samma typ av sökväg som i SaveWP
            string profileDirectory = Path.Combine("Data", "Users", Session.CurrentUser.Id.ToString());
            Directory.CreateDirectory(profileDirectory);

            string profileFilePath = Path.Combine(profileDirectory, "profile.json");

            string json = JsonSerializer.Serialize(profile, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(profileFilePath, json);
        }

        public static Profile? LoadProfile()
        {
            if (Session.CurrentUser == null)
                throw new InvalidOperationException("No user is logged in.");

            // Samma mappstruktur som i SaveProfile
            string filePath = Path.Combine("Data", "Users", Session.CurrentUser.Id.ToString(), "profile.json");

            if (!File.Exists(filePath))
            {
                Console.WriteLine("No profile found for this user.");
                return null;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                Profile? profile = JsonSerializer.Deserialize<Profile>(json);
                return profile;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to load profile: " + ex.Message);
                return null;
            }
        }
    }
}



    

