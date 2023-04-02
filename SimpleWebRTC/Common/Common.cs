using System.Threading;
using System;

namespace cakeslice.SimpleWebRTC
{
	public static class Common
	{
		[Serializable]
		public enum DeliveryMethod : byte
		{
			Unreliable = 4,
			ReliableOrdered = 2,
		}

		public enum EventType
		{
			Connected,
			Data,
			Disconnected,
			Error
		}

		public static void CheckForInterrupt()
		{
			// sleep in order to check for ThreadInterruptedException
			Thread.Sleep(1);
		}
	}
}
