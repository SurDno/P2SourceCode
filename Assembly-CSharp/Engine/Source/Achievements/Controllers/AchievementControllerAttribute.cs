using System;
using System.Collections.Generic;
using Cofe.Meta;

namespace Engine.Source.Achievements.Controllers
{
  [AttributeUsage(AttributeTargets.Class)]
  public class AchievementControllerAttribute(string id) : TypeAttribute 
  {
    private static Dictionary<string, Type> factory = new();

    public static IDictionary<string, Type> Factory => factory;

    public string Id { get; private set; } = id;

    public override void ComputeType(Type type)
    {
      base.ComputeType(type);
      factory[Id] = type;
    }
  }
}
