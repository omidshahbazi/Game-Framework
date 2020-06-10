// Copyright 2015-2016 Zorvan Game Studio. All Rights Reserved.
namespace GameFramework.Deterministic.Navigation.GridBased
{
	public class GridNode : INode
	{
		internal Vector3 position;
		internal Bounds bound;

		public int X
		{
			get;
			private set;
		}

		public int Y
		{
			get;
			private set;
		}

		public Vector3 Position
		{
			get { return position; }
		}

		public Number Cost
		{
			get;
			set;
		}

		public Bounds Bound
		{
			get { return bound; }
		}

		public int IsBlock
		{
			get;
			set;
		}

		public NodeStates State
		{
			get { return (IsBlock == 0 ? NodeStates.Walkable : NodeStates.Blocked); }
		}

		public GridNode(int X, int Y)
		{
			this.X = X;
			this.Y = Y;

			Cost = 1;
			IsBlock = 0;
		}
	}
}
