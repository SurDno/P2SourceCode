// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.Parameters.IParameterValue`1
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using System;

#nullable disable
namespace Engine.Common.Components.Parameters
{
  public interface IParameterValue<T> where T : struct
  {
    T Value { get; set; }

    T MinValue { get; set; }

    T MaxValue { get; set; }

    event Action<T> ChangeValueEvent;
  }
}
