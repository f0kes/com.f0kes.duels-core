using Core.Combat;
using RiptideNetworking;

namespace Core.Events
{
	public class DamageEventArgs : TriggerEventArgs
	{
		public Damage Damage { get; set; }

		public DamageEventArgs(Damage damage)
		{
			Damage = damage;
		}

		public DamageEventArgs()
		{
		}

		public override Message Serialize(Message message)
		{
			message = Damage.Serialize(message);
			return message;
		}

		public override void Deserialize(Message message)
		{
			Damage = new Damage(message);
		}
	}
}