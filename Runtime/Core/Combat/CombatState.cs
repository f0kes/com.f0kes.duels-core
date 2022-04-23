using Core.Combat;
using Core.State;

namespace Core.CoreEnums
{
	public abstract class CombatState : IState
	{
		public Attack CurrentAttack { get; set; }

		public abstract void Tick();

		public abstract void OnEnter();

		public abstract void OnExit();
	}

	public enum CombatStateType : ushort
	{
		Idle = 0,
		PreparingAttack,
		Attacking,
		Cooldown,
	}
}