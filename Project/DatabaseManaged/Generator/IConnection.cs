// Copyright 2019. All Rights Reserved.
using System.Data;

namespace GameFramework.DatabaseManaged.Generator
{
	public interface IConnection
	{
		void Execute(string Transact, params object[] Parameters);

		DataTable ExecuteWithReturn(string Transact, params object[] Parameters);
	}
}