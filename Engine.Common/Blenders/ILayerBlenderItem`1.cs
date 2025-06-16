// Decompiled with JetBrains decompiler
// Type: Engine.Common.Blenders.ILayerBlenderItem`1
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using System;

#nullable disable
namespace Engine.Common.Blenders
{
  public interface ILayerBlenderItem<T> where T : class, IObject, IBlendable<T>
  {
    ISmoothBlender<T> Blender { get; set; }

    float Opacity { get; }

    float TargetOpacity { get; }

    void SetOpacity(float value);

    void SetOpacity(float value, TimeSpan interval);

    event Action<ILayerBlenderItem<T>> OnChanged;
  }
}
