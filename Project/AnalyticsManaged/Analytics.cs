// Copyright 2019. All Rights Reserved.
using GameFramework.DatabaseManaged;
using System;

namespace GameFramework.Analytics
{
	public class Analytics
	{
		private Database database = null;

		public Analytics(Database Database)
		{
			database = Database;
		}

		public void UpdateDatabaseStructure()
		{
			DatabaseGenerator.UpdateStructure(database);
		}

		public void AddResourceEvent<P, RT, FT>(int UserID, P Place, RT ResourceType, FT FlowType, uint Amount, int Progress = 0)
		{
			if (!typeof(P).IsEnum && typeof(P) != typeof(int))
				throw new ArgumentException("Place must be int or enum");

			if (!typeof(RT).IsEnum && typeof(RT) != typeof(int))
				throw new ArgumentException("ResourceType must be int or enum");

			if (!typeof(FT).IsEnum && typeof(FT) != typeof(int))
				throw new ArgumentException("FlowType must be int or enum");

			int place = Convert.ToInt32(Place);
			int resourceType = Convert.ToInt32(ResourceType);
			int flowType = Convert.ToInt32(FlowType);

			database.Execute("INSERT INTO resources_flow(user_id, place, resource_type, flow_type, amount, progress, occurs_time) VALUES(@UserID, @Place, @ResourceType, @FlowType, @Amount, @Progress, NOW())",
				"UserID", UserID,
				"Place", place,
				"ResourceType", resourceType,
				"FlowType", flowType,
				"Amount", Amount,
				"Progress", Progress);
		}
	}
}
