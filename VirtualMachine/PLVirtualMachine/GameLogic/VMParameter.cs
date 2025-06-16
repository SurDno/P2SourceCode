// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.GameLogic.VMParameter
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

#nullable disable
namespace PLVirtualMachine.GameLogic
{
  [TypeData(EDataType.TParameter)]
  [DataFactory("Parameter")]
  public class VMParameter : 
    IStub,
    IEditorDataReader,
    IObject,
    IEditorBaseTemplate,
    IParam,
    IVariable,
    INamed,
    IContext
  {
    private ulong guid;
    [FieldData("Name", DataFieldType.None)]
    private string name = "";
    [FieldData("OwnerComponent", DataFieldType.Reference)]
    private IFunctionalComponent ownerComponent;
    [FieldData("Type", DataFieldType.None)]
    private VMType valueType;
    [FieldData("Value", DataFieldType.None)]
    private object defValue;
    [FieldData("Implicit", DataFieldType.None)]
    private bool isImplicit;
    [FieldData("Custom", DataFieldType.None)]
    private bool isCustom;
    [FieldData("Parent", DataFieldType.Reference)]
    private IObject parent;
    private bool isAfterLoaded;
    private List<string> typeFunctionalList;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "Custom":
              this.isCustom = EditorDataReadUtility.ReadValue(xml, this.isCustom);
              continue;
            case "Implicit":
              this.isImplicit = EditorDataReadUtility.ReadValue(xml, this.isImplicit);
              continue;
            case "Name":
              this.name = EditorDataReadUtility.ReadValue(xml, this.name);
              continue;
            case "OwnerComponent":
              this.ownerComponent = EditorDataReadUtility.ReadReference<IFunctionalComponent>(xml, creator);
              continue;
            case "Parent":
              this.parent = EditorDataReadUtility.ReadReference<IObject>(xml, creator);
              continue;
            case "Type":
              this.valueType = EditorDataReadUtility.ReadTypeSerializable(xml);
              continue;
            case "Value":
              this.defValue = EditorDataReadUtility.ReadObjectValue(xml);
              continue;
            default:
              if (XMLDataLoader.Logs.Add(typeContext + " : " + xml.Name))
                Logger.AddError(typeContext + " : " + xml.Name);
              XmlReaderUtility.SkipNode(xml);
              continue;
          }
        }
        else if (xml.NodeType == XmlNodeType.EndElement)
          break;
      }
    }

    public VMParameter(ulong guid) => this.guid = guid;

    public ulong BaseGuid => this.guid;

    public virtual bool IsEqual(IVariable other)
    {
      return other != null && typeof (VMParameter) == other.GetType() && (long) this.BaseGuid == (long) ((VMParameter) other).BaseGuid;
    }

    public virtual bool IsEqual(IObject other)
    {
      return other != null && typeof (VMParameter) == other.GetType() && (long) this.BaseGuid == (long) ((VMParameter) other).BaseGuid;
    }

    public EContextVariableCategory Category
    {
      get => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM;
    }

    public bool IsCustom => this.isCustom;

    public string Name => this.name;

    public string ComponentName => this.ownerComponent == null ? "" : this.ownerComponent.Name;

    public VMType Type => this.valueType;

    public object Value
    {
      get
      {
        if (!this.isAfterLoaded)
          this.OnAfterLoad();
        return this.defValue;
      }
      set => Logger.AddError("!!! Такого быть не должно !!!");
    }

    public bool Implicit => this.isImplicit;

    public IObject Parent => this.parent;

    public void OnAfterLoad()
    {
      if (this.isAfterLoaded)
        return;
      if (this.defValue != null && this.defValue.GetType() == typeof (string))
      {
        if (this.valueType.BaseType != typeof (string))
        {
          try
          {
            this.defValue = PLVirtualMachine.Common.Data.StringSerializer.ReadValue((string) this.defValue, this.valueType.BaseType);
          }
          catch (Exception ex)
          {
            Logger.AddError(ex.ToString());
          }
        }
      }
      this.isAfterLoaded = true;
    }

    public bool IsDynamicObject => typeof (IObjRef).IsAssignableFrom(this.valueType.BaseType);

    public IEnumerable<string> GetComponentNames()
    {
      if (this.typeFunctionalList == null)
        this.LoadTypeFunctionals();
      return (IEnumerable<string>) this.typeFunctionalList;
    }

    public IEnumerable<IVariable> GetContextVariables(EContextVariableCategory contextVarCategory)
    {
      return this.TypedBlueprint != null ? this.TypedBlueprint.GetContextVariables(contextVarCategory) : this.LoadContextVariables(contextVarCategory);
    }

    public IVariable GetContextVariable(string variableName)
    {
      foreach (string componentName in this.GetComponentNames())
      {
        foreach (IVariable contextVariable in EngineAPIManager.GetAbstractVariablesByFunctionalName(componentName, EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_APIFUNCTION))
        {
          if (contextVariable.Name == variableName)
            return contextVariable;
        }
        foreach (IVariable contextVariable in EngineAPIManager.GetAbstractVariablesByFunctionalName(componentName, EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM))
        {
          if (contextVariable.Name == variableName)
            return contextVariable;
        }
      }
      return this.TypedBlueprint != null ? this.TypedBlueprint.GetContextVariable(variableName) : (IVariable) null;
    }

    public bool IsFunctionalSupport(string componentName)
    {
      return this.GetComponentNames().Contains<string>(componentName);
    }

    public bool IsFunctionalSupport(IEnumerable<string> functionals)
    {
      IEnumerable<string> componentNames = this.GetComponentNames();
      foreach (string functional in functionals)
      {
        if (!componentNames.Contains<string>(functional))
          return false;
      }
      return true;
    }

    public IBlueprint TypedBlueprint
    {
      get => this.Type.IsComplexSpecial ? this.Type.SpecialTypeBlueprint : (IBlueprint) null;
    }

    public IGameObjectContext OwnerContext
    {
      get
      {
        IObject parent = this.Parent;
        while (parent != null)
        {
          if (typeof (IGameObjectContext).IsAssignableFrom(parent.GetType()))
            return (IGameObjectContext) parent;
          if (typeof (INamedElement).IsAssignableFrom(parent.GetType()))
            parent = (IObject) ((INamedElement) parent).Parent;
        }
        return (IGameObjectContext) null;
      }
    }

    private IEnumerable<IVariable> LoadContextVariables(EContextVariableCategory contextVarCategory)
    {
      foreach (string componentName in this.GetComponentNames())
      {
        foreach (IVariable variable in EngineAPIManager.GetAbstractVariablesByFunctionalName(componentName, contextVarCategory))
          yield return variable;
      }
    }

    private void LoadTypeFunctionals()
    {
      this.typeFunctionalList = new List<string>();
      if (this.defValue != null && typeof (IObjRef).IsAssignableFrom(this.defValue.GetType()) && ((IObjRef) this.defValue).Object != null)
      {
        this.typeFunctionalList.AddRange(((IObjRef) this.defValue).Object.GetComponentNames());
      }
      else
      {
        if (this.Type == null || !this.Type.IsFunctionalSpecial)
          return;
        this.typeFunctionalList.AddRange(this.Type.GetFunctionalParts());
      }
    }

    public string GuidStr => this.guid.ToString();

    public void Clear()
    {
      this.ownerComponent = (IFunctionalComponent) null;
      this.valueType.Clear();
      this.defValue = (object) null;
      this.parent = (IObject) null;
      if (this.typeFunctionalList == null)
        return;
      this.typeFunctionalList.Clear();
      this.typeFunctionalList = (List<string>) null;
    }
  }
}
