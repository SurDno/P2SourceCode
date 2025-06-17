using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Saves;
using Engine.Source.Services;
using Engine.Source.Services.Saves;
using Engine.Source.Settings;
using Engine.Source.Settings.External;
using Inspectors;
using UnityEngine;

[RuntimeService(typeof (LodService))]
public class LodService : IInitialisable, IUpdatable, ISavesController
{
  [Inspected]
  private List<Slot> registeredLods = [];
  [Inspected]
  private bool unloaded;

  [Inspected]
  private int RegisteredCount => registeredLods.Count();

  [Inspected]
  private int LodEnabledCount
  {
    get
    {
      return registeredLods.Count(e => e.Enabled);
    }
  }

  void IInitialisable.Initialise()
  {
    if (!ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.useAiLods)
      return;
    InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
  }

  void IInitialisable.Terminate()
  {
    if (!ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.useAiLods)
      return;
    InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
  }

  private float LodAngle(float fov, float aspect)
  {
    return (float) ((fov * (double) aspect + 45.0) / 180.0 * 3.1415927410125732);
  }

  void IUpdatable.ComputeUpdate()
  {
    Vector3 position = GameCamera.Instance.CameraTransform.position;
    Vector3 forward = GameCamera.Instance.CameraTransform.forward;
    float angle = LodAngle(GameCamera.Instance.Camera.fieldOfView, GameCamera.Instance.Camera.aspect);
    IValue<float> aiLodDistance = InstanceByRequest<GraphicsGameSettings>.Instance.AILodDistance;
    float lodDistance = aiLodDistance.Value;
    float minValue = aiLodDistance.MinValue;
    foreach (Slot registeredLod in registeredLods)
    {
      if (!(registeredLod.npcState == null) && UpdaterLodState(registeredLod, lodDistance, minValue, angle, position, forward))
        registeredLod.npcState.OnLodStateChanged(registeredLod.Enabled);
    }
  }

  private bool UpdaterLodState(
    Slot lod,
    float lodDistance,
    float lodMinDistance,
    float angle,
    Vector3 cameraPosition,
    Vector3 cameraDirection)
  {
    Vector3 lhs = lod.npcState.transform.position - cameraPosition;
    float magnitude = lhs.magnitude;
    bool flag1 = magnitude > (double) lodDistance;
    if (!flag1 && magnitude > (double) lodMinDistance)
      flag1 = Vector3.Dot(lhs, cameraDirection) / magnitude < (double) Mathf.Cos(angle);
    bool flag2 = lod.Enabled != flag1;
    lod.Enabled = flag1;
    return flag2;
  }

  public void RegisterLod(NpcState npcState)
  {
    Slot slot = new Slot {
      npcState = npcState
    };
    registeredLods.Add(slot);
    IValue<float> aiLodDistance = InstanceByRequest<GraphicsGameSettings>.Instance.AILodDistance;
    UpdaterLodState(registeredLods.Last(), aiLodDistance.Value, aiLodDistance.MinValue, LodAngle(GameCamera.Instance.Camera.fieldOfView, GameCamera.Instance.Camera.aspect), GameCamera.Instance.CameraTransform.position, GameCamera.Instance.CameraTransform.forward);
    slot.npcState.OnLodStateChanged(slot.Enabled);
  }

  public void UnregisterLod(NpcState npcState)
  {
    if (unloaded)
      return;
    for (int index = 0; index < registeredLods.Count; ++index)
    {
      if (registeredLods[index].npcState == npcState)
      {
        registeredLods[index].npcState.OnLodStateChanged(false);
        registeredLods.RemoveAt(index);
        break;
      }
    }
  }

  IEnumerator ISavesController.Load(IErrorLoadingHandler errorHandler)
  {
    unloaded = false;
    yield break;
  }

  IEnumerator ISavesController.Load(
    XmlElement element,
    string context,
    IErrorLoadingHandler errorHandler)
  {
    unloaded = false;
    yield break;
  }

  void ISavesController.Unload()
  {
    registeredLods.Clear();
    unloaded = true;
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
