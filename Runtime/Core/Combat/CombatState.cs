using System;
using Core.CoreEnums;
using Core.Interfaces;
using RiptideNetworking;

namespace Core.Combat
{
	public class CombatStateContainer : ISerializableData
	{
		public Action<CombatState, Attack> OnStateChanged;
		public CombatState CurrentState { get; private set; }
		public Attack CurrentAttack { get; private set; }

		public CombatStateContainer()
		{
			CurrentState = CombatState.Idle;
			CurrentAttack = null;
		}

		public void ChangeState(CombatState state, Attack attack)
		{
			CurrentState = state;
			CurrentAttack = attack;
			OnStateChanged?.Invoke(state, attack);
		}

		public void ChangeState(CombatState state)
		{
			CurrentState = state;
			OnStateChanged?.Invoke(state, CurrentAttack);
		}


		public Message Serialize(Message message)
		{
			message.AddUShort((ushort)CurrentState);
			CurrentAttack.Serialize(message);
			return message;
		}

		public void Deserialize(Message message)
		{
			throw new NotImplementedException();
		}
	}
}