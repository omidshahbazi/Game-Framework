// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
using System.Collections.Generic;
using System;
using System.Threading;
using Zorvan.Framework.Common.Timing;
using Zorvan.Framework.Common.Diagnostics;

namespace Zorvan.Framework.Common.SystemController
{
	public class ThreadManager : ISystem
	{
		private class Item
		{
			private Action action = null;

			public Item(Action Action)
			{
				this.action = Action;
			}

			public virtual void Do()
			{
				Development.SafeCall(action);
			}
		}

		private class QueueItem : Item
		{
			public double ExecutionTime
			{
				get;
				private set;
			}

			public QueueItem(Action Action, double ExecutionTime) :
				base(Action)
			{
				this.ExecutionTime = ExecutionTime;
			}
		}

		private class AsynQueueItem : Item
		{
			private Action onComplete = null;

			public bool RunOnCompleteOnMainThread
			{
				get;
				private set;
			}

			public bool IsDone
			{
				get;
				private set;
			}

			public AsynQueueItem(Action Action, Action OnComplete, bool RunOnCompleteOnMainThread) :
				base(Action)
			{
				this.RunOnCompleteOnMainThread = RunOnCompleteOnMainThread;
			}

			public override void Do()
			{
				base.Do();

				if (!RunOnCompleteOnMainThread)
					OnCompleteCallback();

				IsDone = true;
			}

			public void OnCompleteCallback()
			{
				Development.SafeCall(onComplete);
			}
		}

		private int maxThreadCount = 0;
		private int theadCount = 0;
		private List<Action> mainThreadActions = new List<Action>(24);
		private List<QueueItem> delayed = new List<QueueItem>(24);
		private List<AsynQueueItem> asyncDoneItems = new List<AsynQueueItem>(24);

		public ThreadManager(int MaxThreadCount = 8)
		{
			maxThreadCount = MaxThreadCount;
		}

		public void Initialize(params object[] Arguments)
		{
		}

		public void Uninitialize(params object[] Arguments)
		{
		}

		public void Update(params object[] Arguments)
		{
			for (int i = 0; i < delayed.Count; i++)
			{
				QueueItem item = delayed[i];

				if (Time.CurrentEpochTime <= item.ExecutionTime)
					break;

				item.Do();

				delayed.RemoveAt(i--);
			}

			lock (asyncDoneItems)
			{
				for (int i = 0; i < asyncDoneItems.Count; ++i)
					asyncDoneItems[i].OnCompleteCallback();

				asyncDoneItems.Clear();
			}
		}

		public void Queue(Action Action, float DelayTime = 0.0F)
		{
			delayed.Add(new QueueItem(Action, Time.CurrentEpochTime + DelayTime));
		}

		public void RunAsync(Action Action, Action OnComplete = null, bool RunOnCompleteOnMainThread = false)
		{
			while (theadCount >= maxThreadCount)
				Thread.Sleep(1);

			Interlocked.Increment(ref theadCount);

			ThreadPool.QueueUserWorkItem(RunAction, new AsynQueueItem(Action, OnComplete, RunOnCompleteOnMainThread));
		}

		private void RunAction(object Item)
		{
			AsynQueueItem item = (AsynQueueItem)Item;

			item.Do();

			if (item.RunOnCompleteOnMainThread)
				asyncDoneItems.Add(item);

			Interlocked.Decrement(ref theadCount);
		}
	}
}