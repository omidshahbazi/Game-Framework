// Copyright 2019. All Rights Reserved.

using System.Runtime.CompilerServices;

namespace GameFramework.Deterministic
{
	public struct Matrix2
	{
		public static readonly Matrix2 Zero = new Matrix2(new Number[2, 2]);
		public static readonly Matrix2 Identity = new Matrix2(new Number[,] { { 1, 0 }, { 0, 1 } });

		public Number[,] Values;

		[CompilerGenerated]
		public Number Angle
		{
			get
			{
				Number sY = Math.Sqrt((Values[0, 0] * Values[0, 0]) + (Values[1, 0] * Values[1, 0]));

				if (sY > 1e-6)
					return Math.Atan2(Values[1, 0], Values[0, 0]);

				return 0;
			}
		}

		public Vector2 AxisX
		{
			get { return new Vector2(Values[0, 0], Values[1, 0]); }
		}

		public Vector2 AxisY
		{
			get { return new Vector2(Values[0, 1], Values[1, 1]); }
		}

		public Matrix2 Abs
		{
			get
			{
				Matrix2 mat = Matrix2.Zero;

				mat.Values[0, 0] = Math.Abs(Values[0, 0]);
				mat.Values[0, 1] = Math.Abs(Values[0, 1]);
				mat.Values[1, 0] = Math.Abs(Values[1, 0]);
				mat.Values[1, 1] = Math.Abs(Values[1, 1]);

				return mat;
			}
		}

		public Matrix2 Transposed
		{
			get
			{
				Matrix2 mat = Matrix2.Zero;

				mat.Values[0, 0] = Values[0, 0];
				mat.Values[0, 1] = Values[1, 0];
				mat.Values[1, 0] = Values[0, 1];
				mat.Values[1, 1] = Values[1, 1];

				return mat;
			}
		}

		public Matrix2(Number[,] Values)
		{
			this.Values = Values;
		}

		public void SetRotation(Number Angle)
		{
			Number sin = Math.Sin(Angle);
			Number cos = Math.Cos(Angle);

			Values[0, 0] = cos;
			Values[0, 1] = -sin;
			Values[1, 0] = sin;
			Values[1, 1] = cos;
		}

		public static Vector2 operator *(Matrix2 Left, Vector2 Right)
		{
			Vector2 result = Vector2.Zero;

			result.X =
				Right.X * Left.Values[0, 0] +
				Right.Y * Left.Values[1, 0];

			result.Y =
				Right.X * Left.Values[0, 1] +
				Right.Y * Left.Values[1, 1];

			return result;
		}

		public static Matrix2 operator *(Matrix2 Left, Matrix2 Right)
		{
			Matrix2 result = Matrix2.Zero;

			for (int i = 0; i < 2; ++i)
				for (int j = 0; j < 2; ++j)
				{
					result.Values[i, j] =
						Left.Values[i, 0] * Right.Values[0, j] +
						Left.Values[i, 1] * Right.Values[1, j];
				}

			return result;
		}
	}
}
