using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
  [AddComponentMenu("BehaviorTree Designer/Variable Synchronizer")]
  public class VariableSynchronizer : MonoBehaviour
  {
    [SerializeField]
    private List<SynchronizedVariable> synchronizedVariables = new List<SynchronizedVariable>();

    public List<SynchronizedVariable> SynchronizedVariables
    {
      get => synchronizedVariables;
      set
      {
        synchronizedVariables = value;
        enabled = true;
      }
    }

    public void Awake()
    {
      for (int index = synchronizedVariables.Count - 1; index > -1; --index)
      {
        SynchronizedVariable synchronizedVariable = synchronizedVariables[index];
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
            case SynchronizationType.BehaviorDesigner:
              BehaviorTree targetComponent = synchronizedVariable.targetComponent as BehaviorTree;
              if (targetComponent == null)
              {
                str = "the target component is not of type BehaviorTree Tree";
                break;
              }
              synchronizedVariable.targetSharedVariable = targetComponent.GetVariable(synchronizedVariable.targetName);
              if (synchronizedVariable.targetSharedVariable == null)
                str = "the target SharedVariable cannot be found";
              break;
            case SynchronizationType.Property:
              PropertyInfo property = synchronizedVariable.targetComponent.GetType().GetProperty(synchronizedVariable.targetName);
              if (property == null)
              {
                str = "the property " + synchronizedVariable.targetName + " doesn't exist";
                break;
              }
              if (synchronizedVariable.setVariable)
              {
                MethodInfo getMethod = property.GetGetMethod();
                if (getMethod == null)
                  str = "the property has no get method";
                else
                  synchronizedVariable.getDelegate = CreateGetDelegate(synchronizedVariable.targetComponent, getMethod);
              }
              else
              {
                MethodInfo setMethod = property.GetSetMethod();
                if (setMethod == null)
                  str = "the property has no set method";
                else
                  synchronizedVariable.setDelegate = CreateSetDelegate(synchronizedVariable.targetComponent, setMethod);
              }
              break;
            case SynchronizationType.Animator:
              synchronizedVariable.animator = synchronizedVariable.targetComponent as Animator;
              if (synchronizedVariable.animator == null)
              {
                str = "the component is not of type Animator";
                break;
              }
              synchronizedVariable.targetID = Animator.StringToHash(synchronizedVariable.targetName);
              Type propertyType = synchronizedVariable.sharedVariable.GetType().GetProperty("Value").PropertyType;
              if (propertyType.Equals(typeof (bool)))
                synchronizedVariable.animatorParameterType = AnimatorParameterType.Bool;
              else if (propertyType.Equals(typeof (float)))
                synchronizedVariable.animatorParameterType = AnimatorParameterType.Float;
              else if (propertyType.Equals(typeof (int)))
                synchronizedVariable.animatorParameterType = AnimatorParameterType.Integer;
              else
                str = "there is no animator parameter type that can synchronize with " + propertyType;
              break;
            case SynchronizationType.PlayMaker:
              Type typeWithinAssembly1 = TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.VariableSynchronizer_PlayMaker");
              if (typeWithinAssembly1 != null)
              {
                MethodInfo method1 = typeWithinAssembly1.GetMethod("Start");
                if (method1 != null)
                {
                  switch ((int) method1.Invoke(null, new object[1]
                  {
                    synchronizedVariable
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
                      if (method2 != null)
                        synchronizedVariable.thirdPartyTick = (Action<SynchronizedVariable>) Delegate.CreateDelegate(typeof (Action<SynchronizedVariable>), method2);
                      break;
                  }
                }
                break;
              }
              str = "has the PlayMaker classes been imported?";
              break;
            case SynchronizationType.uFrame:
              Type typeWithinAssembly2 = TaskUtility.GetTypeWithinAssembly("BehaviorDesigner.Runtime.VariableSynchronizer_uFrame");
              if (typeWithinAssembly2 != null)
              {
                MethodInfo method3 = typeWithinAssembly2.GetMethod("Start");
                if (method3 != null)
                {
                  switch ((int) method3.Invoke(null, new object[1]
                  {
                    synchronizedVariable
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
                      if (method4 != null)
                        synchronizedVariable.thirdPartyTick = (Action<SynchronizedVariable>) Delegate.CreateDelegate(typeof (Action<SynchronizedVariable>), method4);
                      break;
                  }
                }
                break;
              }
              str = "has the uFrame classes been imported?";
              break;
          }
        }
        if (!string.IsNullOrEmpty(str))
        {
          Debug.LogError(string.Format("Unable to synchronize {0}: {1}", synchronizedVariable.sharedVariable.Name, str));
          synchronizedVariables.RemoveAt(index);
        }
      }
      if (synchronizedVariables.Count != 0)
        return;
      enabled = false;
    }

    private void Update() => Tick();

    public void Tick()
    {
      for (int index = 0; index < synchronizedVariables.Count; ++index)
      {
        SynchronizedVariable synchronizedVariable = synchronizedVariables[index];
        switch (synchronizedVariable.synchronizationType)
        {
          case SynchronizationType.BehaviorDesigner:
            if (synchronizedVariable.setVariable)
            {
              synchronizedVariable.sharedVariable.SetValue(synchronizedVariable.targetSharedVariable.GetValue());
              break;
            }
            synchronizedVariable.targetSharedVariable.SetValue(synchronizedVariable.sharedVariable.GetValue());
            break;
          case SynchronizationType.Property:
            if (synchronizedVariable.setVariable)
            {
              synchronizedVariable.sharedVariable.SetValue(synchronizedVariable.getDelegate());
              break;
            }
            synchronizedVariable.setDelegate(synchronizedVariable.sharedVariable.GetValue());
            break;
          case SynchronizationType.Animator:
            if (synchronizedVariable.setVariable)
            {
              switch (synchronizedVariable.animatorParameterType)
              {
                case AnimatorParameterType.Bool:
                  synchronizedVariable.sharedVariable.SetValue(synchronizedVariable.animator.GetBool(synchronizedVariable.targetID));
                  continue;
                case AnimatorParameterType.Float:
                  synchronizedVariable.sharedVariable.SetValue(synchronizedVariable.animator.GetFloat(synchronizedVariable.targetID));
                  continue;
                case AnimatorParameterType.Integer:
                  synchronizedVariable.sharedVariable.SetValue(synchronizedVariable.animator.GetInteger(synchronizedVariable.targetID));
                  continue;
                default:
                  continue;
              }
            }

            switch (synchronizedVariable.animatorParameterType)
            {
              case AnimatorParameterType.Bool:
                synchronizedVariable.animator.SetBool(synchronizedVariable.targetID, (bool) synchronizedVariable.sharedVariable.GetValue());
                break;
              case AnimatorParameterType.Float:
                synchronizedVariable.animator.SetFloat(synchronizedVariable.targetID, (float) synchronizedVariable.sharedVariable.GetValue());
                break;
              case AnimatorParameterType.Integer:
                synchronizedVariable.animator.SetInteger(synchronizedVariable.targetID, (int) synchronizedVariable.sharedVariable.GetValue());
                break;
            }
            break;
          case SynchronizationType.PlayMaker:
          case SynchronizationType.uFrame:
            synchronizedVariable.thirdPartyTick(synchronizedVariable);
            break;
        }
      }
    }
    
    private static Func<object> CreateGetDelegate(object instance, MethodInfo method)
    {
      ConstantExpression constantExpression = Expression.Constant(instance);
      MethodCallExpression methodCallExpression = Expression.Call(constantExpression, method);
      return Expression.Lambda<Func<object>>(Expression.TypeAs(methodCallExpression, typeof(object)), Array.Empty<ParameterExpression>()).Compile();
    }

    private static Action<object> CreateSetDelegate(object instance, MethodInfo method)
    {
      ConstantExpression instance1 = Expression.Constant(instance);
      ParameterExpression parameterExpression = Expression.Parameter(typeof (object), "p");
      UnaryExpression unaryExpression = Expression.Convert(parameterExpression, method.GetParameters()[0].ParameterType);
      return Expression.Lambda<Action<object>>(Expression.Call(instance1, method, unaryExpression), parameterExpression).Compile();
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
      public SynchronizationType synchronizationType;
      public bool setVariable;
      public BehaviorTree behavior;
      public string variableName;
      public Component targetComponent;
      public string targetName;
      public SharedVariable targetSharedVariable;
      public Action<object> setDelegate;
      public Func<object> getDelegate;
      public Animator animator;
      public AnimatorParameterType animatorParameterType;
      public int targetID;
      public Action<SynchronizedVariable> thirdPartyTick;
      public Enum variableType;
      public object thirdPartyVariable;
      public SharedVariable sharedVariable;

      public SynchronizedVariable(
        SynchronizationType synchronizationType,
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
