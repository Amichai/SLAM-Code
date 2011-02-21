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
			set { map = value; Invalidate(); }
		}

		///<summary>Gets the size of a single cell on the control.</summary>
		protected int CellSize { get; private set; }
		///<summary>Gets the location on the control to draw the top left corner of the map.</summary>
		protected Point MapOrigin { get; private set; }

		protected override void OnLayout(LayoutEventArgs levent) {
			base.OnLayout(levent);

			if (Map == null)
				return;
			Location topLeft = Map.TopLeft, bottomRight = Map.BottomRight;
			int mapWidth = bottomRight.X - topLeft.X;
			int mapHeight = bottomRight.Y - topLeft.Y;

			//The size of each cell in pixels
			CellSize = Math.Min(ClientSize.Width / mapWidth, ClientSize.Height / mapHeight);

			//Get the location in the control to draw the (centered) map.
			MapOrigin = new Point(
				(ClientSize.Width - mapWidth * CellSize) / 2,
				(ClientSize.Height - mapHeight * CellSize) / 2
			);
		}

		protected override void OnPaint(PaintEventArgs e) {
			if (Map == null) {
				DrawEmptyMessage(e);
				return;
			}

			Location topLeft = Map.TopLeft, bottomRight = Map.BottomRight;
			int mapWidth = bottomRight.X - topLeft.X;
			int mapHeight = bottomRight.Y - topLeft.Y;


			for (int x = 0; x < mapWidth; x++) {
				for (int y = 0; y < mapHeight; y++) {
					Rectangle cell = new Rectangle(
						MapOrigin.X + x * CellSize,
						MapOrigin.Y + y *CellSize,	
						CellSize, CellSize
					);

					int c = (int)(255 * map[topLeft.X+ x,topLeft.Y+ mapHeight-y]);	//Our Y axis is upside-down
					using (var brush = new SolidBrush(Color.FromArgb(c, c, c)))
						e.Graphics.FillRectangle(brush, cell);
				}
			}

			DrawGridLines(e.Graphics);
			var originCenter = MapOrigin.X
		}

		private void DrawGridLines(Graphics g) {
			if (CellSize < 10) return;
			Location topLeft = Map.TopLeft, bottomRight = Map.BottomRight;
			int mapWidth = bottomRight.X - topLeft.X;
			int mapHeight = bottomRight.Y - topLeft.Y;

			for (int x = 0; x <= mapWidth; x++) {
				var lineX = MapOrigin.X + x * CellSize;
				g.DrawLine(Pens.DarkGray,
					lineX, MapOrigin.Y,
					lineX, MapOrigin.Y + mapHeight * CellSize
				);
			}
			for (int y = 0; y <= mapHeight; y++) {
				var lineY = MapOrigin.Y + y * CellSize;
				g.DrawLine(Pens.DarkGray,
					MapOrigin.X, lineY,
					MapOrigin.X + mapWidth * CellSize, lineY
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
