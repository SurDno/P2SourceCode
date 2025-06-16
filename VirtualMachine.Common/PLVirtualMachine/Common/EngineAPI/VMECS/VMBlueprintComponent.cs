using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("BlueprintComponent", typeof (IBlueprintComponent))]
  public class VMBlueprintComponent : VMEngineComponent<IBlueprintComponent>
  {
    public const string ComponentName = "BlueprintComponent";

    [Property("BlueprintObject", "", false)]
    public IBlueprintObject Blueprint
    {
      get => this.Component.Blueprint;
      set => this.Component.Blueprint = value;
    }

    [Property("Is Started", "", false)]
    public bool IsStarted => this.Component.IsStarted;

    [Property("Is Attached", "", false)]
    public bool IsAttached => this.Component.IsAttached;

    [Method("Start", "", "")]
    public void Start()
    {
      try
      {
        this.Component.Start();
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Blueprint component start method error: {0} in object {1} at {2}", (object) ex.ToString(), (object) this.Parent.Name, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
      }
    }

    [Method("Start with target", "IEntity", "")]
    public void Start_v1(IEntity target)
    {
      try
      {
        this.Component.Start(target);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Blueprint component start method error: {0} in object {1} at {2}", (object) ex.ToString(), (object) this.Parent.Name, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
      }
    }

    [Method("Stop", "", "")]
    public void Stop() => this.Component.Stop();

    [Method("Send event", "name", "")]
    public void SendEvent(string name)
    {
      try
      {
        this.Component.SendEvent(name);
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Blueprint component send event method error: {0} in object {1} at {2}", (object) ex.ToString(), (object) this.Parent.Name, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
      }
    }

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      this.Component.CompleteEvent -= new Action<IBlueprintComponent>(this.CompleteEvent);
      this.Component.AttachEvent -= new Action<IBlueprintComponent>(this.AttachEvent);
      base.Clear();
    }

    protected override void Init()
    {
      if (this.IsTemplate)
        return;
      this.Component.CompleteEvent += new Action<IBlueprintComponent>(this.CompleteEvent);
      this.Component.AttachEvent += new Action<IBlueprintComponent>(this.AttachEvent);
    }

    private void CompleteEvent(IBlueprintComponent target)
    {
      Action complete = this.Complete;
      if (complete == null)
        return;
      complete();
    }

    private void AttachEvent(IBlueprintComponent target)
    {
      Action attach = this.Attach;
      if (attach == null)
        return;
      attach();
    }

    [Event("Complete", "")]
    public event Action Complete;

    [Event("Attach", "")]
    public event Action Attach;
  }
}
