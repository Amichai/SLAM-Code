namespace SLAMBot.UI.Forms {
	partial class FakeRobotHost {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.label = new System.Windows.Forms.Label();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.mapDisplay = new SLAMBot.UI.Controls.RoboMap();
			this.SuspendLayout();
			// 
			// label
			// 
			this.label.AutoSize = true;
			this.label.BackColor = System.Drawing.Color.White;
			this.label.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.label.Location = new System.Drawing.Point(0, 642);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(35, 13);
			this.label.TabIndex = 1;
			this.label.Text = "label1";
			// 
			// timer
			// 
			this.timer.Enabled = true;
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// mapDisplay
			// 
			this.mapDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mapDisplay.Location = new System.Drawing.Point(0, 0);
			this.mapDisplay.Name = "mapDisplay";
			this.mapDisplay.Size = new System.Drawing.Size(831, 655);
			this.mapDisplay.TabIndex = 3;
			this.mapDisplay.Text = "roboMap1";
			// 
			// FakeRobotHost
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(831, 655);
			this.Controls.Add(this.label);
			this.Controls.Add(this.mapDisplay);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "FakeRobotHost";
			this.Text = "FakeRobotHost";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label;
		private System.Windows.Forms.Timer timer;
		private Controls.RoboMap mapDisplay;
	}
}