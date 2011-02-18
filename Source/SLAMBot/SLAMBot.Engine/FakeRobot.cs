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

		public void Rotate(int degrees, Action callback) {
			if (motionAction != null)
				throw new InvalidOperationException("Cannot do two things at once");
			motionAction = RotationIterator(degrees, callback);
		}
		public void Move(int steps, Action callback) {
			if (motionAction != null)
				throw new InvalidOperationException("Cannot do two things at once");
			motionAction = MotionIterator(steps, callback);
		}
		public void SonarPing(Action<double> callback) {
			//TODO: Get distance to nearest occupied cell in a 45(?)-degree cone in front of the robot.
		}

		///<summary>The iterator performing the current motion.</summary>
		///<remarks>To support concurrent actions, change this to a List of IEnumerators.</remarks>
		IEnumerator<object> motionAction;

		private IEnumerator<object> RotationIterator(int degrees, Action callback) {
			DateTime lastTime = DateTime.Now;
			int targetAngle = KnownHeading + degrees;
			while (KnownHeading < targetAngle) {
				var now = DateTime.Now;
				var step = now - lastTime;

				if (step > TimeSpan.Zero)
					KnownHeading += Math.Min(1, (int)(step.Ticks / degreeDuration.Ticks));

				lastTime = now;
				yield return null;
			}
		}
		private IEnumerator<object> MotionIterator(int steps, Action callback) {
			//TODO: Trigonometric interpolation
			DateTime lastTime = DateTime.Now;
			while (true) {
				var now = DateTime.Now;
				var step = now - lastTime;

				if (step > TimeSpan.Zero) {
					//TODO: Move in the KnownHeading direction by (step.Ticks / unitDuration.Ticks)
				}
				lastTime = now;
				yield return null;
			}
		}

		///<summary>Runs a single step of the robot's simulation.  This method should be called repeatedly.</summary>
		public void Step() {
			if (motionAction != null && !motionAction.MoveNext()) {
				motionAction.Dispose();	//Important!  This runs finally blocks and using statements.
				motionAction = null;
			}
		}

		///<summary>Gets the robot's heading in degrees, counterclockwise from positive Y.</summary>
		public int KnownHeading { get; private set; }
		///<summary>Gets the robot's X coordinate.</summary>
		public double KnownX { get; private set; }
		///<summary>Gets the robot's Y coordinate.</summary>
		public double KnownY { get; private set; }
	}
}
