using System;
using Core.CoreEnums;
using Core.Interfaces;
using RiptideNetworking;
using UnityEngine;

namespace Core.Combat
{
	public class CombatStateContainer : ISerializableData
	{
		public Action<CombatState, Attack> OnStateChanged;
		public CombatState CurrentState { get; private set; }
		public Attack CurrentAttack { get; private set; }
		public float CurrentAttackTime { get; private set; }

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
			CurrentAttackTime = 0;
			CurrentState = state;
			OnStateChanged?.Invoke(state, CurrentAttack);
		}


		public Message Serialize(Message message)
		{
			message.AddUShort((ushort) CurrentState);
			var serializeAttack = CurrentAttack != null;
			message.AddBool(serializeAttack);
			if (serializeAttack)
			{
				CurrentAttack.Serialize(message);
			}

			message.AddFloat(CurrentAttackTime);
			return message;
		}

		public void Deserialize(Message message)
		{
			CurrentState = (CombatState) message.GetUShort();
			var serializeAttack = message.GetBool();
			if (serializeAttack)
			{
				CurrentAttack = new Attack(message);
			}
			CurrentAttackTime = message.GetFloat();
		}

		public void Tick()
		{
			CurrentAttackTime += Time.fixedDeltaTime;
		}
	}
}