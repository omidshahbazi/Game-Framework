// Copyright 2019. All Rights Reserved.
using GameFramework.Common.Extensions;
using System;

namespace GameFramework.Deterministic.Physics
{
	public static class Simulation
	{
		public class Config
		{
			public Number StepTime;
			public Vector3 Gravity = new Vector3(0, -9.8F, 0);
			public int CCDStepCount = 10;
		}

		private static readonly Action<Manifold>[,] ManifoldDispatchers = new Action<Manifold>[,] { { DispatchSphereToSphere, DispatchSphereToPolygon }, { DispatchPolygonToSphere, DispatchPolygonToPolygon } };

		public static void Simulate(Scene Scene, Config Config, ContactList Contacts)
		{
			Simulate(Scene.Bodies, Config, Contacts);
		}

		public static void Simulate(Body[] Bodies, Config Config, ContactList Contacts)
		{
			for (int i = 0; i < Bodies.Length; ++i)
			{
				Body a = Bodies[i];

				for (int j = i + 1; j < Bodies.Length; ++j)
				{
					Body b = Bodies[j];

					if (a.Mass == 0 && b.Mass == 0)
						continue;

					Manifold manifold = new Manifold() { BodyA = a, BodyB = b };

					DispatchManifold(manifold);

					if (manifold.Points != null && manifold.Points.Length != 0)
						Contacts.Add(manifold);
				}
			}

			for (int i = 0; i < Bodies.Length; ++i)
				IntegrateForces(Bodies[i], Config);

			for (int i = 0; i < Contacts.Count; ++i)
				InitializeManifold((Manifold)Contacts[i], Config);

			for (int i = 0; i < Config.CCDStepCount; ++i)
				for (int j = 0; j < Contacts.Count; ++j)
					SolveManifold((Manifold)Contacts[j]);

			for (int i = 0; i < Bodies.Length; ++i)
				IntegrateVelocity(Bodies[i], Config);

			for (int i = 0; i < Contacts.Count; ++i)
				CorrectPosition((Manifold)Contacts[i]);

			for (int i = 0; i < Bodies.Length; ++i)
			{
				Body body = Bodies[i];

				body.Force = Vector3.Zero;
				body.Torque = 0;
			}
		}

		public static void ApplyForce(Body Body, Vector3 Force)
		{
			Body.Force += Force;
		}

		public static void ApplyImpulse(Body Body, Vector3 Impulse, Vector3 ContactDirection)
		{
			Number invMass = (Body.Mass == 0 ? (Number)0 : 1 / Body.Mass);
			Number invInertia = (Body.Inertia == 0 ? (Number)0 : 1 / Body.Inertia);

			Body.Velocity += Impulse * invMass;
			Body.AngularVelocity += ContactDirection * Impulse * invInertia;

			if (Body.Velocity.Magnitude >= 1000)
			{
				int a = 1;
			}
		}

		private static void IntegrateForces(Body Body, Config Config)
		{
			if (Body.Mass == 0)
				return;

			Number invInertia = (Body.Inertia == 0 ? (Number)0 : 1 / Body.Inertia);

			Body.Velocity += (Body.Force / Body.Mass + Config.Gravity) * (Config.StepTime / 2.0f);
			Body.AngularVelocity += Body.Torque * invInertia * (Config.StepTime / 2.0F);

			if (Body.Velocity.Magnitude >= 1000)
			{
				int a = 1;
			}
		}

		private static void DispatchManifold(Manifold Manifold)
		{
			//bool aIsSphere = (Manifold.BodyA.Shape is SphereShape);
			//bool bIsSphere = (Manifold.BodyB.Shape is SphereShape);

			//if (aIsSphere)
			//{
			//	if (bIsSphere)
			//		DispatchSphereToSphere(Manifold);
			//	else
			//		DispatchSphereToPolygon(Manifold);
			//}
			//else
			//{
			//	if (bIsSphere)
			//		DispatchPolygonToSphere(Manifold);
			//	else
			//		DispatchPolygonToPolygon(Manifold);
			//}

			ManifoldDispatchers[(int)Manifold.BodyA.Shape.GetType(), (int)Manifold.BodyB.Shape.GetType()](Manifold);
		}

		private static void InitializeManifold(Manifold Manifold, Config Config)
		{
			// Calculate average restitution
			Manifold.MixedRestitution = Math.Min(Manifold.BodyA.Restitution, Manifold.BodyB.Restitution);

			// Calculate static and dynamic friction
			Manifold.MixedStaticFriction = Math.Sqrt(Manifold.BodyA.StaticFriction * Manifold.BodyB.StaticFriction);
			Manifold.MixedDynamicFriction = Math.Sqrt(Manifold.BodyA.DynamicFriction * Manifold.BodyB.DynamicFriction);

			for (uint i = 0; i < Manifold.Points.Length; ++i)
			{
				// Calculate radii from COM to contact
				Vector3 ra = Manifold.Points[i] - Manifold.BodyA.Position;
				Vector3 rb = Manifold.Points[i] - Manifold.BodyB.Position;

				Vector3 rv = Manifold.BodyB.Velocity + (Manifold.BodyB.AngularVelocity * rb) - Manifold.BodyA.Velocity - (Manifold.BodyA.AngularVelocity * ra);

				// Determine if we should perform a resting collision or not
				// The idea is if the only thing moving this object is gravity,
				// then the collision should be performed without any restitution
				if (rv.SqrMagnitude < (Config.Gravity * Config.StepTime).SqrMagnitude + Math.Epsilon)
					Manifold.MixedRestitution = 0.0F;
			}
		}

		private static void SolveManifold(Manifold Manifold)
		{
			// Early out and positional correct if both objects have infinite mass
			if (Manifold.BodyA.Mass + Manifold.BodyB.Mass == 0)
			{
				Manifold.BodyA.Velocity = Vector3.Zero;
				Manifold.BodyB.Velocity = Vector3.Zero;

				return;
			}

			for (uint i = 0; i < Manifold.Points.Length; ++i)
			{
				// Calculate radii from COM to contact
				Vector3 rA = Manifold.Points[i] - Manifold.BodyA.Position;
				Vector3 rB = Manifold.Points[i] - Manifold.BodyB.Position;

				// Relative velocity
				Vector3 rv = Manifold.BodyB.Velocity + (Manifold.BodyB.AngularVelocity * rB) - Manifold.BodyA.Velocity - (Manifold.BodyA.AngularVelocity * rA);

				// Relative velocity along the normal
				Number contactVel = rv.Dot(Manifold.Normal);

				// Do not resolve if velocities are separating
				if (contactVel > 0)
					return;

				Number invMassA = (Manifold.BodyA.Mass == 0 ? (Number)0 : 1 / Manifold.BodyA.Mass);
				Number invMassB = (Manifold.BodyB.Mass == 0 ? (Number)0 : 1 / Manifold.BodyB.Mass);

				Number invInertiaA = (Manifold.BodyA.Inertia == 0 ? (Number)0 : 1 / Manifold.BodyA.Inertia);
				Number invInertiaB = (Manifold.BodyB.Inertia == 0 ? (Number)0 : 1 / Manifold.BodyB.Inertia);

				Vector3 rACrossN = rA * Manifold.Normal;
				Vector3 rBCrossN = rB * Manifold.Normal;
				Number invMassSum = ((rACrossN * rACrossN) * invInertiaA + (rBCrossN * rBCrossN) * invInertiaB + invMassA + invMassB).Magnitude;
				invMassSum = Math.Max(1, invMassSum);

				// Calculate impulse scalar
				Number j = -(1.0f + Manifold.MixedRestitution) * contactVel;
				j /= invMassSum;
				j /= Manifold.Points.Length;

				// Apply impulse
				Vector3 impulse = Manifold.Normal * j;
				ApplyImpulse(Manifold.BodyA, -impulse, rA);
				ApplyImpulse(Manifold.BodyB, impulse, rB);

				// Friction impulse
				rv = Manifold.BodyB.Velocity + (Manifold.BodyB.AngularVelocity * rB) - Manifold.BodyA.Velocity - (Manifold.BodyA.AngularVelocity * rA);

				Vector3 t = rv - (Manifold.Normal * rv.Dot(Manifold.Normal));
				t.Normalize();

				// j tangent magnitude
				Number jt = -rv.Dot(t);
				jt /= invMassSum;
				jt /= (Number)Manifold.Points.Length;

				// Don't apply tiny friction impulses
				if (jt == 0)
					return;

				// Coulumb's law
				Vector3 tangentImpulse;
				if (Math.Abs(jt) < j * Manifold.MixedStaticFriction)
					tangentImpulse = t * jt;
				else
					tangentImpulse = t * -j * Manifold.MixedDynamicFriction;

				// Apply friction impulse
				ApplyImpulse(Manifold.BodyA, -tangentImpulse, rA);
				ApplyImpulse(Manifold.BodyB, tangentImpulse, rB);
			}
		}

		private static void CorrectPosition(Manifold Manifold)
		{
			Number Slope = 0.05F; // Penetration allowance
			Number Percent = 0.4F; // Penetration percentage to correct

			Number invMassA = (Manifold.BodyA.Mass == 0 ? (Number)0 : 1 / Manifold.BodyA.Mass);
			Number invMassB = (Manifold.BodyB.Mass == 0 ? (Number)0 : 1 / Manifold.BodyB.Mass);

			Vector3 correction = Manifold.Normal * (Math.Max(Manifold.Penetration - Slope, 0.0F) / (invMassA + invMassB)) * Percent;

			Manifold.BodyA.Position -= correction * invMassA;
			Manifold.BodyB.Position += correction * invMassB;
		}

		private static void IntegrateVelocity(Body Body, Config Config)
		{
			if (Body.Mass == 0)
				return;

			Body.Position += Body.Velocity * Config.StepTime;

			Matrix3 rot = Matrix3.Zero;
			rot.SetRotation(Body.AngularVelocity * Config.StepTime);
			Body.Orientation *= rot;

			IntegrateForces(Body, Config);
		}

		private static void DispatchSphereToSphere(Manifold Manifold)
		{
			SphereShape a = (SphereShape)Manifold.BodyA.Shape;
			SphereShape b = (SphereShape)Manifold.BodyB.Shape;

			// Calculate translational vector, which is normal
			Vector3 normal = Manifold.BodyB.Position - Manifold.BodyA.Position;

			Number distanceSqr = normal.SqrMagnitude;
			Number radius = a.Radius + b.Radius;

			// Not in contact
			if (distanceSqr >= radius * radius)
			{
				Manifold.Points = null;

				return;
			}

			Number distance = Math.Sqrt(distanceSqr);

			Manifold.Points = new Vector3[1];

			if (distance == 0)
			{
				Manifold.Penetration = a.Radius;
				Manifold.Normal = Vector3.Right;
				Manifold.Points[0] = Manifold.BodyA.Position;

				return;
			}

			Manifold.Penetration = radius - distance;
			Manifold.Normal = normal / distance; // Faster than using Normalized since we already performed sqrt

			Manifold.Points[0] = Manifold.Normal * a.Radius + Manifold.BodyA.Position;
		}

		private static void DispatchSphereToPolygon(Manifold Manifold)
		{
			SphereShape a = (SphereShape)Manifold.BodyA.Shape;
			PolygonShape b = (PolygonShape)Manifold.BodyB.Shape;

			Matrix3 orientationB = Manifold.BodyB.Orientation;

			Manifold.Points = null;

			// Transform SphereShape center to Polygon model space
			Vector3 center = Manifold.BodyA.Position;
			center = orientationB.Transpose() * (center - Manifold.BodyB.Position);

			// Find edge with minimum penetration
			// Exact concept as using support points in Polygon vs Polygon
			Number separation = Number.MinValue;
			uint faceNormal = 0;
			for (uint i = 0; i < b.Vertices.Length; ++i)
			{
				Number s = b.Normals[i].Dot(center - b.Vertices[i]);

				if (s > a.Radius)
					return;

				if (s > separation)
				{
					separation = s;
					faceNormal = i;
				}
			}

			// Grab face's vertices
			Vector3 v1 = b.Vertices[faceNormal];
			uint i2 = faceNormal + 1 < b.Vertices.Length ? faceNormal + 1 : 0;
			Vector3 v2 = b.Vertices[i2];

			// Check to see if center is within polygon
			if (separation < Math.Epsilon)
			{
				Manifold.Normal = -(orientationB * b.Normals[faceNormal]);
				Manifold.Points = new Vector3[] { Manifold.Normal * a.Radius + Manifold.BodyA.Position };
				Manifold.Penetration = a.Radius;
				return;
			}

			// Determine which voronoi region of the edge center of SphereShape lies within
			Number dot1 = (center - v1).Dot(v2 - v1);
			Number dot2 = (center - v2).Dot(v1 - v2);
			Manifold.Penetration = a.Radius - separation;

			if (dot1 <= 0) // Closest to v1
			{
				if ((center - v1).SqrMagnitude > a.Radius * a.Radius)
					return;

				Vector3 n = v1 - center;
				n = orientationB * n;
				n.Normalize();
				Manifold.Normal = n;
				v1 = orientationB * v1 + Manifold.BodyB.Position;
				Manifold.Points = new Vector3[] { v1 };
			}
			else if (dot2 <= 0) // Closest to v2
			{
				if ((center - v2).SqrMagnitude > a.Radius * a.Radius)
					return;

				Vector3 n = v2 - center;
				v2 = orientationB * v2 + Manifold.BodyB.Position;
				Manifold.Points = new Vector3[] { v2 };
				n = orientationB * n;
				n.Normalize();
				Manifold.Normal = n;
			}
			else // Closest to face
			{
				Vector3 n = b.Normals[faceNormal];
				if ((center - v1).Dot(n) > a.Radius)
					return;

				n = orientationB * n;
				Manifold.Normal = -n;
				Manifold.Points = new Vector3[] { Manifold.Normal * a.Radius + Manifold.BodyA.Position };
			}
		}

		private static void DispatchPolygonToSphere(Manifold Manifold)
		{
			Body temp = Manifold.BodyB;
			Manifold.BodyB = Manifold.BodyA;
			Manifold.BodyA = temp;

			DispatchSphereToPolygon(Manifold);

			temp = Manifold.BodyB;
			Manifold.BodyB = Manifold.BodyA;
			Manifold.BodyA = temp;

			Manifold.Normal = -Manifold.Normal;
		}

		private static void DispatchPolygonToPolygon(Manifold Manifold)
		{
			PolygonShape a = (PolygonShape)Manifold.BodyA.Shape;
			PolygonShape b = (PolygonShape)Manifold.BodyB.Shape;

			Manifold.Points = null;

			// Check for a separating axis with A's face planes
			uint faceA;
			Number penetrationA;
			FindAxisLeastPenetration(Manifold, out faceA, out penetrationA);
			if (penetrationA >= 0)
				return;

			// Check for a separating axis with B's face planes
			uint faceB;
			Number penetrationB;
			FindAxisLeastPenetration(Manifold, out faceB, out penetrationB);
			if (penetrationB >= 0)
				return;

			uint referenceIndex;
			bool flip; // Always point from a to b

			Body refBody;
			Body incBody;

			PolygonShape refPoly;
			PolygonShape incPoly;

			// Determine which shape contains reference face
			if (Math.BiasGreaterThan(penetrationA, penetrationB))
			{
				refBody = Manifold.BodyA;
				refPoly = a;

				incBody = Manifold.BodyB;
				incPoly = b;

				referenceIndex = faceA;
				flip = false;
			}
			else
			{
				refBody = Manifold.BodyB;
				refPoly = b;

				incBody = Manifold.BodyA;
				incPoly = a;

				referenceIndex = faceB;
				flip = true;
			}

			Vector3 refBodyRotation = refBody.Orientation.EulerAngles;

			// World space incident face
			Vector3[] incidentFace = new Vector3[2];
			FindIncidentFace(incidentFace, refBody, refPoly, incBody, incPoly, referenceIndex);

			//        y
			//        ^  ->n       ^
			//      +---c ------posPlane--
			//  x < | i |\
			//      +---+ c-----negPlane--
			//             \       v
			//              r
			//
			//  r : reference face
			//  i : incident poly
			//  c : clipped point
			//  n : incident normal

			// Setup reference face vertices
			Vector3 v1 = refPoly.Vertices[referenceIndex];
			referenceIndex = referenceIndex + 1 == refPoly.Vertices.Length ? 0 : referenceIndex + 1;
			Vector3 v2 = refPoly.Vertices[referenceIndex];

			// Transform vertices to world space
			v1 = refBodyRotation * v1 + refBody.Position;
			v2 = refBodyRotation * v2 + refBody.Position;

			// Calculate reference face side normal in world space
			Vector3 sidePlaneNormal = (v2 - v1);
			sidePlaneNormal.Normalize();

			// Orthogonalize
			Vector3 refFaceNormal = new Vector3(sidePlaneNormal.Y, -sidePlaneNormal.X, sidePlaneNormal.Z);

			// ax + by = c
			// c is distance from origin
			Number refC = refFaceNormal.Dot(v1);
			Number negSide = -sidePlaneNormal.Dot(v1);
			Number posSide = sidePlaneNormal.Dot(v2);

			// Clip incident face to reference face side planes
			if (Clip(-sidePlaneNormal, negSide, incidentFace) < 2)
				return; // Due to floating point error, possible to not have required points

			if (Clip(sidePlaneNormal, posSide, incidentFace) < 2)
				return; // Due to floating point error, possible to not have required points

			//TODO: handle Z axis

			// Flip
			Manifold.Normal = flip ? -refFaceNormal : refFaceNormal;

			// Keep points behind reference face
			uint cp = 0; // clipped points behind reference face
			Number separation = refFaceNormal.Dot(incidentFace[0]) - refC;
			if (separation <= 0)
			{
				ArrayUtilities.Add(ref Manifold.Points, incidentFace[0]);
				Manifold.Penetration = -separation;
				++cp;
			}
			else
				Manifold.Penetration = 0;

			separation = refFaceNormal.Dot(incidentFace[1]) - refC;
			if (separation <= 0)
			{
				ArrayUtilities.Add(ref Manifold.Points, incidentFace[1]);

				Manifold.Penetration += -separation;
				++cp;

				// Average penetration
				Manifold.Penetration /= (int)cp;
			}
		}

		private static void FindAxisLeastPenetration(Manifold Manifold, out uint FaceIndex, out Number FaceDistance)
		{
			FaceIndex = 0;
			FaceDistance = Number.MinValue;

			PolygonShape a = (PolygonShape)Manifold.BodyA.Shape;
			PolygonShape b = (PolygonShape)Manifold.BodyB.Shape;

			Vector3 bodyARotation = Manifold.BodyA.Orientation.EulerAngles;

			for (uint i = 0; i < a.Vertices.Length; ++i)
			{
				// Retrieve a face normal from A
				Vector3 n = a.Normals[i];
				Vector3 nw = bodyARotation * n;

				// Transform face normal into B's model space
				Matrix3 buT = Manifold.BodyB.Orientation.Transpose();
				n = buT * nw;

				// Retrieve support point from B along -n
				Vector3 s = GetSupport(b, -n);

				// Retrieve vertex on face from A, transform into
				// B's model space
				Vector3 v = a.Vertices[i];
				v = bodyARotation * v + Manifold.BodyA.Position;
				v -= Manifold.BodyB.Position;
				v = buT * v;

				// Compute penetration distance (in B's model space)
				Number d = n.Dot(s - v);

				// Store greatest distance
				if (d > FaceDistance)
				{
					FaceDistance = d;
					FaceIndex = i;
				}
			}
		}

		private static void FindIncidentFace(Vector3[] Faces, Body ReferenceBody, PolygonShape ReferencePolygon, Body IncidentBody, PolygonShape IncidentPolygon, uint ReferenceIndex)
		{
			Vector3 referenceNormal = ReferencePolygon.Normals[ReferenceIndex];

			// Calculate normal in incident's frame of reference
			referenceNormal = ReferenceBody.Orientation.EulerAngles * referenceNormal; // To world space
			referenceNormal = IncidentBody.Orientation.Transpose() * referenceNormal; // To incident's model space

			// Find most anti-normal face on incident polygon
			uint incidentFace = 0;
			Number minDot = Number.MaxValue;
			for (uint i = 0; i < IncidentPolygon.Vertices.Length; ++i)
			{
				Number dot = referenceNormal.Dot(IncidentPolygon.Normals[i]);
				if (dot < minDot)
				{
					minDot = dot;
					incidentFace = i;
				}
			}

			Vector3 incidentBodyRotation = IncidentBody.Orientation.EulerAngles;

			// Assign face vertices for incidentFace
			Faces[0] = incidentBodyRotation * IncidentPolygon.Vertices[incidentFace] + IncidentBody.Position;
			incidentFace = incidentFace + 1 >= IncidentPolygon.Vertices.Length ? 0 : incidentFace + 1;
			Faces[1] = incidentBodyRotation * IncidentPolygon.Vertices[incidentFace] + IncidentBody.Position;
		}

		private static uint Clip(Vector3 Normal, Number C, Vector3[] Faces)
		{
			uint sp = 0;

			Vector3[] value = new Vector3[] { Faces[0], Faces[1] };

			// Retrieve distances from each endpoint to the line
			// d = ax + by - c
			Number d1 = Normal.Dot(Faces[0]) - C;
			Number d2 = Normal.Dot(Faces[1]) - C;

			// If negative (behind plane) clip
			if (d1 <= 0)
				value[sp++] = Faces[0];

			if (d2 <= 0)
				value[sp++] = Faces[1];

			// If the points are on different sides of the plane
			if (d1 * d2 < 0) // less than to ignore -0
			{
				// Push interesection point
				Number alpha = d1 / (d1 - d2);
				value[sp] = Faces[0] + (Faces[1] - Faces[0]) * alpha;
				++sp;
			}

			// Assign our new converted values
			Faces[0] = value[0];
			Faces[1] = value[1];

			return sp;
		}

		// The extreme point along a direction within a polygon
		private static Vector3 GetSupport(PolygonShape Polygon, Vector3 Direction)
		{
			Number bestProjection = Number.MinValue;
			Vector3 bestVertex = Vector3.Zero;

			for (uint i = 0; i < Polygon.Vertices.Length; ++i)
			{
				Vector3 v = Polygon.Vertices[i];
				Number projection = v.Dot(Direction);

				if (projection > bestProjection)
				{
					bestVertex = v;
					bestProjection = projection;
				}
			}

			return bestVertex;
		}
	}
}