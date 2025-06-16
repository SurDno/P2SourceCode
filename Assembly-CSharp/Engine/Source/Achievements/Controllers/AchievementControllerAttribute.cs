// Decompiled with JetBrains decompiler
// Type: Engine.Source.Achievements.Controllers.AchievementControllerAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using System;
using System.Collections.Generic;

#nullable disable
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
