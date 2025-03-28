namespace celestia.network
{
	public abstract class Packet
	{
		public enum PacketType
		{
			HEARTBEAT,
		}

		public PacketType Type { get; protected set; }

		public abstract void Serialize(ref NetworkWriter writer);
		public abstract void Deserialize(ref NetworkReader reader);
	}
}