// Copyright 2019. All Rights Reserved.

using System.Runtime.CompilerServices;

namespace GameFramework.Deterministic
{
	public struct Matrix3
	{
		public static readonly Matrix3 Zero = new Matrix3(new Number[3, 3]);
		public static readonly Matrix3 Identity = new Matrix3(new Number[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } });

		public Number[,] Values;

		[CompilerGenerated]
		public Vector3 Angles
		{
			get
			{
				Number sY = Math.Sqrt((Values[0, 0] * Values[0, 0]) + (Values[1, 0] * Values[1, 0]));

				bool singular = sY < 1e-6; // If

				Vector3 value = Vector3.Zero;
				if (singular)
				{
					value.X = Math.Atan2(-Values[1, 2], Values[1, 1]);
					value.Y = Math.Atan2(-Values[2, 0], sY);
					value.Z = 0;
				}
				else
				{
					value.X = Math.Atan2(Values[2, 1], Values[2, 2]);
					value.Y = Math.Atan2(-Values[2, 0], sY);
					value.Z = Math.Atan2(Values[1, 0], Values[0, 0]);
				}

				return value;
			}
		}

		public Vector3 AxisX
		{
			get { return new Vector3(Values[0, 0], Values[1, 0], Values[2, 0]); }
		}

		public Vector3 AxisY
		{
			get { return new Vector3(Values[0, 1], Values[1, 1], Values[2, 1]); }
		}

		public Vector3 AxisZ
		{
			get { return new Vector3(Values[0, 2], Values[1, 2], Values[2, 2]); }
		}

		public Matrix3 Abs
		{
			get
			{
				Matrix3 mat = Matrix3.Zero;

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
		}

		public Matrix3 Transposed
		{
			get
			{
				Matrix3 mat = Matrix3.Zero;

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

		public static Vector3 operator *(Matrix3 Left, Vector3 Right)
		{
			Vector3 result = Vector3.Zero;

			result.X =
				Right.X * Left.Values[0, 0] +
				Right.Y * Left.Values[1, 0] +
				Right.Z * Left.Values[2, 0];

			result.Y =
				Right.X * Left.Values[0, 1] +
				Right.Y * Left.Values[1, 1] +
				Right.Z * Left.Values[2, 1];

			result.Z =
				Right.X * Left.Values[0, 2] +
				Right.Y * Left.Values[1, 2] +
				Right.Z * Left.Values[2, 2];

			return result;
		}

		public static Matrix3 operator *(Matrix3 Left, Matrix3 Right)
		{
			Matrix3 result = Matrix3.Zero;

			for (int i = 0; i < 3; ++i)
				for (int j = 0; j < 3; ++j)
				{
					result.Values[i, j] =
						Left.Values[i, 0] * Right.Values[0, j] +
						Left.Values[i, 1] * Right.Values[1, j] +
						Left.Values[i, 2] * Right.Values[2, j];
				}

			return result;
		}
	}
}
