// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.IBlueprint
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System.Collections.Generic;

#nullable disable
namespace PLVirtualMachine.Common
{
  public interface IBlueprint : 
    IGameObjectContext,
    IContainer,
    IObject,
    IEditorBaseTemplate,
    INamedElement,
    INamed,
    IStaticUpdateable,
    IContext,
    ILogicObject
  {
    List<IBlueprint> BaseBlueprints { get; }

    List<BaseFunction> Functions { get; }

    IFiniteStateMachine StateGraph { get; }

    Dictionary<string, IFunctionalComponent> FunctionalComponents { get; }

    bool TryGetProperty(string name, out IParam param);

    IParam GetProperty(string componentName, string propertyName);

    bool IsDerivedFrom(ulong blueprintGuid, bool withSelf = false);

    new IVariable GetSelf();
  }
}
