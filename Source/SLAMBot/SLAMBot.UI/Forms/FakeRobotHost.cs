using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SLAMBot.Engine;

namespace SLAMBot.UI.Forms {
	public partial class FakeRobotHost : Form {
		public FakeRobotHost(EnvironmentMap map) {
			InitializeComponent();

			mapDisplay.Map = map;
			mapDisplay.Robot = Robot = new FakeRobot(map);

			for (int x = mapDisplay.TopLeft.X; x < mapDisplay.BottomRight.X; x++) {
				for (int y = mapDisplay.TopLeft.Y; y < mapDisplay.BottomRight.Y; y++) {
					if (map[x, y] == 0) {
						Robot.Teleport(new Location(x, y));
						goto found;
					}
				}
			}

		found: Robot.Navigate(new Engine.Navigation.RandomWalker());
		}

		public FakeRobot Robot { get; private set; }

		private void timer_Tick(object sender, EventArgs e) {
			Robot.Step();
			mapDisplay.Invalidate();
			label.Text = "Location: " + Robot.KnownX.ToString("0.##") + ", " + Robot.KnownY.ToString("0.##") + "\r\n"
					   + "Heading: " + Robot.KnownHeading.ToString("+0;-0") + "°";
		}
	}
}
