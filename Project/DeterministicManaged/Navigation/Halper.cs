// Copyright 2015-2016 Zorvan Game Studio. All Rights Reserved.
using System.Collections.Generic;

namespace GameFramework.Deterministic.Navigation
{
	public static class Helper
	{
		public static Vector3[] GetPositions(INode[] Nodes)
		{
			Vector3[] positionList = new Vector3[Nodes.Length];

			for (int i = 0; i < Nodes.Length; ++i)
				positionList[i] = Nodes[i].Position;

			return positionList;
		}

		public static bool IsOrthogonal(Directions Direction)
		{
			switch (Direction)
			{
				case Directions.North:
				case Directions.South:
				case Directions.East:
				case Directions.West:
					return true;
			}

			return false;
		}

		public static bool IsDiagonal(Directions Direction)
		{
			switch (Direction)
			{
				case Directions.NorthEast:
				case Directions.NorthWest:
				case Directions.SouthEast:
				case Directions.SouthWest:
					return true;
			}

			return false;
		}

		public static bool IsWalkable(INode Node)
		{
			return (Node != null && Node.State == NodeStates.Walkable);
		}

		public static INode[] GetOptimizedNodes(IMapData Map, INode[] Nodes)
		{
			List<INode> positionList = new List<INode>();

			INode start = Nodes[0];
			INode lastChecked = start;

			positionList.Add(start);

			for (int i = 1; i < Nodes.Length; ++i)
			{
				INode curr = Nodes[i];

				bool intersects = CheckIntersection(Map, start, curr);

				if (intersects)
				{
					positionList.Add(lastChecked);

					start = curr;
				}

				lastChecked = curr;
			}

			positionList.Add(lastChecked);

			return positionList.ToArray();
		}

		public static INode GetFirstIntersectedNode(IMapData Map, INode From, INode To)
		{
			INode[] nodes = Map.GetNodesBetween(From, To);

			for (int i = 0; i < nodes.Length; ++i)
			{
				INode node = nodes[i];

				if (node.State == NodeStates.Walkable)
					continue;

				if (Physics.LineIntersectsBounds(From.Position, To.Position, node.Bound.Min, node.Bound.Max))
					return node;
			}

			return null;
		}

		private static bool CheckIntersection(IMapData Map, INode From, INode To)
		{
			return (GetFirstIntersectedNode(Map, From, To) != null);
		}
	}
}