using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;
using System;
using System.Collections.Generic;

namespace Engine.Source.Components
{
  [Factory(typeof (IMarketComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class MarketComponent : EngineComponent, IMarketComponent, IComponent, INeedSave
  {
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool isEnabled = true;
    private Dictionary<string, Dictionary<string, float>> storablesFactor = new Dictionary<string, Dictionary<string, float>>();

    [Inspected(Mutable = true)]
    public bool IsEnabled
    {
      get => this.isEnabled;
      set
      {
        this.isEnabled = value;
        this.OnChangeEnabled();
      }
    }

    public event Action OnFillPrices;

    public Dictionary<string, Dictionary<string, float>> StorablesFactor => this.storablesFactor;

    public bool NeedSave => true;

    public void FillPrices()
    {
      Action onFillPrices = this.OnFillPrices;
      if (onFillPrices == null)
        return;
      onFillPrices();
    }
  }
}
