using System.IO;
using celestia.math;

namespace celestia.network
{
	public class NetworkReader
	{
		private MemoryStream _stream;
		private BinaryReader _reader;
		
		public NetworkReader(byte[] data)
		{
			_stream = new MemoryStream(data);
			_reader = new BinaryReader(_stream);
		}
		
		#region Native types
		public bool ReadBool()
		{
			return _reader.ReadBoolean();
		}
		
		public int ReadInt()
		{
			return _reader.ReadInt32();
		}
		#endregion
		
		#region Celestia types
		public TilePos ReadTilePos()
		{
			var x = _reader.ReadInt32();
			var y = _reader.ReadInt32();
			return new TilePos(x, y);
		}
		#endregion
	}
}