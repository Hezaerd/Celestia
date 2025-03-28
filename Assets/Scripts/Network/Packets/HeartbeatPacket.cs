namespace celestia.network.packets
{
	public class HeartbeatPacket : Packet
	{
		public bool IsResponse { get; set; }
		
		public HeartbeatPacket()
		{
			Type = PacketType.HEARTBEAT;
		}

		public override void Serialize(ref NetworkWriter writer)
		{
			writer.WriteBool(IsResponse);
		}

		public override void Deserialize(ref NetworkReader reader)
		{
			IsResponse = reader.ReadBool();
		}
	}
}