// Decompiled with JetBrains decompiler
// Type: Engine.Impl.SceneObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Assets.Internal.Reference;
using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;
using System;
using System.Collections.Generic;

#nullable disable
namespace Engine.Impl
{
  [Factory(typeof (IScene))]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class SceneObject : EngineObject, IScene, IObject
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected]
    public List<SceneObjectItem> Items = new List<SceneObjectItem>();

    public void Sort() => this.Sort(this.Items);

    private void Sort(List<SceneObjectItem> items)
    {
      items.Sort((Comparison<SceneObjectItem>) ((a, b) => a.Id.CompareTo(b.Id)));
      foreach (SceneObjectItem sceneObjectItem in items)
        this.Sort(sceneObjectItem.Items);
    }

    public IEnumerable<SceneObjectItem> GetChilds()
    {
      foreach (SceneObjectItem child in this.Items)
      {
        yield return child;
        foreach (SceneObjectItem child2 in SceneObject.GetChilds(child))
          yield return child2;
      }
    }

    private static IEnumerable<SceneObjectItem> GetChilds(SceneObjectItem item)
    {
      foreach (SceneObjectItem child in item.Items)
      {
        yield return child;
        foreach (SceneObjectItem child2 in SceneObject.GetChilds(child))
          yield return child2;
      }
    }
  }
}
