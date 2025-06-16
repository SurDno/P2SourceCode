using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SRF.Components;
using SRF.Helpers;
using UnityEngine;

namespace SRF.Service;

[AddComponentMenu("SRF/Service/Service Manager")]
public class SRServiceManager : SRAutoSingleton<SRServiceManager> {
	public const bool EnableLogging = false;
	public static int LoadingCount = 0;
	private readonly SRList<Service> _services = new();
	private List<ServiceStub> _serviceStubs;
	private static bool _hasQuit;

	public static bool IsLoading => LoadingCount > 0;

	public static T GetService<T>() where T : class {
		var t = GetServiceInternal(typeof(T)) as T;
		if (t == null && !_hasQuit)
			Debug.LogWarning("Service {0} not found. (HasQuit: {1})".Fmt(typeof(T).Name, _hasQuit));
		return t;
	}

	public static object GetService(Type t) {
		var serviceInternal = GetServiceInternal(t);
		if (serviceInternal == null && !_hasQuit)
			Debug.LogWarning("Service {0} not found. (HasQuit: {1})".Fmt(t.Name, _hasQuit));
		return serviceInternal;
	}

	private static object GetServiceInternal(Type t) {
		if (_hasQuit || !Application.isPlaying)
			return null;
		var services = Instance._services;
		for (var index = 0; index < services.Count; ++index) {
			var service = services[index];
			if (t.IsAssignableFrom(service.Type)) {
				if (service.Object != null)
					return service.Object;
				UnRegisterService(t);
				break;
			}
		}

		return Instance.AutoCreateService(t);
	}

	public static bool HasService<T>() where T : class {
		return HasService(typeof(T));
	}

	public static bool HasService(Type t) {
		if (_hasQuit || !Application.isPlaying)
			return false;
		var services = Instance._services;
		for (var index = 0; index < services.Count; ++index) {
			var service = services[index];
			if (t.IsAssignableFrom(service.Type))
				return service.Object != null;
		}

		return false;
	}

	public static void RegisterService<T>(object service) where T : class {
		RegisterService(typeof(T), service);
	}

	private static void RegisterService(Type t, object service) {
		if (_hasQuit)
			return;
		if (HasService(t)) {
			if (GetServiceInternal(t) != service)
				throw new Exception("Service already registered for type " + t.Name);
		} else {
			UnRegisterService(t);
			if (!t.IsInstanceOfType(service))
				throw new ArgumentException("service {0} must be assignable from type {1}".Fmt(service.GetType(), t));
			Instance._services.Add(new Service {
				Object = service,
				Type = t
			});
		}
	}

	public static void UnRegisterService<T>() where T : class {
		UnRegisterService(typeof(T));
	}

	private static void UnRegisterService(Type t) {
		if (_hasQuit || !HasInstance || !HasService(t))
			return;
		var services = Instance._services;
		for (var index = services.Count - 1; index >= 0; --index)
			if (services[index].Type == t)
				services.RemoveAt(index);
	}

	protected override void Awake() {
		_hasQuit = false;
		base.Awake();
		DontDestroyOnLoad(CachedGameObject);
		CachedGameObject.hideFlags = HideFlags.NotEditable;
	}

	protected void UpdateStubs() {
		if (_serviceStubs != null)
			return;
		_serviceStubs = new List<ServiceStub>();
		var typeList = new List<Type>();
		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			if (!assembly.IsDynamic) {
				var fullName = assembly.FullName;
				if (!fullName.StartsWith("mscorlib") && !fullName.StartsWith("System") &&
				    !fullName.StartsWith("UnityEngine") && !fullName.StartsWith("Mono.") &&
				    !fullName.StartsWith("Boo.") && !fullName.StartsWith("UnityEditor") &&
				    !fullName.StartsWith("Unity.") && !fullName.StartsWith("UnityScript") &&
				    !fullName.StartsWith("nunit.") && !fullName.StartsWith("I18N") &&
				    !fullName.StartsWith("ICSharpCode") && !fullName.StartsWith("Newtonsoft.Json"))
					try {
						var flag = false;
						foreach (var num in assembly.GetName().GetPublicKeyToken())
							if (num > 0) {
								flag = true;
								break;
							}

						if (!flag)
							typeList.AddRange(assembly.GetExportedTypes());
					} catch (Exception ex) {
						Debug.LogError("[SRServiceManager] Error loading assembly {0}".Fmt(assembly.FullName), this);
						Debug.LogException(ex);
					}
			}

		foreach (var type in typeList)
			ScanType(type);
		_serviceStubs.Select(p => "\t{0}".Fmt(p)).ToArray();
	}

	protected object AutoCreateService(Type t) {
		UpdateStubs();
		foreach (var serviceStub in _serviceStubs)
			if (!(serviceStub.InterfaceType != t)) {
				object service;
				if (serviceStub.Constructor != null)
					service = serviceStub.Constructor();
				else {
					var implType = serviceStub.Type;
					if (implType == null)
						implType = serviceStub.Selector();
					service = DefaultServiceConstructor(t, implType);
				}

				if (!HasService(t))
					RegisterService(t, service);
				return service;
			}

		return null;
	}

	protected void OnApplicationQuit() {
		_hasQuit = true;
	}

	private static object DefaultServiceConstructor(Type serviceIntType, Type implType) {
		if (typeof(MonoBehaviour).IsAssignableFrom(implType))
			return new GameObject("_S_" + serviceIntType.Name).AddComponent(implType);
		return typeof(ScriptableObject).IsAssignableFrom(implType)
			? ScriptableObject.CreateInstance(implType)
			: Activator.CreateInstance(implType);
	}

	private void ScanType(Type type) {
		var attribute = SRReflection.GetAttribute<ServiceAttribute>(type);
		if (attribute != null)
			_serviceStubs.Add(new ServiceStub {
				Type = type,
				InterfaceType = attribute.ServiceType
			});
		ScanTypeForConstructors(type, _serviceStubs);
		ScanTypeForSelectors(type, _serviceStubs);
	}

	private static void ScanTypeForSelectors(Type t, List<ServiceStub> stubs) {
		foreach (var staticMethod in GetStaticMethods(t)) {
			var attrib = SRReflection.GetAttribute<ServiceSelectorAttribute>(staticMethod);
			if (attrib != null) {
				if (staticMethod.ReturnType != typeof(Type))
					Debug.LogError(
						"ServiceSelector must have return type of Type ({0}.{1}())".Fmt(t.Name, staticMethod.Name));
				else if (staticMethod.GetParameters().Length != 0)
					Debug.LogError(
						"ServiceSelector must have no parameters ({0}.{1}())".Fmt(t.Name, staticMethod.Name));
				else {
					var serviceStub = stubs.FirstOrDefault(p => p.InterfaceType == attrib.ServiceType);
					if (serviceStub == null) {
						serviceStub = new ServiceStub {
							InterfaceType = attrib.ServiceType
						};
						stubs.Add(serviceStub);
					}

					serviceStub.Selector = (Func<Type>)Delegate.CreateDelegate(typeof(Func<Type>), staticMethod);
				}
			}
		}
	}

	private static void ScanTypeForConstructors(Type t, List<ServiceStub> stubs) {
		foreach (var staticMethod in GetStaticMethods(t)) {
			var method = staticMethod;
			var attrib = SRReflection.GetAttribute<ServiceConstructorAttribute>(method);
			if (attrib != null) {
				if (method.ReturnType != attrib.ServiceType)
					Debug.LogError(
						"ServiceConstructor must have return type of {2} ({0}.{1}())".Fmt(t.Name, method.Name,
							attrib.ServiceType));
				else if (method.GetParameters().Length != 0)
					Debug.LogError("ServiceConstructor must have no parameters ({0}.{1}())".Fmt(t.Name, method.Name));
				else {
					var serviceStub = stubs.FirstOrDefault(p => p.InterfaceType == attrib.ServiceType);
					if (serviceStub == null) {
						serviceStub = new ServiceStub {
							InterfaceType = attrib.ServiceType
						};
						stubs.Add(serviceStub);
					}

					serviceStub.Constructor = (Func<object>)(() => method.Invoke(null, null));
				}
			}
		}
	}

	private static MethodInfo[] GetStaticMethods(Type t) {
		return t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
	}

	[Serializable]
	private class Service {
		public object Object;
		public Type Type;
	}

	[Serializable]
	private class ServiceStub {
		public Func<object> Constructor;
		public Type InterfaceType;
		public Func<Type> Selector;
		public Type Type;

		public override string ToString() {
			var str = InterfaceType.Name + " (";
			if (Type != null)
				str = str + "Type: " + Type;
			else if (Selector != null)
				str = str + "Selector: " + Selector;
			else if (Constructor != null)
				str = str + "Constructor: " + Constructor;
			return str + ")";
		}
	}
}