using System;
using System.Collections.Generic;
using Core.Interfaces;
using Core.Types;

namespace Core.Events
{
	public class AuthorizableAction<TArgs> where TArgs : TriggerEventArgs
	{
		public bool Authorized { get; set; }
		private Action<TArgs> AuthorizedAction { get; set; }
		private Action<TArgs> UnauthorizedAction { get; set; }

		public void Subscribe(Action<TArgs> action, bool needsToBeAuthorised)
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

		public void Invoke(TArgs args, bool forceAuthorization = false)
		{
			if (Authorized || forceAuthorization)
			{
				AuthorizedAction?.Invoke(args);
			}

			UnauthorizedAction?.Invoke(args);
		}
	}

	public class AuthorizableActionSync<TArgs> : AuthorizableAction<TArgs>, IIdentifiable where TArgs : TriggerEventArgs

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
			ActionSyncCollector.OnAuthorizeAll+=Authorize;
		}

		private void Authorize()
		{
			Authorized = true;
		}
	}

	public static class ActionSyncCollector
	{
		public static bool IsAuthorized { get;private set; }
		public static Action<Identity, TriggerEventArgs> OnAnyAction { get; set; }
		public static Action OnAuthorizeAll { get; set; }

		public static void AuthorizeAll()
		{
			IsAuthorized = true;
			OnAuthorizeAll?.Invoke();
		}
	}
}