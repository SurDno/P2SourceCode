// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.CrowdUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Components.Crowds;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Components
{
  public static class CrowdUtility
  {
    public static List<CrowdPointInfo> Points = new List<CrowdPointInfo>(1024);

    public static void SetAsCrowd(IEntity entity)
    {
      ((Entity) entity).DontSave = true;
      if (entity.Childs == null)
        return;
      foreach (Entity child in entity.Childs)
        child.DontSave = true;
    }
  }
}
