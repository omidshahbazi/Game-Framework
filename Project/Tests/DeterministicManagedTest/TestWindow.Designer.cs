namespace DeterministicTest
{
	partial class TestWindow
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestWindow));
			this.editorCanvas1 = new GameFramework.GDIRenderer.EditorCanvas();
			this.SuspendLayout();
			// 
			// editorCanvas1
			// 
			this.editorCanvas1.BackColor = System.Drawing.Color.DimGray;
			this.editorCanvas1.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
			this.editorCanvas1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.editorCanvas1.DrawGridLines = false;
			this.editorCanvas1.DrawOriginLines = false;
			this.editorCanvas1.FlipY = true;
			this.editorCanvas1.GraphicsUnit = System.Drawing.GraphicsUnit.Pixel;
			this.editorCanvas1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
			this.editorCanvas1.Location = new System.Drawing.Point(0, 0);
			this.editorCanvas1.MaximumZoom = 1F;
			this.editorCanvas1.MinimumZoom = 1F;
			this.editorCanvas1.Name = "editorCanvas1";
			this.editorCanvas1.Origin = new System.Drawing.Point(0, 0);
			this.editorCanvas1.Pan = ((System.Drawing.PointF)(resources.GetObject("editorCanvas1.Pan")));
			this.editorCanvas1.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Default;
			this.editorCanvas1.Size = new System.Drawing.Size(784, 561);
			this.editorCanvas1.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
			this.editorCanvas1.TabIndex = 0;
			this.editorCanvas1.TextContrast = 0;
			this.editorCanvas1.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
			this.editorCanvas1.Zoom = 1F;
			this.editorCanvas1.DrawCanvas += new GameFramework.GDIRenderer.Canvas.DrawCanvasHandler(this.editorCanvas1_DrawCanvas);
			this.editorCanvas1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.editorCanvas1_MouseUp);
			// 
			// TestWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 561);
			this.Controls.Add(this.editorCanvas1);
			this.Name = "TestWindow";
			this.Text = "DeterministicManagedTest";
			this.ResumeLayout(false);

		}

		#endregion

		private GameFramework.GDIRenderer.EditorCanvas editorCanvas1;
	}
}