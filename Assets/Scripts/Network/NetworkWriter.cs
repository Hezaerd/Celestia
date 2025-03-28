using System.IO;
using celestia.math;

namespace celestia.network
{
	public class NetworkWriter
	{
		private MemoryStream _stream;
		private BinaryWriter _writer;

		public NetworkWriter()
		{
			_stream = new MemoryStream();
			_writer = new BinaryWriter(_stream);
		}
		
		
		#region Native types
		public void WriteBool(bool value)
		{
			_writer.Write(value);
		}
		
		public void WriteInt(int value)
		{
			_writer.Write(value);
		}
		#endregion
		
		#region Celestia types
		public void WriteTilePos(TilePos tilePos)
		{
			_writer.Write(tilePos.x);
			_writer.Write(tilePos.y);
		}
		#endregion
		
		public byte[] ToArray()
		{
			return _stream.ToArray();
		}
	}
}