// Copyright 2019. All Rights Reserved.

using System.Runtime.CompilerServices;

namespace GameFramework.Deterministic
{
	public struct Matrix3
	{
		public static readonly Matrix3 Zero = new Matrix3(0, 0, 0, 0, 0, 0, 0, 0, 0);
		public static readonly Matrix3 Identity = new Matrix3(1, 0, 0, 0, 1, 0, 0, 0, 1);

		public Number Value00;
		public Number Value01;
		public Number Value02;
		public Number Value10;
		public Number Value11;
		public Number Value12;
		public Number Value20;
		public Number Value21;
		public Number Value22;

		[CompilerGenerated]
		public Vector3 Angles
		{
			get
			{
				Number sY = Math.Sqrt((Value00 * Value00) + (Value10 * Value10));

				bool singular = sY < 1e-6; // If

				Vector3 value = Vector3.Zero;
				if (singular)
				{
					value.X = Math.Atan2(-Value12, Value11);
					value.Y = Math.Atan2(-Value20, sY);
					value.Z = 0;
				}
				else
				{
					value.X = Math.Atan2(Value21, Value22);
					value.Y = Math.Atan2(-Value20, sY);
					value.Z = Math.Atan2(Value10, Value00);
				}

				return value;
			}
			set
			{
				Number sinX = Math.Sin(value.X);
				Number cosX = Math.Cos(value.X);

				Number sinY = Math.Sin(value.Y);
				Number cosY = Math.Cos(value.Y);

				Number sinZ = Math.Sin(value.Z);
				Number cosZ = Math.Cos(value.Z);

				Value00 = (cosX * cosY * cosZ) - (sinX * sinZ);
				Value01 = -(cosX * cosY * sinZ) - (sinX * cosZ);
				Value02 = cosX * sinY;
				Value10 = (sinX * cosY * cosZ) + (cosX * sinZ);
				Value11 = -(sinX * cosY * sinZ) + (cosX * cosZ);
				Value12 = sinX * sinY;
				Value20 = -sinY * cosZ;
				Value21 = sinY * sinZ;
				Value22 = cosY;
			}
		}

		public Vector3 AxisX
		{
			get { return new Vector3(Value00, Value10, Value20); }
		}

		public Vector3 AxisY
		{
			get { return new Vector3(Value01, Value11, Value21); }
		}

		public Vector3 AxisZ
		{
			get { return new Vector3(Value02, Value12, Value22); }
		}

		public Matrix3 Abs
		{
			get
			{
				Matrix3 mat = Matrix3.Zero;

				mat.Value00 = Math.Abs(Value00);
				mat.Value01 = Math.Abs(Value01);
				mat.Value02 = Math.Abs(Value02);
				mat.Value10 = Math.Abs(Value10);
				mat.Value11 = Math.Abs(Value11);
				mat.Value12 = Math.Abs(Value12);
				mat.Value20 = Math.Abs(Value20);
				mat.Value21 = Math.Abs(Value21);
				mat.Value22 = Math.Abs(Value22);

				return mat;
			}
		}

		public Matrix3 Transposed
		{
			get
			{
				Matrix3 mat = Matrix3.Zero;

				mat.Value00 = Value00;
				mat.Value01 = Value10;
				mat.Value02 = Value20;
				mat.Value10 = Value01;
				mat.Value11 = Value11;
				mat.Value12 = Value21;
				mat.Value20 = Value02;
				mat.Value21 = Value12;
				mat.Value22 = Value22;

				return mat;
			}
		}

		public Matrix3(Number Value00, Number Value01, Number Value02, Number Value10, Number Value11, Number Value12, Number Value20, Number Value21, Number Value22)
		{
			this.Value00 = Value00;
			this.Value01 = Value01;
			this.Value02 = Value02;
			this.Value10 = Value10;
			this.Value11 = Value11;
			this.Value12 = Value12;
			this.Value20 = Value20;
			this.Value21 = Value21;
			this.Value22 = Value22;
		}

		public static Vector3 operator *(Matrix3 Left, Vector3 Right)
		{
			Vector3 result = Vector3.Zero;

			result.X =
				Right.X * Left.Value00 +
				Right.Y * Left.Value10 +
				Right.Z * Left.Value20;

			result.Y =
				Right.X * Left.Value01 +
				Right.Y * Left.Value11 +
				Right.Z * Left.Value21;

			result.Z =
				Right.X * Left.Value02 +
				Right.Y * Left.Value12 +
				Right.Z * Left.Value22;

			return result;
		}

		public static Matrix3 operator *(Matrix3 Left, Matrix3 Right)
		{
			Matrix3 result = Matrix3.Zero;

			//for (int i = 0; i < 3; ++i)
			//	for (int j = 0; j < 3; ++j)
			//	{
			//		result.Values[i, j] =
			//			Left.Values[i, 0] * Right.Values[0, j] +
			//			Left.Values[i, 1] * Right.Values[1, j] +
			//			Left.Values[i, 2] * Right.Values[2, j];
			//	}

			result.Value00 = Left.Value00 * Right.Value00 + Left.Value01 * Right.Value10 + Left.Value02 * Right.Value20;
			result.Value01 = Left.Value00 * Right.Value01 + Left.Value01 * Right.Value11 + Left.Value02 * Right.Value21;
			result.Value02 = Left.Value00 * Right.Value02 + Left.Value01 * Right.Value12 + Left.Value02 * Right.Value22;

			result.Value10 = Left.Value10 * Right.Value00 + Left.Value11 * Right.Value10 + Left.Value12 * Right.Value20;
			result.Value11 = Left.Value10 * Right.Value01 + Left.Value11 * Right.Value11 + Left.Value12 * Right.Value21;
			result.Value12 = Left.Value10 * Right.Value02 + Left.Value11 * Right.Value12 + Left.Value12 * Right.Value22;

			result.Value20 = Left.Value20 * Right.Value00 + Left.Value21 * Right.Value10 + Left.Value22 * Right.Value20;
			result.Value21 = Left.Value20 * Right.Value01 + Left.Value21 * Right.Value11 + Left.Value22 * Right.Value21;
			result.Value22 = Left.Value20 * Right.Value02 + Left.Value21 * Right.Value12 + Left.Value22 * Right.Value22;

			return result;
		}
	}
}
