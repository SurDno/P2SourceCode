using System;
using System.Collections.Generic;
using System.Linq;
using ParadoxNotion;
using UnityEngine;

namespace NodeCanvas.Framework.Internal;

[Serializable]
public sealed class BlackboardData {
	[SerializeField] private Dictionary<string, Variable> _variables = new();

	public Dictionary<string, Variable> variables {
		get => _variables;
		set => _variables = value;
	}

	public Variable AddVariable(string varName, object value) {
		if (value == null) {
			Debug.LogError(
				"<b>Blackboard:</b> You can't use AddVariable with a null value. Use AddVariable(string, Type) to add the new data first");
			return null;
		}

		var variable = AddVariable(varName, value.GetType());
		if (variable != null)
			variable.value = value;
		return variable;
	}

	public Variable AddVariable(string varName, Type type) {
		if (variables.ContainsKey(varName)) {
			var variable = GetVariable(varName, type);
			if (variable == null)
				Debug.LogError(string.Format(
					"<b>Blackboard:</b> Variable with name '{0}' already exists in blackboard '{1}', but is of different type! Returning null instead of new.",
					varName, ""));
			else
				Debug.LogWarning(string.Format(
					"<b>Blackboard:</b> Variable with name '{0}' already exists in blackboard '{1}'. Returning existing instead of new.",
					varName, ""));
			return variable;
		}

		var instance = (Variable)Activator.CreateInstance(typeof(Variable<>).RTMakeGenericType(new Type[1] {
			type
		}));
		instance.name = varName;
		variables[varName] = instance;
		return instance;
	}

	public Variable RemoveVariable(string varName) {
		Variable variable = null;
		if (variables.TryGetValue(varName, out variable))
			variables.Remove(varName);
		return variable;
	}

	public T GetValue<T>(string varName) {
		try {
			return (variables[varName] as Variable<T>).value;
		} catch {
			try {
				return (T)variables[varName].value;
			} catch {
				if (!variables.ContainsKey(varName)) {
					Debug.LogError(string.Format(
						"<b>Blackboard:</b> No Variable of name '{0}' and type '{1}' exists on Blackboard '{2}'. Returning default T...",
						varName, typeof(T).FriendlyName(), ""));
					return default;
				}
			}
		}

		Debug.LogError(string.Format("<b>Blackboard:</b> Can't cast value of variable with name '{0}' to type '{1}'",
			varName, typeof(T).FriendlyName()));
		return default;
	}

	public Variable SetValue(string varName, object value) {
		try {
			var variable = variables[varName];
			variable.value = value;
			return variable;
		} catch {
			if (!variables.ContainsKey(varName)) {
				Debug.Log(string.Format(
					"<b>Blackboard:</b> No Variable of name '{0}' and type '{1}' exists on Blackboard '{2}'. Adding new instead...",
					varName, value != null ? value.GetType().FriendlyName() : (object)"null", ""));
				var variable = AddVariable(varName, value);
				if (variable != null) {
					variable.isProtected = true;
					return variable;
				}

				Debug.LogError("1");
			} else
				Debug.LogError("2");
		}

		Debug.LogError("<b>Blackboard:</b> Can't cast value '" + (value != null ? value.ToString() : "null") +
		               "' to blackboard variable of name '" + varName + "' and type " +
		               variables[varName].varType.Name);
		return null;
	}

	public Variable GetVariable(string varName, Type ofType = null) {
		Variable variable;
		return variables != null && varName != null && variables.TryGetValue(varName, out variable) &&
		       (ofType == null || variable.CanConvertTo(ofType))
			? variable
			: null;
	}

	public Variable GetVariableByID(string ID) {
		if (variables != null && ID != null)
			foreach (var variable in variables)
				if (variable.Value.ID == ID)
					return variable.Value;
		return null;
	}

	public Variable<T> GetVariable<T>(string varName) {
		return (Variable<T>)GetVariable(varName, typeof(T));
	}

	public string[] GetVariableNames(Type ofType) {
		return variables.Values.Where(v => v.CanConvertTo(ofType)).Select(v => v.name).ToArray();
	}
}