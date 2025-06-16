using System.Collections.Generic;
using Assets.Engine.Source.Services.Profiles;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;

namespace Engine.Source.Services.Profiles
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ProfileData
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    public string Name = "";
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    public string LastSave = "";
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected]
    public List<CustomProfileData> Data = new List<CustomProfileData>();
  }
}
