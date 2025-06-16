using System;
using System.Collections.Generic;
using Engine.Common.Comparers;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.Common;

public abstract class BaseRef : IRef, IVariable, INamed, IVMStringSerializable {
	private static HashSet<ulong> dublicate = new(UlongComparer.Instance);
	private ulong baseGuid;
	private IObject staticInstance;

	public virtual bool IsEqual(IVariable other) {
		return Name == other.Name;
	}

	public virtual string Name {
		get {
			if (staticInstance != null)
				return staticInstance.GuidStr;
			return baseGuid != 0UL ? baseGuid.ToString() : "";
		}
	}

	public virtual VMType Type => new(typeof(Nullable));

	public ulong BaseGuid {
		get => baseGuid;
		protected set => baseGuid = value;
	}

	public virtual bool Empty => baseGuid == 0UL;

	public virtual bool Exist => !Empty;

	public virtual EContextVariableCategory Category => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_OBJECT;

	public virtual string Write() {
		return Name;
	}

	public virtual void Read(string data) {
		if (!(data != ""))
			return;
		this.baseGuid = StringUtility.ToUInt64(data);
		var baseGuid = (long)this.baseGuid;
		Load();
	}

	public virtual IObject StaticInstance => staticInstance;

	public virtual bool IsDynamic => false;

	protected virtual void Load() {
		if (baseGuid == 0UL)
			return;
		var objectByGuid = IStaticDataContainer.StaticDataContainer.GetObjectByGuid(baseGuid);
		if (objectByGuid == null || !NeedInstanceType.IsAssignableFrom(objectByGuid.GetType()))
			return;
		staticInstance = objectByGuid;
	}

	protected abstract Type NeedInstanceType { get; }

	protected void LoadStaticInstance(IObject instance) {
		staticInstance = instance;
		if (staticInstance == null)
			return;
		baseGuid = staticInstance.BaseGuid;
	}
}