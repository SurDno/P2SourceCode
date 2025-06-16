using Cofe.Serializations.Converters;
using Engine.Common.Types;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using System;

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
      this.engineTemplateGuid = dynamicGuid;
      this.Load();
    }

    public void Initialize(IBlueprint obj) => this.LoadStaticInstance((IObject) obj);

    public override EContextVariableCategory Category
    {
      get => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_BLUEPRINT;
    }

    public Guid EngineTemplateGuid => this.engineTemplateGuid;

    public IBlueprint Blueprint
    {
      get
      {
        if (this.StaticInstance == null && (this.BaseGuid != 0UL || this.engineTemplateGuid != Guid.Empty))
          this.Load();
        return (IBlueprint) this.StaticInstance;
      }
    }

    public override bool IsEqual(IVariable other)
    {
      if (other == null)
        return false;
      if (!typeof (IBlueprintRef).IsAssignableFrom(other.GetType()))
        return base.IsEqual(other);
      return this.BaseGuid != 0UL ? base.IsEqual(other) : this.engineTemplateGuid == ((VMBlueprintRef) other).engineTemplateGuid;
    }

    public override VMType Type => new VMType(typeof (IBlueprintRef));

    public override bool Empty => this.Blueprint == null && base.Empty;

    public override string Write()
    {
      return this.engineTemplateGuid != Guid.Empty ? GuidUtility.GetGuidString(this.engineTemplateGuid) : base.Write();
    }

    public override void Read(string data)
    {
      if (DefaultConverter.TryParseGuid(data, out this.engineTemplateGuid))
        this.Load();
      else
        base.Read(data);
    }

    protected override void Load()
    {
      base.Load();
      if (this.StaticInstance != null || !(this.engineTemplateGuid != Guid.Empty))
        return;
      this.LoadStaticInstance(IStaticDataContainer.StaticDataContainer.GetObjectByGuid(IStaticDataContainer.StaticDataContainer.GameRoot.GetBaseGuidByEngineTemplateGuid(this.engineTemplateGuid)));
    }

    protected override System.Type NeedInstanceType => typeof (IBlueprint);
  }
}
