using System;
using System.Collections.Generic;
using System.Linq;
using Cofe.Loggers;
using Cofe.Utility;

namespace Engine.Common.Services;

public static class ServiceLocator {
	private static Dictionary<Type, object> serviceMap = new();
	private static List<object> services = new();
	private static object lockObject = new();

	public static object GetService(Type type) {
		object service;
		lock (lockObject) {
			serviceMap.TryGetValue(type, out service);
		}

		return service;
	}

	public static T GetService<T>() where T : class {
		object service;
		lock (lockObject) {
			serviceMap.TryGetValue(typeof(T), out service);
		}

		return service as T;
	}

	public static void AddService(Type[] types, object service) {
		lock (lockObject) {
			services.Add(service);
			services.Sort((a, b) => a.GetType().Name.CompareTo(b.GetType().Name));
			var type1 = service.GetType();
			foreach (var type2 in types) {
				object obj;
				if (serviceMap.TryGetValue(type2, out obj))
					Logger.AddWarning("Exist service , face : " + type2 + " , type : " + obj.GetType());
				if (!TypeUtility.IsAssignableFrom(type2, type1))
					Logger.AddError("Wrong service type, original : " + type1 + " , target : " + type2);
				else
					serviceMap[type2] = service;
			}
		}
	}

	public static void RemoveService(object instance) {
		lock (lockObject) {
			label_2:
			foreach (var service in serviceMap)
				if (service.Value == instance) {
					serviceMap.Remove(service.Key);
					goto label_2;
				}

			if (!services.Remove(instance))
				return;
			services.Sort((a, b) => a.GetType().Name.CompareTo(b.GetType().Name));
		}
	}

	public static void Clear() {
		lock (lockObject) {
			serviceMap.Clear();
			services.Clear();
		}
	}

	public static IEnumerable<object> GetServices() {
		lock (lockObject) {
			return services.ToList();
		}
	}
}