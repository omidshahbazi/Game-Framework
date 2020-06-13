// Copyright 2019. All Rights Reserved.
using GameFramework.Common.Extensions;

namespace GameFramework.Deterministic.Physics
{
	public static class Simulation
	{
		public class Config
		{
			public Number StepTime;
			public int CCDStepCount = 10;
		}

		public static void Simulate(Scene Scene, Config Config, ContactList Contacts)
		{
			for (int i = 0; i < Scene.Bodies.Length; ++i)
			{
				Body a = Scene.Bodies[i];

				for (int j = i + 1; j < Scene.Bodies.Length; ++j)
				{
					Body b = Scene.Bodies[j];

					if (a.Mass == 0 && b.Mass == 0)
						continue;

					Manifold manifold = new Manifold() { BodyA = a, BodyB = b };

					DispatchManifold(manifold);

					if (manifold.Points != null && manifold.Points.Length != 0)
						Contacts.Add(manifold);
				}
			}

			for (int i = 0; i < Scene.Bodies.Length; ++i)
				IntegrateForces(Scene.Bodies[i], Config);

			for (int i = 0; i < Contacts.Count; ++i)
				InitializeManifold((Manifold)Contacts[i]);

			for (int i = 0; i < Config.CCDStepCount; ++i)
				for (int j = 0; j < Contacts.Count; ++j)
					SolveManifold((Manifold)Contacts[j]);

			for (int i = 0; i < Scene.Bodies.Length; ++i)
				IntegrateVelocity(Scene.Bodies[i]);

			for (int i = 0; i < Contacts.Count; ++i)
				CorrectPosition((Manifold)Contacts[i]);

			for (int i = 0; i < Scene.Bodies.Length; ++i)
			{
				Body body = Scene.Bodies[i];

				//TODO: remove any force and torque
			}
		}

		public static void IntegrateForces(Body Body, Config Config)
		{

		}

		private static void DispatchManifold(Manifold Manifold)
		{
			bool aIsSphere = (Manifold.BodyA.Shape is SphereShape);
			bool bIsSphere = (Manifold.BodyB.Shape is SphereShape);

			if (aIsSphere)
			{
				if (bIsSphere)
					DispatchSphereToSphere(Manifold);
				else
					DispatchSphereToPolygon(Manifold);
			}
			else
			{
				if (bIsSphere)
					DispatchPolygonToSphere(Manifold);
				else
					DispatchPolygonToPolygon(Manifold);
			}
		}

		private static void InitializeManifold(Manifold Manifold)
		{

		}

		private static void SolveManifold(Manifold Manifold)
		{

		}

		private static void CorrectPosition(Manifold Manifold)
		{
		}

		private static void IntegrateVelocity(Body Body)
		{
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

			if (distance == 0.0f)
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

			Manifold.Points = null;

			// Transform SphereShape center to Polygon model space
			Vector3 center = Manifold.BodyA.Position;
			center = Manifold.BodyB.Rotation.Transpose() * (center - Manifold.BodyB.Position);

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
			if (separation < Number.Epsilon)
			{
				Manifold.Points = new Vector3[1];
				Manifold.Normal = -(Manifold.BodyB.Rotation * b.Normals[faceNormal]);
				Manifold.Points = new Vector3[] { Manifold.Normal * a.Radius + Manifold.BodyA.Position };
				Manifold.Penetration = a.Radius;
				return;
			}

			// Determine which voronoi region of the edge center of SphereShape lies within
			Number dot1 = (center - v1).Dot(v2 - v1);
			Number dot2 = (center - v2).Dot(v1 - v2);
			Manifold.Penetration = a.Radius - separation;

			// Closest to v1
			if (dot1 <= 0.0f)
			{
				if ((center - v1).SqrMagnitude > a.Radius * a.Radius)
					return;

				Manifold.Points = new Vector3[1];
				Vector3 n = v1 - center;
				n = Manifold.BodyB.Rotation * n;
				n.Normalize();
				Manifold.Normal = n;
				v1 = Manifold.BodyB.Rotation * v1 + Manifold.BodyB.Position;
				Manifold.Points = new Vector3[] { v1 };
			}

			// Closest to v2
			else if (dot2 <= 0.0f)
			{
				if ((center - v2).SqrMagnitude > a.Radius * a.Radius)
					return;

				Manifold.Points = new Vector3[1];
				Vector3 n = v2 - center;
				v2 = Manifold.BodyB.Rotation * v2 + Manifold.BodyB.Position;
				Manifold.Points = new Vector3[] { v2 };
				n = Manifold.BodyB.Rotation * n;
				n.Normalize();
				Manifold.Normal = n;
			}

			// Closest to face
			else
			{
				Vector3 n = b.Normals[faceNormal];
				if ((center - v1).Dot(n) > a.Radius)
					return;

				n = Manifold.BodyB.Rotation * n;
				Manifold.Normal = -n;
				Manifold.Points = new Vector3[] { Manifold.Normal * a.Radius + Manifold.BodyA.Position };
			}
		}

		private static void DispatchPolygonToSphere(Manifold Manifold)
		{
			DispatchSphereToPolygon(Manifold);

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
			if (penetrationA >= 0.0f)
				return;

			// Check for a separating axis with B's face planes
			uint faceB;
			Number penetrationB;
			FindAxisLeastPenetration(Manifold, out faceB, out penetrationB);
			if (penetrationB >= 0.0f)
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
			v1 = refBody.Rotation * v1 + refBody.Position;
			v2 = refBody.Rotation * v2 + refBody.Position;

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
			if (separation <= 0.0f)
			{
				ArrayUtilities.Add(ref Manifold.Points, incidentFace[0]);
				Manifold.Penetration = -separation;
				++cp;
			}
			else
				Manifold.Penetration = 0;

			separation = refFaceNormal.Dot(incidentFace[1]) - refC;
			if (separation <= 0.0f)
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
			FaceDistance = 0;

			PolygonShape a = (PolygonShape)Manifold.BodyA.Shape;
			PolygonShape b = (PolygonShape)Manifold.BodyB.Shape;

			FaceDistance = Number.MinValue;

			for (uint i = 0; i < a.Vertices.Length; ++i)
			{
				// Retrieve a face normal from A
				Vector3 n = a.Normals[i];
				Vector3 nw = Manifold.BodyA.Rotation * n;

				// Transform face normal into B's model space
				Matrix3 buT = Manifold.BodyB.Rotation.Transpose();
				n = buT * nw;

				// Retrieve support point from B along -n
				Vector3 s = GetSupport(b, -n);

				// Retrieve vertex on face from A, transform into
				// B's model space
				Vector3 v = a.Vertices[i];
				v = Manifold.BodyA.Rotation * v + Manifold.BodyA.Position;
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
			referenceNormal = ReferenceBody.Rotation * referenceNormal; // To world space
			referenceNormal = IncidentBody.Rotation.Transpose() * referenceNormal; // To incident's model space

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

			// Assign face vertices for incidentFace
			Faces[0] = IncidentBody.Rotation * IncidentPolygon.Vertices[incidentFace] + IncidentBody.Position;
			incidentFace = incidentFace + 1 >= IncidentPolygon.Vertices.Length ? 0 : incidentFace + 1;
			Faces[1] = IncidentBody.Rotation * IncidentPolygon.Vertices[incidentFace] + IncidentBody.Position;
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
			if (d1 <= 0.0f)
				value[sp++] = Faces[0];

			if (d2 <= 0.0f)
				value[sp++] = Faces[1];

			// If the points are on different sides of the plane
			if (d1 * d2 < 0.0f) // less than to ignore -0.0f
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