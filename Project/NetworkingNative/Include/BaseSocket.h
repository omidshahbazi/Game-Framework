// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef BASE_SOCKET_H
#define BASE_SOCKET_H

#include "Common.h"

namespace GameFramework::NetworkingManaged
{
	class NETWORKING_API BaseSocket
	{
	protected:
		class EventBase
		{
		public:
			EventBase()
			{
			}
		};

	private:
		class NETWORKING_API EventBaseList : List<EventBase>
		{ };

	protected:
		class NETWORKING_API SendCommand
		{
			public BufferStream Buffer
			{
				get;
				private set;
			}

				public double SendTime
			{
				get;
				private set;
			}

				public SendCommand(BufferStream Buffer, double SendTime)
			{
				this.Buffer = Buffer;
				this.SendTime = SendTime;
			}
		};

	private:
		class NETWORKING_API SendCommandList : List<SendCommand>
		{ };

		private Thread receiveThread = null;
		private Thread sendThread = null;

		private EventBaseList events = null;
		private SendCommandList sendCommands = null;

		protected Socket Socket
		{
			get;
			private set;
		}

			protected byte[] ReceiveBuffer
		{
			get;
			private set;
		}

			public abstract bool IsReady
		{
			get;
		}

			public abstract double Timestamp
		{
			get;
		}

			public ulong BandwidthIn
		{
			get;
			protected set;
		}

			public ulong BandwidthOut
		{
			get;
			protected set;
		}

			public bool MultithreadedCallbacks
		{
			get;
			set;
		}

			public bool MultithreadedReceive
		{
			get;
			set;
		}

			public bool MultithreadedSend
		{
			get;
			set;
		}

			public float PacketLossSimulation
		{
			get;
			set;
		}

			public int LatencySimulation
		{
			get;
			set;
		}

			public BaseSocket(Protocols Type)
		{
			events = new EventBaseList();
			sendCommands = new SendCommandList();

			Socket = SocketUtilities.CreateSocket(Type);
			Socket.Blocking = false;
			Socket.ReceiveBufferSize = (int)Constants.RECEIVE_BUFFER_SIZE;
			Socket.SendBufferSize = (int)Constants.SEND_BUFFER_SIZE;
			Socket.ReceiveTimeout = (int)Constants.RECEIVE_TIMEOUT;
			Socket.SendTimeout = (int)Constants.SEND_TIMEOUT;
			Socket.Ttl = Constants.TIME_TO_LIVE;

			SocketUtilities.SetIPv6OnlyEnabled(Socket, false);
			SocketUtilities.SetChecksumEnabled(Socket, false);
			SocketUtilities.SetNagleAlgorithmEnabled(Socket, false);
			SocketUtilities.SetBSDUrgentEnabled(Socket, true);

			ReceiveBuffer = new byte[Constants.RECEIVE_BUFFER_SIZE];

			MultithreadedCallbacks = true;
			MultithreadedReceive = true;
			MultithreadedSend = true;
		}

		public virtual void Service()
		{
			if (!MultithreadedReceive && IsReady)
				Receive();

			if (!MultithreadedSend && IsReady)
				HandleSendCommands();

			if (!MultithreadedCallbacks)
			{
				lock(events)
				{
					for (int i = 0; i < events.Count; ++i)
						ProcessEvent(events[i]);

					events.Clear();
				}
			}
		}

		protected void Shutdown()
		{
			SocketUtilities.CloseSocket(Socket);

			if (MultithreadedReceive)
				receiveThread.Abort();

			if (MultithreadedSend)
				sendThread.Abort();
		}

		protected void RunReceiveThread()
		{
			if (!MultithreadedReceive)
				return;

			receiveThread = new Thread(ReceiverWorker);
			receiveThread.Start();
		}

		protected void RunSenndThread()
		{
			if (!MultithreadedSend)
				return;

			sendThread = new Thread(SendWorker);
			sendThread.Start();
		}

		protected abstract void Receive();

		protected virtual void Send(Socket Target, BufferStream Buffer)
		{
			try
			{
				lock(Target)
				{
					if (PacketLossSimulation == 0 || Constants.Random.NextDouble() > PacketLossSimulation)
						Target.Send(Buffer.Buffer);

					BandwidthOut += Buffer.Size;
				}
			}
			catch (SocketException e)
			{
				if (e.SocketErrorCode == SocketError.ConnectionReset)
				{
					HandleDisconnection(Target);

					return;
				}

				throw e;
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		protected abstract bool HandleSendCommand(SendCommand Command);

		protected abstract void ProcessEvent(EventBase Event);

		protected void AddEvent(EventBase Event)
		{
			lock(events)
				events.Add(Event);
		}

		protected void AddSendCommand(SendCommand Command)
		{
			lock(sendCommands)
				sendCommands.Add(Command);
		}

		protected virtual void HandleDisconnection(Socket Socket)
		{
			//Internationally Left Blank
		}

		private void ReceiverWorker()
		{
			while (true)
			{
				Thread.Sleep(1);

				Receive();
			}
		}

		private void SendWorker()
		{
			while (true)
			{
				Thread.Sleep(1);

				HandleSendCommands();
			}
		}

		private void HandleSendCommands()
		{
			lock(sendCommands)
			{
				for (int i = 0; i < sendCommands.Count; ++i)
				{
					if (!HandleSendCommand(sendCommands[i]))
						continue;

					sendCommands.RemoveAt(i--);
				}
			}
		}
	};
}

#endif