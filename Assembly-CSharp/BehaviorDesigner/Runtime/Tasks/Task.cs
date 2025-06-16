using System;
using System.Collections;
using Engine.Common.Generator;

namespace BehaviorDesigner.Runtime.Tasks
{
  public abstract class Task
  {
    protected GameObject gameObject;
    protected Transform transform;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    protected NodeData nodeData;
    private BehaviorTree owner;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    protected int id = -1;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    protected string friendlyName = "";
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    protected bool instant = true;
    private int referenceID = -1;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    protected bool disabled;

    public virtual void OnAwake()
    {
    }

    public virtual void OnStart()
    {
    }

    public virtual TaskStatus OnUpdate() => TaskStatus.Success;

    public virtual void OnEnd()
    {
    }

    public virtual void OnPause(bool paused)
    {
    }

    public virtual float GetPriority() => 0.0f;

    public virtual float GetUtility() => 0.0f;

    public virtual void OnBehaviorRestart()
    {
    }

    public virtual void OnBehaviorComplete()
    {
    }

    public virtual void OnReset()
    {
    }

    public virtual void OnDrawGizmos()
    {
    }

    protected void StartCoroutine(string methodName)
    {
      Owner.StartTaskCoroutine(this, methodName);
    }

    protected Coroutine StartCoroutine(IEnumerator routine) => Owner.StartCoroutine(routine);

    protected Coroutine StartCoroutine(string methodName, object value)
    {
      return Owner.StartTaskCoroutine(this, methodName, value);
    }

    protected void StopCoroutine(string methodName) => Owner.StopTaskCoroutine(methodName);

    protected void StopCoroutine(IEnumerator routine) => Owner.StopCoroutine(routine);

    protected void StopAllCoroutines() => Owner.StopAllTaskCoroutines();

    public GameObject GameObject
    {
      set => gameObject = value;
    }

    public Transform Transform
    {
      set => transform = value;
    }

    protected T GetComponent<T>() where T : Component => gameObject.GetComponent<T>();

    protected Component GetComponent(Type type) => gameObject.GetComponent(type);

    protected GameObject GetDefaultGameObject(GameObject go)
    {
      return (UnityEngine.Object) go == (UnityEngine.Object) null ? gameObject : go;
    }

    public NodeData NodeData
    {
      get => nodeData;
      set => nodeData = value;
    }

    public BehaviorTree Owner
    {
      get => owner;
      set => owner = value;
    }

    public int Id
    {
      get => id;
      set => id = value;
    }

    public string FriendlyName
    {
      get => friendlyName;
      set => friendlyName = value;
    }

    public bool IsInstant
    {
      get => instant;
      set => instant = value;
    }

    public int ReferenceID
    {
      get => referenceID;
      set => referenceID = value;
    }

    public bool Disabled
    {
      get => disabled;
      set => disabled = value;
    }
  }
}
