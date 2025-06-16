// Decompiled with JetBrains decompiler
// Type: Engine.Common.Services.IProfilesService
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

#nullable disable
namespace Engine.Common.Services
{
  public interface IProfilesService
  {
    string GetValue(string name);

    void SetValue(string name, string value);

    int GetIntValue(string name);

    void SetIntValue(string name, int value);

    bool GetBoolValue(string name);

    void SetBoolValue(string name, bool value);

    float GetFloatValue(string name);

    void SetFloatValue(string name, float value);
  }
}
