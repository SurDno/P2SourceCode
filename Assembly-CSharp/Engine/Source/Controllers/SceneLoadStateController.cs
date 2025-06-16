using Engine.Common;
using Engine.Common.Components;
using Engine.Source.Commons;
using Inspectors;
using StateSetters;

namespace Engine.Source.Controllers
{
  public class SceneLoadStateController : MonoBehaviour, IEntityAttachable
  {
    [SerializeField]
    private StateSetterItem[] sceneLoadState;
    private ILocationComponent location;

    public void Attach(IEntity owner)
    {
      location = owner.GetComponent<ILocationComponent>();
      if (location == null)
        return;
      location.OnHibernationChanged += LocationOnChangeHibernation;
      LocationOnChangeHibernation(location);
    }

    public void Detach()
    {
      if (location == null)
        return;
      location.OnHibernationChanged -= LocationOnChangeHibernation;
    }

    private void LocationOnChangeHibernation(ILocationComponent sender)
    {
      if (location.IsHibernation)
        SceneLoadOff();
      else
        SceneLoadOn();
    }

    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    private void SceneLoadOn() => sceneLoadState.Apply(true);

    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    private void SceneLoadOff() => sceneLoadState.Apply(false);
  }
}
