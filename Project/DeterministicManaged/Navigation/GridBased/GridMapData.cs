// Copyright 2015-2016 Zorvan Game Studio. All Rights Reserved.
using GameFramework.Common.Utilities;
using System.Collections.Generic;

namespace GameFramework.Deterministic.Navigation.GridBased
{
	public class GridMapData : IMapData
	{
		static class GridHelper
		{
			private const int COUNT_X = 256;
			private const int COUNT_Y = 256;

			private static Vector3[,] positions = null;
			private static Bounds[,] bounds = null;

			public static void GetData(int X, int Z, Number TileSize, out Vector3 Position, out Bounds Bounds)
			{
				if (positions == null)
				{
					Vector3 halfTileize = new Vector3(TileSize / 2, 0, TileSize / 2);

					positions = new Vector3[COUNT_X, COUNT_Y];
					bounds = new Bounds[COUNT_X, COUNT_Y];

					for (int i = 0; i < COUNT_X; ++i)
						for (int j = 0; j < COUNT_Y; ++j)
						{
							Vector3 pos = new Vector3(i * TileSize, 0, j * TileSize);

							positions[i, j] = pos;

							bounds[i, j] = new Bounds(pos - halfTileize, pos + halfTileize);
						}
				}

				Position = positions[X, Z];
				Bounds = bounds[X, Z];
			}
		}

		private GridNode[] nodes = null;

		public Number TileSize
		{
			get;
			private set;
		}

		public int XCount
		{
			get;
			private set;
		}

		public int YCount
		{
			get;
			private set;
		}

		public GridMapData(Number TileSize, int XCount, int YCount)
		{
			this.TileSize = TileSize;
			this.XCount = XCount;
			this.YCount = YCount;

			nodes = new GridNode[XCount * YCount];

			for (int i = 0; i < XCount; ++i)
				for (int j = 0; j < YCount; ++j)
				{
					GridNode node = nodes[i + (j * XCount)] = new GridNode(i, j);
					GridHelper.GetData(i, j, TileSize, out node.position, out node.bound);
				}
		}

		public INode GetNode(Vector3 Position)
		{
			return GetNode(PositionToIndex(Position.X), PositionToIndex(Position.Z));
		}

		public INode GetNode(Vector3 Position, int Distance, Directions Direction)
		{
			Number distance = Distance * TileSize;

			if (Helper.IsDiagonal(Direction))
				distance = Math.Sqrt((distance * distance) * 2);

			return GetNode(Position, distance, Direction);
		}

		public INode GetNode(Vector3 Position, Number Distance, Directions Direction)
		{
			if (Helper.IsDiagonal(Direction))
				Distance = Math.Sqrt((Distance * Distance) / 2);

			switch (Direction)
			{
				case Directions.North:
					Position.Z += Distance;
					break;

				case Directions.South:
					Position.Z -= Distance;
					break;

				case Directions.East:
					Position.X += Distance;
					break;

				case Directions.West:
					Position.X -= Distance;
					break;

				case Directions.NorthEast:
					Position.X += Distance;
					Position.Z += Distance;
					break;

				case Directions.NorthWest:
					Position.X -= Distance;
					Position.Z += Distance;
					break;

				case Directions.SouthEast:
					Position.X += Distance;
					Position.Z -= Distance;
					break;

				case Directions.SouthWest:
					Position.X -= Distance;
					Position.Z -= Distance;
					break;
			}

			return GetNode(Position);
		}

		public INode GetNearestNode(Vector3 Position, Number MaxRadius, NodeStates StateMask)
		{
			return GetNearestNode(Position, PositionToIndex(Position.X), PositionToIndex(Position.Z), 1, MaxRadius * MaxRadius, StateMask);
		}

		public INode[] GetAdjucentNodes(INode Node, NodeStates StateMask)
		{
			return GetAdjucentNodes(((GridNode)Node).X, ((GridNode)Node).Y, StateMask);
		}

		public INode[] GetDirectionalAdjucentNodes(INode Node, Directions Direction = Directions.All, NodeStates StateMask = NodeStates.Blocked | NodeStates.Walkable)
		{
			return GetDirectionalAdjucentNodes(((GridNode)Node).X, ((GridNode)Node).Y, 1, Direction, StateMask);
		}

		public GridNode[] GetAdjucentNodes(int X, int Y, NodeStates StateMask)
		{
			return GetAroundNodes(X, Y, 1, StateMask);
		}

		public GridNode GetNode(int X, int Y)
		{
			if (0 > X || X >= XCount)
				return null;
			if (0 > Y || Y >= YCount)
				return null;

			return nodes[X + (Y * XCount)];
		}

		public GridNode[] GetAroundNodes(int X, int Y, int Offset, NodeStates StateMask)
		{
			List<GridNode> nodes = new List<GridNode>();

			GridNode node = null;

			node = GetNode(X - Offset, Y);
			if (node != null && BitwiseHelper.IsEnabled((int)StateMask, (ushort)node.State))
				nodes.Add(node);

			node = GetNode(X, Y - Offset);
			if (node != null && BitwiseHelper.IsEnabled((int)StateMask, (ushort)node.State))
				nodes.Add(node);

			node = GetNode(X + Offset, Y);
			if (node != null && BitwiseHelper.IsEnabled((int)StateMask, (ushort)node.State))
				nodes.Add(node);

			node = GetNode(X, Y + Offset);
			if (node != null && BitwiseHelper.IsEnabled((int)StateMask, (ushort)node.State))
				nodes.Add(node);

			node = GetNode(X - Offset, Y - Offset);
			if (node != null && BitwiseHelper.IsEnabled((int)StateMask, (ushort)node.State))
				nodes.Add(node);

			node = GetNode(X + Offset, Y - Offset);
			if (node != null && BitwiseHelper.IsEnabled((int)StateMask, (ushort)node.State))
				nodes.Add(node);

			node = GetNode(X - Offset, Y + Offset);
			if (node != null && BitwiseHelper.IsEnabled((int)StateMask, (ushort)node.State))
				nodes.Add(node);

			node = GetNode(X + Offset, Y + Offset);
			if (node != null && BitwiseHelper.IsEnabled((int)StateMask, (ushort)node.State))
				nodes.Add(node);

			return nodes.ToArray();
		}

		public GridNode[] GetDirectionalAdjucentNodes(int X, int Y, int Offset, Directions Direction = Directions.All, NodeStates StateMask = NodeStates.Blocked | NodeStates.Walkable)
		{
			List<GridNode> nodes = new List<GridNode>();

			GridNode node = null;

			if (BitwiseHelper.IsEnabled((long)Direction, (ushort)Directions.North))
			{
				node = GetNode(X, Y + Offset);
				if (node != null && BitwiseHelper.IsEnabled((int)StateMask, (ushort)node.State))
					nodes.Add(node);
			}

			if (BitwiseHelper.IsEnabled((long)Direction, (ushort)Directions.South))
			{
				node = GetNode(X, Y - Offset);
				if (node != null && BitwiseHelper.IsEnabled((int)StateMask, (ushort)node.State))
					nodes.Add(node);
			}

			if (BitwiseHelper.IsEnabled((long)Direction, (ushort)Directions.East))
			{
				node = GetNode(X + Offset, Y);
				if (node != null && BitwiseHelper.IsEnabled((int)StateMask, (ushort)node.State))
					nodes.Add(node);
			}

			if (BitwiseHelper.IsEnabled((long)Direction, (ushort)Directions.West))
			{
				node = GetNode(X - Offset, Y);
				if (node != null && BitwiseHelper.IsEnabled((int)StateMask, (ushort)node.State))
					nodes.Add(node);
			}

			if (BitwiseHelper.IsEnabled((long)Direction, (ushort)Directions.NorthEast))
			{
				node = GetNode(X + Offset, Y + Offset);
				if (node != null && BitwiseHelper.IsEnabled((int)StateMask, (ushort)node.State))
					nodes.Add(node);
			}

			if (BitwiseHelper.IsEnabled((long)Direction, (ushort)Directions.NorthWest))
			{
				node = GetNode(X - Offset, Y + Offset);
				if (node != null && BitwiseHelper.IsEnabled((int)StateMask, (ushort)node.State))
					nodes.Add(node);
			}

			if (BitwiseHelper.IsEnabled((long)Direction, (ushort)Directions.SouthEast))
			{
				node = GetNode(X + Offset, Y - Offset);
				if (node != null && BitwiseHelper.IsEnabled((int)StateMask, (ushort)node.State))
					nodes.Add(node);
			}

			if (BitwiseHelper.IsEnabled((long)Direction, (ushort)Directions.SouthWest))
			{
				node = GetNode(X - Offset, Y - Offset);
				if (node != null && BitwiseHelper.IsEnabled((int)StateMask, (ushort)node.State))
					nodes.Add(node);
			}

			return nodes.ToArray();
		}

		public void SetAreaState(Bounds Bounds, NodeStates State)
		{
			int minX = PositionToIndex(Bounds.Min.X);
			int minY = PositionToIndex(Bounds.Min.Z);
			int maxX = PositionToIndex(Bounds.Max.X);
			int maxY = PositionToIndex(Bounds.Max.Z);

			for (int i = minX; i < maxX; ++i)
				for (int j = minY; j < maxY; ++j)
				{
					GridNode node = GetNode(i, j);
					if (node == null)
						continue;
					node.IsBlock = (int)Math.Max(0, node.IsBlock + (State == NodeStates.Blocked ? 1 : -1));
				}
		}

		public NodeStates GetPositionState(Vector3 Position)
		{
			INode node = GetNode(Position);

			if (node == null)
				return NodeStates.Blocked;

			return node.State;
		}

		public INode[] GetNodesBetween(INode From, INode To)
		{
			int x1 = PositionToIndex(From.Position.X);
			int x2 = PositionToIndex(To.Position.X);
			int y1 = PositionToIndex(From.Position.Z);
			int y2 = PositionToIndex(To.Position.Z);

			int xMin = System.Math.Min(x1, x2);
			int xMax = System.Math.Max(x1, x2);
			int yMin = System.Math.Min(y1, y2);
			int yMax = System.Math.Max(y1, y2);

			int xCount = (xMax - xMin) + 1;
			int yCount = (yMax - yMin) + 1;
			GridNode[] nodes = new GridNode[xCount * yCount];

			for (int i = 0; i < xCount; ++i)
				for (int j = 0; j < yCount; ++j)
					nodes[i + (j * xCount)] = GetNode(xMin + i, yMin + j);

			return nodes;
		}

		private GridNode GetNearestNode(Vector3 OriginPosition, int X, int Y, int Offset, Number SqrMaxRadius, NodeStates StateMask)
		{
			if ((Offset * TileSize) * (Offset * TileSize) > SqrMaxRadius)
				return null;

			GridNode[] adjNode = GetAroundNodes(X, Y, Offset, NodeStates.Blocked | NodeStates.Walkable);

			bool isOutOfDistance = false;

			for (int i = 0; i < adjNode.Length; ++i)
			{
				GridNode node = adjNode[i];

				if ((node.Position - OriginPosition).SqrMagnitude > SqrMaxRadius)
				{
					isOutOfDistance = true;
					continue;
				}

				if (!BitwiseHelper.IsEnabled((int)StateMask, (ushort)node.State))
					continue;

				return node;
			}

			if (isOutOfDistance)
				return null;

			return GetNearestNode(OriginPosition, X, Y, Offset + 1, SqrMaxRadius, StateMask);
		}

		private int PositionToIndex(Number Value)
		{
			return (int)Math.Round(Value / TileSize);
		}
	}
}
