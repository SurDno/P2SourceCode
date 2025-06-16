using System.Collections;
using System.Xml;
using Cofe.Serializations.Data;
using Cofe.Serializations.Data.Xml;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Saves;
using Engine.Source.Services;
using Engine.Source.Services.Saves;
using Inspectors;
using UnityEngine;

namespace Engine.Impl.Services;

[GameService(typeof(SteppeHerbService), typeof(ISteppeHerbService))]
[GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
public class SteppeHerbService : ISteppeHerbService, IUpdatable, ISavesController {
	[Inspected] private SteppeHerbContainer containerBrownTwyre;
	[Inspected] private SteppeHerbContainer containerBloodTwyre;
	[Inspected] private SteppeHerbContainer containerBlackTwyre;
	[Inspected] private SteppeHerbContainer containerSwevery;
	private int brownTwyreAmount;
	private int bloodTwyreAmount;
	private int blackTwyreAmount;
	private int sweveryAmount;

	[StateSaveProxy]
	[StateLoadProxy()]
	[Inspected(Mutable = true)]
	public int BrownTwyreAmount {
		get => brownTwyreAmount;
		set {
			brownTwyreAmount = value;
			if (containerBrownTwyre == null)
				return;
			containerBrownTwyre.Amount = value;
		}
	}

	[StateSaveProxy]
	[StateLoadProxy()]
	[Inspected(Mutable = true)]
	public int BloodTwyreAmount {
		get => bloodTwyreAmount;
		set {
			bloodTwyreAmount = value;
			if (containerBloodTwyre == null)
				return;
			containerBloodTwyre.Amount = value;
		}
	}

	[StateSaveProxy]
	[StateLoadProxy()]
	[Inspected(Mutable = true)]
	public int BlackTwyreAmount {
		get => blackTwyreAmount;
		set {
			blackTwyreAmount = value;
			if (containerBlackTwyre == null)
				return;
			containerBlackTwyre.Amount = value;
		}
	}

	[StateSaveProxy]
	[StateLoadProxy()]
	[Inspected(Mutable = true)]
	public int SweveryAmount {
		get => sweveryAmount;
		set {
			sweveryAmount = value;
			if (containerSwevery == null)
				return;
			containerSwevery.Amount = value;
		}
	}

	public void Reset() {
		containerBrownTwyre.Reset();
		containerBloodTwyre.Reset();
		containerBlackTwyre.Reset();
		containerSwevery.Reset();
	}

	public void ComputeUpdate() {
		if (!InstanceByRequest<EngineApplication>.Instance.ViewEnabled)
			return;
		var playerPosition = EngineApplication.PlayerPosition;
		containerBrownTwyre.Update(playerPosition);
		containerBloodTwyre.Update(playerPosition);
		containerBlackTwyre.Update(playerPosition);
		containerSwevery.Update(playerPosition);
	}

	public void LoadBase() {
		containerBrownTwyre =
			new SteppeHerbContainer(ScriptableObjectInstance<ResourceFromCodeData>.Instance.HerbBrownTwyre.Entity.Value,
				ScriptableObjectInstance<ResourceFromCodeData>.Instance.HerbBrownTwyre.PointsPrefab);
		containerBloodTwyre =
			new SteppeHerbContainer(ScriptableObjectInstance<ResourceFromCodeData>.Instance.HerbBloodTwyre.Entity.Value,
				ScriptableObjectInstance<ResourceFromCodeData>.Instance.HerbBloodTwyre.PointsPrefab);
		containerBlackTwyre =
			new SteppeHerbContainer(ScriptableObjectInstance<ResourceFromCodeData>.Instance.HerbBlackTwyre.Entity.Value,
				ScriptableObjectInstance<ResourceFromCodeData>.Instance.HerbBlackTwyre.PointsPrefab);
		containerSwevery =
			new SteppeHerbContainer(ScriptableObjectInstance<ResourceFromCodeData>.Instance.HerbSwevery.Entity.Value,
				ScriptableObjectInstance<ResourceFromCodeData>.Instance.HerbSwevery.PointsPrefab);
		containerBrownTwyre.Amount = brownTwyreAmount;
		containerBloodTwyre.Amount = bloodTwyreAmount;
		containerBlackTwyre.Amount = blackTwyreAmount;
		containerSwevery.Amount = sweveryAmount;
		InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
	}

	public IEnumerator Load(IErrorLoadingHandler errorHandler) {
		LoadBase();
		yield break;
	}

	public IEnumerator Load(XmlElement element, string context, IErrorLoadingHandler errorHandler) {
		LoadBase();
		var node = element[TypeUtility.GetTypeName(GetType())];
		if (node == null)
			errorHandler.LogError(TypeUtility.GetTypeName(GetType()) + " node not found , context : " + context);
		else {
			var reader = new XmlNodeDataReader(node, context);
			((ISerializeStateLoad)this).StateLoad(reader, GetType());
			yield break;
		}
	}

	public void Unload() {
		InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
		containerBrownTwyre.Dispose();
		containerBloodTwyre.Dispose();
		containerBlackTwyre.Dispose();
		containerSwevery.Dispose();
	}

	public void Save(IDataWriter writer, string context) {
		DefaultStateSaveUtility.SaveSerialize(writer, TypeUtility.GetTypeName(GetType()), this);
	}
}