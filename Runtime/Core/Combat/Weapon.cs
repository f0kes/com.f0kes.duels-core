using System;
using System.Collections.Generic;
using System.Linq;
using Core.Character;
using Core.CoreEnums;
using Core.Enums;
using Core.Events;
using Core.Interfaces;
using Core.StatResource;
using Core.Stats;
using RiptideNetworking;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Combat
{
	public class Weapon
	{
		public Action<float> OnStaminaChanged;
		public Action OnBreak;
		public Action<CombatState, Attack> OnWeaponStateChanged;

		public WeaponType WeaponType { get; private set; }
		public WeaponName WeaponName { get; private set; }


		private List<Attack> Attacks;


		private int _currentAttackCount = 0;


		private bool _isBroken;


		public ResourceContainer Stamina { get; private set; }
		public CombatStateContainer CombatStateContainer { get; private set; }
		
		private WeaponQueueProcessor _weaponQueueProcessor;
		

		public bool IsBroken => _isBroken;

		public Weapon(WeaponObject weaponObject,
			VictimGetter victimGetter, Entity wielder, Stat staminaStat, CombatStateContainer combatStateContainer)
		{
			WeaponType = weaponObject.Type;

			Attacks = new List<Attack>();
			WeaponName = weaponObject.WeaponName;
			foreach (var attack in weaponObject.Attacks)
			{
				Attacks.Add(new Attack(attack));
			}

			Stamina = new ResourceContainer(staminaStat);
			Stamina.OnValueChanged += (newStamina)=> OnStaminaChanged?.Invoke(newStamina);
			Stamina.OnDepleted += ()=> OnBreak?.Invoke();

			CombatStateContainer = combatStateContainer;
			_weaponQueueProcessor = new WeaponQueueProcessor(victimGetter, wielder, Stamina, combatStateContainer);
			Init();
		}


		public void Init()
		{
			Ticker.OnTick += OnTick;
		}

		private void OnTick(ushort obj)
		{
			_weaponQueueProcessor.Tick();
		}


		public void EnqueueAttack()
		{
			if (!CanAttack())
			{
				return;
			}

			_weaponQueueProcessor.EnqueueAttack(GetNextAttack(), Attacks.Last());
		}


		private Attack GetNextAttack(bool setCurrentCount = true)
		{
			int nextAttackCount = (_currentAttackCount + 1) % Attacks.Count;
			Attack result = Attacks[nextAttackCount];
			if (setCurrentCount)
			{
				_currentAttackCount = nextAttackCount;
			}

			return result;
		}

		private bool CanAttack()
		{
			var staminaCost = GetNextAttack(false).StaminaCost;
			if (Stamina.CurrentValue <= 0)
			{
				return false;
			}

			return !_isBroken;
		}


		public Message Serialize(Message message)
		{
			// message.AddUShort((ushort) _currentAttackCount);
			// Stamina.Serialize(message);
			// CombatStateContainer.Serialize(message);
			// _weaponQueueProcessor.Serialize(message);
			return message;
		}

		public void Deserialize(Message message)
		{
			throw new NotImplementedException();
		}

		public void Break()
		{
			_isBroken = true;
			OnBreak?.Invoke();
		}
	}
}