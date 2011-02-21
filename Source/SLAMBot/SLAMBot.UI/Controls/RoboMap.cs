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
			g.FillEllipse(brush, bounds);

			//Draw a rectangle in back of the robot
			var angle = (Robot.KnownHeading % 360);
			if (angle < 0) angle += 360;
			switch ((angle) / 90) {
				case 0:	//Up
					g.FillRectangle(brush, x + 1, y + CellSize / 2, CellSize - 2, CellSize / 2 - 1);
					break;
				case 1:	//Right
					g.FillRectangle(brush, x + 1, y + 1, CellSize / 2 - 1, CellSize - 2);
					break;
				case 2:	//Down
					g.FillRectangle(brush, x + 1, y + 1, CellSize - 2, CellSize / 2 - 1);
					break;
				case 3:	//Left
					g.FillRectangle(brush, x + CellSize / 2, y + 1, CellSize / 2 - 1, CellSize - 2);
					break;
				default:
					throw new InvalidOperationException("Weird heading: " + Robot.KnownHeading);
			}
		}
	}
}
