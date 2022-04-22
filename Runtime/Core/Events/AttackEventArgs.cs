using Core.Combat;
using RiptideNetworking;

namespace Core.Events
{
	public class AttackEventArgs : TriggerEventArgs
	{
		public Attack Attack;

		public AttackEventArgs(Attack attack)
		{
			Attack = attack;
		}

		public AttackEventArgs()
		{
		}

		public override Message Serialize(Message message)
		{
			message = Attack.Serialize(message);
			return message;
		}

		public override void Deserialize(Message message)
		{
			Attack = new Attack(message);
		}
	}
}