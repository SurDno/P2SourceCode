using System;
using Inspectors;

namespace Engine.Source.Commons
{
  public class EntityView
  {
    [Inspected]
    public GameObject GameObject;

    [Inspected]
    public Vector3 Position { get; set; }

    [Inspected]
    public Quaternion Rotation { get; set; } = Quaternion.identity;

    public event Action OnGameObjectChangedEvent;

    public void InvokeEvent()
    {
      if (OnGameObjectChangedEvent == null)
        return;
      foreach (Delegate invocation in OnGameObjectChangedEvent.GetInvocationList())
      {
        try
        {
          invocation.DynamicInvoke(null);
        }
        catch (Exception ex)
        {
          Debug.LogError((object) ("Error invoke listener, target : " + invocation.Target.GetInfo() + " , owner : " + this.GetInfo() + " , ex : " + ex));
        }
      }
    }
  }
}
