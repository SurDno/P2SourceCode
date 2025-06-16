// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.IGameMode
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using PLVirtualMachine.Common.EngineAPI;

#nullable disable
namespace PLVirtualMachine.Common
{
  public interface IGameMode : IObject, IEditorBaseTemplate
  {
    string Name { get; }

    bool IsMain { get; }

    GameTime StartGameTime { get; }

    GameTime StartSolarTime { get; }

    float GameTimeSpeed { get; }

    float SolarTimeSpeed { get; }

    CommonVariable PlayCharacterVariable { get; }
  }
}
