using Core.Character;
using RiptideNetworking;

namespace Core.Events
{
	public class DeathEventArgs : TriggerEventArgs
	{
		public Entity Killer { get; set; }

		public DeathEventArgs()
		{
		}

		public DeathEventArgs(Entity killer)
		{
			Killer = killer;
		}

		public override Message Serialize(Message message)
		{
			message.AddUShort(Killer.Id);
			return message;
		}

		public override void Deserialize(Message message)
		{
			Killer = Entity.EntityDict[message.GetUShort()];
		}
	}
}