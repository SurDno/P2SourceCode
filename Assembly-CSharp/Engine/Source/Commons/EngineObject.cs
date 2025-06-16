// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.EngineObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using AssetDatabases;
using Engine.Common;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Services;
using Inspectors;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Source.Commons
{
  public abstract class EngineObject : IObject, IDisposable, IIdSetter, ITemplateSetter
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [StateSaveProxy(MemberEnum.None)]
    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    protected Guid id = Guid.Empty;
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Header = true)]
    protected string name = "";
    private IObject template;
    private EngineObject.Parameters parameters;

    public Guid Id
    {
      get => this.id;
      set => this.id = !(value == Guid.Empty) ? value : throw new Exception("Set id as Guid.Empty");
    }

    public string Name
    {
      get => this.name;
      set => this.name = value;
    }

    [Inspected]
    public Guid TemplateId => this.template != null ? this.template.Id : Guid.Empty;

    [Inspected]
    public IObject Template
    {
      get => this.template;
      set => this.template = value;
    }

    [Inspected]
    public bool IsDisposed
    {
      get => this.GetParameter(EngineObject.Parameters.IsDisposed);
      protected set => this.SetParameter(EngineObject.Parameters.IsDisposed, value);
    }

    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    public string Source => AssetDatabaseService.Instance.GetPath(this.Id);

    public bool IsTemplate
    {
      get => this.GetParameter(EngineObject.Parameters.IsTemplate);
      set => this.SetParameter(EngineObject.Parameters.IsTemplate, value);
    }

    public virtual void Dispose()
    {
      if (this.IsDisposed)
        Debug.LogError((object) ("Object already disposed : " + this.GetInfo()));
      else
        this.IsDisposed = true;
    }

    [Inspected(Type = typeof (StoreToSelectionMethodDrawer))]
    private void StoreToSelection(int index)
    {
      ServiceLocator.GetService<SelectionService>().SetSelection(index, (object) this);
    }

    protected bool GetParameter(EngineObject.Parameters parameter)
    {
      return (this.parameters & parameter) != 0;
    }

    protected void SetParameter(EngineObject.Parameters parameter, bool value)
    {
      this.parameters &= ~parameter;
      if (!value)
        return;
      this.parameters |= parameter;
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
