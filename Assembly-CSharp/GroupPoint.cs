using Engine.Common.Services;
using Engine.Source.Services;

public class GroupPoint : MonoBehaviour
{
  private bool registered;

  private void OnEnable() => RegisterInService();

  private void OnDisable()
  {
    GroupPointsService service = ServiceLocator.GetService<GroupPointsService>();
    if (service == null)
      return;
    registered = false;
    service.RemovePoint(this);
  }

  private void OnUpdate()
  {
    if (registered)
      return;
    RegisterInService();
  }

  private void RegisterInService()
  {
    GroupPointsService service = ServiceLocator.GetService<GroupPointsService>();
    if (service == null)
      return;
    registered = true;
    service.AddPoint(this);
  }
}
