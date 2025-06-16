using System;
using Cofe.Serializations.Converters;
using Engine.Common.Types;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.GameLogic
{
  [VMType("IBlueprintRef")]
  [VMFactory(typeof (IBlueprintRef))]
  public class VMBlueprintRef : 
    BaseRef,
    IBlueprintRef,
    IRef,
    IVariable,
    INamed,
    IVMStringSerializable,
    IEngineTemplated
  {
    private Guid engineTemplateGuid = Guid.Empty;

    public void Initialize(Guid dynamicGuid)
    {
      engineTemplateGuid = dynamicGuid;
      Load();
    }

    public void Initialize(IBlueprint obj) => LoadStaticInstance(obj);

    public override EContextVariableCategory Category
    {
      get => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_BLUEPRINT;
    }

    public Guid EngineTemplateGuid => engineTemplateGuid;

    public IBlueprint Blueprint
    {
      get
      {
        if (StaticInstance == null && (BaseGuid != 0UL || engineTemplateGuid != Guid.Empty))
          Load();
        return (IBlueprint) StaticInstance;
      }
    }

    public override bool IsEqual(IVariable other)
    {
      if (other == null)
        return false;
      if (!typeof (IBlueprintRef).IsAssignableFrom(other.GetType()))
        return base.IsEqual(other);
      return BaseGuid != 0UL ? base.IsEqual(other) : engineTemplateGuid == ((VMBlueprintRef) other).engineTemplateGuid;
    }

    public override VMType Type => new VMType(typeof (IBlueprintRef));

    public override bool Empty => Blueprint == null && base.Empty;

    public override string Write()
    {
      return engineTemplateGuid != Guid.Empty ? GuidUtility.GetGuidString(engineTemplateGuid) : base.Write();
    }

    public override void Read(string data)
    {
      if (DefaultConverter.TryParseGuid(data, out engineTemplateGuid))
        Load();
      else
        base.Read(data);
    }

    protected override void Load()
    {
      base.Load();
      if (StaticInstance != null || !(engineTemplateGuid != Guid.Empty))
        return;
      LoadStaticInstance(IStaticDataContainer.StaticDataContainer.GetObjectByGuid(IStaticDataContainer.StaticDataContainer.GameRoot.GetBaseGuidByEngineTemplateGuid(engineTemplateGuid)));
    }

    protected override Type NeedInstanceType => typeof (IBlueprint);
  }
}
