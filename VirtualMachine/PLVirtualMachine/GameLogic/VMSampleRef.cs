using System;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

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
      Load();
    }

    public void Initialize(ISample obj) => LoadStaticInstance(obj);

    public override EContextVariableCategory Category => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_SAMPLE;

    public Guid EngineTemplateGuid
    {
      get
      {
        if (Guid.Empty == engineSampleGuid)
          Load();
        return engineSampleGuid;
      }
    }

    public ISample Sample
    {
      get
      {
        if (StaticInstance == null && (BaseGuid != 0UL || engineSampleGuid != Guid.Empty))
          Load();
        return (ISample) StaticInstance;
      }
    }

    public override bool IsEqual(IVariable other)
    {
      if (other == null)
        return false;
      if (!typeof (ISampleRef).IsAssignableFrom(other.GetType()))
        return base.IsEqual(other);
      return BaseGuid != 0UL ? base.IsEqual(other) : engineSampleGuid == ((VMSampleRef) other).engineSampleGuid;
    }

    public override VMType Type => Sample != null ? new VMType(typeof (ISampleRef), Sample.SampleType) : new VMType(typeof (ISampleRef));

    public override bool Empty => StaticInstance == null && base.Empty;

    protected override void Load()
    {
      base.Load();
      if (StaticInstance == null && engineSampleGuid != Guid.Empty)
        LoadStaticInstance(IStaticDataContainer.StaticDataContainer.GetObjectByGuid(IStaticDataContainer.StaticDataContainer.GameRoot.GetBaseGuidByEngineTemplateGuid(engineSampleGuid)));
      if (StaticInstance == null || !(Guid.Empty == engineSampleGuid))
        return;
      engineSampleGuid = ((IEngineTemplated) StaticInstance).EngineTemplateGuid;
    }

    protected override Type NeedInstanceType => typeof (ISample);
  }
}
