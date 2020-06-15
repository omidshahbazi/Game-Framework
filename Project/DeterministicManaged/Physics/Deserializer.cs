// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
using System;

namespace GameFramework.Deterministic.Physics
{
	public static class Deserializer
	{
		public static Scene DeserializeScene(byte[] Data)
		{
			return DeserializeScene(new BufferStream(Data));
		}

		public static Scene DeserializeScene(BufferStream Buffer)
		{
			Scene data = new Scene();

			data.Bodies.Deserialize(ref data.Bodies, Buffer, DeserializeBody);

			return data;
		}

		public static Body DeserializeBody(byte[] Data)
		{
			return DeserializeBody(new BufferStream(Data));
		}

		public static Body DeserializeBody(BufferStream Buffer)
		{
			Body data = new Body();

			data.Position = Buffer.ReadVector3();
			data.Orientation = Buffer.ReadMatrix3();
			data.Mass = Buffer.ReadNumber();
			data.Inertia = Buffer.ReadNumber();
			data.Restitution = Buffer.ReadNumber();
			data.DynamicFriction = Buffer.ReadNumber();
			data.StaticFriction = Buffer.ReadNumber();

			data.Force = Buffer.ReadVector3();
			data.Velocity = Buffer.ReadVector3();
			data.AngularVelocity = Buffer.ReadVector3();
			data.Torque = Buffer.ReadNumber();

			data.Shape = DeserializeShape(Buffer);

			return data;
		}

		public static Shape DeserializeShape(byte[] Data)
		{
			return DeserializeShape(new BufferStream(Data));
		}

		public static Shape DeserializeShape(BufferStream Buffer)
		{
			Shape.Types type = (Shape.Types)Buffer.ReadInt32();

			switch (type)
			{
				case Shape.Types.Sphere:
					return DeserializeSphereShape(Buffer);

				case Shape.Types.Polygon:
					return DeserializePolygonShape(Buffer);
			}

			throw new Exception("Unknow shape type");
		}

		public static SphereShape DeserializeSphereShape(BufferStream Buffer)
		{
			SphereShape data = new SphereShape();

			data.Radius = Buffer.ReadNumber();

			return data;
		}

		public static PolygonShape DeserializePolygonShape(BufferStream Buffer)
		{
			PolygonShape data = new PolygonShape();

			data.Vertices.Deserialize(ref data.Vertices, Buffer);
			data.Normals.Deserialize(ref data.Normals, Buffer);

			return data;
		}
	}
}