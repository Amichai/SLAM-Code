using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAMBot.Engine {
	///<summary>Manipulates the robot hardware.</summary>
	///<remarks>These methods should execute asynchronously and call a callback parameter when they finish.</remarks>
	public interface IRobot {
		///<summary>Rotates the robot in place.</summary>
		///<param name="degrees">The number of degrees to rotate by.  Currently restricted to +-90.</param>
		///<param name="callback">A callback to call when the rotation completes.</param>
		void Rotate(int degrees, Action callback);
		///<summary>Moves the robot forwards or backwards.</summary>
		///<param name="steps">The (signed) number of units to move.</param>
		///<param name="callback">A callback to call when the movement completes.</param>
		void Move(int steps, Action callback);

		///<summary>Sends a SONAR ping.</summary>
		void SonarPing(Action<double> callback);

		///<summary>Gets the robot's heading in degrees, counterclockwise from positive Y.</summary>
		int KnownHeading { get; }
		///<summary>Gets the robot's approximate X coordinate.</summary>
		double KnownX { get; }
		///<summary>Gets the robot's approximate Y coordinate.</summary>
		double KnownY { get; }
	}
}
