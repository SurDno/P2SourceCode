using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Runtime.Tasks;
using Cofe.Utility;
using UnityEngine;
using Action = System.Action;
using Object = UnityEngine.Object;

namespace BehaviorDesigner.Runtime;

public class BehaviorTree : MonoBehaviour, IBehaviorTree {
	[SerializeField] private bool startWhenEnabled = true;
	[SerializeField] private bool pauseWhenDisabled;
	[SerializeField] private bool restartWhenComplete;
	[SerializeField] private bool resetValuesOnRestart;
	[SerializeField] private ExternalBehaviorTree externalBehavior;
	private BehaviorSource behaviorSource;
	private bool hasInheritedVariables;
	private bool isPaused;
	private TaskStatus executionStatus = TaskStatus.Inactive;
	private bool initialized;
	private Dictionary<Task, Dictionary<string, object>> defaultValues;
	private Dictionary<string, object> defaultVariableValues;
	private Dictionary<string, List<TaskCoroutine>> activeTaskCoroutines;
	private Dictionary<Type, Dictionary<string, Delegate>> eventTable;

	public event BehaviorHandler OnBehaviorStart;

	public event BehaviorHandler OnBehaviorRestart;

	public event BehaviorHandler OnBehaviorEnd;

	public bool StartWhenEnabled {
		get => startWhenEnabled;
		set => startWhenEnabled = value;
	}

	public bool PauseWhenDisabled {
		get => pauseWhenDisabled;
		set => pauseWhenDisabled = value;
	}

	public bool RestartWhenComplete {
		get => restartWhenComplete;
		set => restartWhenComplete = value;
	}

	public bool ResetValuesOnRestart {
		get => resetValuesOnRestart;
		set => resetValuesOnRestart = value;
	}

	public ExternalBehaviorTree ExternalBehaviorTree {
		get => externalBehavior;
		set {
			MonoBehaviourInstance<BehaviorTreeManager>.Instance.DisableBehavior(this);
			behaviorSource.HasSerialized = false;
			initialized = false;
			externalBehavior = value;
			if (!startWhenEnabled)
				return;
			EnableBehavior();
		}
	}

	public bool HasInheritedVariables {
		get => hasInheritedVariables;
		set => hasInheritedVariables = value;
	}

	public BehaviorSource BehaviorSource {
		get => behaviorSource;
		set => behaviorSource = value;
	}

	public Object GetObject() {
		return this;
	}

	public string GetOwnerName() {
		return gameObject.name;
	}

	public TaskStatus ExecutionStatus {
		get => executionStatus;
		set => executionStatus = value;
	}

	public BehaviorTree() {
		behaviorSource = new BehaviorSource(this);
	}

	public void Start() {
		if (!startWhenEnabled)
			return;
		EnableBehavior();
	}

	public void EnableBehavior() {
		MonoBehaviourInstance<BehaviorTreeManager>.Instance.EnableBehavior(this);
		if (initialized)
			return;
		initialized = true;
	}

	public void DisableBehavior() {
		MonoBehaviourInstance<BehaviorTreeManager>.Instance.DisableBehavior(this, pauseWhenDisabled);
		isPaused = pauseWhenDisabled;
	}

	public void DisableBehavior(bool pause) {
		MonoBehaviourInstance<BehaviorTreeManager>.Instance.DisableBehavior(this, pause);
		isPaused = pause;
	}

	public void OnEnable() {
		if (isPaused) {
			MonoBehaviourInstance<BehaviorTreeManager>.Instance.EnableBehavior(this);
			isPaused = false;
		} else {
			if (!startWhenEnabled || !initialized)
				return;
			EnableBehavior();
		}
	}

	public void OnDisable() {
		DisableBehavior();
	}

	public void OnDestroy() {
		MonoBehaviourInstance<BehaviorTreeManager>.Instance.DestroyBehavior(this);
	}

	public SharedVariable GetVariable(string name) {
		CheckForSerialization();
		return behaviorSource.GetVariable(name);
	}

	public void SetVariable(string name, SharedVariable item) {
		CheckForSerialization();
		behaviorSource.SetVariable(name, item);
	}

	public void SetVariableValue(string name, object value) {
		var variable = GetVariable(name);
		if (variable != null) {
			if (value is SharedVariable) {
				var sharedVariable = value as SharedVariable;
				if (!TypeUtility.IsAssignableFrom(variable.GetType(), sharedVariable.GetType()))
					Debug.LogError("Set wrong type, go : " + gameObject);
				variable.SetValue(sharedVariable.GetValue());
			} else
				variable.SetValue(value);
		} else if (value is SharedVariable) {
			var sharedVariable = value as SharedVariable;
			var instance = TaskUtility.CreateInstance(sharedVariable.GetType()) as SharedVariable;
			instance.Name = sharedVariable.Name;
			instance.IsShared = sharedVariable.IsShared;
			instance.SetValue(sharedVariable.GetValue());
			behaviorSource.SetVariable(name, instance);
		} else
			Debug.LogError("Error: No variable exists with name " + name + " in: " + gameObject);
	}

	public List<SharedVariable> GetAllVariables() {
		CheckForSerialization();
		return behaviorSource.GetAllVariables();
	}

	public void CheckForSerialization() {
		if (externalBehavior != null) {
			List<SharedVariable> sharedVariableList = null;
			var force = false;
			if (!hasInheritedVariables) {
				behaviorSource.CheckForSerialization(false, null, GetOwnerName());
				sharedVariableList = behaviorSource.GetAllVariables();
				hasInheritedVariables = true;
				force = true;
			}

			externalBehavior.BehaviorSource.Owner = ExternalBehaviorTree;
			externalBehavior.BehaviorSource.CheckForSerialization(force, BehaviorSource, GetOwnerName());
			externalBehavior.BehaviorSource.EntryTask = behaviorSource.EntryTask;
			if (sharedVariableList == null)
				return;
			for (var index = 0; index < sharedVariableList.Count; ++index)
				if (sharedVariableList[index] != null)
					behaviorSource.SetVariable(sharedVariableList[index].Name, sharedVariableList[index]);
		} else
			behaviorSource.CheckForSerialization(false, null, GetOwnerName());
	}

	private T FindTask<T>(Task task) where T : Task {
		if (task.GetType().Equals(typeof(T)))
			return (T)task;
		if (task is ParentTask parentTask && parentTask.Children != null)
			for (var index = 0; index < parentTask.Children.Count; ++index) {
				var obj = default(T);
				T task1;
				if ((task1 = FindTask<T>(parentTask.Children[index])) != null)
					return task1;
			}

		return default;
	}

	private void FindTasks<T>(Task task, ref List<T> taskList) where T : Task {
		if (typeof(T).IsAssignableFrom(task.GetType()))
			taskList.Add((T)task);
		if (!(task is ParentTask parentTask) || parentTask.Children == null)
			return;
		for (var index = 0; index < parentTask.Children.Count; ++index)
			FindTasks(parentTask.Children[index], ref taskList);
	}

	private Task FindTaskWithName(string taskName, Task task) {
		if (task.FriendlyName.Equals(taskName))
			return task;
		if (task is ParentTask parentTask && parentTask.Children != null)
			for (var index = 0; index < parentTask.Children.Count; ++index) {
				Task taskWithName;
				if ((taskWithName = FindTaskWithName(taskName, parentTask.Children[index])) != null)
					return taskWithName;
			}

		return null;
	}

	private void FindTasksWithName(string taskName, Task task, ref List<Task> taskList) {
		if (task.FriendlyName.Equals(taskName))
			taskList.Add(task);
		if (!(task is ParentTask parentTask) || parentTask.Children == null)
			return;
		for (var index = 0; index < parentTask.Children.Count; ++index)
			FindTasksWithName(taskName, parentTask.Children[index], ref taskList);
	}

	public Coroutine StartTaskCoroutine(Task task, string methodName) {
		var method = task.GetType()
			.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (method == null) {
			Debug.LogError("Unable to start coroutine " + methodName + ": method not found");
			return null;
		}

		if (activeTaskCoroutines == null)
			activeTaskCoroutines = new Dictionary<string, List<TaskCoroutine>>();
		var taskCoroutine = new TaskCoroutine(this, (IEnumerator)method.Invoke(task, new object[0]), methodName);
		if (activeTaskCoroutines.ContainsKey(methodName)) {
			var activeTaskCoroutine = activeTaskCoroutines[methodName];
			activeTaskCoroutine.Add(taskCoroutine);
			activeTaskCoroutines[methodName] = activeTaskCoroutine;
		} else
			activeTaskCoroutines.Add(methodName, new List<TaskCoroutine> {
				taskCoroutine
			});

		return taskCoroutine.Coroutine;
	}

	public Coroutine StartTaskCoroutine(Task task, string methodName, object value) {
		var method = task.GetType()
			.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (method == null) {
			Debug.LogError("Unable to start coroutine " + methodName + ": method not found");
			return null;
		}

		if (activeTaskCoroutines == null)
			activeTaskCoroutines = new Dictionary<string, List<TaskCoroutine>>();
		var taskCoroutine = new TaskCoroutine(this, (IEnumerator)method.Invoke(task, new object[1] {
			value
		}), methodName);
		if (activeTaskCoroutines.ContainsKey(methodName)) {
			var activeTaskCoroutine = activeTaskCoroutines[methodName];
			activeTaskCoroutine.Add(taskCoroutine);
			activeTaskCoroutines[methodName] = activeTaskCoroutine;
		} else
			activeTaskCoroutines.Add(methodName, new List<TaskCoroutine> {
				taskCoroutine
			});

		return taskCoroutine.Coroutine;
	}

	public void StopTaskCoroutine(string methodName) {
		if (!activeTaskCoroutines.ContainsKey(methodName))
			return;
		var activeTaskCoroutine = activeTaskCoroutines[methodName];
		for (var index = 0; index < activeTaskCoroutine.Count; ++index)
			activeTaskCoroutine[index].Stop();
	}

	public void StopAllTaskCoroutines() {
		StopAllCoroutines();
		foreach (var activeTaskCoroutine in activeTaskCoroutines) {
			var taskCoroutineList = activeTaskCoroutine.Value;
			for (var index = 0; index < taskCoroutineList.Count; ++index)
				taskCoroutineList[index].Stop();
		}
	}

	public void TaskCoroutineEnded(TaskCoroutine taskCoroutine, string coroutineName) {
		if (!activeTaskCoroutines.ContainsKey(coroutineName))
			return;
		var activeTaskCoroutine = activeTaskCoroutines[coroutineName];
		if (activeTaskCoroutine.Count == 1)
			activeTaskCoroutines.Remove(coroutineName);
		else {
			activeTaskCoroutine.Remove(taskCoroutine);
			activeTaskCoroutines[coroutineName] = activeTaskCoroutine;
		}
	}

	public void OnBehaviorStarted() {
		var onBehaviorStart = OnBehaviorStart;
		if (onBehaviorStart == null)
			return;
		onBehaviorStart(this);
	}

	public void OnBehaviorRestarted() {
		var onBehaviorRestart = OnBehaviorRestart;
		if (onBehaviorRestart == null)
			return;
		onBehaviorRestart(this);
	}

	public void OnBehaviorEnded() {
		var onBehaviorEnd = OnBehaviorEnd;
		if (onBehaviorEnd == null)
			return;
		onBehaviorEnd(this);
	}

	private void RegisterEvent(string name, Delegate handler) {
		if (eventTable == null)
			eventTable = new Dictionary<Type, Dictionary<string, Delegate>>();
		Dictionary<string, Delegate> dictionary;
		if (!eventTable.TryGetValue(handler.GetType(), out dictionary)) {
			dictionary = new Dictionary<string, Delegate>();
			eventTable.Add(handler.GetType(), dictionary);
		}

		Delegate a;
		if (dictionary.TryGetValue(name, out a))
			dictionary[name] = Delegate.Combine(a, handler);
		else
			dictionary.Add(name, handler);
	}

	public void RegisterEvent(string name, Action handler) {
		RegisterEvent(name, (Delegate)handler);
	}

	public void RegisterEvent<T>(string name, Action<T> handler) {
		RegisterEvent(name, (Delegate)handler);
	}

	public void RegisterEvent<T, U>(string name, Action<T, U> handler) {
		RegisterEvent(name, (Delegate)handler);
	}

	public void RegisterEvent<T, U, V>(string name, Action<T, U, V> handler) {
		RegisterEvent(name, (Delegate)handler);
	}

	private Delegate GetDelegate(string name, Type type) {
		Dictionary<string, Delegate> dictionary;
		Delegate @delegate;
		return eventTable != null && eventTable.TryGetValue(type, out dictionary) &&
		       dictionary.TryGetValue(name, out @delegate)
			? @delegate
			: null;
	}

	public void SendEvent(string name) {
		if (!(GetDelegate(name, typeof(Action)) is Action action))
			return;
		action();
	}

	public void SendEvent<T>(string name, T arg1) {
		if (!(GetDelegate(name, typeof(Action<T>)) is Action<T> action))
			return;
		action(arg1);
	}

	public void SendEvent<T, U>(string name, T arg1, U arg2) {
		if (!(GetDelegate(name, typeof(Action<T, U>)) is Action<T, U> action))
			return;
		action(arg1, arg2);
	}

	public void SendEvent<T, U, V>(string name, T arg1, U arg2, V arg3) {
		if (!(GetDelegate(name, typeof(Action<T, U, V>)) is Action<T, U, V> action))
			return;
		action(arg1, arg2, arg3);
	}

	private void UnregisterEvent(string name, Delegate handler) {
		Dictionary<string, Delegate> dictionary;
		Delegate source;
		if (eventTable == null || !eventTable.TryGetValue(handler.GetType(), out dictionary) ||
		    !dictionary.TryGetValue(name, out source))
			return;
		dictionary[name] = Delegate.Remove(source, handler);
	}

	public void UnregisterEvent(string name, Action handler) {
		UnregisterEvent(name, (Delegate)handler);
	}

	public void UnregisterEvent<T>(string name, Action<T> handler) {
		UnregisterEvent(name, (Delegate)handler);
	}

	public void UnregisterEvent<T, U>(string name, Action<T, U> handler) {
		UnregisterEvent(name, (Delegate)handler);
	}

	public void UnregisterEvent<T, U, V>(string name, Action<T, U, V> handler) {
		UnregisterEvent(name, (Delegate)handler);
	}

	public void SaveResetValues() {
		if (defaultValues == null) {
			CheckForSerialization();
			defaultValues = new Dictionary<Task, Dictionary<string, object>>();
			defaultVariableValues = new Dictionary<string, object>();
			SaveValues();
		} else
			ResetValues();
	}

	private void SaveValues() {
		var allVariables = behaviorSource.GetAllVariables();
		if (allVariables != null)
			for (var index = 0; index < allVariables.Count; ++index)
				defaultVariableValues.Add(allVariables[index].Name, allVariables[index].GetValue());
		SaveValue(behaviorSource.RootTask);
	}

	private void SaveValue(Task task) {
		if (task == null)
			return;
		var publicFields = TaskUtility.GetPublicFields(task.GetType());
		var dictionary = new Dictionary<string, object>();
		for (var index = 0; index < publicFields.Length; ++index) {
			var obj = publicFields[index].GetValue(task);
			if (!(obj is SharedVariable) || !(obj as SharedVariable).IsShared)
				dictionary.Add(publicFields[index].Name, publicFields[index].GetValue(task));
		}

		defaultValues.Add(task, dictionary);
		if (!(task is ParentTask))
			return;
		var parentTask = task as ParentTask;
		if (parentTask.Children != null)
			for (var index = 0; index < parentTask.Children.Count; ++index)
				SaveValue(parentTask.Children[index]);
	}

	private void ResetValues() {
		foreach (var defaultVariableValue in defaultVariableValues)
			SetVariableValue(defaultVariableValue.Key, defaultVariableValue.Value);
		ResetValue(behaviorSource.RootTask);
	}

	private void ResetValue(Task task) {
		Dictionary<string, object> dictionary;
		if (task == null || !defaultValues.TryGetValue(task, out dictionary))
			return;
		foreach (var keyValuePair in dictionary) {
			var field = task.GetType().GetField(keyValuePair.Key);
			if (field != null)
				field.SetValue(task, keyValuePair.Value);
		}

		if (!(task is ParentTask))
			return;
		var parentTask = task as ParentTask;
		if (parentTask.Children != null)
			for (var index = 0; index < parentTask.Children.Count; ++index)
				ResetValue(parentTask.Children[index]);
	}

	public override string ToString() {
		return behaviorSource.ToString();
	}

	int IBehaviorTree.GetInstanceID() {
		return GetInstanceID();
	}

	public delegate void BehaviorHandler(BehaviorTree behavior);
}