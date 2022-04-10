using System.Collections.Generic;
using Core.Interfaces;
using UnityEngine;

namespace Core.Character
{
	public abstract class Player : MonoBehaviour, IPlayer
	{
		public static Dictionary<ushort, Player> _playerDict = new Dictionary<ushort, Player>();

		public ushort Id { get; protected set; }
		public string Username { get; protected  set; }
		

	}
}