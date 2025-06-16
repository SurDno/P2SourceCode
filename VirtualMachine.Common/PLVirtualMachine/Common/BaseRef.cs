// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.BaseRef
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Engine.Common.Comparers;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using System;
using System.Collections.Generic;

#nullable disable
namespace PLVirtualMachine.Common
{
  public abstract class BaseRef : IRef, IVariable, INamed, IVMStringSerializable
  {
    private static HashSet<ulong> dublicate = new HashSet<ulong>((IEqualityComparer<ulong>) UlongComparer.Instance);
    private ulong baseGuid;
    private IObject staticInstance;

    public virtual bool IsEqual(IVariable other) => this.Name == other.Name;

    public virtual string Name
    {
      get
      {
        if (this.staticInstance != null)
          return this.staticInstance.GuidStr;
        return this.baseGuid != 0UL ? this.baseGuid.ToString() : "";
      }
    }

    public virtual VMType Type => new VMType(typeof (Nullable));

    public ulong BaseGuid
    {
      get => this.baseGuid;
      protected set => this.baseGuid = value;
    }

    public virtual bool Empty => this.baseGuid == 0UL;

    public virtual bool Exist => !this.Empty;

    public virtual EContextVariableCategory Category
    {
      get => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_OBJECT;
    }

    public virtual string Write() => this.Name;

    public virtual void Read(string data)
    {
      if (!(data != ""))
        return;
      this.baseGuid = StringUtility.ToUInt64(data);
      long baseGuid = (long) this.baseGuid;
      this.Load();
    }

    public virtual IObject StaticInstance => this.staticInstance;

    public virtual bool IsDynamic => false;

    protected virtual void Load()
    {
      if (this.baseGuid == 0UL)
        return;
      IObject objectByGuid = IStaticDataContainer.StaticDataContainer.GetObjectByGuid(this.baseGuid);
      if (objectByGuid == null || !this.NeedInstanceType.IsAssignableFrom(objectByGuid.GetType()))
        return;
      this.staticInstance = objectByGuid;
    }

    protected abstract System.Type NeedInstanceType { get; }

    protected void LoadStaticInstance(IObject instance)
    {
      this.staticInstance = instance;
      if (this.staticInstance == null)
        return;
      this.baseGuid = this.staticInstance.BaseGuid;
    }
  }
}
