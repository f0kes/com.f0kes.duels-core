using System;
using System.Collections.Generic;

namespace Core.Types
{
	public class Identity
	{
		private static Identity _root;
		public static Identity Root => _root ??= new Identity(new IdentityPoint());
		public struct IdentityPoint
		{
			public Type Type;
			public ushort Index;
		}
		

		private IdentityPoint _id;
		private LinkedList<Identity> _identityPath;
		private Dictionary<Type, ushort> _indexes;
		private Dictionary<IdentityPoint, Identity> _identityChildren;

		public Identity(IdentityPoint point)
		{
			_id = point;
			_identityPath = new LinkedList<Identity>();
			_indexes = new Dictionary<Type, ushort>();
			_identityChildren = new Dictionary<IdentityPoint, Identity>();
		}
		public Identity(LinkedList<Identity> initialPath, IdentityPoint point)
		{
			_identityPath = new LinkedList<Identity>(initialPath);
			_identityPath.AddLast(this);
			_id = point;
			_indexes = new Dictionary<Type, ushort>();
			_identityChildren = new Dictionary<IdentityPoint, Identity>();
		}

		public Identity AddChild(Type type)
		{
			IdentityPoint point;
			if (_indexes.ContainsKey(type))
			{
				point = new IdentityPoint() {Type = type, Index = _indexes[type]++};
			}
			else
			{
				 point = new IdentityPoint() {Type = type, Index = 0};
				_indexes.Add(type, 0);
			}
			Identity newIdentity = new Identity(_identityPath, point);
			_identityChildren.Add(point, newIdentity);
			return newIdentity;
		}
	}
}