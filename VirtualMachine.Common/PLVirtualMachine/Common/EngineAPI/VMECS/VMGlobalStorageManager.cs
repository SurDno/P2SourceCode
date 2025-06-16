using System.Collections.Generic;
using Cofe.Loggers;
using Engine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Common.VMSpecialAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS;

[Info("GlobalStorageManager")]
public class VMGlobalStorageManager : VMComponent {
	public const string ComponentName = "GlobalStorageManager";
	protected Dictionary<string, ITextRef> templateRTFieldValuesDict = new();
	protected bool templateRTDataUpdated;

	public override void Initialize(VMBaseEntity parent) {
		base.Initialize(parent);
	}

	[Method("Clear storages", "guids,tags,types", "")]
	[SpecialFunction(ESpecialFunctionName.SFN_FREE_STORAGES)]
	public virtual void FreeStorages(
		string storagesRootInfo,
		string storageTags,
		string storageTypes) { }

	[Method("Clear storages linear", "guids,tags,types", "")]
	[SpecialFunction(ESpecialFunctionName.SFN_FREE_STORAGES)]
	public virtual void FreeStoragesAssync(
		string storagesRootInfo,
		string storageTags,
		string storageTypes) { }

	[Method("", "guids", "")]
	[SpecialFunction(ESpecialFunctionName.SFN_RESET_TAGS)]
	public virtual void ResetTags(string storagesRootInfo) { }

	[Method("", ",,", "")]
	[SpecialFunction(ESpecialFunctionName.SFN_SET_TAGS_DISTRIBUTION)]
	public virtual void SetTagsDistribution(
		string storagesRootInfo,
		string storagTagDistributionInfo,
		string storageTypes) { }

	[Method("", ",,,,", "")]
	[SpecialFunction(ESpecialFunctionName.SFN_SET_CONTAINER_TAGS_DISTRIBUTION)]
	public virtual void SetContainerTagsDistribution(
		string storagesRootInfo,
		string storageTags,
		string containerTypes,
		string storagTagDistributionInfo,
		string storageTypes) { }

	[Method("", ",,,,", "")]
	[SpecialFunction(ESpecialFunctionName.SFN_SET_CONTAINER_TAGS_COMPLEX_DISTRIBUTION)]
	public virtual void SetContainerTagsComplexDistribution(
		string storagesRootInfo,
		string storageTags,
		string containerTypes,
		string storagTagDistributionInfo,
		string storageTypes) { }

	[Method("", ",,,,,", "")]
	[SpecialFunction(ESpecialFunctionName.SFN_ADD_ITEMS_TO_STORAGES)]
	public virtual void AddItemsToStorages(
		string storagesRootInfo,
		string storageTags,
		string sMinValue,
		string sMaxValue,
		string itemTemplateGuid,
		string storageTypes) { }

	[Method("", ",,,,,", "")]
	[SpecialFunction(ESpecialFunctionName.SFN_ADD_ITEMS_TO_STORAGES_LINEAR)]
	public virtual void AddItemsToStoragesLinear(
		string storagesRootInfo,
		string storageTags,
		string containerTypes,
		string containerTags,
		string itemTemplateGuid,
		string storageTypes) { }

	[Method("", ",,,,,", "")]
	[SpecialFunction(ESpecialFunctionName.SFN_SET_STORAGE_CONTAINER_STATUSES)]
	public virtual void SetStorageContainerStatuses(
		string storagesRootInfo,
		string storageTags,
		string containerTypes,
		string containerTags,
		string containerStatusesData,
		string storageTypes) { }

	[Method("", ",,,,,", "")]
	[SpecialFunction(ESpecialFunctionName.SFN_SET_STORAGE_CONTAINER_PARAMS)]
	public virtual void SetStorageContainerParams(
		string storagesRootInfo,
		string storageTags,
		string containerTypes,
		string containerTags,
		string containerParamsData,
		string storageTypes) { }

	[Method("Set storables title", "Item template:Storable, text", "")]
	public virtual void SetAllStorablesTitle([Template] IEntity template, ITextRef text) { }

	[Method("Set storables description", "Item template:Storable, text", "")]
	public virtual void SetAllStorablesDescription([Template] IEntity template, ITextRef text) { }

	[Method("Set storables description", "Item template:Storable, text", "")]
	public virtual void SetAllStorablesSpecialDescription([Template] IEntity template, ITextRef text) { }

	[Method("Set storables tooltip", "Item template:Storable, text", "")]
	public virtual void SetAllStorablesTooltip([Template] IEntity template, ITextRef text) { }

	[Method("Is storable exist in combination", "Combination template:Storable, item template:Storable", "")]
	public virtual bool IsStorableExistInCombibation(
		IBlueprintRef combination,
		IBlueprintRef storable) {
		return false;
	}

	[Method("Set storables template title", "Item template:Storable, text", "")]
	public virtual void SetStorablesTemplateTitle(IBlueprintRef templateRef, ITextRef text) {
		DoSetTemplateFieldValue(templateRef,
			EngineAPIManager.GetSpecialPropertyName(ESpecialPropertyName.SPN_STORABLE_TITLE, typeof(VMStorable)), text);
	}

	[Method("Set storables template description", "Item template:Storable, text", "")]
	public virtual void SetStorablesTemplateDescription(IBlueprintRef templateRef, ITextRef text) {
		DoSetTemplateFieldValue(templateRef,
			EngineAPIManager.GetSpecialPropertyName(ESpecialPropertyName.SPN_STORABLE_DESCRIPTION, typeof(VMStorable)),
			text);
	}

	[Method("Set storables template special description", "Item template:Storable, text", "")]
	public virtual void SetStorablesTemplateSpecialDescription(
		IBlueprintRef templateRef,
		ITextRef text) {
		DoSetTemplateFieldValue(templateRef,
			EngineAPIManager.GetSpecialPropertyName(ESpecialPropertyName.SPN_STORABLE_SPECIALDESCRIPTION,
				typeof(VMStorable)), text);
	}

	[Method("Set storables template tooltip", "Item template:Storable, text", "")]
	public virtual void SetStorablesTemplateTooltip(IBlueprintRef templateRef, ITextRef text) {
		DoSetTemplateFieldValue(templateRef,
			EngineAPIManager.GetSpecialPropertyName(ESpecialPropertyName.SPN_STORABLE_TOOLTIP, typeof(VMStorable)),
			text);
	}

	public bool TemplateRTDataUpdated => templateRTDataUpdated;

	public ITextRef GetTemplateRTFieldValue(string fieldKey) {
		ITextRef templateRtFieldValue = null;
		templateRTFieldValuesDict.TryGetValue(fieldKey, out templateRtFieldValue);
		return templateRtFieldValue;
	}

	private void DoSetTemplateFieldValue(
		IBlueprintRef templateRef,
		string fieldName,
		ITextRef value) {
		if (templateRef == null)
			Logger.AddError("template for field value setting not defined !");
		if (value == null)
			Logger.AddError("value for template field setting not defined !");
		var blueprint = templateRef.Blueprint;
		if (templateRef == null)
			Logger.AddError("template for field value setting not found !");
		templateRTFieldValuesDict[blueprint.BaseGuid + fieldName] = value;
		OnModify();
		templateRTDataUpdated = true;
	}

	public VMGameComponent GlobalRootManager => (VMGameComponent)Parent.GetComponentByName("GameComponent");

	public static VMGlobalStorageManager Instance { get; protected set; }
}