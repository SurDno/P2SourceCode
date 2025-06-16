using Engine.Common;
using Engine.Common.Components.Gate;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Settings.External;
using Inspectors;
using SoundPropagation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class InteriorDoor : MonoBehaviour, IEntityAttachable
{
  [SerializeField]
  private float angle;
  [SerializeField]
  private OcclusionPortal occlusionPortal;
  [SerializeField]
  private SPPortal soundPropagationPortal;
  [SerializeField]
  private NavMeshObstacle navMeshObstacle;
  [Inspected]
  private DoorComponent gate;
  [Inspected]
  private float currentAngle;
  [Inspected]
  private bool update;
  [Inspected]
  private bool openned;
  private HashSet<IEntity> targets = new HashSet<IEntity>();

  private bool IsOppening
  {
    get
    {
      return this.gate.Opened.Value || this.targets.Count != 0 && this.gate.LockState.Value == LockState.Unlocked;
    }
  }

  public void Attach(IEntity owner)
  {
    this.update = true;
    this.gate = owner.GetComponent<DoorComponent>();
    if (this.gate != null)
    {
      this.openned = this.IsOppening;
      this.UpdateAngle();
    }
    this.UpdateOcclusion();
    this.UpdateObstacle();
  }

  public void Detach() => this.gate = (DoorComponent) null;

  public void Update()
  {
    if (this.gate == null)
      return;
    if (this.openned != this.IsOppening)
    {
      this.openned = this.IsOppening;
      this.UpdateAngle();
    }
    if (!this.update)
      return;
    float num = Time.deltaTime * ExternalSettingsInstance<ExternalCommonSettings>.Instance.DoorSpeed;
    float f = this.IsOppening ? this.GetAngle() : 0.0f;
    if (float.IsNaN(f))
      return;
    if ((double) this.currentAngle > (double) f)
    {
      this.currentAngle -= num;
      if ((double) this.currentAngle <= (double) f)
      {
        this.currentAngle = f;
        this.update = false;
      }
    }
    else if ((double) this.currentAngle < (double) f)
    {
      this.currentAngle += num;
      if ((double) this.currentAngle >= (double) f)
      {
        this.currentAngle = f;
        this.update = false;
      }
    }
    else
      this.update = false;
    this.transform.localRotation = Quaternion.Euler(0.0f, this.currentAngle, 0.0f);
    this.UpdateOcclusion();
    this.UpdateObstacle();
  }

  private void UpdateOcclusion()
  {
    if ((Object) this.occlusionPortal != (Object) null)
      this.occlusionPortal.open = (double) this.currentAngle != 0.0;
    if (!((Object) this.soundPropagationPortal != (Object) null))
      return;
    this.soundPropagationPortal.Occlusion = (float) ((1.0 - (double) Mathf.Min(1f, Mathf.Abs(this.currentAngle) / 45f)) * 1.5);
  }

  private void UpdateObstacle()
  {
    if (!((Object) this.navMeshObstacle != (Object) null))
      return;
    bool flag = Mathf.Approximately(Mathf.Abs(this.currentAngle), Mathf.Abs(this.angle));
    if (this.navMeshObstacle.enabled != flag)
      this.navMeshObstacle.enabled = flag;
  }

  private float GetAngle()
  {
    return this.targets.Count != 0 ? this.GetAngle(this.targets.First<IEntity>()) : this.GetAngle(ServiceLocator.GetService<ISimulation>().Player);
  }

  private void UpdateAngle()
  {
    if (this.update || (this.IsOppening ? (double) this.GetAngle() : 0.0) == (double) this.currentAngle)
      return;
    this.update = true;
  }

  private float GetAngle(IEntity entity)
  {
    GameObject gameObject = ((IEntityView) entity).GameObject;
    if ((Object) gameObject == (Object) null)
      return float.NaN;
    float num = Mathf.Sign(Vector3.Dot(this.transform.parent.rotation * Vector3.forward, gameObject.transform.rotation * Vector3.forward));
    return Mathf.Abs(this.angle) * -num;
  }

  public void Invalidate(HashSet<IEntity> targets)
  {
    this.targets = targets;
    this.update = true;
  }
}
