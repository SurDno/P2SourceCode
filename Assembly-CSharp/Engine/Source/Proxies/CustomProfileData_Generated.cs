using System;
using Assets.Engine.Source.Services.Profiles;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (CustomProfileData))]
  public class CustomProfileData_Generated : 
    CustomProfileData,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      CustomProfileData_Generated instance = Activator.CreateInstance<CustomProfileData_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      CustomProfileData_Generated profileDataGenerated = (CustomProfileData_Generated) target2;
      profileDataGenerated.Name = Name;
      profileDataGenerated.Value = Value;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", Name);
      DefaultDataWriteUtility.Write(writer, "Value", Value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      Name = DefaultDataReadUtility.Read(reader, "Name", Name);
      Value = DefaultDataReadUtility.Read(reader, "Value", Value);
    }
  }
}
