using System.IO;
using RiptideNetworking;

namespace Core.Interfaces
{
	public interface ISerializableData
	{
		Message Serialize(Message message);
		void Deserialize(Message message);
	}
}