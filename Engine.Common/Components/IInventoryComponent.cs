// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.IInventoryComponent
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;

#nullable disable
namespace Engine.Common.Components
{
  public interface IInventoryComponent : IComponent
  {
    IParameter<bool> Enabled { get; }

    IParameter<bool> Available { get; }

    IParameter<float> Disease { get; }

    IParameter<ContainerOpenStateEnum> OpenState { get; }
  }
}
