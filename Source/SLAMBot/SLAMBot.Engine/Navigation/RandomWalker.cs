using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAMBot.Engine.Navigation {
	///<summary>Navigates the robot in a random walk.</summary>
	public class RandomWalker : INavigator {
		readonly Random rand = new Random();

		public IEnumerator<INavigationAction> Execute(IRobot robot) {
			while (true) {
				yield return new TurnAction(90 * (1 - rand.Next(2)));
				yield return new MotionAction(rand.Next(10));
			}
		}
	}
}
