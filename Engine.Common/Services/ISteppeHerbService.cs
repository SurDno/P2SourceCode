// Decompiled with JetBrains decompiler
// Type: Engine.Common.Services.ISteppeHerbService
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

#nullable disable
namespace Engine.Common.Services
{
  public interface ISteppeHerbService
  {
    int BrownTwyreAmount { get; set; }

    int BloodTwyreAmount { get; set; }

    int BlackTwyreAmount { get; set; }

    int SweveryAmount { get; set; }

    void Reset();
  }
}
