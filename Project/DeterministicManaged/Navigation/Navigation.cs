// Copyright 2015-2016 Zorvan Game Studio. All Rights Reserved.
using System.Collections.Generic;

namespace GameFramework.Deterministic.Navigation
{
	public static class Navigation
	{
		public static class AStar
		{
			private class NavigationNode
			{
				public INode Node
				{
					get;
					private set;
				}

				public NavigationNode Parent
				{
					get;
					private set;
				}

				public Number ExactCost
				{
					get;
					private set;
				}

				public Number HeuristicCost
				{
					get;
					private set;
				}

				public Number TotalCost
				{
					get;
					private set;
				}

				public NavigationNode(INode Node)
				{
					this.Node = Node;

					ExactCost += this.Node.Cost;
				}

				public void SetParent(NavigationNode Node)
				{
					Parent = Node;

					ExactCost += Parent.ExactCost;
				}

				public Number GetHeuristicCost(INode GoalNode)
				{
					//return (Node.Position - GoalNode.Position).SqrMagnitued;
					//return Math.Abs(Node.Position.X - GoalNode.Position.X) + Math.Abs(Node.Position.Z - GoalNode.Position.Z);
					return Math.Max(Math.Abs(Node.Position.X - GoalNode.Position.X), Math.Abs(Node.Position.Z - GoalNode.Position.Z));
				}

				public Number GetTotalCost(INode GoalNode)
				{
					return Node.Cost + ExactCost + GetHeuristicCost(GoalNode);
				}

				public void CalculateHeuristicCost(INode GoalNode)
				{
					HeuristicCost = GetHeuristicCost(GoalNode);
				}

				public void CalculateTotalCost()
				{
					TotalCost = Node.Cost + ExactCost + HeuristicCost;
				}
			}

			private class NavigationNodeList : List<NavigationNode>
			{ }

			public static NavigationResult GetPath(IMapData Map, Vector3 StartPosition, Vector3 GoalPosition)
			{
				return GetPath(Map, StartPosition, GoalPosition, 1);
			}

			public static NavigationResult GetPath(IMapData Map, Vector3 StartPosition, Vector3 GoalPosition, Number Radius)
			{
				INode goalNode = Map.GetNode(GoalPosition);
				INode startNode = Map.GetNode(StartPosition);

				NavigationResult.Types type = NavigationResult.Types.Complete;

				if (goalNode == null || (goalNode.State != NodeStates.Walkable && goalNode != startNode))
				{
					goalNode = Map.GetNearestNode(GoalPosition, Radius, NodeStates.Walkable);
					type = NavigationResult.Types.Partial;
				}

				Vector3[] path = CalculatePath(Map, startNode, goalNode);

				if (path == null)
					return null;

				return new NavigationResult(path, type);
			}

			public static NavigationResult GetPath(IMapData Map, INode StartNode, INode GoalNode)
			{
				Vector3[] path = CalculatePath(Map, StartNode, GoalNode);

				if (path == null)
					return null;

				return new NavigationResult(path, NavigationResult.Types.Complete);
			}

			private static Vector3[] CalculatePath(IMapData Map, INode StartNode, INode GoalNode)
			{
				//1. Add the starting node to the open list.
				//2. Repeat the following steps:
				//	a. Look for the node which has the lowest f on the open list. Refer to this node as the current node.
				//	b. Switch it to the closed list.
				//	c. For each reachable node from the current node
				//		i. If it is on the closed list, ignore it.
				//		ii. If it isn’t on the open list, add it to the open list. Make the current node the parent of this node. Record the f, g, and h value of this node.
				//		iii. If it is on the open list already, check to see if this is a better path. If so, change its parent to the current node, and recalculate the f and g value.
				//	d. Stop when
				//		i. Add the target node to the closed list.
				//		ii. Fail to find the target node, and the open list is empty.
				//3. Tracing backwards from the target node to the starting node. That is your path. 


				if (Map == null)
					return null;
				if (StartNode == null)
					return null;
				if (GoalNode == null)
					return null;
				if (StartNode == GoalNode)
					return null;
				if (StartNode.State == NodeStates.Blocked || GoalNode.State == NodeStates.Blocked)
					return null;

				NavigationNodeList openNodes = new NavigationNodeList();
				NavigationNodeList closedNodes = new NavigationNodeList();

				NavigationNode startNavNode = new NavigationNode(StartNode);
				openNodes.Add(startNavNode);

				NavigationNode currNode = null;

				bool goalFound = false;

				while (true)
				{
					if (openNodes.Count == 0)
						break;

					currNode = GetLowestCost(openNodes);
					closedNodes.Add(currNode);
					openNodes.Remove(currNode);

					if (currNode.Node == GoalNode)
					{
						goalFound = true;

						break;
					}

					INode[] adjNodes = Map.GetAdjucentNodes(currNode.Node, NodeStates.Walkable);

					for (int i = 0; i < adjNodes.Length; ++i)
					{
						INode adjNode = adjNodes[i];

						if (adjNode.State == NodeStates.Blocked)
							continue;

						if (Contains(closedNodes, adjNode))
							continue;

						NavigationNode adjNavNode = GetNavigationNode(openNodes, adjNode);
						if (adjNavNode == null)
						{
							adjNavNode = new NavigationNode(adjNode);
							openNodes.Add(adjNavNode);
						}
						else if (adjNavNode.GetTotalCost(GoalNode) >= currNode.ExactCost)
							continue;

						adjNavNode.SetParent(currNode);
						adjNavNode.CalculateHeuristicCost(GoalNode);
						adjNavNode.CalculateTotalCost();
					}
				}

				if (!goalFound)
					return null;

				return Helper.GetPositions(Helper.GetOptimizedNodes(Map, (GetFullPositions(currNode))));
			}

			private static INode[] GetFullPositions(NavigationNode LastNode)
			{
				List<INode> nodesList = new List<INode>();

				NavigationNode parent = LastNode;
				do
				{
					nodesList.Insert(0, parent.Node);
				} while ((parent = parent.Parent) != null);

				return nodesList.ToArray();
			}

			private static NavigationNode GetLowestCost(NavigationNodeList Nodes)
			{
				if (Nodes.Count == 0)
					return null;

				NavigationNode node = Nodes[0];

				for (int i = 1; i < Nodes.Count; ++i)
					if (Nodes[i].TotalCost < node.TotalCost)
						node = Nodes[i];

				return node;
			}

			private static bool Contains(NavigationNodeList Nodes, INode Node)
			{
				if (Nodes.Count == 0)
					return false;

				NavigationNode node = Nodes[0];

				for (int i = 1; i < Nodes.Count; ++i)
					if (Nodes[i].Node == Node)
						return true;

				return false;
			}

			private static NavigationNode GetNavigationNode(NavigationNodeList Nodes, INode Node)
			{
				for (int i = 0; i < Nodes.Count; ++i)
					if (Nodes[i].Node == Node)
						return Nodes[i];

				return null;
			}
		}

		// The JPS Pathfinding System
		// https://github.com/qiao/PathFinding.js
		public static class JumpPointSearch
		{
			private class NavigationNode
			{
				public INode Node
				{
					get;
					private set;
				}

				public NavigationNode Parent
				{
					get;
					private set;
				}

				public Number ExactCost
				{
					get;
					private set;
				}

				public Number HeuristicCost
				{
					get;
					private set;
				}

				public Number TotalCost
				{
					get { return ExactCost + HeuristicCost; }
				}

				public NavigationNode(INode Node)
				{
					this.Node = Node;
				}

				public void SetParent(NavigationNode Node)
				{
					Parent = Node;
				}

				public void SetExactCost(Number Cost)
				{
					ExactCost = Cost;
				}

				public void SetHeuristicCost(Number Cost)
				{
					HeuristicCost = Cost;
				}

				public static Number CalculateHeuristicCost(INode A, INode B)
				{
					return Math.Max(Math.Abs(A.Position.X - B.Position.X), Math.Abs(A.Position.Z - B.Position.Z));
				}
			}

			private class NavigationNodeStack : Queue<NavigationNode>
			{ }

			public static NavigationResult GetPath(IMapData Map, Vector3 StartPosition, Vector3 GoalPosition)
			{
				return GetPath(Map, StartPosition, GoalPosition, 1);
			}

			public static NavigationResult GetPath(IMapData Map, Vector3 StartPosition, Vector3 GoalPosition, Number Radius)
			{
				INode goalNode = Map.GetNode(GoalPosition);
				INode startNode = Map.GetNode(StartPosition);

				NavigationResult.Types type = NavigationResult.Types.Complete;

				if (goalNode == null || (goalNode.State != NodeStates.Walkable && goalNode != startNode))
				{
					goalNode = Map.GetNearestNode(GoalPosition, Radius, NodeStates.Walkable);
					type = NavigationResult.Types.Partial;
				}

				Vector3[] path = CalculatePath(Map, startNode, goalNode);

				if (path == null)
					return null;

				return new NavigationResult(path, type);
			}

			public static NavigationResult GetPath(IMapData Map, INode StartNode, INode GoalNode)
			{
				Vector3[] path = CalculatePath(Map, StartNode, GoalNode);

				if (path == null)
					return null;

				return new NavigationResult(path, NavigationResult.Types.Complete);
			}

			private static Vector3[] CalculatePath(IMapData Map, INode StartNode, INode GoalNode)
			{
				if (Map == null)
					return null;
				if (StartNode == null)
					return null;
				if (GoalNode == null)
					return null;
				if (StartNode == GoalNode)
					return null;
				if (StartNode.State == NodeStates.Blocked || GoalNode.State == NodeStates.Blocked)
					return null;

				NavigationNodeStack openedNodes = new NavigationNodeStack();
				NavigationNodeStack closedNodes = new NavigationNodeStack();
				GetOrAddNavigationNode(StartNode, openedNodes);

				NavigationNode goalNode = null;

				while (openedNodes.Count != 0)
				{
					NavigationNode node = openedNodes.Dequeue();
					closedNodes.Enqueue(node);

					if (node.Node == GoalNode)
					{
						goalNode = node;
						break;
					}

					IdentifySuccessors(Map, node, GoalNode, openedNodes, closedNodes);
				}

				if (goalNode == null)
					return null;

				return Helper.GetPositions(Helper.GetOptimizedNodes(Map, GetFullPositions(goalNode)));
			}

			private static void IdentifySuccessors(IMapData Map, NavigationNode Node, INode Goal, NavigationNodeStack OpenedNodes, NavigationNodeStack ClosedNodes)
			{
				INode[] neighbors = GetNeighbors(Map, Node);

				for (int i = 0; i < neighbors.Length; ++i)
				{
					INode neighborNode = neighbors[i];

					INode jumpNode = Jump(Map, neighborNode, Node.Node, Goal, OpenedNodes);

					if (jumpNode == null || IsNavigationNodeInList(jumpNode, ClosedNodes))
						continue;

					NavigationNode jumpNavNode = GetNavigationNode(jumpNode, OpenedNodes);

					bool alreadyIsInOpenNodes = (jumpNavNode != null);

					if (jumpNavNode == null)
						jumpNavNode = GetOrAddNavigationNode(jumpNode, OpenedNodes);

					Number heuristic = NavigationNode.CalculateHeuristicCost(Node.Node, jumpNode);
					Number nextExactCost = Node.ExactCost + heuristic;

					if (!alreadyIsInOpenNodes || nextExactCost < jumpNavNode.ExactCost)
					{
						jumpNavNode.SetExactCost(nextExactCost);
						jumpNavNode.SetHeuristicCost(jumpNavNode.HeuristicCost != 0 ? jumpNavNode.HeuristicCost : NavigationNode.CalculateHeuristicCost(jumpNode, Goal));
						jumpNavNode.SetParent(Node);
					}
				}
			}

			private static INode Jump(IMapData Map, INode Node, INode Parent, INode Goal, NavigationNodeStack OpenedNodes)
			{
				if (!Helper.IsWalkable(Node))
					return null;

				if (Node == Goal)
					return Node;

				Vector3 diff = Node.Position - Parent.Position;

				if (diff.X != 0 && diff.Z != 0)
				{
					if ((Helper.IsWalkable(GetNode(Map, Node, diff)) && !Helper.IsWalkable(GetNode(Map, Node, diff.X * -1, 0))) ||
					   (Helper.IsWalkable(GetNode(Map, Node, diff * -1)) && !Helper.IsWalkable(GetNode(Map, Node, 0, diff.Z * -1))))
						return Node;

					if (Jump(Map, GetNode(Map, Node, diff.X, 0), Node, Goal, OpenedNodes) != null || Jump(Map, GetNode(Map, Node, 0, diff.Z), Node, Goal, OpenedNodes) != null)
						return Node;
				}
				else if (diff.X == 0)
				{
					if ((Helper.IsWalkable(GetNode(Map, Node, 1, diff.Z)) && !Helper.IsWalkable(GetNode(Map, Node, 1, 0))) ||
					   (Helper.IsWalkable(GetNode(Map, Node, -1, diff.Z)) && !Helper.IsWalkable(GetNode(Map, Node, -1, 0))))
						return Node;
				}
				else if (diff.Z == 0)
				{
					if ((Helper.IsWalkable(GetNode(Map, Node, diff.X, 1)) && !Helper.IsWalkable(GetNode(Map, Node, 0, 1))) ||
					   (Helper.IsWalkable(GetNode(Map, Node, diff.X, -1)) && !Helper.IsWalkable(GetNode(Map, Node, 0, -1))))
						return Node;
				}

				return Jump(Map, GetNode(Map, Node, diff), Node, Goal, OpenedNodes);
			}

			private static INode[] GetNeighbors(IMapData Map, NavigationNode Node)
			{
				List<INode> result = new List<INode>();

				if (Node.Parent == null)
				{
					INode[] nodes = Map.GetAdjucentNodes(Node.Node);
					for (int i = 0; i < nodes.Length; ++i)
						result.Add(nodes[i]);
				}
				else
				{
					Vector3 origin = Node.Node.Position;
					Vector3 diff = origin - Node.Parent.Node.Position;
					int dX = (int)(diff.X / Math.Max(Math.Abs(diff.X), 1));
					int dY = (int)(diff.Z / Math.Max(Math.Abs(diff.Z), 1));

					INode tempNode = null;

					if (dX != 0 && dY != 0)
					{
						tempNode = GetNode(Map, Node.Node, 0, dY);
						if (Helper.IsWalkable(tempNode))
							result.Add(tempNode);

						tempNode = GetNode(Map, Node.Node, dX, 0);
						if (Helper.IsWalkable(tempNode))
							result.Add(tempNode);

						tempNode = GetNode(Map, Node.Node, dX, dY);
						if (Helper.IsWalkable(tempNode))
							result.Add(tempNode);

						tempNode = GetNode(Map, Node.Node, -dX, 0);
						if (!Helper.IsWalkable(tempNode))
						{
							tempNode = GetNode(Map, Node.Node, -dX, dY);
							if (tempNode != null)
								result.Add(tempNode);
						}

						tempNode = GetNode(Map, Node.Node, 0, -dY);
						if (!Helper.IsWalkable(tempNode))
						{
							tempNode = GetNode(Map, Node.Node, dX, -dY);
							if (tempNode != null)
								result.Add(tempNode);
						}
					}
					else if (dX == 0)
					{
						tempNode = GetNode(Map, Node.Node, 0, dY);
						if (Helper.IsWalkable(tempNode))
							result.Add(tempNode);

						tempNode = GetNode(Map, Node.Node, 1, 0);
						if (!Helper.IsWalkable(tempNode))
						{
							tempNode = GetNode(Map, Node.Node, 1, dY);
							if (tempNode != null)
								result.Add(tempNode);
						}

						tempNode = GetNode(Map, Node.Node, -1, 0);
						if (!Helper.IsWalkable(tempNode))
						{
							tempNode = GetNode(Map, Node.Node, -1, dY);
							if (tempNode != null)
								result.Add(tempNode);
						}
					}
					else if (dY == 0)
					{
						tempNode = GetNode(Map, Node.Node, dX, 0);
						if (Helper.IsWalkable(tempNode))
							result.Add(tempNode);

						tempNode = GetNode(Map, Node.Node, 0, 1);
						if (!Helper.IsWalkable(tempNode))
						{
							tempNode = GetNode(Map, Node.Node, dX, 1);
							if (tempNode != null)
								result.Add(tempNode);
						}

						tempNode = GetNode(Map, Node.Node, 0, -1);
						if (!Helper.IsWalkable(tempNode))
						{
							tempNode = GetNode(Map, Node.Node, dX, -1);
							if (tempNode != null)
								result.Add(tempNode);
						}
					}
				}

				return result.ToArray();
			}

			private static INode GetNode(IMapData Map, INode Node, Vector3 Offset)
			{
				if (Offset.X != 0)
				{
					Node = Map.GetNode(Node.Position, Offset.X, Directions.East);
					if (Node == null)
						return null;
				}

				if (Offset.Z != 0)
				{
					Node = Map.GetNode(Node.Position, Offset.Z, Directions.North);
					if (Node == null)
						return null;
				}

				return Node;
			}

			private static INode GetNode(IMapData Map, INode Node, Number X, int Z)
			{
				if (X != 0)
				{
					Node = Map.GetNode(Node.Position, X, Directions.East);
					if (Node == null)
						return null;
				}

				if (Z != 0)
				{
					Node = Map.GetNode(Node.Position, Z, Directions.North);
					if (Node == null)
						return null;
				}

				return Node;
			}

			private static INode GetNode(IMapData Map, INode Node, int X, Number Z)
			{
				if (X != 0)
				{
					Node = Map.GetNode(Node.Position, X, Directions.East);
					if (Node == null)
						return null;
				}

				if (Z != 0)
				{
					Node = Map.GetNode(Node.Position, Z, Directions.North);
					if (Node == null)
						return null;
				}

				return Node;
			}

			private static INode GetNode(IMapData Map, INode Node, int X, int Z)
			{
				if (X != 0)
				{
					Node = Map.GetNode(Node.Position, X, Directions.East);
					if (Node == null)
						return null;
				}

				if (Z != 0)
				{
					Node = Map.GetNode(Node.Position, Z, Directions.North);
					if (Node == null)
						return null;
				}

				return Node;
			}

			private static NavigationNode GetNavigationNode(INode Node, NavigationNodeStack Nodes)
			{
				var it = Nodes.GetEnumerator();
				while (it.MoveNext())
					if (it.Current.Node == Node)
						return it.Current;

				return null;
			}

			private static bool IsNavigationNodeInList(INode Node, NavigationNodeStack Nodes)
			{
				return (GetNavigationNode(Node, Nodes) != null);
			}

			private static NavigationNode GetOrAddNavigationNode(INode Node, NavigationNodeStack Nodes)
			{
				NavigationNode navNode = GetNavigationNode(Node, Nodes);

				if (navNode != null)
					return navNode;

				navNode = new NavigationNode(Node);
				Nodes.Enqueue(navNode);
				return navNode;
			}

			private static INode[] GetFullPositions(NavigationNode LastNode)
			{
				List<INode> nodesList = new List<INode>();

				NavigationNode parent = LastNode;
				do
				{
					nodesList.Insert(0, parent.Node);
				} while ((parent = parent.Parent) != null);

				return nodesList.ToArray();
			}
		}

		public static Number GetPathCost(NavigationResult Result)
		{
			if (Result == null)
				return 0;

			Number cost = 0;
			for (int i = 1; i < Result.Corners.Length; ++i)
				cost += (Result.Corners[i - 1] - Result.Corners[i]).SqrMagnitude;

			return cost;
		}
	}
}