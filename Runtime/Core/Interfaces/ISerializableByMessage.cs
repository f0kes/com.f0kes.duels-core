using RiptideNetworking;

namespace Core.Interfaces
{
	public interface ISerializableByMessage
	{
		Message Serialize(Message message, ushort onTick);
		void Deserialize(Message message, ushort onTick);
	}
}