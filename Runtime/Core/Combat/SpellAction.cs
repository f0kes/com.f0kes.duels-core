using System;
using System.Collections.Generic;
using Core.Character;
using Core.Combat;
using UnityEngine;

namespace Combat
{
	public class SpellAction : ScriptableObject
	{
		[SerializeField] private List<SpellAction> _toAdd = new List<SpellAction>() { };

		protected List<SpellAction> _actions;

		private void Awake()
		{
			_actions = new List<SpellAction>();
			_actions.AddRange(_toAdd);
		}

		public bool IsEmpty()
		{
			return _actions.Count == 0;
		}

		public void AddAction(SpellAction spellAction)
		{
			_actions.Add(spellAction);
		}

		public void RemoveAction(SpellAction spellAction)
		{
			if (_actions.Contains(spellAction))
			{
				_actions.Remove(spellAction);
			}
		}

		public virtual void Perform(List<Entity> victims, Entity caster, Attack attack)
		{
			if (_actions.Count == 0) return;
			foreach (var action in _actions)
			{
				action.Perform(victims, caster, attack);
			}
		}

		public void Perform(Entity victim, Entity caster, Attack attack)
		{
			List<Entity> victims = new List<Entity> {victim};
			Perform(victims, caster, attack);
		}
	}
}