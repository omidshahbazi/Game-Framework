// Copyright 2019. All Rights Reserved.

namespace GameFramework.Deterministic
{
	public struct Matrix3
	{
		public Number[,] Values;

		public Matrix3(Vector3 Angles)
		{
			Values = new Number[3, 3];

			//Number c = Math.Cos(radians);
			//Number s = Math.Sin(radians);

			//m00 = c; m01 = -s;
			//m10 = s; m11 = c;

			Values[0, 0] = 1;
			Values[1, 1] = 1;
			Values[2, 2] = 1;
		}

		public Matrix3 Abs()
		{
			Matrix3 mat = new Matrix3();

			mat.Values[0, 0] = Math.Abs(Values[0, 0]);
			mat.Values[0, 1] = Math.Abs(Values[0, 1]);
			mat.Values[0, 2] = Math.Abs(Values[0, 2]);
			mat.Values[1, 0] = Math.Abs(Values[1, 0]);
			mat.Values[1, 1] = Math.Abs(Values[1, 1]);
			mat.Values[1, 2] = Math.Abs(Values[1, 2]);
			mat.Values[2, 0] = Math.Abs(Values[2, 0]);
			mat.Values[2, 1] = Math.Abs(Values[2, 1]);
			mat.Values[2, 2] = Math.Abs(Values[2, 2]);

			return mat;
		}

		public Vector3 AxisX()
		{
			return new Vector3(Values[0, 0], Values[1, 0], Values[2, 0]);
		}

		public Vector3 AxisY()
		{
			return new Vector3(Values[0, 1], Values[1, 1], Values[2, 1]);
		}

		public Vector3 AxisZ()
		{
			return new Vector3(Values[0, 2], Values[1, 2], Values[2, 2]);
		}

		public Matrix3 Transpose()
		{
			Matrix3 mat = new Matrix3();

			mat.Values[0, 0] = Values[0, 0];
			mat.Values[0, 1] = Values[1, 0];
			mat.Values[0, 2] = Values[2, 0];
			mat.Values[1, 0] = Values[0, 1];
			mat.Values[1, 1] = Values[1, 1];
			mat.Values[1, 2] = Values[2, 1];
			mat.Values[2, 0] = Values[0, 2];
			mat.Values[2, 1] = Values[1, 2];
			mat.Values[2, 2] = Values[2, 2];

			return mat;
		}

		public static Vector3 operator *(Matrix3 lhs, Vector3 rhs)
		{
			//return new Vector3(lhs.values[0, 0] * rhs.X + lhs.values[0, 1] * rhs.Y, lhs.values[1, 0] * rhs.X + lhs.values[1, 1] * rhs.Y);
			return rhs;
		}

		public static Matrix3 operator *(Matrix3 lhs, Matrix3 rhs)
		{
			// [00 01]  [00 01]
			// [10 11]  [10 11]

			//return new Matrix3(
			//	values[0, 0] * rhs.values[0, 0] + values[0, 1] * rhs.values[1, 0],
			//	values[0, 0] * rhs.values[0, 1] + values[0, 1] * rhs.values[1, 1],
			//	values[1, 0] * rhs.values[0, 0] + values[1, 1] * rhs.values[1, 0],
			//	values[1, 0] * rhs.values[0, 1] + values[1, 1] * rhs.values[1, 1]
			//);

			return rhs;
		}
	}
}
