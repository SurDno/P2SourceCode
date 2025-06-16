// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.ProfileData_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Assets.Engine.Source.Services.Profiles;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Services.Profiles;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ProfileData))]
  public class ProfileData_Generated : 
    ProfileData,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ProfileData_Generated instance = Activator.CreateInstance<ProfileData_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ProfileData_Generated profileDataGenerated = (ProfileData_Generated) target2;
      profileDataGenerated.Name = this.Name;
      profileDataGenerated.LastSave = this.LastSave;
      CloneableObjectUtility.CopyListTo<CustomProfileData>(profileDataGenerated.Data, this.Data);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.Name);
      DefaultDataWriteUtility.Write(writer, "LastSave", this.LastSave);
      DefaultDataWriteUtility.WriteListSerialize<CustomProfileData>(writer, "Data", this.Data);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Name = DefaultDataReadUtility.Read(reader, "Name", this.Name);
      this.LastSave = DefaultDataReadUtility.Read(reader, "LastSave", this.LastSave);
      this.Data = DefaultDataReadUtility.ReadListSerialize<CustomProfileData>(reader, "Data", this.Data);
    }
  }
}
