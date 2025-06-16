using Engine.Common.Services;
using Engine.Source.Services;
using UnityEngine;

public class GroupPoint : MonoBehaviour
{
  private bool registered = false;

  private void OnEnable() => this.RegisterInService();

  private void OnDisable()
  {
    GroupPointsService service = ServiceLocator.GetService<GroupPointsService>();
    if (service == null)
      return;
    this.registered = false;
    service.RemovePoint(this);
  }

  private void OnUpdate()
  {
    if (this.registered)
      return;
    this.RegisterInService();
  }

  private void RegisterInService()
  {
    GroupPointsService service = ServiceLocator.GetService<GroupPointsService>();
    if (service == null)
      return;
    this.registered = true;
    service.AddPoint(this);
  }
}
