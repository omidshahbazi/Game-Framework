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

		public void AddResourceEvent<RT, FT>(int UserID, RT ResourceType, FT FlowType, int Amount, int Progress) where RT : struct, IConvertible
		{
			if (!typeof(RT).IsEnum && typeof(RT) != typeof(int))
				throw new ArgumentException("ResourceType must be int or enum");

			if (!typeof(FT).IsEnum && typeof(FT) != typeof(int))
				throw new ArgumentException("FlowType must be int or enum");

			int resourceType = Convert.ToInt32(ResourceType);
			int flowType = Convert.ToInt32(FlowType);

			database.Execute("INSERT INTO resources_flow(user_id, resource_type, flow_type, amount, progress, occurs_time) VALUES(@UserID, @ResourceType, @FlowType, @Amount, @Progress, NOW())",
				"UserID", UserID,
				"ResourceType", resourceType,
				"FlowType", flowType,
				"Amount", Amount,
				"Progress", Progress);
		}
	}
}
