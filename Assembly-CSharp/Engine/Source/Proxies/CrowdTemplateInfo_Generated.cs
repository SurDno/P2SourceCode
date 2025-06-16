using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components.Crowds;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(CrowdTemplateInfo))]
public class CrowdTemplateInfo_Generated :
	CrowdTemplateInfo,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<CrowdTemplateInfo_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var templateInfoGenerated = (CrowdTemplateInfo_Generated)target2;
		CloneableObjectUtility.FillListTo(templateInfoGenerated.Templates, Templates);
		templateInfoGenerated.Min = Min;
		templateInfoGenerated.Max = Max;
		templateInfoGenerated.Chance = Chance;
	}

	public void DataWrite(IDataWriter writer) {
		UnityDataWriteUtility.WriteList(writer, "Templates", Templates);
		DefaultDataWriteUtility.Write(writer, "Min", Min);
		DefaultDataWriteUtility.Write(writer, "Max", Max);
		DefaultDataWriteUtility.Write(writer, "Chance", Chance);
	}

	public void DataRead(IDataReader reader, Type type) {
		Templates = UnityDataReadUtility.ReadList(reader, "Templates", Templates);
		Min = DefaultDataReadUtility.Read(reader, "Min", Min);
		Max = DefaultDataReadUtility.Read(reader, "Max", Max);
		Chance = DefaultDataReadUtility.Read(reader, "Chance", Chance);
	}
}