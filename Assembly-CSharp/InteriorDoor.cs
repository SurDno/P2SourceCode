using System.Collections.Generic;
using System.Linq;
using Engine.Common;
using Engine.Common.Components.Gate;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Settings.External;
using Inspectors;
using SoundPropagation;

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
      return gate.Opened.Value || targets.Count != 0 && gate.LockState.Value == LockState.Unlocked;
    }
  }

  public void Attach(IEntity owner)
  {
    update = true;
    gate = owner.GetComponent<DoorComponent>();
    if (gate != null)
    {
      openned = IsOppening;
      UpdateAngle();
    }
    UpdateOcclusion();
    UpdateObstacle();
  }

  public void Detach() => gate = null;

  public void Update()
  {
    if (gate == null)
      return;
    if (openned != IsOppening)
    {
      openned = IsOppening;
      UpdateAngle();
    }
    if (!update)
      return;
    float num = Time.deltaTime * ExternalSettingsInstance<ExternalCommonSettings>.Instance.DoorSpeed;
    float f = IsOppening ? GetAngle() : 0.0f;
    if (float.IsNaN(f))
      return;
    if (currentAngle > (double) f)
    {
      currentAngle -= num;
      if (currentAngle <= (double) f)
      {
        currentAngle = f;
        update = false;
      }
    }
    else if (currentAngle < (double) f)
    {
      currentAngle += num;
      if (currentAngle >= (double) f)
      {
        currentAngle = f;
        update = false;
      }
    }
    else
      update = false;
    this.transform.localRotation = Quaternion.Euler(0.0f, currentAngle, 0.0f);
    UpdateOcclusion();
    UpdateObstacle();
  }

  private void UpdateOcclusion()
  {
    if ((Object) occlusionPortal != (Object) null)
      occlusionPortal.open = currentAngle != 0.0;
    if (!((Object) soundPropagationPortal != (Object) null))
      return;
    soundPropagationPortal.Occlusion = (float) ((1.0 - (double) Mathf.Min(1f, Mathf.Abs(currentAngle) / 45f)) * 1.5);
  }

  private void UpdateObstacle()
  {
    if (!((Object) navMeshObstacle != (Object) null))
      return;
    bool flag = Mathf.Approximately(Mathf.Abs(currentAngle), Mathf.Abs(angle));
    if (navMeshObstacle.enabled != flag)
      navMeshObstacle.enabled = flag;
  }

  private float GetAngle()
  {
    return targets.Count != 0 ? GetAngle(targets.First()) : GetAngle(ServiceLocator.GetService<ISimulation>().Player);
  }

  private void UpdateAngle()
  {
    if (update || (IsOppening ? GetAngle() : 0.0) == currentAngle)
      return;
    update = true;
  }

  private float GetAngle(IEntity entity)
  {
    GameObject gameObject = ((IEntityView) entity).GameObject;
    if ((Object) gameObject == (Object) null)
      return float.NaN;
    float num = Mathf.Sign(Vector3.Dot(this.transform.parent.rotation * Vector3.forward, gameObject.transform.rotation * Vector3.forward));
    return Mathf.Abs(angle) * -num;
  }

  public void Invalidate(HashSet<IEntity> targets)
  {
    this.targets = targets;
    update = true;
  }
}
