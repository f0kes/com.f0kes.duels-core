using System;
using System.Collections.Generic;
using Core.CoreEnums;
using Core.Enums;
using UnityEngine;

namespace Core.Combat
{
	[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
	public class WeaponObject : ScriptableObject
	{
		public List<Attack> Attacks;
		public WeaponName WeaponName;
		public WeaponType Type;
		public AttributeStat BaseAttribute;


	}
}