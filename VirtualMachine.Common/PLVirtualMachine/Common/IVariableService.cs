using Cofe.Loggers;
using PLVirtualMachine.Common.Data;

namespace PLVirtualMachine.Common;

public abstract class IVariableService {
	public static void Initialize(IVariableService service) {
		Instance = service;
	}

	public string GetContextData(IContext context) {
		if (context == null)
			return "";
		if (typeof(IGameObjectContext).IsAssignableFrom(context.GetType())) {
			var gameObjectContext = (IGameObjectContext)context;
			if (typeof(IHierarchyObject).IsAssignableFrom(gameObjectContext.GetType())) {
				var hierarchyGuid = ((IHierarchyObject)gameObjectContext).HierarchyGuid;
				if (hierarchyGuid.IsHierarchy) {
					hierarchyGuid = ((IHierarchyObject)gameObjectContext).HierarchyGuid;
					return hierarchyGuid.Write();
				}
			}

			return gameObjectContext.BaseGuid.ToString();
		}

		if (typeof(IVariable).IsAssignableFrom(context.GetType()))
			return GetVariableData((IVariable)context);
		Logger.AddError(string.Format("Invalid context: {0}", GetContextString(context)));
		return "";
	}

	public string GetVariableData(IVariable variable) {
		if (variable == null)
			return "";
		if (typeof(IVMStringSerializable).IsAssignableFrom(variable.GetType()))
			return ((IVMStringSerializable)variable).Write();
		return typeof(IObject).IsAssignableFrom(variable.GetType())
			? ((IEditorBaseTemplate)variable).BaseGuid.ToString()
			: variable.Name;
	}

	public abstract IContext GetContextByData(
		IGameObjectContext globalContext,
		string data,
		ILocalContext localContext = null,
		IContextElement contextElement = null);

	public IVariable GetContextVariableByData(
		IContext contextObj,
		string data,
		ILocalContext localContext,
		IContextElement contextElement) {
		if (contextObj == null)
			return null;
		var contextVariableByData = contextObj.GetContextVariable(data);
		if (contextVariableByData == null && localContext != null &&
		    typeof(IObject).IsAssignableFrom(contextObj.GetType()) && localContext.Owner != null &&
		    localContext.Owner.IsEqual((IObject)contextObj) && CommonVariableUtility.IsLocalVariableData(data))
			contextVariableByData = localContext.GetLocalContextVariable(data, contextElement);
		return contextVariableByData;
	}

	public string GetContextString(IContext context) {
		if (typeof(IObject).IsAssignableFrom(context.GetType()))
			return ((IObject)context).GuidStr;
		return typeof(IVariable).IsAssignableFrom(context.GetType()) ? context.Name : "invalid context";
	}

	public static IVariableService Instance { get; set; }
}