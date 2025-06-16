using BehaviorDesigner.Runtime.Tasks;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.Settings.External;
using Inspectors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Profiling;

namespace BehaviorDesigner.Runtime
{
  public class BehaviorTreeManager : 
    MonoBehaviourInstance<BehaviorTreeManager>,
    IUpdateItem<BehaviorTreeClient>
  {
    private bool reduceUpdateFarObjects;
    private float reduceUpdateFarObjectsDistance;
    private bool updateSkipped;
    private List<BehaviorTreeClient> behaviorTrees = new List<BehaviorTreeClient>();
    private Dictionary<BehaviorTree, BehaviorTreeClient> pausedBehaviorTrees = new Dictionary<BehaviorTree, BehaviorTreeClient>();
    private Dictionary<BehaviorTree, BehaviorTreeClient> behaviorTreeMap = new Dictionary<BehaviorTree, BehaviorTreeClient>();
    private List<int> conditionalParentIndexes = new List<int>();
    [Inspected]
    private ReduceUpdateProxy<BehaviorTreeClient> updater;

    public List<BehaviorTreeClient> BehaviorTrees => this.behaviorTrees;

    protected override void Awake()
    {
      base.Awake();
      this.updater = new ReduceUpdateProxy<BehaviorTreeClient>(this.behaviorTrees, (IUpdateItem<BehaviorTreeClient>) this, ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.BehaviorTreeUpdateDelay);
    }

    public void OnDestroy()
    {
      List<BehaviorTreeClient> list = this.behaviorTrees.ToList<BehaviorTreeClient>();
      for (int index = list.Count - 1; index > -1; --index)
        this.DisableBehavior(list[index].behavior);
    }

    public void OnApplicationQuit()
    {
      List<BehaviorTreeClient> list = this.behaviorTrees.ToList<BehaviorTreeClient>();
      for (int index = list.Count - 1; index > -1; --index)
        this.DisableBehavior(list[index].behavior);
    }

    public void EnableBehavior(BehaviorTree behavior)
    {
      if (this.IsBehaviorEnabled(behavior))
        return;
      BehaviorTreeClient behaviorTreeClient;
      if (this.pausedBehaviorTrees.TryGetValue(behavior, out behaviorTreeClient))
      {
        this.behaviorTrees.Add(behaviorTreeClient);
        this.pausedBehaviorTrees.Remove(behavior);
        behavior.ExecutionStatus = TaskStatus.Running;
        for (int index = 0; index < behaviorTreeClient.taskList.Count; ++index)
          behaviorTreeClient.taskList[index].OnPause(false);
      }
      else
      {
        TaskAddData data = ObjectPool.Get<TaskAddData>();
        data.Initialize();
        behavior.CheckForSerialization();
        Task rootTask = behavior.BehaviorSource.RootTask;
        if (rootTask == null)
        {
          Debug.LogWarning((object) string.Format("The behavior \"{0}\" on GameObject \"{1}\" contains no root task. This behavior will be disabled.", (object) behavior.BehaviorSource.Name, (object) behavior.gameObject.name));
        }
        else
        {
          BehaviorTreeClient behaviorTree = ObjectPool.Get<BehaviorTreeClient>();
          behaviorTree.Initialize(behavior);
          behaviorTree.parentIndex.Add(-1);
          behaviorTree.relativeChildIndex.Add(-1);
          behaviorTree.parentCompositeIndex.Add(-1);
          bool hasExternalBehavior = (UnityEngine.Object) behavior.ExternalBehaviorTree != (UnityEngine.Object) null;
          TaskState taskList = this.AddToTaskList(behaviorTree, rootTask, ref hasExternalBehavior, data);
          if (taskList != 0)
          {
            if (behavior.BehaviorSource.Name == null)
              Debug.LogError((object) "behavior.BehaviorSource.Name == null");
            if (behavior.gameObject.name == null)
              Debug.LogError((object) "behavior.gameObject.name == null");
            switch (taskList)
            {
              case TaskState.RootTaskDisabled:
                Debug.LogError((object) string.Format("The behavior \"{0}\" on GameObject \"{1}\" contains a root task which is disabled. This behavior will be disabled.", (object) behavior.BehaviorSource.Name, (object) behavior.gameObject.name));
                break;
              case TaskState.BehaviorTreeReferenceTaskContainsNullExternalTree:
                Debug.LogError((object) string.Format("The behavior \"{0}\" on GameObject \"{1}\" contains a BehaviorTree Tree Reference task ({2} (index {3})) that which has an element with a null value in the externalBehaviors array. This behavior will be disabled.", (object) behavior.BehaviorSource.Name, (object) behavior.gameObject.name, (object) data.errorTaskName, (object) data.errorTask));
                break;
              case TaskState.MultipleExternalBehaviorTreesAndParentTaskIsNullOrCannotHandleAsManyBehaviorTreesSpecified:
                Debug.LogError((object) string.Format("The behavior \"{0}\" on GameObject \"{1}\" contains multiple external behavior trees at the root task or as a child of a parent task which cannot contain so many children (such as a decorator task). This behavior will be disabled.", (object) behavior.BehaviorSource.Name, (object) behavior.gameObject.name));
                break;
              case TaskState.TaskIsNull:
                Debug.LogError((object) string.Format("The behavior \"{0}\" on GameObject \"{1}\" contains a null task (referenced from parent task {2} (index {3})). This behavior will be disabled.", (object) behavior.BehaviorSource.Name, (object) behavior.gameObject.name, (object) data.errorTaskName, (object) data.errorTask));
                break;
              case TaskState.ExternalTaskCannotBeFound:
                Debug.LogError((object) string.Format("The behavior \"{0}\" on GameObject \"{1}\" cannot find the referenced external task. This behavior will be disabled.", (object) behavior.BehaviorSource.Name, (object) behavior.gameObject.name));
                break;
              case TaskState.ParentNotHaveAnyChildren:
                Debug.LogError((object) string.Format("The behavior \"{0}\" on GameObject \"{1}\" contains a parent task ({2} (index {3})) with no children. This behavior will be disabled.", (object) behavior.BehaviorSource.Name, (object) behavior.gameObject.name, (object) data.errorTaskName, (object) data.errorTask));
                break;
            }
          }
          else
          {
            if (behavior.ResetValuesOnRestart)
              behavior.SaveResetValues();
            Stack<int> intStack = ObjectPool.Get<Stack<int>>();
            intStack.Clear();
            behaviorTree.activeStack.Add(intStack);
            behaviorTree.interruptionIndex.Add(-1);
            behaviorTree.nonInstantTaskStatus.Add(TaskStatus.Inactive);
            for (int index = 0; index < behaviorTree.taskList.Count; ++index)
              behaviorTree.taskList[index].OnAwake();
            this.behaviorTrees.Add(behaviorTree);
            this.behaviorTreeMap.Add(behavior, behaviorTree);
            if (behaviorTree.taskList[0].Disabled)
              return;
            behaviorTree.behavior.OnBehaviorStarted();
            behavior.ExecutionStatus = TaskStatus.Running;
            this.PushTask(behaviorTree, 0, 0);
          }
        }
      }
    }

    private TaskState AddToTaskList(
      BehaviorTreeClient behaviorTree,
      Task task,
      ref bool hasExternalBehavior,
      TaskAddData data)
    {
      if (task == null)
        return TaskState.TaskIsNull;
      task.GameObject = behaviorTree.behavior.gameObject;
      task.Transform = behaviorTree.behavior.transform;
      task.Owner = behaviorTree.behavior;
      if (task is BehaviorReference)
      {
        if (!(task is BehaviorReference behaviorReference))
          return TaskState.ExternalTaskCannotBeFound;
        ExternalBehaviorTree[] externalBehaviors;
        if ((externalBehaviors = behaviorReference.GetExternalBehaviors()) == null)
          return TaskState.ExternalTaskCannotBeFound;
        BehaviorSource[] behaviorSourceArray = new BehaviorSource[externalBehaviors.Length];
        for (int index = 0; index < externalBehaviors.Length; ++index)
        {
          if ((UnityEngine.Object) externalBehaviors[index] == (UnityEngine.Object) null)
          {
            data.errorTask = behaviorTree.taskList.Count;
            data.errorTaskName = !string.IsNullOrEmpty(task.FriendlyName) ? task.FriendlyName : task.GetType().ToString();
            return TaskState.BehaviorTreeReferenceTaskContainsNullExternalTree;
          }
          behaviorSourceArray[index] = externalBehaviors[index].BehaviorSource;
          behaviorSourceArray[index].Owner = (IBehaviorTree) externalBehaviors[index];
        }
        if (behaviorSourceArray == null)
          return TaskState.ExternalTaskCannotBeFound;
        ParentTask parentTask = data.parentTask;
        int parentIndex = data.parentIndex;
        int compositeParentIndex = data.compositeParentIndex;
        ++data.depth;
        for (int index1 = 0; index1 < behaviorSourceArray.Length; ++index1)
        {
          BehaviorSource behaviorSource = ObjectPool.Get<BehaviorSource>();
          behaviorSource.Initialize(behaviorSourceArray[index1].Owner);
          behaviorSourceArray[index1].CheckForSerialization(true, behaviorSource, "");
          Task rootTask = behaviorSource.RootTask;
          if (rootTask != null)
          {
            rootTask.Disabled = task.Disabled;
            if (behaviorReference.variables != null)
            {
              for (int index2 = 0; index2 < behaviorReference.variables.Length; ++index2)
              {
                if (data.overrideFields == null)
                {
                  data.overrideFields = ObjectPool.Get<Dictionary<string, OverrideFieldValue>>();
                  data.overrideFields.Clear();
                }
                if (!data.overrideFields.ContainsKey(behaviorReference.variables[index2].Value.name))
                {
                  OverrideFieldValue overrideFieldValue1 = ObjectPool.Get<OverrideFieldValue>();
                  overrideFieldValue1.Initialize((object) behaviorReference.variables[index2].Value, data.depth);
                  if (behaviorReference.variables[index2].Value != null)
                  {
                    GenericVariable genericVariable = (GenericVariable) behaviorReference.variables[index2].Value;
                    if (genericVariable.value != null)
                    {
                      if (string.IsNullOrEmpty(genericVariable.value.Name))
                      {
                        Debug.LogWarning((object) ("Warning: Named variable on reference task " + behaviorReference.FriendlyName + " (id " + (object) behaviorReference.Id + ") is null"));
                        continue;
                      }
                      OverrideFieldValue overrideFieldValue2;
                      if (data.overrideFields.TryGetValue(genericVariable.value.Name, out overrideFieldValue2))
                        overrideFieldValue1 = overrideFieldValue2;
                    }
                  }
                  else if (behaviorReference.variables[index2].Value != null)
                  {
                    NamedVariable namedVariable = behaviorReference.variables[index2].Value;
                    if (string.IsNullOrEmpty(namedVariable.value.Name))
                    {
                      Debug.LogWarning((object) ("Warning: Named variable on reference task " + behaviorReference.FriendlyName + " (id " + (object) behaviorReference.Id + ") is null"));
                      continue;
                    }
                    OverrideFieldValue overrideFieldValue3;
                    if (namedVariable.value != null && data.overrideFields.TryGetValue(namedVariable.value.Name, out overrideFieldValue3))
                      overrideFieldValue1 = overrideFieldValue3;
                  }
                  data.overrideFields.Add(behaviorReference.variables[index2].Value.name, overrideFieldValue1);
                }
              }
            }
            if (behaviorSource.Variables != null)
            {
              for (int index3 = 0; index3 < behaviorSource.Variables.Count; ++index3)
              {
                SharedVariable variable;
                if ((variable = behaviorTree.behavior.GetVariable(behaviorSource.Variables[index3].Name)) == null)
                {
                  variable = behaviorSource.Variables[index3];
                  behaviorTree.behavior.SetVariable(variable.Name, variable);
                }
                else
                  behaviorSource.Variables[index3].SetValue(variable.GetValue());
                if (data.overrideFields == null)
                {
                  data.overrideFields = ObjectPool.Get<Dictionary<string, OverrideFieldValue>>();
                  data.overrideFields.Clear();
                }
                if (!data.overrideFields.ContainsKey(variable.Name))
                {
                  OverrideFieldValue overrideFieldValue = ObjectPool.Get<OverrideFieldValue>();
                  overrideFieldValue.Initialize((object) variable, data.depth);
                  data.overrideFields.Add(variable.Name, overrideFieldValue);
                }
              }
            }
            ObjectPool.Return<BehaviorSource>(behaviorSource);
            if (index1 > 0)
            {
              data.parentTask = parentTask;
              data.parentIndex = parentIndex;
              data.compositeParentIndex = compositeParentIndex;
              if (data.parentTask == null || index1 >= data.parentTask.MaxChildren())
                return TaskState.MultipleExternalBehaviorTreesAndParentTaskIsNullOrCannotHandleAsManyBehaviorTreesSpecified;
              behaviorTree.parentIndex.Add(data.parentIndex);
              behaviorTree.relativeChildIndex.Add(data.parentTask.Children.Count);
              behaviorTree.parentCompositeIndex.Add(data.compositeParentIndex);
              behaviorTree.childrenIndex[data.parentIndex].Add(behaviorTree.taskList.Count);
              data.parentTask.AddChild(rootTask, data.parentTask.Children.Count);
            }
            hasExternalBehavior = true;
            bool fromExternalTask = data.fromExternalTask;
            data.fromExternalTask = true;
            TaskState taskList;
            if ((taskList = this.AddToTaskList(behaviorTree, rootTask, ref hasExternalBehavior, data)) != 0)
              return taskList;
            data.fromExternalTask = fromExternalTask;
          }
          else
          {
            ObjectPool.Return<BehaviorSource>(behaviorSource);
            return TaskState.ExternalTaskCannotBeFound;
          }
        }
        if (data.overrideFields != null)
        {
          Dictionary<string, OverrideFieldValue> dictionary = ObjectPool.Get<Dictionary<string, OverrideFieldValue>>();
          dictionary.Clear();
          foreach (KeyValuePair<string, OverrideFieldValue> overrideField in data.overrideFields)
          {
            if (overrideField.Value.Depth != data.depth)
              dictionary.Add(overrideField.Key, overrideField.Value);
          }
          ObjectPool.Return<Dictionary<string, OverrideFieldValue>>(data.overrideFields);
          data.overrideFields = dictionary;
        }
        --data.depth;
      }
      else
      {
        if (behaviorTree.taskList.Count == 0 && task.Disabled)
          return TaskState.RootTaskDisabled;
        task.ReferenceID = behaviorTree.taskList.Count;
        behaviorTree.taskList.Add(task);
        if (data.overrideFields != null)
          this.OverrideFields(behaviorTree, data, (object) task);
        if (data.fromExternalTask)
        {
          int index = behaviorTree.relativeChildIndex[behaviorTree.relativeChildIndex.Count - 1];
          data.parentTask.ReplaceAddChild(task, index);
        }
        if (task is ParentTask)
        {
          ParentTask parentTask = task as ParentTask;
          if (parentTask.Children == null || parentTask.Children.Count == 0)
          {
            data.errorTask = behaviorTree.taskList.Count - 1;
            data.errorTaskName = !string.IsNullOrEmpty(behaviorTree.taskList[data.errorTask].FriendlyName) ? behaviorTree.taskList[data.errorTask].FriendlyName : behaviorTree.taskList[data.errorTask].GetType().ToString();
            return TaskState.ParentNotHaveAnyChildren;
          }
          int index4 = behaviorTree.taskList.Count - 1;
          List<int> intList1 = ObjectPool.Get<List<int>>();
          intList1.Clear();
          behaviorTree.childrenIndex.Add(intList1);
          List<int> intList2 = ObjectPool.Get<List<int>>();
          intList2.Clear();
          behaviorTree.childConditionalIndex.Add(intList2);
          int count = parentTask.Children.Count;
          for (int index5 = 0; index5 < count; ++index5)
          {
            behaviorTree.parentIndex.Add(index4);
            behaviorTree.relativeChildIndex.Add(index5);
            behaviorTree.childrenIndex[index4].Add(behaviorTree.taskList.Count);
            data.parentTask = task as ParentTask;
            data.parentIndex = index4;
            if (task is Composite)
              data.compositeParentIndex = index4;
            behaviorTree.parentCompositeIndex.Add(data.compositeParentIndex);
            TaskState taskList;
            if ((taskList = this.AddToTaskList(behaviorTree, parentTask.Children[index5], ref hasExternalBehavior, data)) != 0)
            {
              if (taskList == TaskState.TaskIsNull)
              {
                data.errorTask = index4;
                data.errorTaskName = !string.IsNullOrEmpty(behaviorTree.taskList[data.errorTask].FriendlyName) ? behaviorTree.taskList[data.errorTask].FriendlyName : behaviorTree.taskList[data.errorTask].GetType().ToString();
              }
              return taskList;
            }
          }
        }
        else
        {
          behaviorTree.childrenIndex.Add((List<int>) null);
          behaviorTree.childConditionalIndex.Add((List<int>) null);
          if (task is Conditional)
          {
            int index6 = behaviorTree.taskList.Count - 1;
            int index7 = behaviorTree.parentCompositeIndex[index6];
            if (index7 != -1)
              behaviorTree.childConditionalIndex[index7].Add(index6);
          }
        }
      }
      return TaskState.Success;
    }

    private void OverrideFields(BehaviorTreeClient behaviorTree, TaskAddData data, object obj)
    {
      if (obj == null || object.Equals(obj, (object) null))
        return;
      FieldInfo[] allFields = TaskUtility.GetAllFields(obj.GetType());
      for (int index1 = 0; index1 < allFields.Length; ++index1)
      {
        object obj1 = allFields[index1].GetValue(obj);
        if (obj1 != null)
        {
          if (typeof (SharedVariable).IsAssignableFrom(allFields[index1].FieldType))
          {
            SharedVariable sharedVariable = this.OverrideSharedVariable(behaviorTree, data, allFields[index1].FieldType, obj1 as SharedVariable);
            if (sharedVariable != null)
              allFields[index1].SetValue(obj, (object) sharedVariable);
          }
          else if (typeof (IList).IsAssignableFrom(allFields[index1].FieldType))
          {
            System.Type fieldType;
            if ((typeof (SharedVariable).IsAssignableFrom(fieldType = allFields[index1].FieldType.GetElementType()) || allFields[index1].FieldType.IsGenericType && typeof (SharedVariable).IsAssignableFrom(fieldType = allFields[index1].FieldType.GetGenericArguments()[0])) && obj1 is IList<SharedVariable> sharedVariableList)
            {
              for (int index2 = 0; index2 < sharedVariableList.Count; ++index2)
              {
                SharedVariable sharedVariable = this.OverrideSharedVariable(behaviorTree, data, fieldType, sharedVariableList[index2]);
                if (sharedVariable != null)
                  sharedVariableList[index2] = sharedVariable;
              }
            }
          }
          else if (allFields[index1].FieldType.IsClass && !allFields[index1].FieldType.Equals(typeof (System.Type)) && !typeof (Delegate).IsAssignableFrom(allFields[index1].FieldType) && !data.overiddenFields.Contains(obj1))
          {
            data.overiddenFields.Add(obj1);
            this.OverrideFields(behaviorTree, data, obj1);
            data.overiddenFields.Remove(obj1);
          }
        }
      }
    }

    private SharedVariable OverrideSharedVariable(
      BehaviorTreeClient behaviorTree,
      TaskAddData data,
      System.Type fieldType,
      SharedVariable sharedVariable)
    {
      SharedVariable sharedVariable1 = sharedVariable;
      if (sharedVariable is SharedGenericVariable)
        sharedVariable = ((sharedVariable as SharedGenericVariable).GetValue() as GenericVariable).value;
      else if (sharedVariable is SharedNamedVariable)
        sharedVariable = ((sharedVariable as SharedNamedVariable).GetValue() as NamedVariable).value;
      OverrideFieldValue overrideFieldValue;
      if (sharedVariable == null || string.IsNullOrEmpty(sharedVariable.Name) || !data.overrideFields.TryGetValue(sharedVariable.Name, out overrideFieldValue))
        return (SharedVariable) null;
      SharedVariable sharedVariable2 = (SharedVariable) null;
      if (overrideFieldValue.Value is SharedVariable)
        sharedVariable2 = overrideFieldValue.Value as SharedVariable;
      else if (overrideFieldValue.Value is NamedVariable)
      {
        sharedVariable2 = (overrideFieldValue.Value as NamedVariable).value;
        if (sharedVariable2.IsShared)
          sharedVariable2 = behaviorTree.behavior.GetVariable(sharedVariable2.Name);
      }
      else if (overrideFieldValue.Value is GenericVariable)
      {
        sharedVariable2 = (overrideFieldValue.Value as GenericVariable).value;
        if (sharedVariable2.IsShared)
          sharedVariable2 = behaviorTree.behavior.GetVariable(sharedVariable2.Name);
      }
      if (sharedVariable1 is SharedNamedVariable || sharedVariable1 is SharedGenericVariable)
      {
        if (fieldType.Equals(typeof (SharedVariable)) || sharedVariable2.GetType().Equals(sharedVariable.GetType()))
        {
          switch (sharedVariable1)
          {
            case SharedNamedVariable _:
              (sharedVariable1 as SharedNamedVariable).Value.value = sharedVariable2;
              break;
            case SharedGenericVariable _:
              (sharedVariable1 as SharedGenericVariable).Value.value = sharedVariable2;
              break;
          }
          behaviorTree.behavior.SetVariableValue(sharedVariable.Name, sharedVariable2.GetValue());
        }
      }
      else if (sharedVariable2 != null)
        return sharedVariable2;
      return (SharedVariable) null;
    }

    public void DisableBehavior(BehaviorTree behavior) => this.DisableBehavior(behavior, false);

    public void DisableBehavior(BehaviorTree behavior, bool paused)
    {
      this.DisableBehavior(behavior, paused, TaskStatus.Success);
    }

    public void DisableBehavior(BehaviorTree behavior, bool paused, TaskStatus executionStatus)
    {
      if (!this.IsBehaviorEnabled(behavior))
      {
        if (!this.pausedBehaviorTrees.ContainsKey(behavior) || paused)
          return;
        this.EnableBehavior(behavior);
      }
      if (paused)
      {
        BehaviorTreeClient behaviorTreeClient;
        if (!this.behaviorTreeMap.TryGetValue(behavior, out behaviorTreeClient) || this.pausedBehaviorTrees.ContainsKey(behavior))
          return;
        this.pausedBehaviorTrees.Add(behavior, behaviorTreeClient);
        behavior.ExecutionStatus = TaskStatus.Inactive;
        for (int index = 0; index < behaviorTreeClient.taskList.Count; ++index)
          behaviorTreeClient.taskList[index].OnPause(true);
        this.behaviorTrees.Remove(behaviorTreeClient);
      }
      else
        this.DestroyBehavior(behavior, executionStatus);
    }

    public void DestroyBehavior(BehaviorTree behavior)
    {
      this.DestroyBehavior(behavior, TaskStatus.Success);
    }

    public void DestroyBehavior(BehaviorTree behavior, TaskStatus executionStatus)
    {
      BehaviorTreeClient behaviorTree;
      if (!this.behaviorTreeMap.TryGetValue(behavior, out behaviorTree) || behaviorTree.destroyBehavior)
        return;
      behaviorTree.destroyBehavior = true;
      if (this.pausedBehaviorTrees.ContainsKey(behavior))
      {
        this.pausedBehaviorTrees.Remove(behavior);
        for (int index = 0; index < behaviorTree.taskList.Count; ++index)
          behaviorTree.taskList[index].OnPause(false);
        behavior.ExecutionStatus = TaskStatus.Running;
      }
      TaskStatus status = executionStatus;
      for (int index = behaviorTree.activeStack.Count - 1; index > -1; --index)
      {
        while (behaviorTree.activeStack[index].Count > 0)
        {
          int count = behaviorTree.activeStack[index].Count;
          this.PopTask(behaviorTree, behaviorTree.activeStack[index].Peek(), index, ref status, true, false);
          if (count == 1)
            break;
        }
      }
      this.RemoveChildConditionalReevaluate(behaviorTree, -1);
      for (int index = 0; index < behaviorTree.taskList.Count; ++index)
        behaviorTree.taskList[index].OnBehaviorComplete();
      this.behaviorTreeMap.Remove(behavior);
      this.behaviorTrees.Remove(behaviorTree);
      behaviorTree.destroyBehavior = false;
      ObjectPool.Return<BehaviorTreeClient>(behaviorTree);
      behavior.ExecutionStatus = status;
      behavior.OnBehaviorEnded();
    }

    public void RestartBehavior(BehaviorTree behavior)
    {
      if (!this.IsBehaviorEnabled(behavior))
        return;
      BehaviorTreeClient behaviorTree = this.behaviorTreeMap[behavior];
      TaskStatus status = TaskStatus.Success;
      for (int index = behaviorTree.activeStack.Count - 1; index > -1; --index)
      {
        while (behaviorTree.activeStack[index].Count > 0)
        {
          int count = behaviorTree.activeStack[index].Count;
          this.PopTask(behaviorTree, behaviorTree.activeStack[index].Peek(), index, ref status, true, false);
          if (count == 1)
            break;
        }
      }
      this.Restart(behaviorTree);
    }

    public bool IsBehaviorEnabled(BehaviorTree behavior)
    {
      return behavior.ExecutionStatus == TaskStatus.Running;
    }

    private void Update()
    {
      if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DisableBehaviourTree || InstanceByRequest<EngineApplication>.Instance.IsPaused && ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.PauseBehaviourTree)
        return;
      this.reduceUpdateFarObjects = ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.ReduceUpdateFarObjects;
      this.reduceUpdateFarObjectsDistance = ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.ReduceUpdateFarObjectsDistance;
      this.updater.Update();
    }

    public void ComputeUpdateItem(BehaviorTreeClient item)
    {
      if (this.reduceUpdateFarObjects && !DetectorUtility.CheckDistance(item.behavior.transform.position, EngineApplication.PlayerPosition, this.reduceUpdateFarObjectsDistance))
      {
        this.updateSkipped = !this.updateSkipped;
        if (this.updateSkipped)
          return;
      }
      if (Profiler.enabled)
        Profiler.BeginSample(item.behaviorName, (UnityEngine.Object) item.behavior.gameObject);
      this.Tick(item);
      if (!Profiler.enabled)
        return;
      Profiler.EndSample();
    }

    private void Tick(BehaviorTreeClient behaviorTree)
    {
      if ((UnityEngine.Object) behaviorTree.behavior == (UnityEngine.Object) null || !behaviorTree.behavior.isActiveAndEnabled)
        return;
      behaviorTree.executionCount = 0;
      this.ReevaluateParentTasks(behaviorTree);
      this.ReevaluateConditionalTasks(behaviorTree);
      for (int index = behaviorTree.activeStack.Count - 1; index > -1; --index)
      {
        TaskStatus status = TaskStatus.Inactive;
        int num1;
        if (index < behaviorTree.interruptionIndex.Count && (num1 = behaviorTree.interruptionIndex[index]) != -1)
        {
          behaviorTree.interruptionIndex[index] = -1;
          while (behaviorTree.activeStack[index].Peek() != num1)
          {
            int count = behaviorTree.activeStack[index].Count;
            this.PopTask(behaviorTree, behaviorTree.activeStack[index].Peek(), index, ref status, true);
            if (count == 1)
              break;
          }
          if (index < behaviorTree.activeStack.Count && behaviorTree.activeStack[index].Count > 0 && behaviorTree.taskList[num1] == behaviorTree.taskList[behaviorTree.activeStack[index].Peek()])
          {
            if (behaviorTree.taskList[num1] is ParentTask)
              status = (behaviorTree.taskList[num1] as ParentTask).OverrideStatus();
            this.PopTask(behaviorTree, num1, index, ref status, true);
          }
        }
        int num2 = -1;
        int taskIndex;
        for (; status != TaskStatus.Running && index < behaviorTree.activeStack.Count && behaviorTree.activeStack[index].Count > 0; status = this.RunTask(behaviorTree, taskIndex, index, status))
        {
          taskIndex = behaviorTree.activeStack[index].Peek();
          if ((index >= behaviorTree.activeStack.Count || behaviorTree.activeStack[index].Count <= 0 || num2 != behaviorTree.activeStack[index].Peek()) && this.IsBehaviorEnabled(behaviorTree.behavior))
            num2 = taskIndex;
          else
            break;
        }
      }
    }

    private void ReevaluateConditionalTasks(BehaviorTreeClient behaviorTree)
    {
      for (int index1 = 0; index1 < behaviorTree.conditionalReevaluate.Count; ++index1)
      {
        if (behaviorTree.conditionalReevaluate[index1].compositeIndex != -1)
        {
          int index2 = behaviorTree.conditionalReevaluate[index1].index;
          if (behaviorTree.taskList[index2].OnUpdate() != behaviorTree.conditionalReevaluate[index1].taskStatus)
          {
            int compositeIndex = behaviorTree.conditionalReevaluate[index1].compositeIndex;
            for (int index3 = behaviorTree.activeStack.Count - 1; index3 > -1; --index3)
            {
              if (behaviorTree.activeStack[index3].Count > 0)
              {
                int num = behaviorTree.activeStack[index3].Peek();
                int lca = this.FindLCA(behaviorTree, index2, num);
                if (this.IsChild(behaviorTree, lca, compositeIndex))
                {
                  for (int count = behaviorTree.activeStack.Count; num != -1 && num != lca && behaviorTree.activeStack.Count == count; num = behaviorTree.parentIndex[num])
                  {
                    TaskStatus status = TaskStatus.Failure;
                    this.PopTask(behaviorTree, num, index3, ref status, false);
                  }
                }
              }
            }
            for (int index4 = behaviorTree.conditionalReevaluate.Count - 1; index4 > index1 - 1; --index4)
            {
              ConditionalReevaluate conditionalReevaluate = behaviorTree.conditionalReevaluate[index4];
              if (this.FindLCA(behaviorTree, compositeIndex, conditionalReevaluate.index) == compositeIndex)
              {
                ObjectPool.Return<ConditionalReevaluate>(behaviorTree.conditionalReevaluate[index4]);
                behaviorTree.conditionalReevaluateMap.Remove(behaviorTree.conditionalReevaluate[index4].index);
                behaviorTree.conditionalReevaluate.RemoveAt(index4);
              }
            }
            Composite task1 = behaviorTree.taskList[behaviorTree.parentCompositeIndex[index2]] as Composite;
            for (int index5 = index1 - 1; index5 > -1; --index5)
            {
              ConditionalReevaluate conditionalReevaluate = behaviorTree.conditionalReevaluate[index5];
              if (task1.AbortType == AbortType.LowerPriority && behaviorTree.parentCompositeIndex[conditionalReevaluate.index] == behaviorTree.parentCompositeIndex[index2])
                behaviorTree.conditionalReevaluate[index5].compositeIndex = -1;
              else if (behaviorTree.parentCompositeIndex[conditionalReevaluate.index] == behaviorTree.parentCompositeIndex[index2])
              {
                for (int index6 = 0; index6 < behaviorTree.childrenIndex[compositeIndex].Count; ++index6)
                {
                  if (this.IsParentTask(behaviorTree, behaviorTree.childrenIndex[compositeIndex][index6], conditionalReevaluate.index))
                  {
                    int index7 = behaviorTree.childrenIndex[compositeIndex][index6];
                    while (!(behaviorTree.taskList[index7] is Composite) && behaviorTree.childrenIndex[index7] != null)
                      index7 = behaviorTree.childrenIndex[index7][0];
                    if (behaviorTree.taskList[index7] is Composite)
                    {
                      conditionalReevaluate.compositeIndex = index7;
                      break;
                    }
                    break;
                  }
                }
              }
            }
            this.conditionalParentIndexes.Clear();
            for (int index8 = behaviorTree.parentIndex[index2]; index8 != compositeIndex; index8 = behaviorTree.parentIndex[index8])
              this.conditionalParentIndexes.Add(index8);
            if (this.conditionalParentIndexes.Count == 0)
              this.conditionalParentIndexes.Add(behaviorTree.parentIndex[index2]);
            (behaviorTree.taskList[compositeIndex] as ParentTask).OnConditionalAbort(behaviorTree.relativeChildIndex[this.conditionalParentIndexes[this.conditionalParentIndexes.Count - 1]]);
            for (int index9 = this.conditionalParentIndexes.Count - 1; index9 > -1; --index9)
            {
              ParentTask task2 = behaviorTree.taskList[this.conditionalParentIndexes[index9]] as ParentTask;
              if (index9 == 0)
                task2.OnConditionalAbort(behaviorTree.relativeChildIndex[index2]);
              else
                task2.OnConditionalAbort(behaviorTree.relativeChildIndex[this.conditionalParentIndexes[index9 - 1]]);
            }
          }
        }
      }
    }

    private void ReevaluateParentTasks(BehaviorTreeClient behaviorTree)
    {
      for (int index = behaviorTree.parentReevaluate.Count - 1; index > -1; --index)
      {
        int num = behaviorTree.parentReevaluate[index];
        if (behaviorTree.taskList[num] is Decorator)
        {
          if (behaviorTree.taskList[num].OnUpdate() == TaskStatus.Failure)
            this.Interrupt(behaviorTree.behavior, behaviorTree.taskList[num]);
        }
        else if (behaviorTree.taskList[num] is Composite)
        {
          ParentTask task = behaviorTree.taskList[num] as ParentTask;
          if (task.OnReevaluationStarted())
          {
            int stackIndex = 0;
            TaskStatus status = this.RunParentTask(behaviorTree, num, ref stackIndex, TaskStatus.Inactive);
            task.OnReevaluationEnded(status);
          }
        }
      }
    }

    private TaskStatus RunTask(
      BehaviorTreeClient behaviorTree,
      int taskIndex,
      int stackIndex,
      TaskStatus previousStatus)
    {
      Task task1 = behaviorTree.taskList[taskIndex];
      if (task1 == null)
        return previousStatus;
      if (task1.Disabled)
      {
        if (behaviorTree.parentIndex[taskIndex] != -1)
        {
          ParentTask task2 = behaviorTree.taskList[behaviorTree.parentIndex[taskIndex]] as ParentTask;
          if (!task2.CanRunParallelChildren())
          {
            task2.OnChildExecuted(TaskStatus.Inactive);
          }
          else
          {
            task2.OnChildExecuted(behaviorTree.relativeChildIndex[taskIndex], TaskStatus.Inactive);
            this.RemoveStack(behaviorTree, stackIndex);
          }
        }
        return previousStatus;
      }
      TaskStatus status1 = previousStatus;
      if (!task1.IsInstant && (behaviorTree.nonInstantTaskStatus[stackIndex] == TaskStatus.Failure || behaviorTree.nonInstantTaskStatus[stackIndex] == TaskStatus.Success))
      {
        TaskStatus instantTaskStatu = behaviorTree.nonInstantTaskStatus[stackIndex];
        this.PopTask(behaviorTree, taskIndex, stackIndex, ref instantTaskStatu, true);
        return instantTaskStatu;
      }
      this.PushTask(behaviorTree, taskIndex, stackIndex);
      TaskStatus status2 = !(task1 is ParentTask) ? task1.OnUpdate() : (task1 as ParentTask).OverrideStatus(this.RunParentTask(behaviorTree, taskIndex, ref stackIndex, status1));
      if (status2 != TaskStatus.Running)
      {
        if (task1.IsInstant)
          this.PopTask(behaviorTree, taskIndex, stackIndex, ref status2, true);
        else
          behaviorTree.nonInstantTaskStatus[stackIndex] = status2;
      }
      return status2;
    }

    private TaskStatus RunParentTask(
      BehaviorTreeClient behaviorTree,
      int taskIndex,
      ref int stackIndex,
      TaskStatus status)
    {
      ParentTask task = behaviorTree.taskList[taskIndex] as ParentTask;
      if (!task.CanRunParallelChildren() || task.OverrideStatus(TaskStatus.Running) != TaskStatus.Running)
      {
        TaskStatus taskStatus = TaskStatus.Inactive;
        int num1 = stackIndex;
        int num2 = -1;
        List<int> intList;
        int num3;
        for (BehaviorTree behavior = behaviorTree.behavior; task.CanExecute() && (taskStatus != TaskStatus.Running || task.CanRunParallelChildren()) && this.IsBehaviorEnabled(behavior); status = taskStatus = this.RunTask(behaviorTree, intList[num3], stackIndex, status))
        {
          intList = behaviorTree.childrenIndex[taskIndex];
          num3 = task.CurrentChildIndex();
          if (num3 == num2)
          {
            status = TaskStatus.Running;
            break;
          }
          num2 = num3;
          if (task.CanRunParallelChildren())
          {
            behaviorTree.activeStack.Add(ObjectPool.Get<Stack<int>>());
            behaviorTree.interruptionIndex.Add(-1);
            behaviorTree.nonInstantTaskStatus.Add(TaskStatus.Inactive);
            stackIndex = behaviorTree.activeStack.Count - 1;
            task.OnChildStarted(num3);
          }
          else
            task.OnChildStarted();
        }
        stackIndex = num1;
      }
      return status;
    }

    private void PushTask(BehaviorTreeClient behaviorTree, int taskIndex, int stackIndex)
    {
      if (!this.IsBehaviorEnabled(behaviorTree.behavior) || stackIndex >= behaviorTree.activeStack.Count)
        return;
      Stack<int> active = behaviorTree.activeStack[stackIndex];
      if (active.Count != 0 && active.Peek() == taskIndex)
        return;
      active.Push(taskIndex);
      behaviorTree.nonInstantTaskStatus[stackIndex] = TaskStatus.Running;
      ++behaviorTree.executionCount;
      Task task = behaviorTree.taskList[taskIndex];
      task.OnStart();
      if (task is ParentTask && (task as ParentTask).CanReevaluate())
        behaviorTree.parentReevaluate.Add(taskIndex);
    }

    private void PopTask(
      BehaviorTreeClient behaviorTree,
      int taskIndex,
      int stackIndex,
      ref TaskStatus status,
      bool popChildren)
    {
      this.PopTask(behaviorTree, taskIndex, stackIndex, ref status, popChildren, true);
    }

    private void PopTask(
      BehaviorTreeClient behaviorTree,
      int taskIndex,
      int stackIndex,
      ref TaskStatus status,
      bool popChildren,
      bool notifyOnEmptyStack)
    {
      if (!this.IsBehaviorEnabled(behaviorTree.behavior) || stackIndex >= behaviorTree.activeStack.Count || behaviorTree.activeStack[stackIndex].Count == 0 || taskIndex != behaviorTree.activeStack[stackIndex].Peek())
        return;
      behaviorTree.activeStack[stackIndex].Pop();
      behaviorTree.nonInstantTaskStatus[stackIndex] = TaskStatus.Inactive;
      Task task1 = behaviorTree.taskList[taskIndex];
      task1.OnEnd();
      int index1 = behaviorTree.parentIndex[taskIndex];
      if (index1 != -1)
      {
        if (task1 is Conditional)
        {
          int index2 = behaviorTree.parentCompositeIndex[taskIndex];
          if (index2 != -1)
          {
            Composite task2 = behaviorTree.taskList[index2] as Composite;
            if (task2.AbortType != 0)
            {
              ConditionalReevaluate conditionalReevaluate1;
              if (behaviorTree.conditionalReevaluateMap.TryGetValue(taskIndex, out conditionalReevaluate1))
              {
                conditionalReevaluate1.compositeIndex = task2.AbortType != AbortType.LowerPriority ? index2 : -1;
                conditionalReevaluate1.taskStatus = status;
              }
              else
              {
                ConditionalReevaluate conditionalReevaluate2 = ObjectPool.Get<ConditionalReevaluate>();
                conditionalReevaluate2.Initialize(taskIndex, status, stackIndex, task2.AbortType != AbortType.LowerPriority ? index2 : -1);
                behaviorTree.conditionalReevaluate.Add(conditionalReevaluate2);
                behaviorTree.conditionalReevaluateMap.Add(taskIndex, conditionalReevaluate2);
              }
            }
          }
        }
        ParentTask task3 = behaviorTree.taskList[index1] as ParentTask;
        if (!task3.CanRunParallelChildren())
        {
          task3.OnChildExecuted(status);
          status = task3.Decorate(status);
        }
        else
          task3.OnChildExecuted(behaviorTree.relativeChildIndex[taskIndex], status);
      }
      if (task1 is ParentTask)
      {
        ParentTask parentTask = task1 as ParentTask;
        if (parentTask.CanReevaluate())
        {
          for (int index3 = behaviorTree.parentReevaluate.Count - 1; index3 > -1; --index3)
          {
            if (behaviorTree.parentReevaluate[index3] == taskIndex)
            {
              behaviorTree.parentReevaluate.RemoveAt(index3);
              break;
            }
          }
        }
        if (parentTask is Composite)
        {
          Composite composite = parentTask as Composite;
          if (composite.AbortType == AbortType.Self || composite.AbortType == AbortType.None || behaviorTree.activeStack[stackIndex].Count == 0)
            this.RemoveChildConditionalReevaluate(behaviorTree, taskIndex);
          else if (composite.AbortType == AbortType.LowerPriority || composite.AbortType == AbortType.Both)
          {
            int num = behaviorTree.parentCompositeIndex[taskIndex];
            if (num != -1)
            {
              if (!(behaviorTree.taskList[num] as ParentTask).CanRunParallelChildren())
              {
                for (int index4 = 0; index4 < behaviorTree.childConditionalIndex[taskIndex].Count; ++index4)
                {
                  int key = behaviorTree.childConditionalIndex[taskIndex][index4];
                  ConditionalReevaluate conditionalReevaluate3;
                  if (behaviorTree.conditionalReevaluateMap.TryGetValue(key, out conditionalReevaluate3))
                  {
                    if (!(behaviorTree.taskList[num] as ParentTask).CanRunParallelChildren())
                    {
                      conditionalReevaluate3.compositeIndex = behaviorTree.parentCompositeIndex[taskIndex];
                    }
                    else
                    {
                      for (int index5 = behaviorTree.conditionalReevaluate.Count - 1; index5 > index4 - 1; --index5)
                      {
                        ConditionalReevaluate conditionalReevaluate4 = behaviorTree.conditionalReevaluate[index5];
                        if (this.FindLCA(behaviorTree, num, conditionalReevaluate4.index) == num)
                        {
                          ObjectPool.Return<ConditionalReevaluate>(behaviorTree.conditionalReevaluate[index5]);
                          behaviorTree.conditionalReevaluateMap.Remove(behaviorTree.conditionalReevaluate[index5].index);
                          behaviorTree.conditionalReevaluate.RemoveAt(index5);
                        }
                      }
                    }
                  }
                }
              }
              else
                this.RemoveChildConditionalReevaluate(behaviorTree, taskIndex);
            }
            for (int index6 = 0; index6 < behaviorTree.conditionalReevaluate.Count; ++index6)
            {
              if (behaviorTree.conditionalReevaluate[index6].compositeIndex == taskIndex)
                behaviorTree.conditionalReevaluate[index6].compositeIndex = behaviorTree.parentCompositeIndex[taskIndex];
            }
          }
        }
      }
      if (popChildren)
      {
        for (int index7 = behaviorTree.activeStack.Count - 1; index7 > stackIndex; --index7)
        {
          if (behaviorTree.activeStack[index7].Count > 0 && this.IsParentTask(behaviorTree, taskIndex, behaviorTree.activeStack[index7].Peek()))
          {
            TaskStatus status1 = TaskStatus.Failure;
            for (int count = behaviorTree.activeStack[index7].Count; count > 0; --count)
              this.PopTask(behaviorTree, behaviorTree.activeStack[index7].Peek(), index7, ref status1, false, notifyOnEmptyStack);
          }
        }
      }
      if (stackIndex >= behaviorTree.activeStack.Count || behaviorTree.activeStack[stackIndex].Count != 0)
        return;
      if (stackIndex == 0)
      {
        if (notifyOnEmptyStack)
        {
          if (behaviorTree.behavior.RestartWhenComplete)
            this.Restart(behaviorTree);
          else
            this.DisableBehavior(behaviorTree.behavior, false, status);
        }
        status = TaskStatus.Inactive;
      }
      else
      {
        this.RemoveStack(behaviorTree, stackIndex);
        status = TaskStatus.Running;
      }
    }

    private void RemoveChildConditionalReevaluate(
      BehaviorTreeClient behaviorTree,
      int compositeIndex)
    {
      for (int index1 = behaviorTree.conditionalReevaluate.Count - 1; index1 > -1; --index1)
      {
        if (behaviorTree.conditionalReevaluate[index1].compositeIndex == compositeIndex)
        {
          ObjectPool.Return<ConditionalReevaluate>(behaviorTree.conditionalReevaluate[index1]);
          int index2 = behaviorTree.conditionalReevaluate[index1].index;
          behaviorTree.conditionalReevaluateMap.Remove(index2);
          behaviorTree.conditionalReevaluate.RemoveAt(index1);
        }
      }
    }

    private void Restart(BehaviorTreeClient behaviorTree)
    {
      this.RemoveChildConditionalReevaluate(behaviorTree, -1);
      if (behaviorTree.behavior.ResetValuesOnRestart)
        behaviorTree.behavior.SaveResetValues();
      for (int index = 0; index < behaviorTree.taskList.Count; ++index)
        behaviorTree.taskList[index].OnBehaviorRestart();
      behaviorTree.behavior.OnBehaviorRestarted();
      this.PushTask(behaviorTree, 0, 0);
    }

    private bool IsParentTask(
      BehaviorTreeClient behaviorTree,
      int possibleParent,
      int possibleChild)
    {
      int num;
      for (int index = possibleChild; index != -1; index = num)
      {
        num = behaviorTree.parentIndex[index];
        if (num == possibleParent)
          return true;
      }
      return false;
    }

    public void Interrupt(BehaviorTree behavior, Task task) => this.Interrupt(behavior, task, task);

    public void Interrupt(BehaviorTree behavior, Task task, Task interruptionTask)
    {
      if (!this.IsBehaviorEnabled(behavior))
        return;
      int num = -1;
      BehaviorTreeClient behaviorTree = this.behaviorTreeMap[behavior];
      for (int index = 0; index < behaviorTree.taskList.Count; ++index)
      {
        if (behaviorTree.taskList[index].ReferenceID == task.ReferenceID)
        {
          num = index;
          break;
        }
      }
      if (num <= -1)
        return;
      for (int index1 = 0; index1 < behaviorTree.activeStack.Count; ++index1)
      {
        if (behaviorTree.activeStack[index1].Count > 0)
        {
          for (int index2 = behaviorTree.activeStack[index1].Peek(); index2 != -1; index2 = behaviorTree.parentIndex[index2])
          {
            if (index2 == num)
            {
              behaviorTree.interruptionIndex[index1] = num;
              break;
            }
          }
        }
      }
    }

    private void RemoveStack(BehaviorTreeClient behaviorTree, int stackIndex)
    {
      Stack<int> active = behaviorTree.activeStack[stackIndex];
      active.Clear();
      ObjectPool.Return<Stack<int>>(active);
      behaviorTree.activeStack.RemoveAt(stackIndex);
      behaviorTree.interruptionIndex.RemoveAt(stackIndex);
      behaviorTree.nonInstantTaskStatus.RemoveAt(stackIndex);
    }

    private int FindLCA(BehaviorTreeClient behaviorTree, int taskIndex1, int taskIndex2)
    {
      HashSet<int> intSet = ObjectPool.Get<HashSet<int>>();
      intSet.Clear();
      for (int index = taskIndex1; index != -1; index = behaviorTree.parentIndex[index])
        intSet.Add(index);
      int index1 = taskIndex2;
      while (!intSet.Contains(index1))
        index1 = behaviorTree.parentIndex[index1];
      return index1;
    }

    private bool IsChild(BehaviorTreeClient behaviorTree, int taskIndex1, int taskIndex2)
    {
      for (int index = taskIndex1; index != -1; index = behaviorTree.parentIndex[index])
      {
        if (index == taskIndex2)
          return true;
      }
      return false;
    }

    public List<Task> GetActiveTasks(BehaviorTree behavior)
    {
      if (!this.IsBehaviorEnabled(behavior))
        return (List<Task>) null;
      List<Task> activeTasks = new List<Task>();
      BehaviorTreeClient behaviorTree = this.behaviorTreeMap[behavior];
      for (int index = 0; index < behaviorTree.activeStack.Count; ++index)
      {
        Task task = behaviorTree.taskList[behaviorTree.activeStack[index].Peek()];
        if (task is BehaviorDesigner.Runtime.Tasks.Action)
          activeTasks.Add(task);
      }
      return activeTasks;
    }

    private static Decimal RoundedTime()
    {
      return Math.Round((Decimal) Time.time, 5, MidpointRounding.AwayFromZero);
    }
  }
}
