using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAMBot.Engine.Navigation {
	///<summary>Implements an algorithm to navigate the robot.</summary>
	public interface INavigator {
		///<summary>Runs the navigation logic.</summary>
		IEnumerator<INavigationAction> Execute(IRobot robot);
	}
}
