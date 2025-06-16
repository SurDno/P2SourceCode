// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.IVariableService
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using PLVirtualMachine.Common.Data;

#nullable disable
namespace PLVirtualMachine.Common
{
  public abstract class IVariableService
  {
    public static void Initialize(IVariableService service) => IVariableService.Instance = service;

    public string GetContextData(IContext context)
    {
      if (context == null)
        return "";
      if (typeof (IGameObjectContext).IsAssignableFrom(context.GetType()))
      {
        IGameObjectContext gameObjectContext = (IGameObjectContext) context;
        if (typeof (IHierarchyObject).IsAssignableFrom(gameObjectContext.GetType()))
        {
          HierarchyGuid hierarchyGuid = ((IHierarchyObject) gameObjectContext).HierarchyGuid;
          if (hierarchyGuid.IsHierarchy)
          {
            hierarchyGuid = ((IHierarchyObject) gameObjectContext).HierarchyGuid;
            return hierarchyGuid.Write();
          }
        }
        return gameObjectContext.BaseGuid.ToString();
      }
      if (typeof (IVariable).IsAssignableFrom(context.GetType()))
        return this.GetVariableData((IVariable) context);
      Logger.AddError(string.Format("Invalid context: {0}", (object) this.GetContextString(context)));
      return "";
    }

    public string GetVariableData(IVariable variable)
    {
      if (variable == null)
        return "";
      if (typeof (IVMStringSerializable).IsAssignableFrom(variable.GetType()))
        return ((IVMStringSerializable) variable).Write();
      return typeof (IObject).IsAssignableFrom(variable.GetType()) ? ((IEditorBaseTemplate) variable).BaseGuid.ToString() : variable.Name;
    }

    public abstract IContext GetContextByData(
      IGameObjectContext globalContext,
      string data,
      ILocalContext localContext = null,
      IContextElement contextElement = null);

    public IVariable GetContextVariableByData(
      IContext contextObj,
      string data,
      ILocalContext localContext,
      IContextElement contextElement)
    {
      if (contextObj == null)
        return (IVariable) null;
      IVariable contextVariableByData = contextObj.GetContextVariable(data);
      if (contextVariableByData == null && localContext != null && typeof (IObject).IsAssignableFrom(contextObj.GetType()) && localContext.Owner != null && localContext.Owner.IsEqual((IObject) contextObj) && CommonVariableUtility.IsLocalVariableData(data))
        contextVariableByData = localContext.GetLocalContextVariable(data, contextElement);
      return contextVariableByData;
    }

    public string GetContextString(IContext context)
    {
      if (typeof (IObject).IsAssignableFrom(context.GetType()))
        return ((IObject) context).GuidStr;
      return typeof (IVariable).IsAssignableFrom(context.GetType()) ? context.Name : "invalid context";
    }

    public static IVariableService Instance { get; set; }
  }
}
