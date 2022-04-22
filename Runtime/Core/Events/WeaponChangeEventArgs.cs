using Core.Combat;
using RiptideNetworking;

namespace Core.Events
{
	public class WeaponChangeEventArgs : TriggerEventArgs
	{
		public byte WeaponId { get; private set; }

		public WeaponChangeEventArgs()
		{
		}

		public WeaponChangeEventArgs(byte weaponId)
		{
			WeaponId = weaponId;
		}

		public override Message Serialize(Message message)
		{
			message.AddByte(WeaponId);
			return message;
		}

		public override void Deserialize(Message message)
		{
			WeaponId = message.GetByte();
		}
	}
}