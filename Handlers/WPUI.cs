using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SnackToSixPack.Handlers
{
    public class WPUI
    {
        public static void ShowWPUI(WorkoutPlan plan)
        {

            // ====== Huvudtabell ======
            var header = new Table()
            {
                Border = TableBorder.Rounded,
                Expand = true
            };

            header.Title = new TableTitle($"[bold yellow]{plan.PlanName}[/]");
            header.AddColumn(new TableColumn("[bold]Info[/]"));

            header.AddRow($"[bold]Mål:[/]\n{plan.Goal}");
            header.AddRow($"[bold]Period:[/] {plan.StartDate.ToShortDateString()} till {plan.EndDate.ToShortDateString()}");
            header.AddEmptyRow();

            AnsiConsole.Write(header);

            // ====== En tabell per träningsdag ======
            foreach (var day in plan.Workouts)
            {
                var dayTable = new Table()
                {
                    Border = TableBorder.Rounded,
                    Expand = true
                };
                
                dayTable.Title = new TableTitle($"[yellow]{day.DayOfWeek} – {day.Title}[/]");

                dayTable.AddColumn(new TableColumn("[bold]Övning[/]").Centered().Width(40));
                dayTable.AddColumn(new TableColumn("[bold]Set[/]").Centered());
                dayTable.AddColumn(new TableColumn("[bold]Reps[/]").Centered());
                dayTable.AddColumn(new TableColumn("[bold]Vikt[/]").Centered());
                dayTable.AddColumn(new TableColumn("[bold]Vila (sek)[/]").Centered());

                foreach (var ex in day.Exercises)
                {
                    string weightText = ex.Weight.HasValue ? $"{ex.Weight} kg" : "-";

                    dayTable.AddRow(
                        ex.Name,
                        ex.Sets.ToString(),
                        ex.Reps.ToString(),
                        weightText,
                        ex.RestTime.ToString()
                    );
                }

                AnsiConsole.Write(dayTable);
                AnsiConsole.WriteLine();
            }
        }
    }
}
