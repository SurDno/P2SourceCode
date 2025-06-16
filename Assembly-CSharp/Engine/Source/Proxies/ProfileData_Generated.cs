using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Services.Profiles;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ProfileData_Generated profileDataGenerated = (ProfileData_Generated) target2;
      profileDataGenerated.Name = Name;
      profileDataGenerated.LastSave = LastSave;
      CloneableObjectUtility.CopyListTo(profileDataGenerated.Data, Data);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", Name);
      DefaultDataWriteUtility.Write(writer, "LastSave", LastSave);
      DefaultDataWriteUtility.WriteListSerialize(writer, "Data", Data);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      Name = DefaultDataReadUtility.Read(reader, "Name", Name);
      LastSave = DefaultDataReadUtility.Read(reader, "LastSave", LastSave);
      Data = DefaultDataReadUtility.ReadListSerialize(reader, "Data", Data);
    }
  }
}
