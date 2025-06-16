using System;
using System.Collections.Generic;
using Cofe.Meta;
using Cofe.Proxies;
using Engine.Common.Services;
using Engine.Common.Types;

namespace Engine.Source.Services;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class RuntimeServiceAttribute : TypeAttribute {
	private Type[] types;
	private static List<Pair<Type[], Type>> services = new();
	private static List<object> instances = new();

	public RuntimeServiceAttribute(params Type[] types) {
		this.types = types;
	}

	public override void ComputeType(Type type) {
		services.Add(new Pair<Type[], Type>(types, type));
	}

	public static void CreateServices() {
		instances.Capacity = services.Count;
		foreach (var service1 in services) {
			var service2 = ProxyFactory.Create(service1.Item2);
			instances.Add(service2);
			ServiceLocator.AddService(service1.Item1, service2);
		}
	}

	public static List<object> GetServices() {
		return instances;
	}
}