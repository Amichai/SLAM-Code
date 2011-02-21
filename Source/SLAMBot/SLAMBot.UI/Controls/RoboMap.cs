using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SLAMBot.Engine;
using System.Windows.Forms;
using System.Drawing;

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

		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);

			Location topLeft = Map.TopLeft, bottomRight = Map.BottomRight;
			int mapWidth = bottomRight.X - topLeft.X;
			int mapHeight = bottomRight.Y - topLeft.Y;

			if (Robot != null) {
				DrawRobot(e.Graphics,
					(int)(MapLocation.X + (robot.KnownX - topLeft.X) * CellSize),
					(int)(MapLocation.Y + (mapHeight - robot.KnownY - topLeft.Y) * CellSize)	//Our Y axis is upside-down
				);
			}
		}

		void DrawRobot(Graphics g, int x, int y) {
			var bounds = new Rectangle(x + 1, y + 1, CellSize - 2, CellSize - 2);

			var brush = Brushes.Teal;

			g.RotateTransform(-robot.KnownHeading);	//We draw for straight up - a heading of zero.  However, KH is clockwise.
			g.TranslateTransform(x, y);

			g.FillEllipse(brush, 1, 1, CellSize / 2, CellSize / 2);
			//Draw a rectangle in back of the robot
			g.FillRectangle(brush, 1, 1 + CellSize / 2, CellSize - 2, CellSize / 2 - 1);

			//Draw a yellow line near the front of the circle portion
			g.DrawLine(Pens.Yellow, CellSize / 3, CellSize / 4, 2 * CellSize / 3, CellSize / 4);
		}
	}
}
