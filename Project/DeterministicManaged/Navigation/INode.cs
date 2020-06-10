// Copyright 2015-2016 Zorvan Game Studio. All Rights Reserved.
namespace GameFramework.Deterministic.Navigation
{
	public enum NodeStates
	{
		Blocked = 2,
		Walkable = 4
	}

	public enum Directions
	{
		North = 2,
		South = 4,
		East = 8,
		West = 16,
		NorthEast = 32,
		NorthWest = 64,
		SouthEast = 128,
		SouthWest = 256,
		Orthogonal = North | South | East | West,
		Diagonal = NorthEast | NorthWest | SouthEast | SouthWest,
		All = Orthogonal | Diagonal
	}

	public interface INode
	{
		Vector3 Position
		{
			get;
		}

		Number Cost
		{
			get;
			set;
		}

		Bounds Bound
		{
			get;
		}

		int IsBlock
		{
			get;
			set;
		}

		NodeStates State
		{
			get;
		}
	}
}
