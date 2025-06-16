using System;
using System.Reflection;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes;

[Description("Subscribes to a static C# System.Action Event and is called when the event is raised")]
[Category("Events/Script")]
public class StaticCodeEvent : EventNode {
	[SerializeField] private string eventName;
	[SerializeField] private Type targetType;
	private FlowOutput o;
	private Action pointer;

	public void SetEvent(EventInfo e) {
		targetType = e.RTReflectedType();
		eventName = e.Name;
		GatherPorts();
	}

	public override void OnGraphStarted() {
		if (string.IsNullOrEmpty(eventName))
			Debug.LogError("No Event Selected for 'Static Code Event'");
		else {
			var eventInfo = targetType.RTGetEvent(eventName);
			if (eventInfo == null)
				Debug.LogError(string.Format("Event {0} is not found", eventName));
			else {
				base.OnGraphStarted();
				pointer = (Action)(() => o.Call());
				eventInfo.AddEventHandler(null, pointer);
			}
		}
	}

	public override void OnGraphStoped() {
		if (string.IsNullOrEmpty(eventName))
			return;
		targetType.RTGetEvent(eventName).RemoveEventHandler(null, pointer);
	}

	protected override void RegisterPorts() {
		if (string.IsNullOrEmpty(eventName))
			return;
		o = AddFlowOutput(eventName);
	}
}