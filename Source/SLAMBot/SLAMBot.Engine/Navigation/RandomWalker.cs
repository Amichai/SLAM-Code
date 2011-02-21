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
				yield return new TurnAction(90 * rand.Next(4));
				yield return new MotionAction(15 - rand.Next(30));
			}
		}
	}
}
