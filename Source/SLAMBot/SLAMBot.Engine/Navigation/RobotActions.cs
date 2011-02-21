using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAMBot.Engine.Navigation {
	///<summary>An action that rotates the robot in place.</summary>
	public class TurnAction : INavigationAction {
		public static readonly TurnAction Left = new TurnAction(-90);
		public static readonly TurnAction Right = new TurnAction(90);

		public TurnAction(int degrees) { Degrees = degrees; }

		public override string ToString() {
			return "Turn " + Degrees.ToString("+0;-0") + "°";
		}

		public int Degrees { get; private set; }

		public void Execute(IRobot robot, Action callback) {
			robot.Rotate(Degrees, callback);
		}
	}

	///<summary>An action that moves the robot.</summary>
	public class MotionAction : INavigationAction {
		public static readonly TurnAction Left = new TurnAction(-90);
		public static readonly TurnAction Right = new TurnAction(90);

		public MotionAction(int amount) { Amount = amount; }

		public override string ToString() {
			return "Move " + Amount.ToString("+0;-0") + " unites";
		}

		public int Amount { get; private set; }

		public void Execute(IRobot robot, Action callback) {
			robot.Move(Amount, callback);
		}
	}

	///<summary>Causes the robot to Spontaneously self-destruct.</summary>
	///<remarks>This action is only available in the NSA version of the robot.
	///http://en.wikipedia.org/wiki/Halt_and_Catch_Fire </remarks>
	public class ExplodeAction : INavigationAction {
		public static readonly ExplodeAction Instance = new ExplodeAction();
		private ExplodeAction() { }

		public override string ToString() { return "Spontaneously self-destruct"; }

		public void Execute(IRobot robot, Action callback) { throw new SystemException("Robot is self-destructing"); }
		//TODO: Attempt to destroy the hardware
	}
}