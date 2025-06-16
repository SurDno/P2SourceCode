using UnityEngine;

namespace SoundPropagation
{
  public class SPCell : MonoBehaviour
  {
    public SPCellProfile Profile;
    public SPPortal[] Portals;
    private bool initialized = false;
    private SPIndoor group;
    private const int SearchBufferLength = 8;
    private static Collider[] searchBuffer = new Collider[8];

    private void Check()
    {
      if (this.initialized)
        return;
      this.group = this.GetComponentInParent<SPIndoor>();
      this.initialized = true;
    }

    public SPIndoor Group
    {
      get
      {
        this.Check();
        return this.group;
      }
    }

    public Filtering FilteringPerMeter
    {
      get
      {
        return (Object) this.Profile != (Object) null ? this.Profile.FilteringPerMeter : new Filtering();
      }
    }

    public float LossPerMeter
    {
      get => (Object) this.Profile != (Object) null ? this.Profile.FilteringPerMeter.Loss : 0.0f;
    }

    public static SPCell Find(Vector3 position, LayerMask layerMask)
    {
      int num = Physics.OverlapSphereNonAlloc(position, 1f / 1000f, SPCell.searchBuffer, (int) layerMask, QueryTriggerInteraction.Collide);
      if (num > 0)
      {
        if (num > 8)
          num = 8;
        for (int index = 0; index < num; ++index)
        {
          SPCell componentInParent = SPCell.searchBuffer[index].GetComponentInParent<SPCell>();
          if ((Object) componentInParent != (Object) null)
            return componentInParent;
        }
      }
      return (SPCell) null;
    }
  }
}
