using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace SLAMBot.Engine {
	///<summary>Stores a map of the robot's environment.</summary>
	///<remarks>
	/// This should be modified to expose IEnumerables of cells 
	/// in whatever fashions are needed (eg, Row(int)).
	///</remarks>
	public class EnvironmentMap {
		class CellCollection : KeyedCollection<Location, MapCell> {
			protected override Location GetKeyForItem(MapCell item) { return item.Location; }
			public MapCell Get(Location location) {
				if (Dictionary == null)
					return this.FirstOrDefault(k => k.Location == location);
				MapCell retVal;
				Dictionary.TryGetValue(location, out retVal);
				return retVal;
			}
		}

		readonly CellCollection cells = new CellCollection();

		//We try to remove 0'd cells
		#region Indexer
		///<summary>Gets or sets the probability that a cell is occupied, between 0 and 1.</summary>
		public double this[int x, int y] {
			get { return this[new Location(x, y)]; }
			set { this[new Location(x, y)] = value; }
		}

		///<summary>Gets or sets the probability that a cell is occupied, between 0 and 1.</summary>
		public double this[Location location] {
			get {
				MapCell cell = cells.Get(location);
				return cell == null ? 0 : cell.Probability;
			}
			set {
				if (value == 0)
					cells.Remove(location);
				else {
					MapCell cell = cells.Get(location);
					if (cell == null)
						cells.Add(new MapCell(location) { Probability = value });
					else
						cell.Probability = value;
				}
			}
		}
		#endregion

		#region Stats
		///<summary>Gets the location of the top-left corner of the map's bounding box.</summary>
		///<remarks>This is an O(n) operation.</remarks>
		public Location TopLeft {
			get {
				int x = int.MaxValue, y = int.MinValue;
				foreach (var cell in cells) {
					if (cell.Probability == 0) continue;
					x = Math.Min(cell.X, x);
					y = Math.Max(cell.Y, y);
				}
				return new Location(x, y);
			}
		}
		///<summary>Gets the location of the bottom-right corner of the map's bounding box.</summary>
		///<remarks>This is an O(n) operation.</remarks>
		public Location BottomRight {
			get {
				int x = int.MinValue, y = int.MaxValue;
				foreach (var cell in cells) {
					if (cell.Probability == 0) continue;
					x = Math.Max(cell.X, x);
					y = Math.Min(cell.Y, y);
				}
				return new Location(x, y);
			}
		}
		#endregion
	}

	///<summary>Represents a single block in a map.</summary>
	class MapCell : INotifyPropertyChanged {
		public MapCell(Location location) {
			Location = location;
		}

		double probability;
		///<summary>Gets or sets the probability that this cell is occupied, between 0 and 1.</summary>
		public double Probability {
			get { return probability; }
			set {
				if (value < 0 || value > 1)
					throw new ArgumentOutOfRangeException("value", "Probability must be between 0 and 1.");
				probability = value;
				OnPropertyChanged("Probability");
			}
		}

		///<summary>Gets the cell's location in the map.</summary>
		public Location Location { get; private set; }
		///<summary>Gets the cell's horizontal location in the map.</summary>
		public int X { get { return Location.X; } }
		///<summary>Gets the cell's vertical location in the map.</summary>
		public int Y { get { return Location.Y; } }

		///<summary>Occurs when a property value is changed.</summary>
		public event PropertyChangedEventHandler PropertyChanged;
		///<summary>Raises the PropertyChanged event.</summary>
		///<param name="name">The name of the property that changed.</param>
		protected virtual void OnPropertyChanged(string name) { OnPropertyChanged(new PropertyChangedEventArgs(name)); }
		///<summary>Raises the PropertyChanged event.</summary>
		///<param name="e">An EventArgs object that provides the event data.</param>
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) {
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}

	}
}
