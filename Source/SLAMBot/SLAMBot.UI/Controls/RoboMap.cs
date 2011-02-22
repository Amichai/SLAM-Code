using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SLAMBot.Engine;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SLAMBot.UI.Controls {
	class RoboMap : EnvironmentDisplay {
		IRobot robot;
		///<summary>Gets or sets the robot displayed by the control.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual IRobot Robot {
			get { return robot; }
			set { robot = value; Invalidate(); }
		}

		protected override void DrawContent(Graphics g) {
			base.DrawContent(g);

			if (Robot != null)
				DrawRobot(g, GetPoint(robot.KnownX, robot.KnownY));
		}

		void DrawRobot(Graphics g, Point loc) {
			var brush = Brushes.Teal;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.RotateTransform(-robot.KnownHeading);		//We draw for straight up - a heading of zero.  However, KH is clockwise.
			g.TranslateTransform(loc.X, loc.Y);

			g.FillEllipse(brush, 1, 1, CellLength - 2, CellLength - 2);
			//Draw a rectangle in back of the robot
			g.FillRectangle(brush, 1, 1 + CellLength / 2, CellLength - 2, CellLength / 2 - 1);

			//Draw a yellow line near the front of the circle portion
			g.DrawLine(Pens.Yellow, CellLength / 3, CellLength / 4, 2 * CellLength / 3, CellLength / 4);

			g.ResetTransform();
		}
	}
}
