// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.IRegionComponent
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Components.Parameters;
using Engine.Common.Components.Regions;

#nullable disable
namespace Engine.Common.Components
{
  public interface IRegionComponent : IComponent
  {
    RegionEnum Region { get; }

    IParameterValue<int> DiseaseLevel { get; }

    IParameterValue<float> Reputation { get; }
  }
}
