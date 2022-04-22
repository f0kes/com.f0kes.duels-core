using RiptideNetworking;

namespace Core.Events
{
	public class EmptyEventArgs : TriggerEventArgs
	{
		public override Message Serialize(Message message)
		{
			return message;
		}

		public override void Deserialize(Message message)
		{
		}
	}
}