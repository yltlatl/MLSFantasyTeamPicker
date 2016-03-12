using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MLSFantasyTeamPicker
{
    class Team : PlayerPool
    {
        #region Constructors

        //have to initialize list properties
        public Team()
        {
            Members = new List<Player>();
        }
        
        //must always have a budget
        public Team(double totalbudget) : this()
        {
            TotalBudget = totalbudget;
        }

        public Team(double totalBudget, double goalieBudgetWeight, double defenderBudgetWeight,
            double midfielderBudgetWeight, double forwardBudgetWeight) : this()
        {
            GoalieBudgetWeight = goalieBudgetWeight;
            DefenderBudgetWeight = defenderBudgetWeight;
            MidfielderBudgetWeight = midfielderBudgetWeight;
            ForwardBudgetWeight = forwardBudgetWeight;
            TotalBudget = totalBudget;
        }
        #endregion

        #region Fields

        private double _totalBudget;
        
        private double _goalieBudgetWeight;

        private double _defenderBudgetWeight;

        private double _midfielderBudgetWeight;

        private double _forwardBudgetWeight;

        private List<Player> _members; 

        public const int GoalieCount = 2;

        public const int DefenderCount = 5;

        public const int MidfielderCount = 5;

        public const int ForwardCount = 4;

        public int TeamCount { get { return (GoalieCount + DefenderCount + MidfielderCount + ForwardCount); } }

        #endregion

        #region Properties

        private new List<Player> Members
        {
            get { return _members; }
            set
            {
                if (value.Count > TeamCount) throw new ArgumentOutOfRangeException(string.Format("Cannot exceed total roster count of {0}", TeamCount));
                if ((value.Where(m => m.Position.Equals("GK"))).ToList().Count > GoalieCount)
                    throw new ArgumentOutOfRangeException(string.Format(
                        "Cannot exceed total goalie roster count of {0}", GoalieCount));
                if ((value.Where(m => m.Position.Equals("DF"))).ToList().Count > DefenderCount)
                    throw new ArgumentOutOfRangeException(
                        string.Format("Cannot exceed total defender roster count of {0}", DefenderCount));
                if ((value.Where(m => m.Position.Equals("MF")).ToList().Count > MidfielderCount))
                    throw new ArgumentOutOfRangeException(
                        string.Format("Cannot exceed total midfielder roster count of {0}", MidfielderCount));
                if ((value.Where(m => m.Position.Equals("FW")).ToList().Count > ForwardCount))
                    throw new ArgumentOutOfRangeException(
                        string.Format("Cannot exceed total forward roster count of {0}", ForwardCount));
                _members = value;
            }
        }
        
        public double TotalBudget
        {
            get { return _totalBudget; }
            set
            {
                GoalieBudget = value*_goalieBudgetWeight;
                DefenderBudget = value*_defenderBudgetWeight;
                MidfielderBudget = value*_midfielderBudgetWeight;
                ForwardBudget = value*_forwardBudgetWeight;
                if (value < 0) throw new ArgumentOutOfRangeException();
                _totalBudget = value;
            }
        }

        public double GoalieBudgetWeight
        {
            get { return _goalieBudgetWeight; }
            set
            {
                if (!ValidateBudgetWeights(value)) throw new ArgumentOutOfRangeException("value", "Budget weights must not exceed 1.");
                _goalieBudgetWeight = value;
                GoalieBudget = TotalBudget*_goalieBudgetWeight;
            }
        }

        public double DefenderBudgetWeight
        {
            get { return _defenderBudgetWeight; }
            set
            {
                if (!ValidateBudgetWeights(value)) throw new ArgumentOutOfRangeException("value", "Budget weights must not exceed 1.");
                _defenderBudgetWeight = value;
                DefenderBudget = TotalBudget*_defenderBudgetWeight;
            }
        }

        public double MidfielderBudgetWeight
        {
            get { return _midfielderBudgetWeight; }
            set
            {
                if (!ValidateBudgetWeights(value)) throw new ArgumentOutOfRangeException("value", "Budget weights must not exceed 1.");
                _midfielderBudgetWeight = value;
                MidfielderBudget = TotalBudget*MidfielderBudgetWeight;
            }
        }

        public double ForwardBudgetWeight
        {
            get { return _forwardBudgetWeight; }
            set
            {
                if (!ValidateBudgetWeights(value)) throw new ArgumentOutOfRangeException("value", "Budget weights must not exceed 1.");
                _forwardBudgetWeight = value;
                ForwardBudget = TotalBudget*_forwardBudgetWeight;
            }
        }

        public double GoalieBudget { get; private set; }

        public double DefenderBudget { get; private set; }

        public double MidfielderBudget { get; private set; }

        public double ForwardBudget { get; private set; }

        public IEnumerable<Player> Goalies
        {
            get { return Members.Where(m => m.Position.Equals("GK")); }
            set
            {
                if (value.Count() > GoalieCount) throw new ArgumentOutOfRangeException(string.Format("Number of goalies cannot exceed {0}", GoalieCount));
                if ((Members.Where(m => m.Position.Equals("GK")).ToList().Count >= GoalieCount)) throw new ArgumentOutOfRangeException(string.Format("Number of goalies cannot exceed {0}.", GoalieCount));
                foreach (var oldGoalie in from newGoalie in value from oldGoalie in Members where newGoalie == oldGoalie select oldGoalie)
                {
                    Members.Remove(oldGoalie);
                }
                Members.AddRange(value);
            }
        }

        public IEnumerable<Player> Defenders
        {
            get { return Members.Where(m => m.Position.Equals("DF")); }
            set
            {
                if (value.Count() > DefenderCount) throw new ArgumentOutOfRangeException(string.Format("Number of defenders cannot exceed {0}", DefenderCount));
                if ((Members.Where(m => m.Position.Equals("DF")).ToList().Count >= DefenderCount)) throw new ArgumentOutOfRangeException(string.Format("Number of defenders cannot exceed {0}", DefenderCount));
                foreach (var oldDefender in from newDefender in value from oldDefender in Members where newDefender == oldDefender select oldDefender)
                {
                    Members.Remove(oldDefender);
                }
                Members.AddRange(value);
            }
        }

        public IEnumerable<Player> Midfielders
        {
            get { return Members.Where(m => m.Position.Equals("MF")); }
            set
            {
                if (value.Count() > MidfielderCount) throw new ArgumentOutOfRangeException(string.Format("Number of midfielders cannot exceed {0}", MidfielderCount));
                if ((Members.Where(m => m.Position.Equals("MF")).ToList().Count >= MidfielderCount)) throw new ArgumentOutOfRangeException(string.Format("Number of midfielders cannot exceed {0}", MidfielderCount));
                foreach (var oldMidfielder in from newMidfielder in value from oldMidfielder in Members where newMidfielder == oldMidfielder select oldMidfielder)
                {
                    Members.Remove(oldMidfielder);
                }
                Members.AddRange(value);
            }
        }

        public IEnumerable<Player> Forwards
        {
            get { return Members.Where(m => m.Position.Equals("FW")); }
            set
            {
                if (value.Count() > ForwardCount) throw new ArgumentOutOfRangeException(string.Format("Number of forwards cannot exceed {0}", ForwardCount));
                if ((Members.Where(m => m.Position.Equals("FW")).ToList().Count >= ForwardCount)) throw new ArgumentOutOfRangeException(string.Format("Number of forwards cannot exceed {0}", ForwardCount));
                foreach (var oldForward in from newForward in value from oldForward in Members where newForward == oldForward select oldForward)
                {
                    Members.Remove(oldForward);
                }
                Members.AddRange(value);
            }
        }

        #endregion

        #region Methods

        private bool ValidateBudgetWeights(double weight)
        {
            if (weight > 1) return false;
            return !(_goalieBudgetWeight + _defenderBudgetWeight + _midfielderBudgetWeight + _forwardBudgetWeight > 1);
        }

        public IEnumerable<Player> ReplacePlayers(IEnumerable<Player> players)
        {
            var playersToRemove = new List<Player>();
            var playerList = players.ToList();
            var newPlayersByPosition = playerList.GroupBy(p => p.Position);
            foreach (var positionList in newPlayersByPosition)
            {
                playersToRemove.AddRange(Members.Where(m => m.Position.Equals(positionList.Key)).ToList());
                foreach (var playerToRemove in playersToRemove)
                {
                    Members.Remove(playerToRemove);
                }
                Members.AddRange(positionList.ToList());
            }
            return playersToRemove;
        }

        public IEnumerable<Player> GetRoster()
        {
            return Members;
        }

        #endregion


    }
}
