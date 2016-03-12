using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLSFantasyTeamPicker
{
    class PlayerPool
    {
        #region Constructors

        //must have an argless constructor because Team inherits from PlayerPool.
        public PlayerPool()
        {
            Members = new List<Player>();
            DiscardedPlayers = new List<Player>();
        }
        
        public PlayerPool(double targetBudget, double goalieBudgetWeight, double defenderBudgetWeight, double midfielderBudgetWeight, double forwardBudgetWeight)
            : this()
        {
            TargetTeam = new Team(targetBudget, goalieBudgetWeight, defenderBudgetWeight, midfielderBudgetWeight,
                forwardBudgetWeight);
        }

        public PlayerPool(Player player)
            : this()
        {
            Members.Add(player);
        }

        public PlayerPool(List<Player> players)
            : this()
        {
            Members = players;
        }

        #endregion

        #region Properties
        public List<Player> Members { get; private set; }

        public Team TargetTeam { get; private set; }

        public List<Player> DiscardedPlayers { get; private set; } 

        #endregion

        #region Methods


        public void PickNextBestTeam(double totalBudget, double goalieBudget, double defenderBudget, double midfielderBudget, double forwardBudget, SearchStrategy searchStrategy)
        {
            
            PickNextBestPlayerList("GK", goalieBudget, searchStrategy);
            PickNextBestPlayerList("DF", defenderBudget, searchStrategy);
            PickNextBestPlayerList("MF", midfielderBudget, searchStrategy);
            PickNextBestPlayerList("FW", forwardBudget, searchStrategy);
        }


        //get the available budget for the player class
        //select two best values out of the players that are (individually) cheaper than the budget
        //see if their combined value is more than the budget
        //depending on the search strategy, either
        //1. discard the most expensive player and add the next cheapest player with highest value, or
        //2. discard the median expensive player and add the next cheapest player with highest value
        //repeat the strategy until the budget test is met
        private void PickNextBestPlayerList(string position, double budget, SearchStrategy searchStrategy)
        {
            var eligiblePlayers = new List<Player>();
            
            foreach (var member in Members.Where(member => member.Position.Equals(position) && member.Price < budget))
            {
                if (DiscardedPlayers.Count == 0)
                {
                    eligiblePlayers.Add(member);
                }
                else
                {
                    eligiblePlayers.AddRange(from discardedPlayer in DiscardedPlayers where member != discardedPlayer select member);
                }
            }

            var nextBestPlayerCount = new int();

            switch (position)
            {
                case "GK":
                    nextBestPlayerCount = Team.GoalieCount;
                    break;
                case "DF":
                    nextBestPlayerCount = Team.DefenderCount;
                    break;
                case "MF":
                    nextBestPlayerCount = Team.ForwardCount;
                    break;
                case "FW":
                    nextBestPlayerCount = Team.ForwardCount;
                    break;
                default:
                    throw new ArgumentException("Invalid position.", "position");
            }
            
            eligiblePlayers.Sort(CompareByValue);
            var nextBestPlayers = eligiblePlayers.Take(nextBestPlayerCount).ToList();
            DiscardedPlayers.AddRange(TargetTeam.ReplacePlayers(nextBestPlayers));
            if (nextBestPlayers.Sum(g => g.Price) > budget)
            {
                nextBestPlayers.Sort(CompareByPrice);
                if (searchStrategy == SearchStrategy.DiscardMostExpensive)
                {
                    DiscardedPlayers.Add(nextBestPlayers.First());
                }
                else
                {
                    if (nextBestPlayers.Count%2 != 0)
                    {
                        var indexToRemove = (int) Math.Ceiling((double)nextBestPlayers.Count/2) - 1;
                        DiscardedPlayers.Add(nextBestPlayers[indexToRemove]);
                    }
                    else
                    {
                        var lowIndex = nextBestPlayers.Count/2;
                        var median = nextBestPlayers[lowIndex].Price + nextBestPlayers[lowIndex + 1].Price/2;
                        var highPriceDistance = Math.Abs(nextBestPlayers[lowIndex].Price - median);
                        var lowPriceDistance = Math.Abs(nextBestPlayers[lowIndex + 1].Price - median);
                        if (highPriceDistance >= lowPriceDistance)
                        {
                            DiscardedPlayers.Add(nextBestPlayers[lowIndex + 1]);
                        }
                        else
                        {
                            DiscardedPlayers.Add(nextBestPlayers[lowIndex]);
                        }
                    }
                }
                PickNextBestPlayerList(position, budget, searchStrategy);
            }
            return nextBestPlayers;
        }

        public static int CompareByValue(Player player1, Player player2)
        {
            if (player1 == null && player2 == null) return 0;
            if (player1 == null) return 1;
            if (player2 == null) return -1;
            
            var value1 = player1.Value;
            var value2 = player2.Value;
            if (Math.Abs(value1 - value2) < .0001) return 0;
            if (value1 > value2) return -1;
            if (value1 < value2) return 1;
            return 0;
        }

        public static int CompareByPrice(Player player1, Player player2)
        {
            if (player1 == null && player2 == null) return 0;
            if (player1 == null) return 1;
            if (player2 == null) return -1;

            var value1 = player1.Price;
            var value2 = player2.Price;
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
