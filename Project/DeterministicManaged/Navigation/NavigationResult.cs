// Copyright 2015-2016 Zorvan Game Studio. All Rights Reserved.
namespace GameFramework.Deterministic.Navigation
{
	public class NavigationResult
	{
		public enum Types
		{
			Complete = 0,
			Partial
		}

		public Vector3[] Corners
		{
			get;
			private set;
		}

		public Types Type
		{
			get;
			private set;
		}

		public NavigationResult(Vector3[] Corners, Types Type)
		{
			this.Corners = Corners;
			this.Type = Type;
		}
	}
}
