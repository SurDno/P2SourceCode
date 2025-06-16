using System;
using System.Linq;
using System.Reflection;
using ParadoxNotion.Serialization.FullSerializer.Internal;
using UnityEngine;

namespace ParadoxNotion.Serialization;

[Serializable]
public class SerializedMethodInfo : ISerializationCallbackReceiver {
	[SerializeField] private string _returnInfo;
	[SerializeField] private string _baseInfo;
	[SerializeField] private string _paramsInfo;
	[NonSerialized] private MethodInfo _method;
	[NonSerialized] private bool _hasChanged;

	void ISerializationCallbackReceiver.OnBeforeSerialize() {
		_hasChanged = false;
		if (!(_method != null))
			return;
		_returnInfo = _method.ReturnType.FullName;
		_baseInfo = string.Format("{0}|{1}", _method.RTReflectedType().FullName, _method.Name);
		_paramsInfo = string.Join("|", _method.GetParameters().Select(p => p.ParameterType.FullName).ToArray());
	}

	void ISerializationCallbackReceiver.OnAfterDeserialize() {
		_hasChanged = false;
		var strArray1 = _baseInfo.Split('|');
		var type1 = fsTypeCache.GetType(strArray1[0], null);
		if (type1 == null)
			_method = null;
		else {
			var name = strArray1[1];
			string[] strArray2;
			if (!string.IsNullOrEmpty(_paramsInfo))
				strArray2 = _paramsInfo.Split('|');
			else
				strArray2 = null;
			var source = strArray2;
			var typeArray = source == null ? new Type[0] : source.Select(n => fsTypeCache.GetType(n, null)).ToArray();
			if (typeArray.All(t => t != null)) {
				_method = type1.RTGetMethod(name, typeArray);
				if (!string.IsNullOrEmpty(_returnInfo)) {
					var type2 = fsTypeCache.GetType(_returnInfo, null);
					if (_method != null && type2 != _method.ReturnType)
						_method = null;
				}
			}

			if (!(_method == null))
				return;
			_hasChanged = true;
			_method = type1.RTGetMethods().FirstOrDefault(m => m.Name == name);
		}
	}

	public SerializedMethodInfo() { }

	public SerializedMethodInfo(MethodInfo method) {
		_hasChanged = false;
		_method = method;
	}

	public MethodInfo Get() {
		return _method;
	}

	public bool HasChanged() {
		return _hasChanged;
	}

	public string GetMethodString() {
		return string.Format("{0} ({1})", _baseInfo.Replace("|", "."), _paramsInfo.Replace("|", ", "));
	}
}