using System;
using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;
using UnityEngine;

namespace Core.Types
{
	[EnumerableType]
	public class Identity : IIdentifiable
	{
		public struct IdentityPoint
		{
			public Type Type;
			public ushort Index;
		}

		private static Identity _root;
		public static Identity Root => _root ??= new Identity(new IdentityPoint {Type = typeof(Identity), Index = 0});
		public IIdentifiable IdentityObject { get; private set; }
		public IdentityPoint ID { get; private set; }


		private readonly LinkedList<Identity> _identityPath;
		private readonly Dictionary<Type, List<ushort>> _indexes;
		private readonly Dictionary<IdentityPoint, Identity> _identityChildren;

		public Identity(IdentityPoint point)
		{
			ID = point;
			_identityPath = new LinkedList<Identity>();
			_indexes = new Dictionary<Type, List<ushort>>();
			_identityChildren = new Dictionary<IdentityPoint, Identity>();
		}

		private Identity(LinkedList<Identity> initialPath, IdentityPoint point)
		{
			_identityPath = new LinkedList<Identity>(initialPath);
			_identityPath.AddLast(this);
			ID = point;
			_indexes = new Dictionary<Type, List<ushort>>();
			_identityChildren = new Dictionary<IdentityPoint, Identity>();
		}

		private ushort GetNextIdForType(Type type)
		{
			if (!_indexes.ContainsKey(type))
			{
				_indexes.Add(type, new List<ushort>());
			}

			var list = _indexes[type];
			if (list.Count == 0)
			{
				list.Add(0);
				return 0;
			}

			var max = list.Max();
			list.Add((ushort) (max + 1));
			return (ushort) (max + 1);
		}

		public Identity GenerateChild(IIdentifiable obj)
		{
			var type = obj.GetType();
			return GenerateChild(obj, GetNextIdForType(type));
		}

		public Identity GenerateChild(IIdentifiable obj, ushort index)
		{
			IdentityObject = obj;
			var type = obj.GetType();
			var point = new IdentityPoint {Type = type, Index = index};
			var newIdentity = new Identity(_identityPath, point);
			_identityChildren[point] = newIdentity;
			if (!_indexes.ContainsKey(type))
			{
				_indexes[type] = new List<ushort>();
			}

			_indexes[type].Add(index);
			return newIdentity;
		}

		public void RemoveChild(Identity child)
		{
			_identityChildren.Remove(child.ID);
			_indexes[child.ID.Type].Remove(child.ID.Index);
		}

		public ushort[] GetPath()
		{
			Debug.Log(GetPathString(_identityPath));
			var path = new ushort[_identityPath.Count * 2];
			var i = 0;
			foreach (Identity identity in _identityPath)
			{
				path[i] = InterfaceChildIDGetter<IIdentifiable>.GetIDByType(identity.ID.Type);
				path[i++] = identity.ID.Index;
				i++;
			}

			return path;
		}

		public Identity GetIdentityByPath(ushort[] path)
		{
			LinkedList<Identity> pathList = GetPathFromShorts(path);
			Debug.Log(GetPathString(pathList));
			
			var typeID = path[0];
			var index = path[1];
			var type = InterfaceChildIDGetter<IIdentifiable>.GetTypeById(typeID);
			var point = new IdentityPoint() {Type = type, Index = index};
			Array.Copy(path, 2, path, 0, path.Length - 2);
			return path.Length == 0 ? this : _identityChildren[point].GetIdentityByPath(path);
		}

		private LinkedList<Identity> GetPathFromShorts(ushort[] path)
		{
			var list = new LinkedList<Identity>();
			var i = 0;
			while (i < path.Length)
			{
				var typeID = path[i];
				var index = path[i + 1];
				var type = InterfaceChildIDGetter<IIdentifiable>.GetTypeById(typeID);
				var point = new IdentityPoint() {Type = type, Index = index};
				list.AddLast(_identityChildren[point]);
				i += 2;
			}

			return list;
		}

		private string GetPathString(LinkedList<Identity> path)
		{
			string ps = "";
			foreach (var p in path)
			{
				ps += p.ID.Type.Name + "." + p.ID.Index + "." + "\n" ;
			}

			return ps;
		}
	}
}