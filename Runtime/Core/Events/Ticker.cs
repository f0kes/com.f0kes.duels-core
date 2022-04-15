using System;

namespace Core.Events
{
	public static class Ticker
	{
		public static Action<ushort> OnTick;
		public static ushort CurrentTick { get; private set; } = 0;

		public static void Tick()
		{
			CurrentTick++;
			OnTick?.Invoke(CurrentTick);
		}
	}
}