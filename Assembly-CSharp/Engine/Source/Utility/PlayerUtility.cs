using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Services.CameraServices;
using Engine.Source.UI;
using InputServices;

namespace Engine.Source.Utility
{
  public static class PlayerUtility
  {
    public static bool IsPlayerCanControlling
    {
      get
      {
        if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
          return false;
        CameraKindEnum kind = ServiceLocator.GetService<CameraService>().Kind;
        if (kind != CameraKindEnum.FirstPerson_Controlling && kind != CameraKindEnum.FirstPerson_Controlling2 || CursorService.Instance.Visible || CursorService.Instance.Free || ServiceLocator.GetService<UIService>().IsTransition || !(ServiceLocator.GetService<UIService>().Active is IHudWindow))
          return false;
        IEntity player = ServiceLocator.GetService<ISimulation>().Player;
        if (player == null || !player.IsEnabledInHierarchy)
          return false;
        ParametersComponent component1 = player.GetComponent<ParametersComponent>();
        if (component1 != null)
        {
          IParameter<bool> byName = component1.GetByName<bool>(ParameterNameEnum.Dead);
          if (byName != null && byName.Value)
            return false;
        }
        ILocationItemComponent component2 = player.GetComponent<ILocationItemComponent>();
        return component2 == null || !component2.IsHibernation;
      }
    }

    public static void ShowPlayerHands(bool show)
    {
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      if (player == null)
        return;
      EngineBehavior component = ((IEntityView) player).GameObject?.GetComponent<EngineBehavior>();
      if ((Object) component != (Object) null)
        component.GeometryVisible = show;
    }
  }
}
