using System;

namespace Core.CoreEnums
{
	[Flags]
	public enum EntityState
	{
		Dead = 0,
		Attacking = 1,
		Moving = 1 << 1,
		Idle = 1 << 2,
		CanMove = 1 << 3,
		CanAttack = 1 << 4,
		CanBeAttacked = 1 << 5,
	}
}