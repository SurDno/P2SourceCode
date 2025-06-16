// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.Parameters.IParameter
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

#nullable disable
namespace Engine.Common.Components.Parameters
{
  public interface IParameter
  {
    ParameterNameEnum Name { get; }

    bool Resetable { get; }

    object ValueData { get; }

    void AddListener(IChangeParameterListener listener);

    void RemoveListener(IChangeParameterListener listener);
  }
}
