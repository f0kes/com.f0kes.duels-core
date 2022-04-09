using System.IO;

namespace Core.Interfaces
{
	public interface ISerializableData
	{
		void Serialize(BinaryWriter bw);
		void Deserialize(BinaryReader br);
	}
}