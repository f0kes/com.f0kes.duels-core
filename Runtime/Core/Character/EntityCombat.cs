using System;
using System.Collections.Generic;
using System.Linq;
using Core.Combat;
using Core.CoreEnums;
using Core.Enums;
using Core.Events;
using Core.Stats;
using UnityEngine;

namespace Core.Character
{
	public class EntityCombat : MonoBehaviour
	{
		//bare hands are always on last index
		public const byte BareHandsIndex = 6;

		public Action<Damage> OnAttack;
		public Action<Weapon[]> OnWeaponsChanged;
		public Action<ushort, byte> OnWeaponChange;

		[SerializeField] private WeaponObject[] _weaponObjects = new WeaponObject[6];
		[SerializeField] private WeaponObject _bareHands;
		private Weapon[] _weapons = new Weapon[BareHandsIndex + 1];
		private Weapon _currentWeapon;

		private CombatState _combatState = CombatState.Idle;


		private StatDict<AttributeStat> _attributes = new StatDict<AttributeStat>();
		private StatDict<BasedStat> _stats = new StatDict<BasedStat>();

		private int _weaponChangeTokens = 5;

		private ushort _entityId;

		public void Init(Entity entity, StatDict<AttributeStat> characterAttributes,
			StatDict<BasedStat> characterStats)
		{
			_entityId = entity.Id;
			_attributes = characterAttributes;
			_stats = characterStats;
			int i = 0;
			foreach (var weaponObject in _weaponObjects)
			{
				if (weaponObject == null)
				{
					continue;
				}

				Weapon weapon = new Weapon(weaponObject, characterStats);
				AddWeapon(weapon, i);
				i++;
			}

			Weapon bareHandsWeapon = new Weapon(_bareHands, characterStats);
			AddWeapon(bareHandsWeapon, BareHandsIndex);

			if (i > 0)
				ChangeWeapon(0);
			else
			{
				ChangeWeapon(BareHandsIndex);
			}


			EventTrigger.I[_entityId, ActionType.OnAttackStarted].Subscribe(Attack, true);
		}

		public Weapon GetWeapon()
		{
			return _currentWeapon;
		}

		public Weapon[] GetWeapons()
		{
			return new List<Weapon>(_weapons).ToArray();
		}

		public void AddWeapon(Weapon weapon, int index)
		{
			_weapons[index] = weapon;
			weapon.Equip(_stats, _entityId);
			OnWeaponsChanged?.Invoke(GetWeapons());
		}

		public void RemoveWeapon(int index)
		{
			_weapons[index] = null;
			OnWeaponsChanged?.Invoke(GetWeapons());
		}

		public void ChangeWeapon(byte index, bool useToken = false)
		{
			if (_weapons[index] == null || _currentWeapon == _weapons[index] || _weapons[index].IsBroken)
				return;

			if (useToken)
			{
				if (_weaponChangeTokens <= 0)
				{
					_currentWeapon.Break();
				}

				_weaponChangeTokens--;
			}


			SetWeapon(index);
		}

		private void SetWeapon(byte index)
		{
			if (_currentWeapon != null)
			{
				_currentWeapon.OnBreak -= OnWeaponBreak;
				_currentWeapon.OnWeaponStateChanged -= OnWeaponStateChanged;
			}

			_currentWeapon = _weapons[index];
			_currentWeapon.OnBreak += OnWeaponBreak;
			_currentWeapon.OnWeaponStateChanged += OnWeaponStateChanged;

			EventTrigger.I[_entityId, ActionType.OnWeaponChanged].Invoke(new WeaponChangeEventArgs(index));
			OnWeaponChange?.Invoke(_entityId, index);
			
			
		}

		private void OnWeaponStateChanged(CombatState state)
		{
			_combatState = state;
		}

		private void OnWeaponBreak()
		{
			SetWeapon(BareHandsIndex);
		}


		public void Attack(TriggerEventArgs args)
		{
			if (_currentWeapon == null)
				return;
			_currentWeapon.EnqueueAttack();
		}


		private bool IsCharacterAttackable(Entity entity)
		{
			return entity.gameObject != gameObject;
		}
	}
}