using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Services.Profiles;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ProfilesData))]
  public class ProfilesData_Generated : 
    ProfilesData,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ProfilesData_Generated instance = Activator.CreateInstance<ProfilesData_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ProfilesData_Generated profilesDataGenerated = (ProfilesData_Generated) target2;
      CloneableObjectUtility.CopyListTo<ProfileData>(profilesDataGenerated.Profiles, this.Profiles);
      profilesDataGenerated.CurrentIndex = this.CurrentIndex;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListSerialize<ProfileData>(writer, "Profiles", this.Profiles);
      DefaultDataWriteUtility.Write(writer, "CurrentIndex", this.CurrentIndex);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Profiles = DefaultDataReadUtility.ReadListSerialize<ProfileData>(reader, "Profiles", this.Profiles);
      this.CurrentIndex = DefaultDataReadUtility.Read(reader, "CurrentIndex", this.CurrentIndex);
    }
  }
}
