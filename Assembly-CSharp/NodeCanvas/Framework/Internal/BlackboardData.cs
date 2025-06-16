// Decompiled with JetBrains decompiler
// Type: NodeCanvas.Framework.Internal.BlackboardData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
namespace NodeCanvas.Framework.Internal
{
  [Serializable]
  public sealed class BlackboardData
  {
    [SerializeField]
    private Dictionary<string, Variable> _variables = new Dictionary<string, Variable>();

    public Dictionary<string, Variable> variables
    {
      get => this._variables;
      set => this._variables = value;
    }

    public Variable AddVariable(string varName, object value)
    {
      if (value == null)
      {
        Debug.LogError((object) "<b>Blackboard:</b> You can't use AddVariable with a null value. Use AddVariable(string, Type) to add the new data first");
        return (Variable) null;
      }
      Variable variable = this.AddVariable(varName, value.GetType());
      if (variable != null)
        variable.value = value;
      return variable;
    }

    public Variable AddVariable(string varName, System.Type type)
    {
      if (this.variables.ContainsKey(varName))
      {
        Variable variable = this.GetVariable(varName, type);
        if (variable == null)
          Debug.LogError((object) string.Format("<b>Blackboard:</b> Variable with name '{0}' already exists in blackboard '{1}', but is of different type! Returning null instead of new.", (object) varName, (object) ""));
        else
          Debug.LogWarning((object) string.Format("<b>Blackboard:</b> Variable with name '{0}' already exists in blackboard '{1}'. Returning existing instead of new.", (object) varName, (object) ""));
        return variable;
      }
      Variable instance = (Variable) Activator.CreateInstance(typeof (Variable<>).RTMakeGenericType(new System.Type[1]
      {
        type
      }));
      instance.name = varName;
      this.variables[varName] = instance;
      return instance;
    }

    public Variable RemoveVariable(string varName)
    {
      Variable variable = (Variable) null;
      if (this.variables.TryGetValue(varName, out variable))
        this.variables.Remove(varName);
      return variable;
    }

    public T GetValue<T>(string varName)
    {
      try
      {
        return (this.variables[varName] as Variable<T>).value;
      }
      catch
      {
        try
        {
          return (T) this.variables[varName].value;
        }
        catch
        {
          if (!this.variables.ContainsKey(varName))
          {
            Debug.LogError((object) string.Format("<b>Blackboard:</b> No Variable of name '{0}' and type '{1}' exists on Blackboard '{2}'. Returning default T...", (object) varName, (object) typeof (T).FriendlyName(), (object) ""));
            return default (T);
          }
        }
      }
      Debug.LogError((object) string.Format("<b>Blackboard:</b> Can't cast value of variable with name '{0}' to type '{1}'", (object) varName, (object) typeof (T).FriendlyName()));
      return default (T);
    }

    public Variable SetValue(string varName, object value)
    {
      try
      {
        Variable variable = this.variables[varName];
        variable.value = value;
        return variable;
      }
      catch
      {
        if (!this.variables.ContainsKey(varName))
        {
          Debug.Log((object) string.Format("<b>Blackboard:</b> No Variable of name '{0}' and type '{1}' exists on Blackboard '{2}'. Adding new instead...", (object) varName, value != null ? (object) value.GetType().FriendlyName() : (object) "null", (object) ""));
          Variable variable = this.AddVariable(varName, value);
          if (variable != null)
          {
            variable.isProtected = true;
            return variable;
          }
          Debug.LogError((object) "1");
        }
        else
          Debug.LogError((object) "2");
      }
      Debug.LogError((object) ("<b>Blackboard:</b> Can't cast value '" + (value != null ? value.ToString() : "null") + "' to blackboard variable of name '" + varName + "' and type " + this.variables[varName].varType.Name));
      return (Variable) null;
    }

    public Variable GetVariable(string varName, System.Type ofType = null)
    {
      Variable variable;
      return this.variables != null && varName != null && this.variables.TryGetValue(varName, out variable) && (ofType == (System.Type) null || variable.CanConvertTo(ofType)) ? variable : (Variable) null;
    }

    public Variable GetVariableByID(string ID)
    {
      if (this.variables != null && ID != null)
      {
        foreach (KeyValuePair<string, Variable> variable in this.variables)
        {
          if (variable.Value.ID == ID)
            return variable.Value;
        }
      }
      return (Variable) null;
    }

    public Variable<T> GetVariable<T>(string varName)
    {
      return (Variable<T>) this.GetVariable(varName, typeof (T));
    }

    public string[] GetVariableNames(System.Type ofType)
    {
      return this.variables.Values.Where<Variable>((Func<Variable, bool>) (v => v.CanConvertTo(ofType))).Select<Variable, string>((Func<Variable, string>) (v => v.name)).ToArray<string>();
    }
  }
}
