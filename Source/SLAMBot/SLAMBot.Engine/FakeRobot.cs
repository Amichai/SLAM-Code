using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAMBot.Engine {
	///<summary>An in-memory simulated robot.</summary>
	public class FakeRobot : IRobot {
		///<summary>The maximum probability that a cell can have an still be considered empty.</summary>
		const double MaxEmptyProbability = .3;

		///<summary>Creates a fake robot that plays in the given map.</summary>
		public FakeRobot(EnvironmentMap map) {
			if (map == null) throw new ArgumentNullException("map");
			Map = map;
		}

		///<summary>Gets the map that the robot plays in.</summary>
		public EnvironmentMap Map { get; private set; }

		///<summary>Instantly teleports the robot to an arbitrary location.</summary>
		///<param name="destination">The location to move to.  This location must be unoccupied.</param>
		public void Teleport(Location destination) {
			if (Map[destination] > MaxEmptyProbability)
				throw new ArgumentOutOfRangeException("destination", "Cannot teleport to non-empty location");
			KnownX = destination.X;
			KnownY = destination.Y;
		}

		///<summary>The amount of time taken to rotate a single degree.</summary>
		readonly TimeSpan degreeDuration = TimeSpan.FromMilliseconds(10);
		///<summary>The amount of time taken to move by one unit.</summary>
		readonly TimeSpan unitDuration = TimeSpan.FromSeconds(.3);

		///<summary>Rotates the robot in place.</summary>
		///<param name="degrees">The number of degrees to rotate by.</param>
		///<param name="callback">A callback to call when the rotation completes.</param>
		public void Rotate(int degrees, Action callback) {
			if (motionAction != null)
				throw new InvalidOperationException("Cannot do two things at once");
			motionAction = RotationIterator(degrees, callback);
		}
		///<summary>Moves the robot forwards or backwards.</summary>
		///<param name="steps">The (signed) number of units to move.</param>
		///<param name="callback">A callback to call when the movement completes.</param>
		public void Move(int steps, Action callback) {
			if (motionAction != null)
				throw new InvalidOperationException("Cannot do two things at once");
			motionAction = MotionIterator(steps, callback);
		}
		///<summary>Sends a SONAR ping.</summary>
		public void SonarPing(Action<double> callback) {
			//TODO: Get distance to nearest occupied cell in a 45(?)-degree cone in front of the robot.
		}

		///<summary>The iterator performing the current motion.</summary>
		///<remarks>
		/// This enumerator will be set to the return value of an iterator method, 
		/// and is invoked by the Step() function.
		/// Thus, each time Step() is called, the code in the iterator will continue
		/// executing until the next yield return.
		/// This allows the iterators to be implemented with normal control flow and
		/// other logic, and to "pause" - to wait for the next timer loop - by writing
		/// yeild return (anything).  Currently, the return value is unused, and might
		/// as well be null.
		/// 
		///To support concurrent actions, change this to a List of IEnumerators, and call MoveNext() on each one in Step().</remarks>
		IEnumerator<object> motionAction;

		private IEnumerator<object> RotationIterator(int degrees, Action callback) {
			DateTime lastTime = DateTime.Now;
			int targetAngle = KnownHeading + degrees;
			while (KnownHeading < targetAngle) {		//While we still need to turn a little more.
				var now = DateTime.Now;
				var step = now - lastTime;

				if (step > TimeSpan.Zero)
					KnownHeading += Math.Min(1, (int)(step.Ticks / degreeDuration.Ticks));

				lastTime = now;
				yield return null;
			}
			callback();	//The rotation finished.
		}

		///<summary>A set of functions that round non-integer coordinates to each of the cells that might contain them.</summary>
		///<remarks>These delegates are used with LINQ to get the four surrounding cells of (.5, .5) - 00, 01, 10, and 11.</remarks>
		static Func<double, int>[] coordinateRounders = { d => (int)Math.Floor(d), d => (int)Math.Ceiling(d) };
		private IEnumerator<object> MotionIterator(int steps, Action callback) {
			//Motion is more complicated than rotation - we
			//will not encroach upon an occupied cell.  If 
			//we hit one, we'll stop early.
			//Remember that steps can be negative!

			double remaining = steps;
			DateTime lastTime = DateTime.Now;
			while (Math.Abs(remaining) > double.Epsilon) {	//While we still have somewhere to go...
				var now = DateTime.Now;
				var step = now - lastTime;

				if (step > TimeSpan.Zero) {
					double distance = Math.Sign(remaining)
									* Math.Min(Math.Abs(remaining), (double)step.Ticks / unitDuration.Ticks);	//Don't do integer division
					if (Math.Abs(distance) < double.Epsilon)
						continue;	//Prevent underflow.  I'm not sure if this line is necessary.

					remaining -= distance;
					double newX = KnownX + distance * Math.Cos(TrigometricHeading);
					double newY = KnownY + distance * Math.Sin(TrigometricHeading);

					//Check whether the new position overlaps with an occupied cell
					var overlaps = coordinateRounders.Select(f => f(newX)).SelectMany(x => coordinateRounders.Select(f => new Location(x, f(newY))));
					if (overlaps.Any(p => Map[p] > MaxEmptyProbability))
						break;	//Boom! Crash!  Stop moving immediately, then callback.

					KnownX = newX;
					KnownY = newY;
				}

				lastTime = now;
				yield return null;
			}

			callback();
		}

		///<summary>Runs a single step of the robot's simulation.  This method should be called repeatedly.</summary>
		public void Step() {
			//Run the next chunk of the current iterator.
			//If MoveNext() returns false, the iterator is
			//finished, and should be gotten rid of.
			if (motionAction != null && !motionAction.MoveNext()) {
				motionAction.Dispose();	//Important!  This runs finally blocks and using statements.
				motionAction = null;
			}
		}

		///<summary>Gets the robot's heading in radians, counterclockwise from positive X.</summary>
		///<remarks>This property is consistent with the expectations for trig functions.</remarks>
		public double TrigometricHeading {
			get { return (90 - KnownHeading) * Math.PI / 180; }
			set { KnownHeading = (int)(90 - (value * 180.0 / Math.PI)); }
		}

		///<summary>Gets the robot's heading in degrees, clockwise from positive Y.</summary>
		///<remarks>This property is consistent with the way we think about angles - +90 means a right turn.</remarks>
		public int KnownHeading { get; private set; }

		///<summary>Gets the robot's X coordinate.</summary>
		public double KnownX { get; private set; }
		///<summary>Gets the robot's Y coordinate.</summary>
		public double KnownY { get; private set; }
	}
}
