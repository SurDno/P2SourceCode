public abstract class PlagueWebLink : MonoBehaviour
{
  public abstract void BeginAnimation(
    PlagueWeb1 manager,
    PlagueWebPoint pointA,
    PlagueWebPoint pointB);

  public abstract void OnPointDisable(PlagueWebPoint point);
}
