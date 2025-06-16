using System;
using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;

namespace Engine.Source.Components;

[Factory(typeof(IMarketComponent))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave |
               TypeEnum.StateLoad)]
public class MarketComponent : EngineComponent, IMarketComponent, IComponent, INeedSave {
	[StateSaveProxy]
	[StateLoadProxy]
	[DataReadProxy]
	[DataWriteProxy]
	[CopyableProxy()]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	protected bool isEnabled = true;

	private Dictionary<string, Dictionary<string, float>> storablesFactor = new();

	[Inspected(Mutable = true)]
	public bool IsEnabled {
		get => isEnabled;
		set {
			isEnabled = value;
			OnChangeEnabled();
		}
	}

	public event Action OnFillPrices;

	public Dictionary<string, Dictionary<string, float>> StorablesFactor => storablesFactor;

	public bool NeedSave => true;

	public void FillPrices() {
		var onFillPrices = OnFillPrices;
		if (onFillPrices == null)
			return;
		onFillPrices();
	}
}