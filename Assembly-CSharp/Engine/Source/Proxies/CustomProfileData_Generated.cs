using Assets.Engine.Source.Services.Profiles;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using System;

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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      CustomProfileData_Generated profileDataGenerated = (CustomProfileData_Generated) target2;
      profileDataGenerated.Name = this.Name;
      profileDataGenerated.Value = this.Value;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.Name);
      DefaultDataWriteUtility.Write(writer, "Value", this.Value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Name = DefaultDataReadUtility.Read(reader, "Name", this.Name);
      this.Value = DefaultDataReadUtility.Read(reader, "Value", this.Value);
    }
  }
}
