// Copyright 2015-2016 Zorvan Game Studio. All Rights Reserved.
namespace GameFramework.Deterministic.Navigation
{
	public interface IMapData
	{
		INode GetNode(Vector3 Position);
		INode GetNode(Vector3 Position, int Distance, Directions Direction);
		INode GetNode(Vector3 Position, Number Distance, Directions Direction);

		INode GetNearestNode(Vector3 Position, Number Radius, NodeStates StatusMask = NodeStates.Blocked | NodeStates.Walkable);

		INode[] GetAdjucentNodes(INode Node, NodeStates StatusMask = NodeStates.Blocked | NodeStates.Walkable);

		INode[] GetDirectionalAdjucentNodes(INode Node, Directions Direction = Directions.All, NodeStates StateMask = NodeStates.Blocked | NodeStates.Walkable);

		void SetAreaState(Bounds Bounds, NodeStates State);
		NodeStates GetPositionState(Vector3 Position);

		INode[] GetNodesBetween(INode From, INode To);
	}
}
