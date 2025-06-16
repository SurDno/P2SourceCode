using System;

namespace Inspectors;

public static class InspectedDrawerUtility {
	public static Foldout BeginComplex(
		string name,
		string displayName,
		IExpandedProvider context,
		IInspectedDrawer drawer,
		Action contextMenu = null) {
		var foldout = new Foldout {
			ExpandInternal = context.GetExpanded(context.DeepName + name)
		};
		foldout.Expand = drawer.Foldout(displayName, foldout.ExpandInternal, contextMenu);
		if (foldout.Expand) {
			foldout.IndentLevel = drawer.IndentLevel;
			++drawer.IndentLevel;
			foldout.Name = context.DeepName;
			context.DeepName += name;
		}

		return foldout;
	}

	public static void EndComplex(
		Foldout fold,
		string name,
		IExpandedProvider context,
		IInspectedDrawer drawer) {
		if (fold.Expand) {
			drawer.IndentLevel = fold.IndentLevel;
			context.DeepName = fold.Name;
		}

		if (fold.ExpandInternal && !fold.Expand)
			context.SetExpanded(context.DeepName + name, false);
		else {
			if (fold.ExpandInternal || !fold.Expand)
				return;
			context.SetExpanded(context.DeepName + name, true);
		}
	}

	public struct Foldout {
		public bool ExpandInternal;
		public bool Expand;
		public int IndentLevel;
		public string Name;
	}
}