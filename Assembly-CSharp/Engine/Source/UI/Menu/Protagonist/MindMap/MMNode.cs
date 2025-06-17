using System;
using Engine.Common.Commons;
using Engine.Common.MindMap;
using Engine.Common.Services;
using Engine.Common.Types;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;

namespace Engine.Source.UI.Menu.Protagonist.MindMap
{
  [Factory(typeof (IMMNode))]
  public class MMNode : IMMNode, IIdSetter
  {
    private IMMContent content;
    private bool undiscovered;
    private bool notificationQueued;

    [Inspected]
    public Guid Id { get; set; }

    [Inspected]
    public MMWindow MindMap { get; set; }

    [Inspected]
    public Position Position { get; set; }

    [Inspected]
    public MMNodeKind NodeKind { get; set; }

    [Inspected]
    public bool Undiscovered
    {
      get => undiscovered;
      set
      {
        if (undiscovered == value)
          return;
        undiscovered = value;
        if (!InstanceByRequest<EngineApplication>.Instance.ViewEnabled)
          return;
        ServiceLocator.GetService<MMService>().FireChangeUndiscoveredEvent();
      }
    }

    [Inspected]
    public IMMContent Content
    {
      get => content;
      set
      {
        if (content == value)
          return;
        content = value;
        if (!InstanceByRequest<EngineApplication>.Instance.ViewEnabled)
          return;
        if (content != null)
        {
          if (!notificationQueued)
          {
            notificationQueued = true;
            CoroutineService.Instance.WaitFrame(Notify);
          }
          Undiscovered = true;
        }
        else
          Undiscovered = false;
      }
    }

    private void Notify()
    {
      if (content != null)
      {
        NotificationService service = ServiceLocator.GetService<NotificationService>();
        service.AddNotify(NotificationEnum.MindMap, []);
        service.AddNotify(NotificationEnum.MindMapNode, content);
      }
      notificationQueued = false;
    }
  }
}
