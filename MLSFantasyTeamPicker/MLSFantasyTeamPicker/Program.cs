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
                    "C:\\Users\\aaron.gardner\\Documents\\Visual Studio 2013\\Projects\\MLSFantasyTeamPicker\\Test Data\\160328 - Round 4.txt",
                    "ascii", '\n', '\t');
            var playerPool = new PlayerPool();
            while (!df.EndOfFile)
            {
                df.GetNextRecord();
                var player = new Player(df.GetFieldByName("Player"), df.GetFieldByName("Position"),
                    int.Parse(df.GetFieldByName("Total Points")), double.Parse(df.GetFieldByName("Price")));
                playerPool.Members.Add(player);
            }
            var targetTeam = new Team(120, 0.125, 0.3125, 0.3125, 0.25);
            playerPool.PickNextBestTeam(targetTeam, SearchStrategy.DiscardMostExpensive);
            var rosterByPosition = targetTeam.GetRoster().GroupBy(p => p.Position);
            Console.WriteLine("{0,-20}{1,5}{2,10}{3,10}{4,10}", "Name", "Position", "Points", "Price", "Value");
            foreach (var positionList in rosterByPosition.Select(position => position.ToList()))
            {
                positionList.Sort(PlayerPool.CompareByOperativePrice);
                foreach (var player in positionList)
                {
                    Console.WriteLine("{0,-20}{1,5}{2,10:F1}{3,10:C}{4,10:F}", player.Name, player.Position, player.CurrentPrice,
                        player.CurrentPoints,
                        player.CurrentValue);
                }
            }
            Console.Read();
        }


    }
}
