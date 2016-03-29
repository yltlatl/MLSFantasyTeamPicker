using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLSFantasyTeamPicker
{
    class Player : IEquatable<Player>
    {
        #region Constructors

        public Player()
        {
            PointsSeries = new List<int>();
            PriceSeries = new List<double>();
            ValueSeries = new List<double>();
        }

        public Player(string name, string position, int points, double price) : this()
        {
            Name = name;
            Position = position;
            PointsSeries.Add(points);
            PriceSeries.Add(price);
            if (Math.Abs(price - 0) < 0.0001) throw new ArgumentException("Price must not equal 0.");
            ValueSeries.Add(points/price);
        }

        public Player(string name, string position, List<int> pointsSeries, List<double> priceSeries) : this()
        {
            Name = name;
            Position = position;
            PointsSeries = pointsSeries;
            PriceSeries = priceSeries;
            if (pointsSeries.Count != priceSeries.Count) throw new ArgumentException("Point and price series must be of equal length.");
            for (var i = 0; i < pointsSeries.Count; i++)
            {
                ValueSeries[i] = pointsSeries[i]/priceSeries[i];
            }
        }

        #endregion

        #region IEquatable Implementation
        public bool Equals(Player other)
        {
            return other != null && Name.Equals(other.Name);
        }

        public override bool Equals(object obj)
        {
            var playerObj = obj as Player;
            return playerObj != null && Name.Equals(playerObj.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator == (Player player1, Player player2)
        {
            if ((object) player1 == null || (object) player2 == null) return Object.Equals(player1, player2);

            return player1.Equals(player2);
        }

        public static bool operator !=(Player player1, Player player2)
        {
            if ((object) player1 == null || (object) player2 == null) return !Object.Equals(player1, player2);

            return ! (player1.Equals(player2));
        }

        #endregion
        
        
        #region Properties
        public string Name { get; private set; }

        public string Position { get; private set; }

        public List<int> PointsSeries { get; private set; }

        public int CurrentPoints
        {
            get { return PointsSeries[0]; }
        }

        public List<double> PriceSeries { get; private set; }

        public double CurrentPrice
        {
            get { return PriceSeries[0]; }
        }

        public double OperativePrice { get; private set; }

        public List<double> ValueSeries { get; private set; }

        public double OperativeStatistic { get; private set; }

        public double CurrentValue
        {
            get { return ValueSeries[0]; }
        }

        #endregion

    }
}
