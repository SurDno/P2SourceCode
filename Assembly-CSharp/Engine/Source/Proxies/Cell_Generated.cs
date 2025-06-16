using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Inventory;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(Cell))]
public class Cell_Generated :
	Cell,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead,
	ISerializeStateSave,
	ISerializeStateLoad {
	public object Clone() {
		var instance = Activator.CreateInstance<Cell_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var cellGenerated = (Cell_Generated)target2;
		cellGenerated.Column = Column;
		cellGenerated.Row = Row;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Column", Column);
		DefaultDataWriteUtility.Write(writer, "Row", Row);
	}

	public void DataRead(IDataReader reader, Type type) {
		Column = DefaultDataReadUtility.Read(reader, "Column", Column);
		Row = DefaultDataReadUtility.Read(reader, "Row", Row);
	}

	public void StateSave(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Column", Column);
		DefaultDataWriteUtility.Write(writer, "Row", Row);
	}

	public void StateLoad(IDataReader reader, Type type) {
		Column = DefaultDataReadUtility.Read(reader, "Column", Column);
		Row = DefaultDataReadUtility.Read(reader, "Row", Row);
	}
}