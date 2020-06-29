// Copyright 2019. All Rights Reserved.

using System.Runtime.CompilerServices;

namespace GameFramework.Deterministic
{
	public struct Matrix2
	{
		public static readonly Matrix2 Zero = new Matrix2(0, 0, 0, 0);
		public static readonly Matrix2 Identity = new Matrix2(1, 0, 0, 1);

		public Number Value00;
		public Number Value01;
		public Number Value10;
		public Number Value11;

		[CompilerGenerated]
		public Number Rotation
		{
			get
			{
				Number sY = Math.Sqrt((Value00 * Value00) + (Value10 * Value10));

				if (sY > 1e-6)
					return Math.Atan2(Value10, Value00);

				return 0;
			}
			set
			{
				Number angle = (value % Math.TwoPI);

				if (angle < 0)
					angle = Math.TwoPI + angle;

				Number sin = Math.Sin(angle);
				Number cos = Math.Cos(angle);

				Value00 = cos;
				Value01 = -sin;
				Value10 = sin;
				Value11 = cos;
			}
		}

		public Vector2 AxisX
		{
			get { return new Vector2(Value00, Value10); }
		}

		public Vector2 AxisY
		{
			get { return new Vector2(Value01, Value11); }
		}

		public Matrix2 Abs
		{
			get
			{
				Matrix2 mat = Matrix2.Zero;

				mat.Value00 = Math.Abs(Value00);
				mat.Value01 = Math.Abs(Value01);
				mat.Value10 = Math.Abs(Value10);
				mat.Value11 = Math.Abs(Value11);

				return mat;
			}
		}

		public Matrix2 Transposed
		{
			get
			{
				Matrix2 mat = Matrix2.Zero;

				mat.Value00 = Value00;
				mat.Value01 = Value10;
				mat.Value10 = Value01;
				mat.Value11 = Value11;

				return mat;
			}
		}

		public Matrix2(Number Value00, Number Value01, Number Value10, Number Value11)
		{
			this.Value00 = Value00;
			this.Value01 = Value01;
			this.Value10 = Value10;
			this.Value11 = Value11;
		}

		public static Vector2 operator *(Matrix2 Left, Vector2 Right)
		{
			Vector2 result = Vector2.Zero;

			result.X =
				Right.X * Left.Value00 +
				Right.Y * Left.Value10;

			result.Y =
				Right.X * Left.Value01 +
				Right.Y * Left.Value11;

			return result;
		}

		public static Matrix2 operator *(Matrix2 Left, Matrix2 Right)
		{
			Matrix2 result = Matrix2.Zero;

			//for (int i = 0; i < 2; ++i)
			//	for (int j = 0; j < 2; ++j)
			//	{
			//		result.Values[i, j] =
			//			Left.Values[i, 0] * Right.Values[0, j] +
			//			Left.Values[i, 1] * Right.Values[1, j];
			//	}

			result.Value00 = Left.Value00 * Right.Value00 + Left.Value01 * Right.Value10;
			result.Value01 = Left.Value00 * Right.Value01 + Left.Value01 * Right.Value11;

			result.Value10 = Left.Value10 * Right.Value00 + Left.Value11 * Right.Value10;
			result.Value11 = Left.Value10 * Right.Value01 + Left.Value11 * Right.Value11;

			return result;
		}
	}
}
