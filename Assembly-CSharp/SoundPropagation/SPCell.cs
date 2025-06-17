using UnityEngine;

namespace SoundPropagation
{
  public class SPCell : MonoBehaviour
  {
    public SPCellProfile Profile;
    public SPPortal[] Portals;
    private bool initialized;
    private SPIndoor group;
    private const int SearchBufferLength = 8;
    private static Collider[] searchBuffer = new Collider[8];

    private void Check()
    {
      if (initialized)
        return;
      group = GetComponentInParent<SPIndoor>();
      initialized = true;
    }

    public SPIndoor Group
    {
      get
      {
        Check();
        return group;
      }
    }

    public Filtering FilteringPerMeter => Profile != null ? Profile.FilteringPerMeter : new Filtering();

    public float LossPerMeter => Profile != null ? Profile.FilteringPerMeter.Loss : 0.0f;

    public static SPCell Find(Vector3 position, LayerMask layerMask)
    {
      int num = Physics.OverlapSphereNonAlloc(position, 1f / 1000f, searchBuffer, layerMask, QueryTriggerInteraction.Collide);
      if (num > 0)
      {
        if (num > 8)
          num = 8;
        for (int index = 0; index < num; ++index)
        {
          SPCell componentInParent = searchBuffer[index].GetComponentInParent<SPCell>();
          if (componentInParent != null)
            return componentInParent;
        }
      }
      return null;
    }
  }
}
