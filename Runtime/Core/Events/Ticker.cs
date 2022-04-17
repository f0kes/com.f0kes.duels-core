using System;
using System.Threading.Tasks;
using UnityEngine;

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
		public static async void InvokeInTime(Action toInvoke, float time)
		{
			float timePassed = 0;
			OnTick += (tick)=> { timePassed += Time.fixedDeltaTime; };
			while (timePassed<time)
			{
				await Task.Yield();
			}
			toInvoke.Invoke();
		}
	}
}