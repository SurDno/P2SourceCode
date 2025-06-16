// Decompiled with JetBrains decompiler
// Type: Engine.Common.Blenders.ILayerBlender`1
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Engine.Common.Blenders
{
  public interface ILayerBlender<T> where T : class, IObject, IBlendable<T>
  {
    T Current { get; }

    IEnumerable<ILayerBlenderItem<T>> Items { get; }

    void AddItem(ILayerBlenderItem<T> item);

    void RemoveItem(ILayerBlenderItem<T> item);

    event Action<ILayerBlender<T>> OnChanged;
  }
}
