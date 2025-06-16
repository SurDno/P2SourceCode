using System;
using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Services;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS;

[Info("Location", typeof(ILocationComponent))]
public class VMLocation : VMEngineComponent<ILocationComponent> {
	public const string ComponentName = "Location";

	[Event("Hibernation changed", "")] public event Action OnHibernationChange;

	[Event("Player inside changed", "is inside")]
	public event Action<bool> OnPlayerInside;

	[Property("Is hibernation", "", false)]
	public bool IsHibernation {
		get {
			if (Component != null)
				return Component.IsHibernation;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return false;
		}
	}

	[Property("Player in Location", "", false)]
	public IEntity Player {
		get {
			if (Component != null)
				return Component.Player;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return null;
		}
	}

	public override void Clear() {
		if (!InstanceValid)
			return;
		Component.OnHibernationChanged -= HibernationChanged;
		Component.OnPlayerChanged -= PlayerChanged;
		base.Clear();
	}

	protected override void Init() {
		if (IsTemplate)
			return;
		Component.OnHibernationChanged += HibernationChanged;
		Component.OnPlayerChanged += PlayerChanged;
	}

	private void HibernationChanged(ILocationComponent sender) {
		var hibernationChange = OnHibernationChange;
		if (hibernationChange == null)
			return;
		hibernationChange();
	}

	private void PlayerChanged() {
		if (OnPlayerInside == null)
			return;
		if (Component.IsDisposed)
			Logger.AddError("Location is disposed");
		else {
			var hierarchyPath = Component.Owner.HierarchyPath;
			var logicLocation = Component.LogicLocation;
			if (logicLocation == null) {
				if (Component.Owner.Parent == ServiceCache.Simulation.Hierarchy)
					return;
				Logger.AddError("Logic location not found in location : " + hierarchyPath);
			} else {
				var str = hierarchyPath + (logicLocation.IsIndoor ? " (indoor)" : " (outdoor)");
				var player = Component.Player;
				if (player == null) {
					Logger.AddInfo("Player exit from location : " + str);
					var onPlayerInside = OnPlayerInside;
					if (onPlayerInside == null)
						return;
					onPlayerInside(false);
				} else {
					var component = player.GetComponent<INavigationComponent>();
					if (component == null)
						Logger.AddError("Navigation not found in player : " + player.HierarchyPath +
						                " , in location : " + str);
					else if (!component.HasPrevTeleport)
						Logger.AddInfo("Prev teleport not found, player : " + player.HierarchyPath +
						               " , in location : " + str);
					else {
						Logger.AddInfo("Player entry to location : " + str);
						var onPlayerInside = OnPlayerInside;
						if (onPlayerInside == null)
							return;
						onPlayerInside(true);
					}
				}
			}
		}
	}
}