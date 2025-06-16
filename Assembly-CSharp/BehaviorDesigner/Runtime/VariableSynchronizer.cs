// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.VariableSynchronizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime
{
  [AddComponentMenu("BehaviorTree Designer/Variable Synchronizer")]
  public class VariableSynchronizer : MonoBehaviour
  {
    [SerializeField]
    private List<VariableSynchronizer.SynchronizedVariable> synchronizedVariables = new List<VariableSynchronizer.SynchronizedVariable>();

    public List<VariableSynchronizer.SynchronizedVariable> SynchronizedVariables
    {
      get => this.synchronizedVariables;
      set
      {
        this.synchronizedVariables = value;
        this.enabled = true;
      }
    }

    public void Awake()
    {
      for (int index = this.synchronizedVariables.Count - 1; index > -1; --index)
      {
        VariableSynchronizer.SynchronizedVariable synchronizedVariable = this.synchronizedVariables[index];
        synchronizedVariable.sharedVariable = synchronizedVariable.behavior.GetVariable(synchronizedVariable.variableName);
        string str = "";
        if (synchronizedVariable.sharedVariable == null)
        {
          str = "the SharedVariable can't be found";
        }
        else
        {
          switch (synchronizedVariable.synchronizationType)
          {
            case VariableSynchronizer.SynchronizationType.BehaviorDesigner:
              BehaviorTree targetComponent = synchronizedVariable.targetComponent as BehaviorTree;
              if ((UnityEngine.Object) targetComponent == (UnityEngine.Object) null)
              {
                str = "the target component is not of type BehaviorTree Tree";
                break;
              }
              synchronizedVariable.targetSharedVariable = targetComponent.GetVariable(synchronizedVariable.targetName);
              if (synchronizedVariable.targetSharedVariable == null)
                str = "the target SharedVariable cannot be found";
              break;
            case VariableSynchronizer.SynchronizationType.Property:
              PropertyInfo property = ((object) synchronizedVariable.targetComponent).GetType().GetProperty(synchronizedVariable.targetName);
              if (property == (PropertyInfo) null)
              {
                str = "the property " + synchronizedVariable.targetName + " doesn't exist";
                break;
              }
              if (synchronizedVariable.setVariable)
              {
                MethodInfo getMethod = property.GetGetMethod();
                if (getMethod == (MethodInfo) null)
                  str = "the property has no get method";
                else
                  synchronizedVariable.getDelegate = VariableSynchronizer.CreateGetDelegate((object) synchronizedVariable.targetComponent, getMethod);
              }
              else
              {
                MethodInfo setMethod = property.GetSetMethod();
                if (setMethod == (MethodInfo) null)
                  str = "the property has no set method";
                else
                  synchronizedVariable.setDelegate = VariableSynchronizer.CreateSetDelegate((object) synchronizedVariable.targetComponent, setMethod);
              }
              break;
            case VariableSynchronizer.SynchronizationType.Animator:
              synchronizedVariable.animator = synchronizedVariable.targetComponent as Animator;
              if ((UnityEngine.Object) synchronizedVariable.animator == (UnityEngine.Object) null)
              {
                str = "the component is not of type Animator";
                break;
              }
              synchronizedVariable.targetID = Animator.StringToHash(synchronizedVariable.targetName);
              System.Type propertyType = synchronizedVariable.sharedVariable.GetType().GetProperty("Value").PropertyType;
              if (propertyType.Equals(typeof (bool)))
                synchronizedVariable.animatorParameterType = VariableSynchronizer.AnimatorParameterType.Bool;
              else if (propertyType.Equals(typeof (float)))
                synchronizedVariable.animatorParameterType = VariableSynchronizer.AnimatorParameterType.Float;
              else if (propertyType.Equals(typeof (int)))
                synchronizedVariable.animatorParameterType = VariableSynchronizer.AnimatorParameterType.Integer;
              else
                str = "there is no animator parameter type that can synchronize with " + (object) propertyType;
              break;
            case VariableSynchronizer.SynchronizationType.PlayMaker:
              System.Type typeWithinAssembly1 = TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.VariableSynchronizer_PlayMaker");
              if (typeWithinAssembly1 != (System.Type) null)
              {
                MethodInfo method1 = typeWithinAssembly1.GetMethod("Start");
                if (method1 != (MethodInfo) null)
                {
                  switch ((int) method1.Invoke((object) null, new object[1]
                  {
                    (object) synchronizedVariable
                  }))
                  {
                    case 1:
                      str = "the PlayMaker NamedVariable cannot be found";
                      break;
                    case 2:
                      str = "the BehaviorTree Designer SharedVariable is not the same type as the PlayMaker NamedVariable";
                      break;
                    default:
                      MethodInfo method2 = typeWithinAssembly1.GetMethod("Tick");
                      if (method2 != (MethodInfo) null)
                        synchronizedVariable.thirdPartyTick = (Action<VariableSynchronizer.SynchronizedVariable>) Delegate.CreateDelegate(typeof (Action<VariableSynchronizer.SynchronizedVariable>), method2);
                      break;
                  }
                  break;
                }
                break;
              }
              str = "has the PlayMaker classes been imported?";
              break;
            case VariableSynchronizer.SynchronizationType.uFrame:
              System.Type typeWithinAssembly2 = TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.VariableSynchronizer_uFrame");
              if (typeWithinAssembly2 != (System.Type) null)
              {
                MethodInfo method3 = typeWithinAssembly2.GetMethod("Start");
                if (method3 != (MethodInfo) null)
                {
                  switch ((int) method3.Invoke((object) null, new object[1]
                  {
                    (object) synchronizedVariable
                  }))
                  {
                    case 1:
                      str = "the uFrame property cannot be found";
                      break;
                    case 2:
                      str = "the BehaviorTree Designer SharedVariable is not the same type as the uFrame property";
                      break;
                    default:
                      MethodInfo method4 = typeWithinAssembly2.GetMethod("Tick");
                      if (method4 != (MethodInfo) null)
                        synchronizedVariable.thirdPartyTick = (Action<VariableSynchronizer.SynchronizedVariable>) Delegate.CreateDelegate(typeof (Action<VariableSynchronizer.SynchronizedVariable>), method4);
                      break;
                  }
                  break;
                }
                break;
              }
              str = "has the uFrame classes been imported?";
              break;
          }
        }
        if (!string.IsNullOrEmpty(str))
        {
          Debug.LogError((object) string.Format("Unable to synchronize {0}: {1}", (object) synchronizedVariable.sharedVariable.Name, (object) str));
          this.synchronizedVariables.RemoveAt(index);
        }
      }
      if (this.synchronizedVariables.Count != 0)
        return;
      this.enabled = false;
    }

    private void Update() => this.Tick();

    public void Tick()
    {
      for (int index = 0; index < this.synchronizedVariables.Count; ++index)
      {
        VariableSynchronizer.SynchronizedVariable synchronizedVariable = this.synchronizedVariables[index];
        switch (synchronizedVariable.synchronizationType)
        {
          case VariableSynchronizer.SynchronizationType.BehaviorDesigner:
            if (synchronizedVariable.setVariable)
            {
              synchronizedVariable.sharedVariable.SetValue(synchronizedVariable.targetSharedVariable.GetValue());
              break;
            }
            synchronizedVariable.targetSharedVariable.SetValue(synchronizedVariable.sharedVariable.GetValue());
            break;
          case VariableSynchronizer.SynchronizationType.Property:
            if (synchronizedVariable.setVariable)
            {
              synchronizedVariable.sharedVariable.SetValue(synchronizedVariable.getDelegate());
              break;
            }
            synchronizedVariable.setDelegate(synchronizedVariable.sharedVariable.GetValue());
            break;
          case VariableSynchronizer.SynchronizationType.Animator:
            if (synchronizedVariable.setVariable)
            {
              switch (synchronizedVariable.animatorParameterType)
              {
                case VariableSynchronizer.AnimatorParameterType.Bool:
                  synchronizedVariable.sharedVariable.SetValue((object) synchronizedVariable.animator.GetBool(synchronizedVariable.targetID));
                  continue;
                case VariableSynchronizer.AnimatorParameterType.Float:
                  synchronizedVariable.sharedVariable.SetValue((object) synchronizedVariable.animator.GetFloat(synchronizedVariable.targetID));
                  continue;
                case VariableSynchronizer.AnimatorParameterType.Integer:
                  synchronizedVariable.sharedVariable.SetValue((object) synchronizedVariable.animator.GetInteger(synchronizedVariable.targetID));
                  continue;
                default:
                  continue;
              }
            }
            else
            {
              switch (synchronizedVariable.animatorParameterType)
              {
                case VariableSynchronizer.AnimatorParameterType.Bool:
                  synchronizedVariable.animator.SetBool(synchronizedVariable.targetID, (bool) synchronizedVariable.sharedVariable.GetValue());
                  break;
                case VariableSynchronizer.AnimatorParameterType.Float:
                  synchronizedVariable.animator.SetFloat(synchronizedVariable.targetID, (float) synchronizedVariable.sharedVariable.GetValue());
                  break;
                case VariableSynchronizer.AnimatorParameterType.Integer:
                  synchronizedVariable.animator.SetInteger(synchronizedVariable.targetID, (int) synchronizedVariable.sharedVariable.GetValue());
                  break;
              }
              break;
            }
          case VariableSynchronizer.SynchronizationType.PlayMaker:
          case VariableSynchronizer.SynchronizationType.uFrame:
            synchronizedVariable.thirdPartyTick(synchronizedVariable);
            break;
        }
      }
    }

    private static Func<object> CreateGetDelegate(object instance, MethodInfo method)
    {
      return ((Expression<Func<object>>) (() => System.Linq.Expressions.Expression.Call(instance, method) as object)).Compile();
    }

    private static Action<object> CreateSetDelegate(object instance, MethodInfo method)
    {
      ConstantExpression instance1 = System.Linq.Expressions.Expression.Constant(instance);
      ParameterExpression parameterExpression = System.Linq.Expressions.Expression.Parameter(typeof (object), "p");
      UnaryExpression unaryExpression = System.Linq.Expressions.Expression.Convert((System.Linq.Expressions.Expression) parameterExpression, method.GetParameters()[0].ParameterType);
      return System.Linq.Expressions.Expression.Lambda<Action<object>>((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call((System.Linq.Expressions.Expression) instance1, method, (System.Linq.Expressions.Expression) unaryExpression), parameterExpression).Compile();
    }

    public enum SynchronizationType
    {
      BehaviorDesigner,
      Property,
      Animator,
      PlayMaker,
      uFrame,
    }

    public enum AnimatorParameterType
    {
      Bool,
      Float,
      Integer,
    }

    [Serializable]
    public class SynchronizedVariable
    {
      public VariableSynchronizer.SynchronizationType synchronizationType;
      public bool setVariable;
      public BehaviorTree behavior;
      public string variableName;
      public Component targetComponent;
      public string targetName;
      public SharedVariable targetSharedVariable;
      public Action<object> setDelegate;
      public Func<object> getDelegate;
      public Animator animator;
      public VariableSynchronizer.AnimatorParameterType animatorParameterType;
      public int targetID;
      public Action<VariableSynchronizer.SynchronizedVariable> thirdPartyTick;
      public Enum variableType;
      public object thirdPartyVariable;
      public SharedVariable sharedVariable;

      public SynchronizedVariable(
        VariableSynchronizer.SynchronizationType synchronizationType,
        bool setVariable,
        BehaviorTree behavior,
        string variableName,
        bool global,
        Component targetComponent,
        string targetName,
        bool targetGlobal)
      {
        this.synchronizationType = synchronizationType;
        this.setVariable = setVariable;
        this.behavior = behavior;
        this.variableName = variableName;
        this.targetComponent = targetComponent;
        this.targetName = targetName;
      }
    }
  }
}
