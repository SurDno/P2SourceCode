using System;
using System.Collections.Generic;
using Cofe.Meta;

namespace Engine.Source.Achievements.Controllers;

[AttributeUsage(AttributeTargets.Class)]
public class AchievementControllerAttribute : TypeAttribute {
	private static Dictionary<string, Type> factory = new();

	public static IDictionary<string, Type> Factory => factory;

	public string Id { get; private set; }

	public AchievementControllerAttribute(string id) {
		Id = id;
	}

	public override void ComputeType(Type type) {
		base.ComputeType(type);
		factory[Id] = type;
	}
}