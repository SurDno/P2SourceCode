using System;
using Cofe.Loggers;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Cofe.Utility;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.OutdoorCrowds;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(OutdoorCrowdTemplate))]
public class OutdoorCrowdTemplate_Generated :
	OutdoorCrowdTemplate,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<OutdoorCrowdTemplate_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var templateGenerated = (OutdoorCrowdTemplate_Generated)target2;
		templateGenerated.Name = Name;
		templateGenerated.Template = Template;
		((ICopyable)Day).CopyTo(templateGenerated.Day);
		((ICopyable)Night).CopyTo(templateGenerated.Night);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Name", Name);
		UnityDataWriteUtility.Write(writer, "Template", Template);
		DefaultDataWriteUtility.WriteSerialize(writer, "Day", Day);
		DefaultDataWriteUtility.WriteSerialize(writer, "Night", Night);
	}

	public void DataRead(IDataReader reader, Type type) {
		Name = DefaultDataReadUtility.Read(reader, "Name", Name);
		Template = UnityDataReadUtility.Read(reader, "Template", Template);
		var child1 = reader.GetChild("Day");
		var day = Day;
		if (day is ISerializeDataRead serializeDataRead1)
			serializeDataRead1.DataRead(child1, typeof(OutdoorCrowdTemplateCount));
		else
			Logger.AddError("Type : " + TypeUtility.GetTypeName(day.GetType()) + " is not " +
			                typeof(ISerializeDataRead).Name);
		var child2 = reader.GetChild("Night");
		var night = Night;
		if (night is ISerializeDataRead serializeDataRead2)
			serializeDataRead2.DataRead(child2, typeof(OutdoorCrowdTemplateCount));
		else
			Logger.AddError("Type : " + TypeUtility.GetTypeName(night.GetType()) + " is not " +
			                typeof(ISerializeDataRead).Name);
	}
}