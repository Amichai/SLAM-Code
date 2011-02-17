using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAMBot.Engine {
	///<summary>Represents a location in a map.</summary>
	///<remarks>
	/// Unlike the WPF / WinForms Point classes, this is immutable.
	/// It also has a better hashcode.
	/// Plus, this prevents the engine from being coupled to WPF or WinForms.
	///</remarks>
	public struct Location : IEquatable<Location> {
		public int X { get; private set; }
		public int Y { get; private set; }

		public Location(int x, int y) : this() { X = x; Y = y; }

		public override string ToString() { return "(" + X + ", " + Y + ")"; }

		public override bool Equals(object obj) { return obj is Location && Equals((Location)obj); }
		public bool Equals(Location obj) { return X == obj.X && Y == obj.Y; }
		public override int GetHashCode() {
			//http://stackoverflow.com/questions/371328/why-is-it-important-to-override-gethashcode-when-equals-method-is-overriden-in-c/371348#371348
			return (13 * 7 + X.GetHashCode())
				  * 7 + Y.GetHashCode();
		}

		public static bool operator ==(Location a, Location b) { return a.Equals(b); }
		public static bool operator !=(Location a, Location b) { return !(a == b); }
	}
}
