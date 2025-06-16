// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.IBoundCharacterComponent
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.BoundCharacters;
using Engine.Common.Commons;
using Engine.Common.Components.Parameters;
using Engine.Common.Types;

#nullable disable
namespace Engine.Common.Components
{
  public interface IBoundCharacterComponent : IComponent
  {
    IParameterValue<BoundHealthStateEnum> BoundHealthState { get; }

    bool Discovered { get; set; }

    BoundCharacterGroup Group { get; set; }

    IEntity HomeRegion { get; set; }

    bool IsEnabled { get; set; }

    LocalizedText Name { get; set; }

    IParameterValue<float> RandomRoll { get; }

    IBoundCharacterPlaceholder Resource { get; set; }

    int SortOrder { get; set; }

    void StorePreRollState();
  }
}
