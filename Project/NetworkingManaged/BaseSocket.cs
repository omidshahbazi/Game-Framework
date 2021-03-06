﻿// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GameFramework.Networking
{
	public abstract class BaseSocket
	{
		protected abstract class EventBase
		{
			public EventBase()
			{
			}
		}

		private class EventBaseList : List<EventBase>
		{ }

		protected class SendCommand
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
		}

		private class SendCommandList : List<SendCommand>
		{ }

		private Thread receiveThread = null;
		private Thread sendThread = null;

		private EventBaseList events = null;
		private SendCommandList sendCommands = null;

		public Protocols Type
		{
			get;
			private set;
		}

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

		protected uint ReceiveBufferIndex
		{
			get;
			set;
		}

		public abstract bool IsReady
		{
			get;
		}

		public abstract double Timestamp
		{
			get;
		}

		public NetworkingStatistics Statistics
		{
			get;
			private set;
		}

		public uint SendBufferSize
		{
			get;
			set;
		}

		public uint ReceiveBufferSize
		{
			get;
			set;
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
			this.Type = Type;

			events = new EventBaseList();
			sendCommands = new SendCommandList();

			SendBufferSize = Constants.SEND_BUFFER_SIZE;
			ReceiveBufferSize = Constants.RECEIVE_BUFFER_SIZE;

			Socket = SocketUtilities.CreateSocket(Type);
			SocketUtilities.SetBlocking(Socket, false);
			SocketUtilities.SetReceiveTimeout(Socket, Constants.RECEIVE_TIMEOUT);
			SocketUtilities.SetSendTimeout(Socket, Constants.SEND_TIMEOUT);
			SocketUtilities.SetTimeToLive(Socket, Constants.TIME_TO_LIVE);
			SocketUtilities.SetIPv6OnlyEnabled(Socket, false);
			//SocketUtilities.SetChecksumEnabled(Socket, false);

			if (Type == Protocols.TCP)
				SocketUtilities.SetNagleAlgorithmEnabled(Socket, false);

			Statistics = new NetworkingStatistics();

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
				lock (events)
				{
					for (int i = 0; i < events.Count; ++i)
						ProcessEvent(events[i]);

					for (int i = 0; i < events.Count; ++i)
						events.RemoveAt(0);
				}
			}
		}

		protected void Shutdown()
		{
			if (MultithreadedReceive)
				receiveThread.Abort();

			if (MultithreadedSend)
				sendThread.Abort();

			SocketUtilities.CloseSocket(Socket);
		}

		protected void RunReceiveThread()
		{
			SocketUtilities.SetSendBufferSize(Socket, SendBufferSize);
			SocketUtilities.SetReceiveBufferSize(Socket, ReceiveBufferSize);
			ReceiveBuffer = new byte[ReceiveBufferSize];

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

		protected virtual bool SendOverSocket(Socket Target, BufferStream Buffer)
		{
			try
			{
				lock (Target)
				{
					if (PacketLossSimulation == 0 || Constants.Random.NextDouble() > PacketLossSimulation)
						Target.Send(Buffer.Buffer);

					Statistics.AddBandwidthOut(Buffer.Size);
				}

				return true;
			}
			catch (SocketException e)
			{
				if (e.SocketErrorCode == SocketError.ConnectionReset ||
					e.SocketErrorCode == SocketError.ConnectionAborted ||
					e.SocketErrorCode == SocketError.ConnectionRefused)
				{
					HandleDisconnection(Target);

					return false;
				}

				throw e;
			}
			catch (ObjectDisposedException e)
			{
				HandleDisconnection(Target);

				return false;
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		protected virtual bool SendOverSocket(IPEndPoint EndPoint, BufferStream Buffer)
		{
			try
			{
				if (PacketLossSimulation == 0 || Constants.Random.NextDouble() > PacketLossSimulation)
					Socket.SendTo(Buffer.Buffer, EndPoint);

				Statistics.AddBandwidthOut(Buffer.Size);

				return true;
			}
			catch (SocketException e)
			{
				if (e.SocketErrorCode == SocketError.ConnectionReset ||
					e.SocketErrorCode == SocketError.ConnectionAborted ||
					e.SocketErrorCode == SocketError.ConnectionRefused)
				{
					return false;
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
			lock (events)
				events.Add(Event);
		}

		protected void AddSendCommand(SendCommand Command)
		{
			lock (sendCommands)
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
			lock (sendCommands)
			{
				for (int i = 0; i < sendCommands.Count; ++i)
				{
					SendCommand command = sendCommands[i];

					if (Timestamp < command.SendTime + (LatencySimulation / 1000.0F))
						continue;

					//if (!HandleSendCommand(sendCommands[i]))
					//	continue;
					HandleSendCommand(sendCommands[i]);

					sendCommands.RemoveAt(i--);
				}
			}
		}
	}
}