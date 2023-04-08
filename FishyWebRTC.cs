using FishNet.Managing;
using FishNet.Managing.Logging;
using FishNet.Managing.Transporting;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections.Generic;
using cakeslice.SimpleWebRTC;

namespace FishNet.Transporting.FishyWebRTC
{
	[DisallowMultipleComponent]
	public class FishyWebRTC : Transport
	{
		[SerializeField]
		/// <summary>
		/// ICE servers (STUN or TURN) to use.
		/// </summary>
		[Tooltip("ICE servers (STUN or TURN) to use. Use Google's server only for testing purposes!")]
		public List<Common.ICEServer> iceServers = new List<Common.ICEServer>(){
			new Common.ICEServer(){
				url = "stun:stun2.l.google.com:19302"
			}
		};

		const string serverOnlyException = "Hosting as a client is not implemented!";

		#region Serialized.

		[Header("Channels")]
		/// <summary>
		/// Maximum transmission unit for this transport.
		/// </summary>
		[Tooltip("Maximum transmission unit for the unreliable channel.")]
		[Range(MINIMUM_MTU, MAXIMUM_MTU)]
		[SerializeField]
		private int _mtu = 1023;

		[Header("Server")]
		/// <summary>
		/// Allowed domain origin.
		/// </summary>
		[Tooltip("Allowed domain origin.")]
		[SerializeField]
		private string _origin = "*";
		/// <summary>
		/// Port to use.
		/// </summary>
		[Tooltip("Port to use.")]
		[SerializeField]
		private ushort _port = 7770;
		/// <summary>
		/// Maximum number of players which may be connected at once.
		/// </summary>
		[Tooltip("Maximum number of players which may be connected at once.")]
		[Range(1, 9999)]
		[SerializeField]
		private int _maximumClients = 2000;

		[Header("Client")]
		/// <summary>
		/// Address to connect.
		/// </summary>
		[Tooltip("Use HTTPS for signaling.")]
		[SerializeField]
		private bool _HTTPS = true;
		/// <summary>
		/// Address to connect.
		/// </summary>
		[Tooltip("Signaling address to connect.")]
		[SerializeField]
		private string _clientAddress = "127.0.0.1";
		/// <summary>
		/// Port to connect.
		/// </summary>
		[Tooltip("If you want to connect to the signaling server without a port.")]
		[SerializeField]
		private bool _noClientPort = true;
		#endregion

		#region Private.
#if !UNITY_WEBGL
		/// <summary>
		/// Server socket and handler.
		/// </summary>
		private Server.ServerSocket _server = new Server.ServerSocket();
#endif
		/// <summary>
		/// Client socket and handler.
		/// </summary>
		private Client.ClientSocket _client = new Client.ClientSocket();
		#endregion

		#region Const.
		/// <summary>
		/// Minimum UDP packet size allowed.
		/// </summary>
		private const int MINIMUM_MTU = 576;
		/// <summary>
		/// Maximum UDP packet size allowed.
		/// </summary>
		private const int MAXIMUM_MTU = ushort.MaxValue;
		#endregion

		#region Initialization and unity.
		protected void OnDestroy()
		{
			Shutdown();
		}
		#endregion

		#region ConnectionStates.
		/// <summary>
		/// Gets the address of a remote connection Id.
		/// </summary>
		/// <param name="connectionId"></param>
		/// <returns></returns>
		public override string GetConnectionAddress(int connectionId)
		{
#if !UNITY_WEBGL
			return _server.GetConnectionAddress(connectionId);
#else
			throw new Exception(serverOnlyException);
#endif
		}

		/// <summary>
		/// Called when a connection state changes for the local client.
		/// </summary>
		public override event Action<ClientConnectionStateArgs> OnClientConnectionState;
		/// <summary>
		/// Called when a connection state changes for the local server.
		/// </summary>
		public override event Action<ServerConnectionStateArgs> OnServerConnectionState;
		/// <summary>
		/// Called when a connection state changes for a remote client.
		/// </summary>
		public override event Action<RemoteConnectionStateArgs> OnRemoteConnectionState;
		/// <summary>
		/// Gets the current local ConnectionState.
		/// </summary>
		/// <param name="server">True if getting ConnectionState for the server.</param>
		public override LocalConnectionState GetConnectionState(bool server)
		{
#if !UNITY_WEBGL
			if (server)
				return _server.GetConnectionState();
			else
#endif
			return _client.GetConnectionState();
		}
		/// <summary>
		/// Gets the current ConnectionState of a remote client on the server.
		/// </summary>
		/// <param name="connectionId">ConnectionId to get ConnectionState for.</param>
		public override RemoteConnectionState GetConnectionState(int connectionId)
		{
#if !UNITY_WEBGL
			return _server.GetConnectionState(connectionId);
#else
			throw new Exception(serverOnlyException);
#endif
		}
		/// <summary>
		/// Handles a ConnectionStateArgs for the local client.
		/// </summary>
		/// <param name="connectionStateArgs"></param>
		public override void HandleClientConnectionState(ClientConnectionStateArgs connectionStateArgs)
		{
			OnClientConnectionState?.Invoke(connectionStateArgs);
		}
		/// <summary>
		/// Handles a ConnectionStateArgs for the local server.
		/// </summary>
		/// <param name="connectionStateArgs"></param>
		public override void HandleServerConnectionState(ServerConnectionStateArgs connectionStateArgs)
		{
			OnServerConnectionState?.Invoke(connectionStateArgs);
		}
		/// <summary>
		/// Handles a ConnectionStateArgs for a remote client.
		/// </summary>
		/// <param name="connectionStateArgs"></param>
		public override void HandleRemoteConnectionState(RemoteConnectionStateArgs connectionStateArgs)
		{
			OnRemoteConnectionState?.Invoke(connectionStateArgs);
		}
		#endregion

		#region Iterating.
		/// <summary>
		/// Processes data received by the socket.
		/// </summary>
		/// <param name="server">True to process data received on the server.</param>
		public override void IterateIncoming(bool server)
		{
#if !UNITY_WEBGL
			if (server)
				_server.IterateIncoming();
			else
#endif
			_client.IterateIncoming();
		}

		/// <summary>
		/// Processes data to be sent by the socket.
		/// </summary>
		/// <param name="server">True to process data received on the server.</param>
		public override void IterateOutgoing(bool server)
		{
#if !UNITY_WEBGL
			if (server)
				_server.IterateOutgoing();
			else
#endif
			_client.IterateOutgoing();
		}
		#endregion

		#region ReceivedData.
		/// <summary>
		/// Called when client receives data.
		/// </summary>
		public override event Action<ClientReceivedDataArgs> OnClientReceivedData;
		/// <summary>
		/// Handles a ClientReceivedDataArgs.
		/// </summary>
		/// <param name="receivedDataArgs"></param>
		public override void HandleClientReceivedDataArgs(ClientReceivedDataArgs receivedDataArgs)
		{
			OnClientReceivedData?.Invoke(receivedDataArgs);
		}
		/// <summary>
		/// Called when server receives data.
		/// </summary>
		public override event Action<ServerReceivedDataArgs> OnServerReceivedData;
		/// <summary>
		/// Handles a ClientReceivedDataArgs.
		/// </summary>
		/// <param name="receivedDataArgs"></param>
		public override void HandleServerReceivedDataArgs(ServerReceivedDataArgs receivedDataArgs)
		{
			OnServerReceivedData?.Invoke(receivedDataArgs);
		}
		#endregion

		#region Sending.
		/// <summary>
		/// Sends to the server or all clients.
		/// </summary>
		/// <param name="channelId">Channel to use.</param>
		/// <param name="segment">Data to send.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void SendToServer(byte channelId, ArraySegment<byte> segment)
		{
			SanitizeChannel(ref channelId);
			_client.SendToServer(channelId, segment);
		}
		/// <summary>
		/// Sends data to a client.
		/// </summary>
		/// <param name="channelId"></param>
		/// <param name="segment"></param>
		/// <param name="connectionId"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void SendToClient(byte channelId, ArraySegment<byte> segment, int connectionId)
		{
#if !UNITY_WEBGL
			SanitizeChannel(ref channelId);
			_server.SendToClient(channelId, segment, connectionId);
#endif
		}
		#endregion

		#region Configuration.
		/// <summary>
		/// How long in seconds until either the server or client socket must go without data before being timed out.
		/// </summary>
		/// <param name="asServer">True to get the timeout for the server socket, false for the client socket.</param>
		/// <returns></returns>
		public override float GetTimeout(bool asServer)
		{
			return -1f;
		}
		/// <summary>
		/// Returns the maximum number of clients allowed to connect to the server. If the transport does not support this method the value -1 is returned.
		/// </summary>
		/// <returns></returns>
		public override int GetMaximumClients()
		{
#if !UNITY_WEBGL
			return _server.GetMaximumClients();
#else
			throw new Exception(serverOnlyException);
#endif
		}
		/// <summary>
		/// Sets maximum number of clients allowed to connect to the server. If applied at runtime and clients exceed this value existing clients will stay connected but new clients may not connect.
		/// </summary>
		/// <param name="value"></param>
		public override void SetMaximumClients(int value)
		{
#if !UNITY_WEBGL
			if (_server.GetConnectionState() != LocalConnectionState.Stopped)
			{
				if (base.NetworkManager.CanLog(LoggingType.Warning))
					Debug.LogWarning($"Cannot set maximum clients when server is running.");
			}
			else
			{
				_maximumClients = value;
			}
#else
			throw new Exception(serverOnlyException);
#endif
		}
		/// <summary>
		/// Sets which address the client will connect to.
		/// </summary>
		/// <param name="address"></param>
		public override void SetClientAddress(string address)
		{
			_clientAddress = address;
		}
		/// <summary>
		/// Gets which address the client will connect to.
		/// </summary>
		public override string GetClientAddress()
		{
			return _clientAddress;
		}

		/// <summary>
		/// Sets which address the server will bind to.
		/// </summary>
		/// <param name="address"></param>
		public override void SetServerBindAddress(string address, IPAddressType addressType) { }
		/// <summary>
		/// Gets which address the server will bind to.
		/// </summary>
		/// <param name="address"></param>
		public override string GetServerBindAddress(IPAddressType addressType)
		{
			return "localhost";
		}
		/// <summary>
		/// Sets which port to use.
		/// </summary>
		/// <param name="port"></param>
		public override void SetPort(ushort port)
		{
			_port = port;
		}
		/// <summary>
		/// Gets which port to use.
		/// </summary>
		/// <param name="port"></param>
		public override ushort GetPort()
		{
			return _port;
		}
		#endregion

		#region Start and stop.
		/// <summary>
		/// Starts the local server or client using configured settings.
		/// </summary>
		/// <param name="server">True to start server.</param>
		public override bool StartConnection(bool server)
		{
#if !UNITY_WEBGL
			if (server)
				return StartServer();
			else
#endif
			return StartClient(_clientAddress);
		}

		/// <summary>
		/// Stops the local server or client.
		/// </summary>
		/// <param name="server">True to stop server.</param>
		public override bool StopConnection(bool server)
		{
#if !UNITY_WEBGL
			if (server)
				return StopServer();
			else
#endif
			return StopClient();
		}

		/// <summary>
		/// Stops a remote client from the server, disconnecting the client.
		/// </summary>
		/// <param name="connectionId">ConnectionId of the client to disconnect.</param>
		/// <param name="immediately">True to abrutly stp the client socket without waiting socket thread.</param>
		public override bool StopConnection(int connectionId, bool immediately)
		{
#if !UNITY_WEBGL
			return StopClient(connectionId, immediately);
#else
			throw new Exception(serverOnlyException);
#endif
		}

		/// <summary>
		/// Stops both client and server.
		/// </summary>
		public override void Shutdown()
		{
			//Stops client then server connections.
			StopConnection(false);
			StopConnection(true);
		}

		#region Privates.
		/// <summary>
		/// Starts server.
		/// </summary>
		private bool StartServer()
		{
#if !UNITY_WEBGL
			_server.Initialize(this, _mtu);
			return _server.StartConnection(iceServers, _port, _maximumClients, _origin);
#else
			throw new Exception(serverOnlyException);
#endif
		}

		/// <summary>
		/// Stops server.
		/// </summary>
		private bool StopServer()
		{
#if !UNITY_WEBGL
			return _server.StopConnection();
#else
			throw new Exception(serverOnlyException);
#endif
		}

		/// <summary>
		/// Starts the client.
		/// </summary>
		/// <param name="address"></param>
		private bool StartClient(string address)
		{
			_client.Initialize(this, _mtu);
			return _client.StartConnection(iceServers, address, _HTTPS, _port, _noClientPort);
		}

		/// <summary>
		/// Stops the client.
		/// </summary>
		private bool StopClient()
		{
			return _client.StopConnection();
		}

#if !UNITY_WEBGL
		/// <summary>
		/// Stops a remote client on the server.
		/// </summary>
		/// <param name="connectionId"></param>
		/// <param name="immediately">True to abrutly stp the client socket without waiting socket thread.</param>
		private bool StopClient(int connectionId, bool immediately)
		{
			return _server.StopConnection(connectionId, immediately);
		}
#endif
		#endregion
		#endregion

		#region Channels.
		/// <summary>
		/// If channelId is invalid then channelId becomes forced to reliable.
		/// </summary>
		/// <param name="channelId"></param>
		private void SanitizeChannel(ref byte channelId)
		{
			if (channelId < 0 || channelId >= TransportManager.CHANNEL_COUNT)
			{
				if (NetworkManager.CanLog(LoggingType.Warning))
					Debug.LogWarning($"Channel of {channelId} is out of range of supported channels. Channel will be defaulted to reliable.");
				channelId = 0;
			}
		}
		/// <summary>
		/// Gets the MTU for a channel. This should take header size into consideration.
		/// For example, if MTU is 1200 and a packet header for this channel is 10 in size, this method should return 1190.
		/// </summary>
		/// <param name="channel"></param>
		/// <returns></returns>
		public override int GetMTU(byte channel)
		{
			return _mtu;
		}
		#endregion

		#region Editor.
#if UNITY_EDITOR
		private void OnValidate()
		{
			if (_mtu < 0)
				_mtu = MINIMUM_MTU;
			else if (_mtu > MAXIMUM_MTU)
				_mtu = MAXIMUM_MTU;
		}
#endif
		#endregion
	}
}
