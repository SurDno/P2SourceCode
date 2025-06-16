using System;
using System.Reflection;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes;

[Description("Subscribes to a C# System.Action<T> Event and is called when the event is raised")]
[Category("Events/Script")]
public class CodeEvent<T> : EventNode<Transform> {
	[SerializeField] private string eventName;
	[SerializeField] private Type targetType;
	private FlowOutput o;
	private Action<T> pointer;
	private T eventValue;

	public void SetEvent(EventInfo e) {
		targetType = e.RTReflectedType();
		eventName = e.Name;
		GatherPorts();
	}

	public override void OnGraphStarted() {
		if (string.IsNullOrEmpty(eventName))
			Debug.LogError("No Event Selected for CodeEvent, or target is NULL");
		else {
			var eventInfo = targetType.RTGetEvent(eventName);
			if (eventInfo == null)
				Debug.LogError(string.Format("Event {0} is not found", eventName));
			else {
				base.OnGraphStarted();
				var component = target.value.GetComponent(targetType);
				if (component == null)
					Debug.LogError("Target is null");
				else {
					pointer = v => {
						eventValue = v;
						o.Call();
					};
					eventInfo.AddEventHandler(component, pointer);
				}
			}
		}
	}

	public override void OnGraphStoped() {
		if (string.IsNullOrEmpty(eventName) || target.value == null)
			return;
		targetType.RTGetEvent(eventName).RemoveEventHandler(target.value.GetComponent(targetType), pointer);
	}

	protected override void RegisterPorts() {
		if (string.IsNullOrEmpty(eventName))
			return;
		o = AddFlowOutput(eventName);
		AddValueOutput("Value", () => eventValue);
	}
}