using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Behaviours.Engines.Controllers;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  [TaskDescription("Look at points of interest. Run this as parallel selector task")]
  [TaskCategory("Pathologic")]
  [TaskIcon("Pathologic_LongIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (LookAtPointOfInterest))]
  public class LookAtPointOfInterest : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    protected DetectorComponent detector;
    protected IEntity entity;
    private IKController lookAtController;
    private const float updateFrequency = 1f;
    private bool inited = false;
    private float timeLeft;
    private List<Transform> potentialPOIs = new List<Transform>();
    private List<Transform> potentialPriorityPOIs = new List<Transform>();

    public override void OnAwake()
    {
      this.inited = false;
      this.entity = EntityUtility.GetEntity(this.gameObject);
      if (this.entity == null)
      {
        Debug.LogWarning((object) (this.gameObject.name + " : entity not found, method : " + this.GetType().Name + ":" + MethodBase.GetCurrentMethod().Name), (UnityEngine.Object) this.gameObject);
      }
      else
      {
        this.detector = (DetectorComponent) this.entity.GetComponent<IDetectorComponent>();
        if (this.detector == null)
        {
          Debug.LogWarningFormat("{0}: doesn't contain {1} engine component", (object) this.gameObject.name, (object) typeof (IDetectorComponent).Name);
        }
        else
        {
          this.lookAtController = this.gameObject.GetComponent<IKController>();
          if ((UnityEngine.Object) this.lookAtController == (UnityEngine.Object) null)
            Debug.LogErrorFormat("{0}: doesn't contain {1} unity component", (object) this.gameObject.name, (object) typeof (IKController).Name);
          else
            this.inited = true;
        }
      }
    }

    public override void OnStart()
    {
      if (!this.inited)
        return;
      this.timeLeft = 0.0f;
    }

    public override void OnEnd()
    {
      if (!this.inited)
        return;
      this.lookAtController.LookTarget = (Transform) null;
    }

    public override TaskStatus OnUpdate()
    {
      if (!this.inited)
        return TaskStatus.Failure;
      this.timeLeft -= Time.deltaTime;
      if ((double) this.timeLeft < 0.0)
      {
        this.timeLeft = 1f;
        this.UpdateLookAtTarget();
      }
      return TaskStatus.Running;
    }

    private void UpdateLookAtTarget()
    {
      this.potentialPOIs.Clear();
      this.potentialPriorityPOIs.Clear();
      foreach (IDetectableComponent detectableComponent in this.detector.Visible)
      {
        if (!detectableComponent.IsDisposed)
        {
          IEntityView owner = (IEntityView) ((EngineComponent) detectableComponent).Owner;
          if (!((UnityEngine.Object) owner.GameObject == (UnityEngine.Object) null))
          {
            Engine.Behaviours.Engines.PointsOfInterest.LookAtPointOfInterest component1 = owner.GameObject.GetComponent<Engine.Behaviours.Engines.PointsOfInterest.LookAtPointOfInterest>();
            if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
            {
              if ((UnityEngine.Object) component1.POI != (UnityEngine.Object) null)
                this.potentialPOIs.Add(component1.POI);
              else
                this.potentialPOIs.Add(component1.transform);
            }
            Pivot component2 = owner.GameObject.GetComponent<Pivot>();
            if ((UnityEngine.Object) component2 != (UnityEngine.Object) null && (UnityEngine.Object) component2.Head != (UnityEngine.Object) null)
              this.potentialPriorityPOIs.Add(component2.Head.transform);
          }
        }
      }
      if (this.potentialPriorityPOIs.Count > 0)
      {
        this.potentialPriorityPOIs.Sort(new Comparison<Transform>(this.ComparePOIs));
        this.lookAtController.LookTarget = this.potentialPriorityPOIs[0];
      }
      else if (this.potentialPOIs.Count > 0)
      {
        this.potentialPOIs.Sort(new Comparison<Transform>(this.ComparePOIs));
        this.lookAtController.LookTarget = this.potentialPOIs[0];
      }
      else
        this.lookAtController.LookTarget = (Transform) null;
    }

    private int ComparePOIs(Transform a, Transform b)
    {
      float sqrMagnitude1 = (a.position - this.gameObject.transform.position).sqrMagnitude;
      float sqrMagnitude2 = (b.position - this.gameObject.transform.position).sqrMagnitude;
      return (UnityEngine.Object) a == (UnityEngine.Object) b ? 0 : ((double) sqrMagnitude1 < (double) sqrMagnitude2 ? -1 : 1);
    }

    public IEnumerator ExecuteSecond(float delay, System.Action action)
    {
      float time = Time.unscaledTime;
      while ((double) time + (double) delay > (double) Time.unscaledTime)
      {
        System.Action action1 = action;
        if (action1 != null)
          action1();
        yield return (object) null;
      }
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
    }
  }
}
