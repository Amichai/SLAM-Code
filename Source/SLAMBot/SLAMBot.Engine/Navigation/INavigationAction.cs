using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SLAMBot.Engine.Navigation {
	///<summary>An action yielded by an iterator to navigate a robot.</summary>
	public interface INavigationAction {
		///<summary>Executes the action.</summary>
		void Execute(IRobot robot, Action callback);

		///<summary>Returns a human-readable description of this action, in verb form.</summary>
		string ToString();
	}

	///<summary>An action that pauses for a specified amount of time.</summary>
	public class WaitAction : INavigationAction {
		public WaitAction(TimeSpan duration) { Duration = duration; }
		public TimeSpan Duration { get; private set; }

		public override string ToString() {
			return "Wait " + Duration.TotalSeconds + " second" + (Duration.TotalSeconds == 1 ? "s" : "");
		}

		public void Execute(IRobot robot, Action callback) {
			if (Duration <= TimeSpan.Zero)
				callback();
			else
				ThreadPool.QueueUserWorkItem(delegate {
					Thread.Sleep(Duration);
					callback();
				});
		}
	}
}
