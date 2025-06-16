using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Settings.External;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(ExternalCommonSettings))]
public class ExternalCommonSettings_Generated :
	ExternalCommonSettings,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<ExternalCommonSettings_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var settingsGenerated = (ExternalCommonSettings_Generated)target2;
		settingsGenerated.Version = Version;
		settingsGenerated.DropBagSearchRadius = DropBagSearchRadius;
		settingsGenerated.DropBagOffset = DropBagOffset;
		settingsGenerated.ChildGunJam = ChildGunJam;
		settingsGenerated.StepsDistance = StepsDistance;
		settingsGenerated.InteractionDistance = InteractionDistance;
		settingsGenerated.AwayDistance = AwayDistance;
		settingsGenerated.DangerDistance = DangerDistance;
		settingsGenerated.DoorSpeed = DoorSpeed;
		settingsGenerated.IdleReplicsFrequencyMin = IdleReplicsFrequencyMin;
		settingsGenerated.IdleReplicsFrequencyMax = IdleReplicsFrequencyMax;
		settingsGenerated.IdleReplicsMaxRangeToPlayer = IdleReplicsMaxRangeToPlayer;
		settingsGenerated.IdleReplicsDistanceMin = IdleReplicsDistanceMin;
		settingsGenerated.IdleReplicsDistanceMax = IdleReplicsDistanceMax;
		settingsGenerated.EntitySubtitlesDistanceMax = EntitySubtitlesDistanceMax;
		settingsGenerated.BleuprintSubtitlesDistanceMax = BleuprintSubtitlesDistanceMax;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Version", Version);
		DefaultDataWriteUtility.Write(writer, "DropBagSearchRadius", DropBagSearchRadius);
		DefaultDataWriteUtility.Write(writer, "DropBagOffset", DropBagOffset);
		DefaultDataWriteUtility.Write(writer, "ChildGunJam", ChildGunJam);
		DefaultDataWriteUtility.Write(writer, "StepsDistance", StepsDistance);
		DefaultDataWriteUtility.Write(writer, "InteractionDistance", InteractionDistance);
		DefaultDataWriteUtility.Write(writer, "AwayDistance", AwayDistance);
		DefaultDataWriteUtility.Write(writer, "DangerDistance", DangerDistance);
		DefaultDataWriteUtility.Write(writer, "DoorSpeed", DoorSpeed);
		DefaultDataWriteUtility.Write(writer, "IdleReplicsFrequencyMin", IdleReplicsFrequencyMin);
		DefaultDataWriteUtility.Write(writer, "IdleReplicsFrequencyMax", IdleReplicsFrequencyMax);
		DefaultDataWriteUtility.Write(writer, "IdleReplicsMaxRangeToPlayer", IdleReplicsMaxRangeToPlayer);
		DefaultDataWriteUtility.Write(writer, "IdleReplicsDistanceMin", IdleReplicsDistanceMin);
		DefaultDataWriteUtility.Write(writer, "IdleReplicsDistanceMax", IdleReplicsDistanceMax);
		DefaultDataWriteUtility.Write(writer, "EntitySubtitlesDistanceMax", EntitySubtitlesDistanceMax);
		DefaultDataWriteUtility.Write(writer, "BleuprintSubtitlesDistanceMax", BleuprintSubtitlesDistanceMax);
	}

	public void DataRead(IDataReader reader, Type type) {
		Version = DefaultDataReadUtility.Read(reader, "Version", Version);
		DropBagSearchRadius = DefaultDataReadUtility.Read(reader, "DropBagSearchRadius", DropBagSearchRadius);
		DropBagOffset = DefaultDataReadUtility.Read(reader, "DropBagOffset", DropBagOffset);
		ChildGunJam = DefaultDataReadUtility.Read(reader, "ChildGunJam", ChildGunJam);
		StepsDistance = DefaultDataReadUtility.Read(reader, "StepsDistance", StepsDistance);
		InteractionDistance = DefaultDataReadUtility.Read(reader, "InteractionDistance", InteractionDistance);
		AwayDistance = DefaultDataReadUtility.Read(reader, "AwayDistance", AwayDistance);
		DangerDistance = DefaultDataReadUtility.Read(reader, "DangerDistance", DangerDistance);
		DoorSpeed = DefaultDataReadUtility.Read(reader, "DoorSpeed", DoorSpeed);
		IdleReplicsFrequencyMin =
			DefaultDataReadUtility.Read(reader, "IdleReplicsFrequencyMin", IdleReplicsFrequencyMin);
		IdleReplicsFrequencyMax =
			DefaultDataReadUtility.Read(reader, "IdleReplicsFrequencyMax", IdleReplicsFrequencyMax);
		IdleReplicsMaxRangeToPlayer =
			DefaultDataReadUtility.Read(reader, "IdleReplicsMaxRangeToPlayer", IdleReplicsMaxRangeToPlayer);
		IdleReplicsDistanceMin = DefaultDataReadUtility.Read(reader, "IdleReplicsDistanceMin", IdleReplicsDistanceMin);
		IdleReplicsDistanceMax = DefaultDataReadUtility.Read(reader, "IdleReplicsDistanceMax", IdleReplicsDistanceMax);
		EntitySubtitlesDistanceMax =
			DefaultDataReadUtility.Read(reader, "EntitySubtitlesDistanceMax", EntitySubtitlesDistanceMax);
		BleuprintSubtitlesDistanceMax =
			DefaultDataReadUtility.Read(reader, "BleuprintSubtitlesDistanceMax", BleuprintSubtitlesDistanceMax);
	}
}