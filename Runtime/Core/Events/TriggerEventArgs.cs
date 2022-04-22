using System;
using RiptideNetworking;

namespace Core.Events
{
	public abstract class TriggerEventArgs : EventArgs
	{
		public abstract Message Serialize(Message message);
		public abstract void Deserialize(Message message);
	}
}