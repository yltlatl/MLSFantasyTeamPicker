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
        { }

        public Player(string name, string position, int points, double price)
        {
            Name = name;
            Position = position;
            Points = points;
            if (Math.Abs(price - 0) < .001) throw new ArgumentException("Price cannot be zero. ");
            Price = price;
            Value = points/price;
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

        public int Points { get; private set; }

        public double Price { get; private set; }

        public double Value { get; private set; }
        #endregion

    }
}
