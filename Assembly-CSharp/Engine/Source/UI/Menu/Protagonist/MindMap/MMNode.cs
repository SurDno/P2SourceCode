// Decompiled with JetBrains decompiler
// Type: Engine.Source.UI.Menu.Protagonist.MindMap.MMNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Commons;
using Engine.Common.MindMap;
using Engine.Common.Services;
using Engine.Common.Types;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;
using System;

#nullable disable
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
      get => this.undiscovered;
      set
      {
        if (this.undiscovered == value)
          return;
        this.undiscovered = value;
        if (!InstanceByRequest<EngineApplication>.Instance.ViewEnabled)
          return;
        ServiceLocator.GetService<MMService>().FireChangeUndiscoveredEvent();
      }
    }

    [Inspected]
    public IMMContent Content
    {
      get => this.content;
      set
      {
        if (this.content == value)
          return;
        this.content = value;
        if (!InstanceByRequest<EngineApplication>.Instance.ViewEnabled)
          return;
        if (this.content != null)
        {
          if (!this.notificationQueued)
          {
            this.notificationQueued = true;
            CoroutineService.Instance.WaitFrame(new Action(this.Notify));
          }
          this.Undiscovered = true;
        }
        else
          this.Undiscovered = false;
      }
    }

    private void Notify()
    {
      if (this.content != null)
      {
        NotificationService service = ServiceLocator.GetService<NotificationService>();
        service.AddNotify(NotificationEnum.MindMap, Array.Empty<object>());
        service.AddNotify(NotificationEnum.MindMapNode, new object[1]
        {
          (object) this.content
        });
      }
      this.notificationQueued = false;
    }
  }
}
