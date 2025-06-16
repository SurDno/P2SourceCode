using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Controllers;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(TimerAbilityController))]
public class TimerAbilityController_Generated :
	TimerAbilityController,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<TimerAbilityController_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var controllerGenerated = (TimerAbilityController_Generated)target2;
		controllerGenerated.realTime = realTime;
		controllerGenerated.interval = interval;
		controllerGenerated.timeout = timeout;
		controllerGenerated.intervalTime = intervalTime;
		controllerGenerated.timeoutTime = timeoutTime;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "RealTime", realTime);
		DefaultDataWriteUtility.Write(writer, "Interval", interval);
		DefaultDataWriteUtility.Write(writer, "Timeout", timeout);
		DefaultDataWriteUtility.Write(writer, "IntervalTime", intervalTime);
		DefaultDataWriteUtility.Write(writer, "TimeoutTime", timeoutTime);
	}

	public void DataRead(IDataReader reader, Type type) {
		realTime = DefaultDataReadUtility.Read(reader, "RealTime", realTime);
		interval = DefaultDataReadUtility.Read(reader, "Interval", interval);
		timeout = DefaultDataReadUtility.Read(reader, "Timeout", timeout);
		intervalTime = DefaultDataReadUtility.Read(reader, "IntervalTime", intervalTime);
		timeoutTime = DefaultDataReadUtility.Read(reader, "TimeoutTime", timeoutTime);
	}
}