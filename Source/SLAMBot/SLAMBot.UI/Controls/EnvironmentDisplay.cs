using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SLAMBot.Engine;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.ComponentModel;

namespace SLAMBot.UI.Controls {
	class EnvironmentDisplay : Control {
		public EnvironmentDisplay() {
			SetStyle(ControlStyles.AllPaintingInWmPaint
				   | ControlStyles.UserPaint
				   | ControlStyles.Opaque
				   | ControlStyles.OptimizedDoubleBuffer
				   | ControlStyles.ResizeRedraw,
					 true);

			ResizeRedraw = true;
		}

		EnvironmentMap map;
		///<summary>Gets or sets the map displayed by the control.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual EnvironmentMap Map {
			get { return map; }
			set { map = value; RecalcLayout(); Invalidate(); }
		}
		protected override void OnLayout(LayoutEventArgs levent) {
			base.OnLayout(levent);
			RecalcLayout();
		}

		#region Layout
		///<summary>Gets the size of a single cell on the control.</summary>
		protected int CellLength { get; private set; }
		///<summary>Gets the point on the control to draw the top left corner of the map.</summary>
		protected Point MapLocation { get; private set; }
		///<summary>Gets the map coordinates of the top-left corner of the map's bounding box.</summary>
		public Location TopLeft { get; private set; }
		///<summary>Gets the map coordinates of the bottom-right corner of the map's bounding box.</summary>
		public Location BottomRight { get; private set; }

		///<summary>Gets the size of a single cell on the control.</summary>
		protected Size CellSize { get { return new Size(CellLength, CellLength); } }

		///<summary>Gets the width in cells of the map's bounding box.</summary>
		protected int MapWidth { get { return BottomRight.X - TopLeft.X + 1; } }
		///<summary>Gets the height in cells of the map's bounding box.</summary>
		protected int MapHeight { get { return BottomRight.Y - TopLeft.Y + 1; } }

		void RecalcLayout() {
			if (Map == null)
				return;

			TopLeft = Map.TopLeft;
			BottomRight = Map.BottomRight;

			//The size of each cell in pixels
			CellLength = Math.Min(ClientSize.Width / MapWidth, ClientSize.Height / MapHeight);

			//Get the location in the control to draw the (centered) map.
			MapLocation = new Point(
				(ClientSize.Width - MapWidth * CellLength) / 2,
				(ClientSize.Height - MapHeight * CellLength) / 2
			);
		}
		///<summary>Gets the point at the upper left corner of the given map location (between TopLeft and BottomRight).</summary>
		public Point GetPoint(double x, double y) {
			return new Point(
				(int)(MapLocation.X + x * CellLength),
				(int)(MapLocation.Y + (MapHeight - y) * CellLength)		//Our Y axis is upside-down
			);
		}
		///<summary>Gets the point at the upper left corner of the given row/column numbers (between 0 and MapWidth/Height).</summary>
		public Point GetPointFromIndex(double x, double y) {
			return GetPoint(TopLeft.X + x, TopLeft.Y + y);
		}
		#endregion

		protected override void OnPaint(PaintEventArgs e) {
			if (Map == null) {
				DrawEmptyMessage(e);
				return;
			}
			e.Graphics.FillRectangle(Brushes.White, ClientRectangle);

			for (int x = TopLeft.X; x < BottomRight.X; x++) {
				for (int y = TopLeft.Y; y < BottomRight.Y; y++) {

					int c = (int)(255 * map[x, y]);
					using (var brush = new SolidBrush(Color.FromArgb(c, c, c)))
						e.Graphics.FillRectangle(brush, new Rectangle(GetPoint(x, y), CellSize));
				}
			}

			DrawContent(e.Graphics);
			base.OnPaint(e);	//Raise the Paint event
			DrawGridLines(e.Graphics);

			//Draw a dot over (0, 0)
			e.Graphics.FillEllipse(Brushes.Green, new Rectangle(GetPoint(0, 0), new Size(1, 1)));
		}
		protected virtual void DrawContent(Graphics g) { }

		private void DrawGridLines(Graphics g) {
			if (CellLength < 10) return;

			for (int x = 0; x <= MapWidth; x++) {
				var lineX = MapLocation.X + x * CellLength;
				g.DrawLine(Pens.DarkGray,
					lineX, MapLocation.Y,
					lineX, MapLocation.Y + MapHeight * CellLength
				);
			}
			for (int y = 0; y <= MapHeight; y++) {
				var lineY = MapLocation.Y + y * CellLength;
				g.DrawLine(Pens.DarkGray,
					MapLocation.X, lineY,
					MapLocation.X + MapWidth * CellLength, lineY
				);
			}
		}

		private void DrawEmptyMessage(PaintEventArgs e) {
			using (var brush = new LinearGradientBrush(ClientRectangle, Color.AliceBlue, Color.DodgerBlue, LinearGradientMode.Vertical)) {
				e.Graphics.FillRectangle(brush, ClientRectangle);
			}
			using (var font = new Font("Segoe UI", 18))
			using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center }) {
				e.Graphics.DrawString("No map selected", font, Brushes.DarkBlue, ClientRectangle, format);
			}
		}
	}
}
