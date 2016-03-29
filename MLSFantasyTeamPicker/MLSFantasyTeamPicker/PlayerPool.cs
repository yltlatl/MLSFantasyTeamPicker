using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLSFantasyTeamPicker
{
    internal class PlayerPool
    {
        #region Constructors

        public PlayerPool()
        {
            Members = new List<Player>();
            InjuredPlayers = new List<Player>();
        }
        

        public PlayerPool(List<Player> players)
            : this()
        {
            Members = players;
        }

        #endregion

        #region Properties
        public List<Player> Members { get; private set; }

        public List<Player> InjuredPlayers { get; private set; } 

        #endregion

        #region Methods


        public Team PickNextBestTeam(Team team, SearchStrategy searchStrategy)
        {
            var ineligiblePlayers = team.Members;
            foreach (var injuredPlayer in InjuredPlayers.Where(injuredPlayer => !ineligiblePlayers.Contains(injuredPlayer)))
                ineligiblePlayers.Add(injuredPlayer);

            team = PickNextBestPlayerList(team.GoalieBudget, searchStrategy, Team.GoalieCount,
                GetEligiblePlayers(team.GoalieBudget, ineligiblePlayers, "GK"), team);

            team = PickNextBestPlayerList(team.DefenderBudget, searchStrategy, Team.DefenderCount,
                GetEligiblePlayers(team.DefenderBudget, ineligiblePlayers, "DF"), team);

            team = PickNextBestPlayerList(team.MidfielderBudget, searchStrategy,
                Team.MidfielderCount, GetEligiblePlayers(team.MidfielderBudget, ineligiblePlayers, "MF"), team);

            team = PickNextBestPlayerList(team.ForwardBudget, searchStrategy, Team.ForwardCount,
                GetEligiblePlayers(team.ForwardBudget, ineligiblePlayers, "FW"), team);

            return team;
        }


        private List<Player> GetEligiblePlayers(double budget, IEnumerable<Player> ineligiblePlayers, string position = null)
        {
            var retVal = position == null ? Members : Members.Where(m => m.Position.Equals(position)).ToList();

            if (retVal.Count == 0) throw new ArgumentException("Position not found.", position);

            retVal.RemoveAll(p => p.OperativePrice > budget);

            foreach (var player in from player in retVal from ineligiblePlayer in ineligiblePlayers.Where(ineligiblePlayer => player == ineligiblePlayer) select player)
            {
                retVal.Remove(player);
            }

            return retVal;
        }

        //get the available budget for the player class
        //select two best values out of the players that are (individually) cheaper than the budget
        //see if their combined value is more than the budget
        //depending on the search strategy, either
        //1. discard the most expensive player and add the next cheapest player with highest value, or
        //2. discard the median expensive player and add the next cheapest player with highest value
        //repeat the strategy until the budget test is met
        private static Team PickNextBestPlayerList(double budget, SearchStrategy searchStrategy, int playerCount, List<Player> eligiblePlayers, Team team)
        {
            while (true)
            {
                eligiblePlayers.Sort(CompareByOperativeStatistic);
                var nextBestPlayers = eligiblePlayers.Take(playerCount).ToList();
                foreach (var eligiblePlayer in team.ReplacePlayers(nextBestPlayers).SelectMany(replacedPlayer => eligiblePlayers.Where(eligiblePlayer => eligiblePlayer == replacedPlayer)))
                {
                    eligiblePlayers.Remove(eligiblePlayer);
                }

                if ((nextBestPlayers.Sum(g => g.OperativePrice) <= budget)) return team;
                nextBestPlayers.Sort(CompareByOperativePrice);
                if (searchStrategy == SearchStrategy.DiscardMostExpensive)
                {
                    eligiblePlayers.Remove(nextBestPlayers.First());
                }
                else
                {
                    if (nextBestPlayers.Count%2 != 0)
                    {
                        var indexToRemove = (int) Math.Ceiling((double) nextBestPlayers.Count/2) - 1;
                        eligiblePlayers.Remove(nextBestPlayers[indexToRemove]);
                    }
                    else
                    {
                        var lowIndex = nextBestPlayers.Count/2;
                        var median = nextBestPlayers[lowIndex].OperativePrice + nextBestPlayers[lowIndex + 1].OperativePrice/2;
                        var highPriceDistance = Math.Abs(nextBestPlayers[lowIndex].OperativePrice - median);
                        var lowPriceDistance = Math.Abs(nextBestPlayers[lowIndex + 1].OperativePrice - median);
                        if (highPriceDistance >= lowPriceDistance)
                        {
                            eligiblePlayers.Remove(nextBestPlayers[lowIndex + 1]);
                        }
                        else
                        {
                            eligiblePlayers.Remove(nextBestPlayers[lowIndex]);
                        }
                    }
                }
            }
        }

        public static int CompareByOperativeStatistic(Player player1, Player player2)
        {
            if (player1 == null && player2 == null) return 0;
            if (player1 == null) return 1;
            if (player2 == null) return -1;
            
            var value1 = player1.OperativeStatistic;
            var value2 = player2.OperativeStatistic;
            if (Math.Abs(value1 - value2) < .0001) return 0;
            if (value1 > value2) return -1;
            if (value1 < value2) return 1;
            return 0;
        }

        public static int CompareByOperativePrice(Player player1, Player player2)
        {
            if (player1 == null && player2 == null) return 0;
            if (player1 == null) return 1;
            if (player2 == null) return -1;

            var value1 = player1.OperativePrice;
            var value2 = player2.OperativePrice;
            if (Math.Abs(value1 - value2) < .0001) return 0;
            if (value1 > value2) return -1;
            if (value1 < value2) return 1;
            return 0;
        }

        #endregion


    }

    public enum SearchStrategy
    {
        DiscardMostExpensive,
        DiscardMedianExpensive
    };
}
