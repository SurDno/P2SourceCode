using System;
using Cofe.Loggers;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Cofe.Utility;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Impl.Weather;
using Engine.Impl.Weather.Element;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(WeatherSnapshot))]
public class WeatherSnapshot_Generated :
	WeatherSnapshot,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		return ServiceCache.Factory.Instantiate(this);
	}

	public void CopyTo(object target2) {
		var snapshotGenerated = (WeatherSnapshot_Generated)target2;
		snapshotGenerated.name = name;
		((ICopyable)clouds).CopyTo(snapshotGenerated.clouds);
		((ICopyable)day).CopyTo(snapshotGenerated.day);
		((ICopyable)fog).CopyTo(snapshotGenerated.fog);
		((ICopyable)location).CopyTo(snapshotGenerated.location);
		((ICopyable)moon).CopyTo(snapshotGenerated.moon);
		((ICopyable)night).CopyTo(snapshotGenerated.night);
		((ICopyable)stars).CopyTo(snapshotGenerated.stars);
		((ICopyable)sun).CopyTo(snapshotGenerated.sun);
		((ICopyable)thunderStorm).CopyTo(snapshotGenerated.thunderStorm);
		((ICopyable)wind).CopyTo(snapshotGenerated.wind);
		((ICopyable)rain).CopyTo((object)snapshotGenerated.rain);
		((ICopyable)fallingLeaves).CopyTo(snapshotGenerated.fallingLeaves);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Id", id);
		DefaultDataWriteUtility.WriteSerialize(writer, "Clouds", clouds);
		DefaultDataWriteUtility.WriteSerialize(writer, "Day", day);
		DefaultDataWriteUtility.WriteSerialize(writer, "Fog", fog);
		DefaultDataWriteUtility.WriteSerialize(writer, "Location", location);
		DefaultDataWriteUtility.WriteSerialize(writer, "Moon", moon);
		DefaultDataWriteUtility.WriteSerialize(writer, "Night", night);
		DefaultDataWriteUtility.WriteSerialize(writer, "Stars", stars);
		DefaultDataWriteUtility.WriteSerialize(writer, "Sun", sun);
		DefaultDataWriteUtility.WriteSerialize(writer, "ThunderStorm", thunderStorm);
		DefaultDataWriteUtility.WriteSerialize(writer, "Wind", wind);
		DefaultDataWriteUtility.WriteSerialize(writer, "Rain", rain);
		DefaultDataWriteUtility.WriteSerialize(writer, "FallingLeaves", fallingLeaves);
	}

	public void DataRead(IDataReader reader, Type type) {
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		var child1 = reader.GetChild("Clouds");
		var clouds = this.clouds;
		if (clouds is ISerializeDataRead serializeDataRead1)
			serializeDataRead1.DataRead(child1, typeof(Clouds));
		else
			Logger.AddError("Type : " + TypeUtility.GetTypeName(clouds.GetType()) + " is not " +
			                typeof(ISerializeDataRead).Name);
		var child2 = reader.GetChild("Day");
		var day = this.day;
		if (day is ISerializeDataRead serializeDataRead2)
			serializeDataRead2.DataRead(child2, typeof(Day));
		else
			Logger.AddError("Type : " + TypeUtility.GetTypeName(day.GetType()) + " is not " +
			                typeof(ISerializeDataRead).Name);
		var child3 = reader.GetChild("Fog");
		var fog = this.fog;
		if (fog is ISerializeDataRead serializeDataRead3)
			serializeDataRead3.DataRead(child3, typeof(Fog));
		else
			Logger.AddError("Type : " + TypeUtility.GetTypeName(fog.GetType()) + " is not " +
			                typeof(ISerializeDataRead).Name);
		var child4 = reader.GetChild("Location");
		var location = this.location;
		if (location is ISerializeDataRead serializeDataRead4)
			serializeDataRead4.DataRead(child4, typeof(Location));
		else
			Logger.AddError("Type : " + TypeUtility.GetTypeName(location.GetType()) + " is not " +
			                typeof(ISerializeDataRead).Name);
		var child5 = reader.GetChild("Moon");
		var moon = this.moon;
		if (moon is ISerializeDataRead serializeDataRead5)
			serializeDataRead5.DataRead(child5, typeof(Moon));
		else
			Logger.AddError("Type : " + TypeUtility.GetTypeName(moon.GetType()) + " is not " +
			                typeof(ISerializeDataRead).Name);
		var child6 = reader.GetChild("Night");
		var night = this.night;
		if (night is ISerializeDataRead serializeDataRead6)
			serializeDataRead6.DataRead(child6, typeof(Night));
		else
			Logger.AddError("Type : " + TypeUtility.GetTypeName(night.GetType()) + " is not " +
			                typeof(ISerializeDataRead).Name);
		var child7 = reader.GetChild("Stars");
		var stars = this.stars;
		if (stars is ISerializeDataRead serializeDataRead7)
			serializeDataRead7.DataRead(child7, typeof(Stars));
		else
			Logger.AddError("Type : " + TypeUtility.GetTypeName(stars.GetType()) + " is not " +
			                typeof(ISerializeDataRead).Name);
		var child8 = reader.GetChild("Sun");
		var sun = this.sun;
		if (sun is ISerializeDataRead serializeDataRead8)
			serializeDataRead8.DataRead(child8, typeof(Sun));
		else
			Logger.AddError("Type : " + TypeUtility.GetTypeName(sun.GetType()) + " is not " +
			                typeof(ISerializeDataRead).Name);
		var child9 = reader.GetChild("ThunderStorm");
		var thunderStorm = this.thunderStorm;
		if (thunderStorm is ISerializeDataRead serializeDataRead9)
			serializeDataRead9.DataRead(child9, typeof(ThunderStorm));
		else
			Logger.AddError("Type : " + TypeUtility.GetTypeName(thunderStorm.GetType()) + " is not " +
			                typeof(ISerializeDataRead).Name);
		var child10 = reader.GetChild("Wind");
		var wind = this.wind;
		if (wind is ISerializeDataRead serializeDataRead10)
			serializeDataRead10.DataRead(child10, typeof(Wind));
		else
			Logger.AddError("Type : " + TypeUtility.GetTypeName(wind.GetType()) + " is not " +
			                typeof(ISerializeDataRead).Name);
		var child11 = reader.GetChild("Rain");
		var rain = this.rain;
		if (rain is ISerializeDataRead serializeDataRead11)
			serializeDataRead11.DataRead(child11, typeof(Impl.Weather.Element.Rain));
		else
			Logger.AddError("Type : " + TypeUtility.GetTypeName(rain.GetType()) + " is not " +
			                typeof(ISerializeDataRead).Name);
		var child12 = reader.GetChild("FallingLeaves");
		var fallingLeaves = this.fallingLeaves;
		if (fallingLeaves is ISerializeDataRead serializeDataRead12)
			serializeDataRead12.DataRead(child12, typeof(FallingLeaves));
		else
			Logger.AddError("Type : " + TypeUtility.GetTypeName(fallingLeaves.GetType()) + " is not " +
			                typeof(ISerializeDataRead).Name);
	}
}