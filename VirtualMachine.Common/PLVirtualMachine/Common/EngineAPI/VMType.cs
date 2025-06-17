using System;
using System.Collections.Generic;
using System.Linq;
using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Reflection;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI
{
  [VMType("VMType")]
  public class VMType : IVMStringSerializable
  {
    public static readonly VMType Empty = new(typeof (Nullable));
    private Type baseType;
    private string specialType = "";
    private bool isEngineStorageMode;
    private int listDepth;
    private List<string> functionalPartsList;
    private VMType listElementType;
    private ulong specialTypeBlueprintId;

    public VMType()
    {
      baseType = typeof (object);
      specialType = "";
      isEngineStorageMode = false;
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
      specialType = "";
      isEngineStorageMode = false;
      ReadSpecialPart();
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
        Logger.AddError("Invalid VMType constructor call !");
        this.specialType = specialType;
      }
      isEngineStorageMode = false;
      ReadSpecialPart();
    }

    public VMType(Type baseType, string specialType, bool engineStorageMode)
    {
      this.baseType = baseType;
      this.specialType = specialType;
      isEngineStorageMode = engineStorageMode;
      ReadSpecialPart();
    }

    public static VMType CreateListTypeByElementType(VMType elemType)
    {
      VMType typeByElementType = new VMType {
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
      if (str.Contains('.'))
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
        Logger.AddError(string.Format("Cannot specialize type by static object {0} at {1}", blueprint.Name, EngineAPIManager.Instance.CurrentFSMStateInfo));
        specialType = "";
      }
      else
      {
        specialTypeBlueprintId = blueprint.BaseGuid;
        if (specialTypeBlueprintId == 0UL)
          return;
        specialType = "cf_" + specialTypeBlueprintId + "&" + MakeFunctionalPart(blueprint.GetComponentNames());
        ReadFunctionalPart(specialType.Substring("cf_".Length));
      }
    }

    public Type BaseType => baseType;

    public string SpecialType => specialType;

    public bool IsList => listDepth > 0;

    public VMType GetListElementType()
    {
      return listElementType == null ? new VMType(typeof (object)) : listElementType;
    }

    public bool IsFunctional => IsList ? listElementType != null && listElementType.IsFunctional : typeof (IObjRef) == BaseType || typeof (IBlueprintRef) == BaseType;

    public bool IsFunctionalSpecial => IsList ? listElementType != null && listElementType.IsFunctionalSpecial : functionalPartsList != null && functionalPartsList.Count > 0;

    public bool IsComplexSpecial => IsSpecial && specialTypeBlueprintId > 0UL;

    private void MakeSpecial(List<string> functionalsList)
    {
      if (IsList)
      {
        if (listElementType != null)
        {
          listElementType.MakeSpecial(functionalsList);
        }
        else
        {
          listElementType = new VMType(typeof (IObjRef));
          listElementType.MakeSpecial(functionalsList);
        }
        listDepth = 1 + listElementType.listDepth;
        specialType = listElementType.Write();
      }
      else if ((functionalsList != null ? (functionalsList.Count > 0 ? 1 : 0) : 0) != 0)
      {
        specialTypeBlueprintId = 0UL;
        specialType = "cf_" + MakeFunctionalPart(functionalsList);
        ReadFunctionalPart(specialType.Substring("cf_".Length));
      }
      else
        specialType = "";
    }

    private string MakeFunctionalPart(IEnumerable<string> functionalsList)
    {
      string str = "";
      int num = 0;
      foreach (string functionals in functionalsList)
      {
        if (!(null == EngineAPIManager.GetComponentTypeByName(functionals)))
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
      if (IsList) {
        if (listElementType == null)
        {
          yield break;
        }

        foreach (string functionalPart in listElementType.GetFunctionalParts(withDepended))
          yield return functionalPart;
      }
      if (withDepended && functionalPartsList != null)
      {
        for (int i = 0; i < functionalPartsList.Count; ++i)
        {
          yield return functionalPartsList[i];
          string dependedFunctional = EngineAPIManager.GetDependedFunctional(functionalPartsList[i]);
          if ("" != dependedFunctional)
            yield return dependedFunctional;
        }
      }
      bool isFunctionalParts = false;
      if (functionalPartsList != null)
        isFunctionalParts = functionalPartsList.Count > 0;
      if (SpecialTypeBlueprint != null && !isFunctionalParts)
      {
        foreach (string componentName in SpecialTypeBlueprint.GetComponentNames())
          yield return componentName;
      }
      if (isFunctionalParts)
      {
        foreach (string functionalParts in functionalPartsList)
          yield return functionalParts;
      }
    }

    public bool IsSimple => IsNumber || baseType == typeof (bool) || baseType == typeof (string) || baseType.IsEnum;

    public bool IsNumber => IsIntegerNumber || baseType == typeof (float) || baseType == typeof (double);

    public bool IsIntegerNumber => baseType == typeof (byte) || baseType == typeof (ushort) || baseType == typeof (short) || baseType == typeof (int) || baseType == typeof (uint) || baseType == typeof (long) || baseType == typeof (ulong);

    public bool EqualsTo(VMType otherType)
    {
      return BaseType == otherType.BaseType & SpecialType.Equals(otherType.SpecialType);
    }

    public string Name => baseType.ToString();

    public void Read(string data)
    {
      switch (data)
      {
        case null:
          Logger.AddError(string.Format("Attempt to read null vmtype data at {0}", EngineAPIManager.Instance.CurrentFSMStateInfo));
          break;
        case "":
          break;
        default:
          isEngineStorageMode = false;
          string[] strArray = data.Split('%');
          baseType = Type.GetType(strArray[0]);
          if (baseType == null)
            baseType = EngineAPIManager.GetObjectTypeByName(strArray[0]);
          specialType = "";
          if (strArray.Length > 1)
          {
            for (int index = 1; index < strArray.Length; ++index)
            {
              if (index > 1)
                specialType += "%";
              specialType += strArray[index];
            }
          }
          if (typeof (Engine.Common.IObject).IsAssignableFrom(baseType))
          {
            if (typeof (IEntity).IsAssignableFrom(baseType))
            {
              baseType = typeof (IObjRef);
            }
            else
            {
              baseType = typeof (ISampleRef);
              specialType = data;
              isEngineStorageMode = true;
            }
          }
          ReadSpecialPart();
          break;
      }
    }

    public string Write()
    {
      if (null == baseType)
      {
        Logger.AddError(string.Format("Type serialization error: base type is null at {0}", EngineAPIManager.Instance.CurrentFSMStateInfo));
        return "";
      }
      try
      {
        string str;
        if (!isEngineStorageMode)
        {
          string objectTypeNameByType = EngineAPIManager.GetObjectTypeNameByType(baseType);
          if ("" == objectTypeNameByType)
            objectTypeNameByType = baseType.ToString();
          if (typeof (ICommonList).IsAssignableFrom(baseType) && listElementType != null)
            specialType = listElementType.Write();
          str = objectTypeNameByType + "%" + specialType;
        }
        else
          str = specialType;
        return str;
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Type serialization error: {0} at {1}", ex, EngineAPIManager.Instance.CurrentFSMStateInfo));
        return "";
      }
    }

    public IBlueprint SpecialTypeBlueprint
    {
      get
      {
        if (specialTypeBlueprintId == 0UL)
          return null;
        IBlueprint specialTypeBlueprint = IStaticDataContainer.StaticDataContainer.GameRoot.GetBlueprintByGuid(specialTypeBlueprintId);
        if (specialTypeBlueprint == null)
        {
          IObject objectByGuid = IStaticDataContainer.StaticDataContainer.GetObjectByGuid(specialTypeBlueprintId);
          if (objectByGuid != null && typeof (IBlueprint).IsAssignableFrom(objectByGuid.GetType()))
            specialTypeBlueprint = (IBlueprint) objectByGuid;
        }
        return specialTypeBlueprint;
      }
    }

    public void Clear()
    {
      if (functionalPartsList != null)
      {
        functionalPartsList.Clear();
        functionalPartsList = null;
      }
      if (listElementType == null)
        return;
      listElementType.Clear();
      listElementType = null;
    }

    protected bool IsSpecial => specialType != null && specialType.Length > 0;

    private void ReadSpecialPart()
    {
      listDepth = 0;
      listElementType = null;
      if (specialType == null)
        return;
      if (typeof (ICommonList).IsAssignableFrom(BaseType))
      {
        if ("" != specialType)
        {
          listElementType = VMTypePool.GetType(specialType);
          listDepth = 1 + listElementType.listDepth;
        }
        else
          listDepth = 1;
      }
      else
      {
        if (!typeof (IObjRef).IsAssignableFrom(BaseType) && !typeof (IBlueprintRef).IsAssignableFrom(BaseType))
          return;
        if (specialType.StartsWith("cf_"))
        {
          ReadFunctionalPart(specialType.Substring("cf_".Length));
        }
        else
        {
          ReadFunctionalPart(specialType);
          if (!("" != specialType))
            return;
          specialType = "cf_" + specialType;
        }
      }
    }

    private void ReadFunctionalPart(string sFuncSpecialPart)
    {
      if (functionalPartsList != null)
        functionalPartsList.Clear();
      string[] strArray = sFuncSpecialPart.Split('&');
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (!InfoAttribute.TryGetValue(strArray[index], out ComponentReplectionInfo _))
        {
          ulong uint64 = StringUtility.ToUInt64(strArray[index]);
          if (uint64 != 0UL)
            specialTypeBlueprintId = uint64;
        }
        else
        {
          if (functionalPartsList == null)
            functionalPartsList = [];
          functionalPartsList.Add(strArray[index]);
        }
      }
    }
  }
}
