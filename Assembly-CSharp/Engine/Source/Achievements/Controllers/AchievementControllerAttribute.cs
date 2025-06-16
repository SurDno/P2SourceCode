using Cofe.Meta;
using System;
using System.Collections.Generic;

namespace Engine.Source.Achievements.Controllers
{
  [AttributeUsage(AttributeTargets.Class)]
  public class AchievementControllerAttribute : TypeAttribute
  {
    private static Dictionary<string, Type> factory = new Dictionary<string, Type>();

    public static IDictionary<string, Type> Factory
    {
      get => (IDictionary<string, Type>) AchievementControllerAttribute.factory;
    }

    public string Id { get; private set; }

    public AchievementControllerAttribute(string id) => this.Id = id;

    public override void ComputeType(Type type)
    {
      base.ComputeType(type);
      AchievementControllerAttribute.factory[this.Id] = type;
    }
  }
}
