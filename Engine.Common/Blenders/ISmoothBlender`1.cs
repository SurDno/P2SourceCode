// Decompiled with JetBrains decompiler
// Type: Engine.Common.Blenders.ISmoothBlender`1
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using System;

#nullable disable
namespace Engine.Common.Blenders
{
  public interface ISmoothBlender<T> where T : class, IObject, IBlendable<T>
  {
    T Current { get; }

    event Action<ISmoothBlender<T>> OnChanged;

    void BlendTo(T value, TimeSpan interval);
  }
}
