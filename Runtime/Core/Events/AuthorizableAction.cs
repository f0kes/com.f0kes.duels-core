using System;

namespace Core.Events
{
	public class AuthorizableAction<TArgs>
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

	public class SearchableAuthorizableAction<TArgs> : AuthorizableAction<TArgs>
	{
		public SearchableAuthorizableAction()
		{
			
		}
	}
}