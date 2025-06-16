using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Saves;
using Engine.Source.Services;
using Engine.Source.Services.Saves;
using Engine.Source.Settings;
using Engine.Source.Settings.External;
using Inspectors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

[RuntimeService(new System.Type[] {typeof (LodService)})]
public class LodService : IInitialisable, IUpdatable, ISavesController
{
  [Inspected]
  private List<LodService.Slot> registeredLods = new List<LodService.Slot>();
  [Inspected]
  private bool unloaded;

  [Inspected]
  private int RegisteredCount => this.registeredLods.Count<LodService.Slot>();

  [Inspected]
  private int LodEnabledCount
  {
    get
    {
      return this.registeredLods.Count<LodService.Slot>((Func<LodService.Slot, bool>) (e => e.Enabled));
    }
  }

  void IInitialisable.Initialise()
  {
    if (!ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.useAiLods)
      return;
    InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
  }

  void IInitialisable.Terminate()
  {
    if (!ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.useAiLods)
      return;
    InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
  }

  private float LodAngle(float fov, float aspect)
  {
    return (float) (((double) fov * (double) aspect + 45.0) / 180.0 * 3.1415927410125732);
  }

  void IUpdatable.ComputeUpdate()
  {
    Vector3 position = GameCamera.Instance.CameraTransform.position;
    Vector3 forward = GameCamera.Instance.CameraTransform.forward;
    float angle = this.LodAngle(GameCamera.Instance.Camera.fieldOfView, GameCamera.Instance.Camera.aspect);
    IValue<float> aiLodDistance = InstanceByRequest<GraphicsGameSettings>.Instance.AILodDistance;
    float lodDistance = aiLodDistance.Value;
    float minValue = aiLodDistance.MinValue;
    foreach (LodService.Slot registeredLod in this.registeredLods)
    {
      if (!((UnityEngine.Object) registeredLod.npcState == (UnityEngine.Object) null) && this.UpdaterLodState(registeredLod, lodDistance, minValue, angle, position, forward))
        registeredLod.npcState.OnLodStateChanged(registeredLod.Enabled);
    }
  }

  private bool UpdaterLodState(
    LodService.Slot lod,
    float lodDistance,
    float lodMinDistance,
    float angle,
    Vector3 cameraPosition,
    Vector3 cameraDirection)
  {
    Vector3 lhs = lod.npcState.transform.position - cameraPosition;
    float magnitude = lhs.magnitude;
    bool flag1 = (double) magnitude > (double) lodDistance;
    if (!flag1 && (double) magnitude > (double) lodMinDistance)
      flag1 = (double) (Vector3.Dot(lhs, cameraDirection) / magnitude) < (double) Mathf.Cos(angle);
    bool flag2 = lod.Enabled != flag1;
    lod.Enabled = flag1;
    return flag2;
  }

  public void RegisterLod(NpcState npcState)
  {
    LodService.Slot slot = new LodService.Slot()
    {
      npcState = npcState
    };
    this.registeredLods.Add(slot);
    IValue<float> aiLodDistance = InstanceByRequest<GraphicsGameSettings>.Instance.AILodDistance;
    this.UpdaterLodState(this.registeredLods.Last<LodService.Slot>(), aiLodDistance.Value, aiLodDistance.MinValue, this.LodAngle(GameCamera.Instance.Camera.fieldOfView, GameCamera.Instance.Camera.aspect), GameCamera.Instance.CameraTransform.position, GameCamera.Instance.CameraTransform.forward);
    slot.npcState.OnLodStateChanged(slot.Enabled);
  }

  public void UnregisterLod(NpcState npcState)
  {
    if (this.unloaded)
      return;
    for (int index = 0; index < this.registeredLods.Count; ++index)
    {
      if ((UnityEngine.Object) this.registeredLods[index].npcState == (UnityEngine.Object) npcState)
      {
        this.registeredLods[index].npcState.OnLodStateChanged(false);
        this.registeredLods.RemoveAt(index);
        break;
      }
    }
  }

  IEnumerator ISavesController.Load(IErrorLoadingHandler errorHandler)
  {
    this.unloaded = false;
    yield break;
  }

  IEnumerator ISavesController.Load(
    XmlElement element,
    string context,
    IErrorLoadingHandler errorHandler)
  {
    this.unloaded = false;
    yield break;
  }

  void ISavesController.Unload()
  {
    this.registeredLods.Clear();
    this.unloaded = true;
  }

  void ISavesController.Save(IDataWriter element, string context)
  {
  }

  private class Slot
  {
    public NpcState npcState;
    public bool Enabled;
  }
}
