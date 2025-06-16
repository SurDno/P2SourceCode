// Decompiled with JetBrains decompiler
// Type: Engine.Source.Connections.TemplateUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using System;

#nullable disable
namespace Engine.Source.Connections
{
  public static class TemplateUtility
  {
    public static T GetTemplate<T>(Guid id) where T : class, IObject
    {
      if (InstanceByRequest<EngineApplication>.Instance.IsInitialized)
        return ServiceLocator.GetService<ITemplateService>().GetTemplate<T>(id);
      throw new Exception();
    }
  }
}
