﻿// Copyright 2016-2017 ?????????????. All Rights Reserved.
using System.Drawing;
using System.Windows.Forms;

namespace GameFramework.GDIRenderer
{
	public class EditorCanvas : Canvas
	{
		private Point lastMousePosition;

		public bool DrawOriginLines
		{
			get;
			set;
		}

		public bool DrawGridLines
		{
			get;
			set;
		}

		protected bool IsPanning
		{
			get;
			set;
		}

		protected override void OnDrawCanvas(IDevice Device)
		{
			PointF min = ScreenToCanvas(PointF.Empty);
			PointF max = ScreenToCanvas(new PointF(Width, Height));

			if (DrawGridLines)
			{
				Pen gridPen = new Pen(Color.Gray, 0.2F);

				float unit = 10;// 100.0F / Zoom;

				for (float x = min.X; x < max.X; x += unit)
					Device.DrawLine(x, min.Y, x, max.Y, gridPen);

				for (float y = min.Y; y < max.Y; y += unit)
					Device.DrawLine(min.X, y, max.X, y, gridPen);
			}

			if (DrawOriginLines)
			{
				Pen originPen = new Pen(Color.Black, 1.0F);

				Device.DrawLine(min.X, 0.0F, max.X, 0.0F, originPen);
				Device.DrawLine(0.0F, min.Y, 0.0F, max.Y, originPen);
			}

			base.OnDrawCanvas(Device);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (e.Button == MouseButtons.Right)
				IsPanning = true;
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			if (e.Button == MouseButtons.Right)
				IsPanning = false;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (IsPanning)
			{
				Pan = new PointF(Pan.X + (e.X - lastMousePosition.X), Pan.Y + (e.Y - lastMousePosition.Y));
				Refresh();
			}

			lastMousePosition = e.Location;
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);

			Zoom += (e.Delta / 1000.0F);

			Refresh();
		}
	}
}