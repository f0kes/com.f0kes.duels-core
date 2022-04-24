using System;
using Core.CoreEnums;

namespace Core.Combat
{
	public class CombatStateContainer
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
	}
}