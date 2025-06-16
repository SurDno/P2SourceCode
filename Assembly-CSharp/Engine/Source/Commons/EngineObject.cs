using System;
using AssetDatabases;
using Engine.Common;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Services;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Commons
{
  public abstract class EngineObject : IObject, IDisposable, IIdSetter, ITemplateSetter
  {
    [DataReadProxy]
    [DataWriteProxy]
    [StateSaveProxy]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    protected Guid id = Guid.Empty;
    [StateSaveProxy]
    [StateLoadProxy]
    [CopyableProxy()]
    [Inspected(Header = true)]
    protected string name = "";
    private IObject template;
    private Parameters parameters;

    public Guid Id
    {
      get => id;
      set => id = !(value == Guid.Empty) ? value : throw new Exception("Set id as Guid.Empty");
    }

    public string Name
    {
      get => name;
      set => name = value;
    }

    [Inspected]
    public Guid TemplateId => template != null ? template.Id : Guid.Empty;

    [Inspected]
    public IObject Template
    {
      get => template;
      set => template = value;
    }

    [Inspected]
    public bool IsDisposed
    {
      get => GetParameter(Parameters.IsDisposed);
      protected set => SetParameter(Parameters.IsDisposed, value);
    }

    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    public string Source => AssetDatabaseService.Instance.GetPath(Id);

    public bool IsTemplate
    {
      get => GetParameter(Parameters.IsTemplate);
      set => SetParameter(Parameters.IsTemplate, value);
    }

    public virtual void Dispose()
    {
      if (IsDisposed)
        Debug.LogError("Object already disposed : " + this.GetInfo());
      else
        IsDisposed = true;
    }

    [Inspected(Type = typeof (StoreToSelectionMethodDrawer))]
    private void StoreToSelection(int index)
    {
      ServiceLocator.GetService<SelectionService>().SetSelection(index, this);
    }

    protected bool GetParameter(Parameters parameter)
    {
      return (parameters & parameter) != 0;
    }

    protected void SetParameter(Parameters parameter, bool value)
    {
      parameters &= ~parameter;
      if (!value)
        return;
      parameters |= parameter;
    }

    [Flags]
    public enum Parameters
    {
      None = 0,
      IsDisposed = 1,
      IsTemplate = 2,
      DontSave = 4,
      IsPlayer = 8,
      IsEnabledInHierarchy = 16, // 0x00000010
      IsAdded = 32, // 0x00000020
      IsEnabled = 64, // 0x00000040
      IsAttached = 128, // 0x00000080
    }
  }
}
