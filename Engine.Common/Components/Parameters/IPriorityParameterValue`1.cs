// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.Parameters.IPriorityParameterValue`1
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

#nullable disable
namespace Engine.Common.Components.Parameters
{
  public interface IPriorityParameterValue<T> : IParameterValue<T> where T : struct
  {
    void SetValue(PriorityParameterEnum priority, T value);

    bool TryGetValue(PriorityParameterEnum priority, out T value);

    void ResetValue(PriorityParameterEnum priority);
  }
}
