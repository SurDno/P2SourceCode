namespace BehaviorDesigner.Runtime.Tasks.Pathologic.PlagueCloud
{
  public abstract class PlagueCloudBase : Action
  {
    private global::PlagueCloud[] clouds;

    public abstract global::PlagueCloud.VisibilityType GetVisibilityType();

    public override void OnStart()
    {
      if (clouds != null)
        return;
      clouds = gameObject.GetComponentsInChildren<global::PlagueCloud>();
      if (clouds != null)
        return;
      Debug.LogError((object) (gameObject.name + ": doesn't contain " + typeof (global::PlagueCloud).Name + " unity component"), (Object) gameObject);
    }

    public override TaskStatus OnUpdate()
    {
      if (clouds == null)
        return TaskStatus.Failure;
      for (int index = 0; index < clouds.Length; ++index)
        clouds[index].Visibility = GetVisibilityType();
      return TaskStatus.Success;
    }
  }
}
