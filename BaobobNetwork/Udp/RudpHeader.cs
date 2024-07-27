namespace BaobabNetwork.Udp
{
	using System;
	using System.Runtime.InteropServices;

	internal struct RudpHeader
	{
		public int SequenceNumber { get; set; }
		public int Checksum { get; set; }
	}
}