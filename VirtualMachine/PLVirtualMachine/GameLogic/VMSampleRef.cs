using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using System;

namespace PLVirtualMachine.GameLogic
{
  [VMType("ISampleRef")]
  [VMFactory(typeof (ISampleRef))]
  public class VMSampleRef : 
    BaseRef,
    ISampleRef,
    IRef,
    IVariable,
    INamed,
    IVMStringSerializable,
    IEngineTemplated
  {
    private Guid engineSampleGuid = Guid.Empty;

    public void Initialize(Guid engineSampleGuid)
    {
      this.engineSampleGuid = engineSampleGuid;
      this.Load();
    }

    public void Initialize(ISample obj) => this.LoadStaticInstance((IObject) obj);

    public override EContextVariableCategory Category
    {
      get => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_SAMPLE;
    }

    public Guid EngineTemplateGuid
    {
      get
      {
        if (Guid.Empty == this.engineSampleGuid)
          this.Load();
        return this.engineSampleGuid;
      }
    }

    public ISample Sample
    {
      get
      {
        if (this.StaticInstance == null && (this.BaseGuid != 0UL || this.engineSampleGuid != Guid.Empty))
          this.Load();
        return (ISample) this.StaticInstance;
      }
    }

    public override bool IsEqual(IVariable other)
    {
      if (other == null)
        return false;
      if (!typeof (ISampleRef).IsAssignableFrom(other.GetType()))
        return base.IsEqual(other);
      return this.BaseGuid != 0UL ? base.IsEqual(other) : this.engineSampleGuid == ((VMSampleRef) other).engineSampleGuid;
    }

    public override VMType Type
    {
      get
      {
        return this.Sample != null ? new VMType(typeof (ISampleRef), this.Sample.SampleType) : new VMType(typeof (ISampleRef));
      }
    }

    public override bool Empty => this.StaticInstance == null && base.Empty;

    protected override void Load()
    {
      base.Load();
      if (this.StaticInstance == null && this.engineSampleGuid != Guid.Empty)
        this.LoadStaticInstance(IStaticDataContainer.StaticDataContainer.GetObjectByGuid(IStaticDataContainer.StaticDataContainer.GameRoot.GetBaseGuidByEngineTemplateGuid(this.engineSampleGuid)));
      if (this.StaticInstance == null || !(Guid.Empty == this.engineSampleGuid))
        return;
      this.engineSampleGuid = ((IEngineTemplated) this.StaticInstance).EngineTemplateGuid;
    }

    protected override System.Type NeedInstanceType => typeof (ISample);
  }
}
