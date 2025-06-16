using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons.Abilities.Controllers;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (FloatParameterCheckAbilityController))]
  public class FloatParameterCheckAbilityController_Generated : 
    FloatParameterCheckAbilityController,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      FloatParameterCheckAbilityController_Generated instance = Activator.CreateInstance<FloatParameterCheckAbilityController_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      FloatParameterCheckAbilityController_Generated controllerGenerated = (FloatParameterCheckAbilityController_Generated) target2;
      controllerGenerated.parameterName = parameterName;
      controllerGenerated.value = value;
      controllerGenerated.compare = compare;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Parameter", parameterName);
      DefaultDataWriteUtility.Write(writer, "Value", value);
      DefaultDataWriteUtility.WriteEnum(writer, "Compare", compare);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      parameterName = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "Parameter");
      value = DefaultDataReadUtility.Read(reader, "Value", value);
      compare = DefaultDataReadUtility.ReadEnum<CompareEnum>(reader, "Compare");
    }
  }
}
