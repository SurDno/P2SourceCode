using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons.Abilities.Controllers;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (BoolParameterCheckAbilityController))]
  public class BoolParameterCheckAbilityController_Generated : 
    BoolParameterCheckAbilityController,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      BoolParameterCheckAbilityController_Generated instance = Activator.CreateInstance<BoolParameterCheckAbilityController_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      BoolParameterCheckAbilityController_Generated controllerGenerated = (BoolParameterCheckAbilityController_Generated) target2;
      controllerGenerated.parameterName = this.parameterName;
      controllerGenerated.value = this.value;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "Parameter", this.parameterName);
      DefaultDataWriteUtility.Write(writer, "Value", this.value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.parameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "Parameter");
      this.value = DefaultDataReadUtility.Read(reader, "Value", this.value);
    }
  }
}
