// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.AbstractParameter
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using PLVirtualMachine.Common.EngineAPI;

#nullable disable
namespace PLVirtualMachine.Common
{
  public class AbstractParameter : IVariable, INamed, IParam
  {
    protected string name;
    protected string componentName;
    protected VMType paramType;
    protected object defaultValue;

    public AbstractParameter(
      string name,
      string componentName,
      VMType type,
      object defValue,
      bool not_used = false)
    {
      this.name = name;
      this.componentName = componentName;
      this.paramType = type;
    }

    public EContextVariableCategory Category
    {
      get => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM;
    }

    public string Name
    {
      get => this.componentName != "" ? this.componentName + "." + this.name : this.name;
    }

    public object Value
    {
      get => this.defaultValue;
      set
      {
      }
    }

    public VMType Type => this.paramType;

    public bool Implicit => false;

    public IGameObjectContext OwnerContext => (IGameObjectContext) null;

    public virtual bool IsEqual(IVariable other)
    {
      if (!typeof (AbstractParameter).IsAssignableFrom(other.GetType()))
        return false;
      AbstractParameter abstractParameter = (AbstractParameter) other;
      return (!("" != this.componentName) || !(this.componentName != abstractParameter.componentName)) && this.Name == abstractParameter.Name;
    }

    public void Clear() => this.defaultValue = (object) null;
  }
}
