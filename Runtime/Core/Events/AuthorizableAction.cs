using System;
using System.Collections.Generic;
using Core.Interfaces;
using Core.Types;
using UnityEngine;

namespace Core.Events
{
	public class AuthorizableAction
	{
		public bool Authorized { get; set; }
		private Action<TriggerEventArgs> AuthorizedAction { get; set; }
		private Action<TriggerEventArgs> UnauthorizedAction { get; set; }

		public void Subscribe(Action<TriggerEventArgs> action, bool needsToBeAuthorised)
		{
			if (needsToBeAuthorised)
			{
				AuthorizedAction -= action;
				AuthorizedAction += action;
			}
			else
			{
				UnauthorizedAction -= action;
				UnauthorizedAction += action;
			}
		}

		public void Invoke(TriggerEventArgs args, bool forceAuthorization = false)
		{
			if (Authorized || forceAuthorization)
			{
				AuthorizedAction?.Invoke(args);
			}

			UnauthorizedAction?.Invoke(args);
		}
	}

	public class AuthorizableActionSync : AuthorizableAction, IIdentifiable

	{
		private Identity _identity;

		public AuthorizableActionSync(Identity parentIdentity)
		{
			_identity = parentIdentity.GenerateChild(this);
			Subscribe((args) => ActionSyncCollector.OnAnyAction?.Invoke(_identity, args), false);
			if (ActionSyncCollector.IsAuthorized)
			{
				Authorized = true;
			}

			ActionSyncCollector.OnAuthorizeAll += Authorize;
		}

		private void Authorize()
		{
			Authorized = true;
		}
	}

	public static class ActionSyncCollector
	{
		public static bool IsAuthorized { get; private set; }
		public static Action<Identity, TriggerEventArgs> OnAnyAction { get; set; }
		public static Action OnAuthorizeAll { get; set; }


		public static void AuthorizeAll()
		{
			IsAuthorized = true;
			OnAuthorizeAll?.Invoke();
		}
	}
}