namespace Core.State
{
	public interface IState
	{
		void Tick();
		void OnEnter();
		void OnExit();
	}
}