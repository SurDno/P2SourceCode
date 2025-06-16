using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Reflection;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PLVirtualMachine.Common.EngineAPI
{
  [VMType("VMType")]
  public class VMType : IVMStringSerializable
  {
    public static readonly VMType Empty = new VMType(typeof (Nullable));
    private Type baseType;
    private string specialType = "";
    private bool isEngineStorageMode;
    private int listDepth;
    private List<string> functionalPartsList;
    private VMType listElementType;
    private ulong specialTypeBlueprintId;

    public VMType()
    {
      this.baseType = typeof (object);
      this.specialType = "";
      this.isEngineStorageMode = false;
    }

    public VMType(Type baseType)
    {
      if (typeof (IObjRef).IsAssignableFrom(baseType))
        baseType = typeof (IObjRef);
      else if (typeof (IBlueprintRef).IsAssignableFrom(baseType))
        baseType = typeof (IBlueprintRef);
      else if (typeof (ISampleRef).IsAssignableFrom(baseType))
        baseType = typeof (ISampleRef);
      else if (typeof (ITextRef).IsAssignableFrom(baseType))
        baseType = typeof (ITextRef);
      else if (typeof (IStateRef).IsAssignableFrom(baseType))
        baseType = typeof (IStateRef);
      else if (typeof (ILogicMapNodeRef).IsAssignableFrom(baseType))
        baseType = typeof (ILogicMapNodeRef);
      this.baseType = baseType;
      this.specialType = "";
      this.isEngineStorageMode = false;
      this.ReadSpecialPart();
    }

    public VMType(Type baseType, string specialType)
    {
      this.baseType = baseType;
      if (typeof (IObjRef) == this.baseType || typeof (IBlueprintRef) == this.baseType)
        this.specialType = !("" != specialType) ? "" : "cf_" + specialType;
      else if (typeof (ISampleRef) == this.baseType || typeof (IStateRef) == this.baseType || typeof (ILogicMapNodeRef) == this.baseType)
      {
        this.specialType = specialType;
      }
      else
      {
        Logger.AddError(string.Format("Invalid VMType constructor call !"));
        this.specialType = specialType;
      }
      this.isEngineStorageMode = false;
      this.ReadSpecialPart();
    }

    public VMType(Type baseType, string specialType, bool engineStorageMode)
    {
      this.baseType = baseType;
      this.specialType = specialType;
      this.isEngineStorageMode = engineStorageMode;
      this.ReadSpecialPart();
    }

    public static VMType CreateListTypeByElementType(VMType elemType)
    {
      VMType typeByElementType = new VMType()
      {
        baseType = typeof (ICommonList),
        listElementType = elemType
      };
      typeByElementType.listDepth = 1 + typeByElementType.listElementType.listDepth;
      typeByElementType.specialType = elemType.Write();
      return typeByElementType;
    }

    public static VMType CreateBlueprintSpecialType(IBlueprint blueprint)
    {
      VMType blueprintSpecialType = new VMType(typeof (IObjRef));
      blueprintSpecialType.MakeSpecial(blueprint);
      return blueprintSpecialType;
    }

    public static VMType CreateStateSpecialType(IGraphObject state)
    {
      if (state == null)
        return new VMType(typeof (IStateRef));
      if (!typeof (IState).IsAssignableFrom(state.GetType()))
        return new VMType(typeof (ILogicMapNodeRef));
      string str = state.GetType().ToString();
      if (str.Contains<char>('.'))
      {
        string[] strArray = str.Split('.');
        str = strArray[strArray.Length - 1];
      }
      if (str.StartsWith("VM"))
        str = str.Substring("VM".Length);
      return new VMType(typeof (IStateRef), str);
    }

    private void MakeSpecial(IBlueprint blueprint)
    {
      if (blueprint == null)
        return;
      if (blueprint.Static)
      {
        Logger.AddError(string.Format("Cannot specialize type by static object {0} at {1}", (object) blueprint.Name, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
        this.specialType = "";
      }
      else
      {
        this.specialTypeBlueprintId = blueprint.BaseGuid;
        if (this.specialTypeBlueprintId == 0UL)
          return;
        this.specialType = "cf_" + this.specialTypeBlueprintId.ToString() + "&" + this.MakeFunctionalPart(blueprint.GetComponentNames());
        this.ReadFunctionalPart(this.specialType.Substring("cf_".Length));
      }
    }

    public Type BaseType => this.baseType;

    public string SpecialType => this.specialType;

    public bool IsList => this.listDepth > 0;

    public VMType GetListElementType()
    {
      return this.listElementType == null ? new VMType(typeof (object)) : this.listElementType;
    }

    public bool IsFunctional
    {
      get
      {
        return this.IsList ? this.listElementType != null && this.listElementType.IsFunctional : typeof (IObjRef) == this.BaseType || typeof (IBlueprintRef) == this.BaseType;
      }
    }

    public bool IsFunctionalSpecial
    {
      get
      {
        return this.IsList ? this.listElementType != null && this.listElementType.IsFunctionalSpecial : this.functionalPartsList != null && this.functionalPartsList.Count > 0;
      }
    }

    public bool IsComplexSpecial => this.IsSpecial && this.specialTypeBlueprintId > 0UL;

    private void MakeSpecial(List<string> functionalsList)
    {
      if (this.IsList)
      {
        if (this.listElementType != null)
        {
          this.listElementType.MakeSpecial(functionalsList);
        }
        else
        {
          this.listElementType = new VMType(typeof (IObjRef));
          this.listElementType.MakeSpecial(functionalsList);
        }
        this.listDepth = 1 + this.listElementType.listDepth;
        this.specialType = this.listElementType.Write();
      }
      else if ((functionalsList != null ? (functionalsList.Count > 0 ? 1 : 0) : 0) != 0)
      {
        this.specialTypeBlueprintId = 0UL;
        this.specialType = "cf_" + this.MakeFunctionalPart((IEnumerable<string>) functionalsList);
        this.ReadFunctionalPart(this.specialType.Substring("cf_".Length));
      }
      else
        this.specialType = "";
    }

    private string MakeFunctionalPart(IEnumerable<string> functionalsList)
    {
      string str = "";
      int num = 0;
      foreach (string functionals in functionalsList)
      {
        if (!((Type) null == EngineAPIManager.GetComponentTypeByName(functionals)))
        {
          if (num > 0)
            str += "&";
          str += functionals;
          ++num;
        }
      }
      return str;
    }

    public IEnumerable<string> GetFunctionalParts(bool withDepended = false)
    {
      if (this.IsList)
      {
        if (this.listElementType == null)
        {
          yield break;
        }
        else
        {
          foreach (string functionalPart in this.listElementType.GetFunctionalParts(withDepended))
            yield return functionalPart;
        }
      }
      if (withDepended && this.functionalPartsList != null)
      {
        for (int i = 0; i < this.functionalPartsList.Count; ++i)
        {
          yield return this.functionalPartsList[i];
          string dependedFunctional = EngineAPIManager.GetDependedFunctional(this.functionalPartsList[i]);
          if ("" != dependedFunctional)
            yield return dependedFunctional;
        }
      }
      bool isFunctionalParts = false;
      if (this.functionalPartsList != null)
        isFunctionalParts = this.functionalPartsList.Count > 0;
      if (this.SpecialTypeBlueprint != null && !isFunctionalParts)
      {
        foreach (string componentName in this.SpecialTypeBlueprint.GetComponentNames())
          yield return componentName;
      }
      if (isFunctionalParts)
      {
        foreach (string functionalParts in this.functionalPartsList)
          yield return functionalParts;
      }
    }

    public bool IsSimple
    {
      get
      {
        return this.IsNumber || this.baseType == typeof (bool) || this.baseType == typeof (string) || this.baseType.IsEnum;
      }
    }

    public bool IsNumber
    {
      get
      {
        return this.IsIntegerNumber || this.baseType == typeof (float) || this.baseType == typeof (double);
      }
    }

    public bool IsIntegerNumber
    {
      get
      {
        return this.baseType == typeof (byte) || this.baseType == typeof (ushort) || this.baseType == typeof (short) || this.baseType == typeof (int) || this.baseType == typeof (uint) || this.baseType == typeof (long) || this.baseType == typeof (ulong);
      }
    }

    public bool EqualsTo(VMType otherType)
    {
      return this.BaseType == otherType.BaseType & this.SpecialType.Equals(otherType.SpecialType);
    }

    public string Name => this.baseType.ToString();

    public void Read(string data)
    {
      switch (data)
      {
        case null:
          Logger.AddError(string.Format("Attempt to read null vmtype data at {0}", (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
          break;
        case "":
          break;
        default:
          this.isEngineStorageMode = false;
          string[] strArray = data.Split('%');
          this.baseType = Type.GetType(strArray[0]);
          if (this.baseType == (Type) null)
            this.baseType = EngineAPIManager.GetObjectTypeByName(strArray[0]);
          this.specialType = "";
          if (strArray.Length > 1)
          {
            for (int index = 1; index < strArray.Length; ++index)
            {
              if (index > 1)
                this.specialType += "%";
              this.specialType += strArray[index];
            }
          }
          if (typeof (Engine.Common.IObject).IsAssignableFrom(this.baseType))
          {
            if (typeof (IEntity).IsAssignableFrom(this.baseType))
            {
              this.baseType = typeof (IObjRef);
            }
            else
            {
              this.baseType = typeof (ISampleRef);
              this.specialType = data;
              this.isEngineStorageMode = true;
            }
          }
          this.ReadSpecialPart();
          break;
      }
    }

    public string Write()
    {
      if ((Type) null == this.baseType)
      {
        Logger.AddError(string.Format("Type serialization error: base type is null at {0}", (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
        return "";
      }
      try
      {
        string str;
        if (!this.isEngineStorageMode)
        {
          string objectTypeNameByType = EngineAPIManager.GetObjectTypeNameByType(this.baseType);
          if ("" == objectTypeNameByType)
            objectTypeNameByType = this.baseType.ToString();
          if (typeof (ICommonList).IsAssignableFrom(this.baseType) && this.listElementType != null)
            this.specialType = this.listElementType.Write();
          str = objectTypeNameByType + "%" + this.specialType;
        }
        else
          str = this.specialType;
        return str;
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Type serialization error: {0} at {1}", (object) ex.ToString(), (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
        return "";
      }
    }

    public IBlueprint SpecialTypeBlueprint
    {
      get
      {
        if (this.specialTypeBlueprintId == 0UL)
          return (IBlueprint) null;
        IBlueprint specialTypeBlueprint = IStaticDataContainer.StaticDataContainer.GameRoot.GetBlueprintByGuid(this.specialTypeBlueprintId);
        if (specialTypeBlueprint == null)
        {
          PLVirtualMachine.Common.IObject objectByGuid = IStaticDataContainer.StaticDataContainer.GetObjectByGuid(this.specialTypeBlueprintId);
          if (objectByGuid != null && typeof (IBlueprint).IsAssignableFrom(objectByGuid.GetType()))
            specialTypeBlueprint = (IBlueprint) objectByGuid;
        }
        return specialTypeBlueprint;
      }
    }

    public void Clear()
    {
      if (this.functionalPartsList != null)
      {
        this.functionalPartsList.Clear();
        this.functionalPartsList = (List<string>) null;
      }
      if (this.listElementType == null)
        return;
      this.listElementType.Clear();
      this.listElementType = (VMType) null;
    }

    protected bool IsSpecial => this.specialType != null && this.specialType.Length > 0;

    private void ReadSpecialPart()
    {
      this.listDepth = 0;
      this.listElementType = (VMType) null;
      if (this.specialType == null)
        return;
      if (typeof (ICommonList).IsAssignableFrom(this.BaseType))
      {
        if ("" != this.specialType)
        {
          this.listElementType = VMTypePool.GetType(this.specialType);
          this.listDepth = 1 + this.listElementType.listDepth;
        }
        else
          this.listDepth = 1;
      }
      else
      {
        if (!typeof (IObjRef).IsAssignableFrom(this.BaseType) && !typeof (IBlueprintRef).IsAssignableFrom(this.BaseType))
          return;
        if (this.specialType.StartsWith("cf_"))
        {
          this.ReadFunctionalPart(this.specialType.Substring("cf_".Length));
        }
        else
        {
          this.ReadFunctionalPart(this.specialType);
          if (!("" != this.specialType))
            return;
          this.specialType = "cf_" + this.specialType;
        }
      }
    }

    private void ReadFunctionalPart(string sFuncSpecialPart)
    {
      if (this.functionalPartsList != null)
        this.functionalPartsList.Clear();
      string[] strArray = sFuncSpecialPart.Split('&');
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (!InfoAttribute.TryGetValue(strArray[index], out ComponentReplectionInfo _))
        {
          ulong uint64 = StringUtility.ToUInt64(strArray[index]);
          if (uint64 != 0UL)
            this.specialTypeBlueprintId = uint64;
        }
        else
        {
          if (this.functionalPartsList == null)
            this.functionalPartsList = new List<string>();
          this.functionalPartsList.Add(strArray[index]);
        }
      }
    }
  }
}
