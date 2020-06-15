// Copyright 2019. All Rights Reserved.

namespace GameFramework.Deterministic
{
	public struct Matrix3
	{
		public static readonly Matrix3 Zero = new Matrix3(new Number[3, 3]);
		public static readonly Matrix3 Identity = new Matrix3(new Number[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } });

		public Number[,] Values;

		public Vector3 EulerAngles
		{
			get
			{
				Number sY = Math.Sqrt((Values[0, 0] * Values[0, 0]) + (Values[1, 0] * Values[1, 0]));

				bool singular = sY < 1e-6; // If

				Vector3 value = Vector3.Zero;
				if (!singular)
				{
					value.X = Math.Atan2(Values[2, 1], Values[2, 2]);
					value.Y = Math.Atan2(-Values[2, 0], sY);
					value.Z = Math.Atan2(Values[1, 0], Values[0, 0]);
				}
				else
				{
					value.X = Math.Atan2(-Values[1, 2], Values[1, 1]);
					value.Y = Math.Atan2(-Values[2, 0], sY);
					value.Z = 0;
				}

				return value;
			}
		}

		public Matrix3(Number[,] Values)
		{
			this.Values = Values;
		}

		public void SetRotation(Vector3 EulerAngles)
		{
			Number sinX = Math.Sin(EulerAngles.X);
			Number cosX = Math.Cos(EulerAngles.X);

			Number sinY = Math.Sin(EulerAngles.Y);
			Number cosY = Math.Cos(EulerAngles.Y);

			Number sinZ = Math.Sin(EulerAngles.Z);
			Number cosZ = Math.Cos(EulerAngles.Z);

			Values[0, 0] = (cosX * cosY * cosZ) - (sinX * sinZ);
			Values[0, 1] = -(cosX * cosY * sinZ) - (sinX * cosZ);
			Values[0, 2] = cosX * sinY;
			Values[1, 0] = (sinX * cosY * cosZ) + (cosX * sinZ);
			Values[1, 1] = -(sinX * cosY * sinZ) + (cosX * cosZ);
			Values[1, 2] = sinX * sinY;
			Values[2, 0] = -sinY * cosZ;
			Values[2, 1] = sinY * sinZ;
			Values[2, 2] = cosY;
		}

		public Matrix3 Abs()
		{
			Matrix3 mat = new Matrix3();
			mat.Values = new Number[3, 3];

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
			mat.Values = new Number[3, 3];

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
