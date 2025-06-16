using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(Impl.Weather.Element.Rain))]
public class Rain_Generated : Impl.Weather.Element.Rain, ICloneable, ICopyable, ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<Rain_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var rainGenerated = (Rain_Generated)target2;
		rainGenerated.direction = direction;
		rainGenerated.intensity = intensity;
		rainGenerated.puddlesDryTime = puddlesDryTime;
		rainGenerated.puddlesFillTime = puddlesFillTime;
		rainGenerated.terrainDryTime = terrainDryTime;
		rainGenerated.terrainFillTime = terrainFillTime;
	}

	public void DataWrite(IDataWriter writer) {
		UnityDataWriteUtility.Write(writer, "Direction", direction);
		DefaultDataWriteUtility.Write(writer, "Intensity", intensity);
		DefaultDataWriteUtility.Write(writer, "PuddlesDryTime", puddlesDryTime);
		DefaultDataWriteUtility.Write(writer, "PuddlesFillTime", puddlesFillTime);
		DefaultDataWriteUtility.Write(writer, "TerrainDryTime", terrainDryTime);
		DefaultDataWriteUtility.Write(writer, "TerrainFillTime", terrainFillTime);
	}

	public void DataRead(IDataReader reader, Type type) {
		direction = UnityDataReadUtility.Read(reader, "Direction", direction);
		intensity = DefaultDataReadUtility.Read(reader, "Intensity", intensity);
		puddlesDryTime = DefaultDataReadUtility.Read(reader, "PuddlesDryTime", puddlesDryTime);
		puddlesFillTime = DefaultDataReadUtility.Read(reader, "PuddlesFillTime", puddlesFillTime);
		terrainDryTime = DefaultDataReadUtility.Read(reader, "TerrainDryTime", terrainDryTime);
		terrainFillTime = DefaultDataReadUtility.Read(reader, "TerrainFillTime", terrainFillTime);
	}
}