using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SLAMBot.Engine {
	///<summary>Contains extension methods.</summary>
	public static class Extensions {
		static Action Contextify(this Action a) {
			var syncContext = SynchronizationContext.Current;
			if (syncContext == null)
				return a;
			else
				return () => syncContext.Post(_ => a(), null);
		}

		///<summary>Asynchronously navigates a robot.</summary>
		public static void Navigate(this IRobot robot, Navigation.INavigator navigator) {
			if (robot == null) throw new ArgumentNullException("robot");
			if (navigator == null) throw new ArgumentNullException("navigator");

			var iterator = navigator.Execute(robot);

			Action stepRunner = null;
			stepRunner = delegate {
				try {
					if (!iterator.MoveNext())
						iterator.Dispose();
					else
						iterator.Current.Execute(robot, stepRunner);	//StepRunner is captured by reference, so it will use the Contextified version
				} catch {
					iterator.Dispose();
					throw;
				}
			};
			stepRunner = Contextify(stepRunner);	//The parameter is captured by value, so this won't create a recursive loop.
			stepRunner();
		}
	}
}
