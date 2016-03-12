using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DelimitedFileLibrary;

namespace MLSFantasyTeamPicker
{
    class Program
    {
        static void Main(string[] args)
        {
            var df =
                new DelimitedFile(
                    "C:\\Users\\agardner\\Documents\\Visual Studio 2013\\Projects\\MLS Fantasy Team Picker\\Test Data\\160308 - After Round 1.txt",
                    "ascii", '\n', '\t');
            var playerPool = new PlayerPool(120, 0.125, 0.3125, 0.3125, 0.25);
            while (!df.EndOfFile)
            {
                df.GetNextRecord();
                var player = new Player(df.GetFieldByName("Player"), df.GetFieldByName("Position"),
                    Int32.Parse(df.GetFieldByName("Total Points")), double.Parse(df.GetFieldByName("Price")));
                playerPool.Members.Add(player);
            }
            playerPool.PickNextBestTeam(playerPool.TargetTeam.TotalBudget, playerPool.TargetTeam.GoalieBudget,
                playerPool.TargetTeam.DefenderBudget, playerPool.TargetTeam.MidfielderBudget,
                playerPool.TargetTeam.ForwardBudget, SearchStrategy.DiscardMostExpensive);
            var rosterByPosition = playerPool.TargetTeam.GetRoster().GroupBy(p => p.Position);
            Console.WriteLine("{0,-20}{1,5}{2,10}{3,10}{4,10}", "Name", "Position", "Points", "Price", "Value");
            foreach (var positionList in rosterByPosition.Select(position => position.ToList()))
            {
                positionList.Sort(PlayerPool.CompareByPrice);
                foreach (var player in positionList)
                {
                    Console.WriteLine("{0,-20}{1,5}{2,10:F1}{3,10:C}{4,10:F}", player.Name, player.Position, player.Points,
                        player.Price,
                        player.Value);
                }
            }
            Console.Read();
        }


    }
}
