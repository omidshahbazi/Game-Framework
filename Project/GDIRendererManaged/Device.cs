﻿// Copyright 2016-2017 ?????????????. All Rights Reserved.
using System.Drawing;

namespace GameFramework.GDIRenderer
{
	class Device : IDevice
	{
		public Graphics Graphics
		{
			get;
			set;
		}

		void IDevice.DrawString(string Value, float X, float Y, Brush Brush, Font Font)
		{
			if (!Graphics.IsVisible(X, Y))
				return;

			Graphics.DrawString(Value, Font, Brush, X, Y);
		}

		void IDevice.DrawTriangle(float X1, float Y1, float X2, float Y2, float X3, float Y3, Pen Pen)
		{
			if (!(
				Graphics.IsVisible(X1, Y1) ||
				Graphics.IsVisible(X2, Y2) ||
				Graphics.IsVisible(X3, Y3)))
				return;

			((IDevice)this).DrawPolygon(Pen, new PointF[] { new PointF(X1, Y1), new PointF(X2, Y2), new PointF(X3, Y3) });
		}

		void IDevice.DrawFillTriangle(float X1, float Y1, float X2, float Y2, float X3, float Y3, Brush Brush)
		{
			if (!(
				Graphics.IsVisible(X1, Y1) ||
				Graphics.IsVisible(X2, Y2) ||
				Graphics.IsVisible(X3, Y3)))
				return;

			((IDevice)this).DrawFillPolygon(Brush, new PointF[] { new PointF(X1, Y1), new PointF(X2, Y2), new PointF(X3, Y3) });
		}

		void IDevice.DrawLine(float X1, float Y1, float X2, float Y2, Pen Pen)
		{
			if (!(
				Graphics.IsVisible(X1, Y1) ||
				Graphics.IsVisible(X2, Y2)))
				return;

			Graphics.DrawLine(Pen, X1, Y1, X2, Y2);
		}

		void IDevice.DrawPolygon(Pen Pen, params PointF[] Points)
		{
			for (int i = 0; i < Points.Length; ++i)
				if (Graphics.IsVisible(Points[i]))
				{
					Graphics.DrawPolygon(Pen, Points);
					break;
				}
		}

		void IDevice.DrawRectangle(float X, float Y, float Width, float Height, Pen Pen)
		{
			if (!Graphics.IsVisible(X, Y, Width, Height))
				return;

			Graphics.DrawRectangle(Pen, X, Y, Width, Height);
		}

		void IDevice.DrawLines(PointF[] Points, Pen Pen)
		{
			for (int i = 0; i < Points.Length; ++i)
				if (Graphics.IsVisible(Points[i]))
				{
					Graphics.DrawLines(Pen, Points);
					break;
				}
		}

		void IDevice.DrawFillPolygon(Brush Brush, params PointF[] Points)
		{
			for (int i = 0; i < Points.Length; ++i)
				if (Graphics.IsVisible(Points[i]))
				{
					Graphics.FillPolygon(Brush, Points);
					break;
				}
		}

		void IDevice.DrawFillRectangle(float X, float Y, float Width, float Height, Brush Brush)
		{
			if (!Graphics.IsVisible(X, Y, Width, Height))
				return;

			Graphics.FillRectangle(Brush, X, Y, Width, Height);
		}

		void IDevice.DrawCircle(float X, float Y, float Radius, Pen Pen)
		{
			if (!Graphics.IsVisible(X, Y, Radius, Radius))
				return;

			Graphics.DrawEllipse(Pen, new RectangleF(X, Y, Radius * 2, Radius * 2));
		}

		void IDevice.DrawFillCircle(float X, float Y, float Radius, Brush Brush)
		{
			if (!Graphics.IsVisible(X, Y, Radius, Radius))
				return;

			Graphics.FillEllipse(Brush, new RectangleF(X, Y, Radius * 2, Radius * 2));
		}

		SizeF IDevice.MeasureString(string Value, Font Font)
		{
			return Graphics.MeasureString(Value, Font);
		}
	}
}
