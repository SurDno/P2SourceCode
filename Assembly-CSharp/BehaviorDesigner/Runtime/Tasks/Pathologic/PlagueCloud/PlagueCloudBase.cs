using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic.PlagueCloud
{
  public abstract class PlagueCloudBase : Action
  {
    private global::PlagueCloud[] clouds;

    public abstract global::PlagueCloud.VisibilityType GetVisibilityType();

    public override void OnStart()
    {
      if (this.clouds != null)
        return;
      this.clouds = this.gameObject.GetComponentsInChildren<global::PlagueCloud>();
      if (this.clouds != null)
        return;
      Debug.LogError((object) (this.gameObject.name + ": doesn't contain " + typeof (global::PlagueCloud).Name + " unity component"), (Object) this.gameObject);
    }

    public override TaskStatus OnUpdate()
    {
      if (this.clouds == null)
        return TaskStatus.Failure;
      for (int index = 0; index < this.clouds.Length; ++index)
        this.clouds[index].Visibility = this.GetVisibilityType();
      return TaskStatus.Success;
    }
  }
}
