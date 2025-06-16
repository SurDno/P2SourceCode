// Decompiled with JetBrains decompiler
// Type: Engine.Impl.Services.SteppeHerbService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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
using System.Collections;
using System.Xml;
using UnityEngine;

#nullable disable
namespace Engine.Impl.Services
{
  [GameService(new System.Type[] {typeof (SteppeHerbService), typeof (ISteppeHerbService)})]
  [GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class SteppeHerbService : ISteppeHerbService, IUpdatable, ISavesController
  {
    [Inspected]
    private SteppeHerbContainer containerBrownTwyre;
    [Inspected]
    private SteppeHerbContainer containerBloodTwyre;
    [Inspected]
    private SteppeHerbContainer containerBlackTwyre;
    [Inspected]
    private SteppeHerbContainer containerSwevery;
    private int brownTwyreAmount;
    private int bloodTwyreAmount;
    private int blackTwyreAmount;
    private int sweveryAmount;

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected(Mutable = true)]
    public int BrownTwyreAmount
    {
      get => this.brownTwyreAmount;
      set
      {
        this.brownTwyreAmount = value;
        if (this.containerBrownTwyre == null)
          return;
        this.containerBrownTwyre.Amount = value;
      }
    }

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected(Mutable = true)]
    public int BloodTwyreAmount
    {
      get => this.bloodTwyreAmount;
      set
      {
        this.bloodTwyreAmount = value;
        if (this.containerBloodTwyre == null)
          return;
        this.containerBloodTwyre.Amount = value;
      }
    }

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected(Mutable = true)]
    public int BlackTwyreAmount
    {
      get => this.blackTwyreAmount;
      set
      {
        this.blackTwyreAmount = value;
        if (this.containerBlackTwyre == null)
          return;
        this.containerBlackTwyre.Amount = value;
      }
    }

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected(Mutable = true)]
    public int SweveryAmount
    {
      get => this.sweveryAmount;
      set
      {
        this.sweveryAmount = value;
        if (this.containerSwevery == null)
          return;
        this.containerSwevery.Amount = value;
      }
    }

    public void Reset()
    {
      this.containerBrownTwyre.Reset();
      this.containerBloodTwyre.Reset();
      this.containerBlackTwyre.Reset();
      this.containerSwevery.Reset();
    }

    public void ComputeUpdate()
    {
      if (!InstanceByRequest<EngineApplication>.Instance.ViewEnabled)
        return;
      Vector3 playerPosition = EngineApplication.PlayerPosition;
      this.containerBrownTwyre.Update(playerPosition);
      this.containerBloodTwyre.Update(playerPosition);
      this.containerBlackTwyre.Update(playerPosition);
      this.containerSwevery.Update(playerPosition);
    }

    public void LoadBase()
    {
      this.containerBrownTwyre = new SteppeHerbContainer(ScriptableObjectInstance<ResourceFromCodeData>.Instance.HerbBrownTwyre.Entity.Value, ScriptableObjectInstance<ResourceFromCodeData>.Instance.HerbBrownTwyre.PointsPrefab);
      this.containerBloodTwyre = new SteppeHerbContainer(ScriptableObjectInstance<ResourceFromCodeData>.Instance.HerbBloodTwyre.Entity.Value, ScriptableObjectInstance<ResourceFromCodeData>.Instance.HerbBloodTwyre.PointsPrefab);
      this.containerBlackTwyre = new SteppeHerbContainer(ScriptableObjectInstance<ResourceFromCodeData>.Instance.HerbBlackTwyre.Entity.Value, ScriptableObjectInstance<ResourceFromCodeData>.Instance.HerbBlackTwyre.PointsPrefab);
      this.containerSwevery = new SteppeHerbContainer(ScriptableObjectInstance<ResourceFromCodeData>.Instance.HerbSwevery.Entity.Value, ScriptableObjectInstance<ResourceFromCodeData>.Instance.HerbSwevery.PointsPrefab);
      this.containerBrownTwyre.Amount = this.brownTwyreAmount;
      this.containerBloodTwyre.Amount = this.bloodTwyreAmount;
      this.containerBlackTwyre.Amount = this.blackTwyreAmount;
      this.containerSwevery.Amount = this.sweveryAmount;
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
    }

    public IEnumerator Load(IErrorLoadingHandler errorHandler)
    {
      this.LoadBase();
      yield break;
    }

    public IEnumerator Load(XmlElement element, string context, IErrorLoadingHandler errorHandler)
    {
      this.LoadBase();
      XmlElement node = element[TypeUtility.GetTypeName(this.GetType())];
      if (node == null)
      {
        errorHandler.LogError(TypeUtility.GetTypeName(this.GetType()) + " node not found , context : " + context);
      }
      else
      {
        XmlNodeDataReader reader = new XmlNodeDataReader((XmlNode) node, context);
        ((ISerializeStateLoad) this).StateLoad((IDataReader) reader, this.GetType());
        yield break;
      }
    }

    public void Unload()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
      this.containerBrownTwyre.Dispose();
      this.containerBloodTwyre.Dispose();
      this.containerBlackTwyre.Dispose();
      this.containerSwevery.Dispose();
    }

    public void Save(IDataWriter writer, string context)
    {
      DefaultStateSaveUtility.SaveSerialize<SteppeHerbService>(writer, TypeUtility.GetTypeName(this.GetType()), this);
    }
  }
}
