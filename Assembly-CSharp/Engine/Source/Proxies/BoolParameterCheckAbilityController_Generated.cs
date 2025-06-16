using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons.Abilities.Controllers;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      BoolParameterCheckAbilityController_Generated controllerGenerated = (BoolParameterCheckAbilityController_Generated) target2;
      controllerGenerated.parameterName = parameterName;
      controllerGenerated.value = value;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Parameter", parameterName);
      DefaultDataWriteUtility.Write(writer, "Value", value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      parameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "Parameter");
      value = DefaultDataReadUtility.Read(reader, "Value", value);
    }
  }
}
