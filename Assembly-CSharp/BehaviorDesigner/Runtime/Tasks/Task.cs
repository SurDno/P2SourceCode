// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.Task
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using System.Collections;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks
{
  public abstract class Task
  {
    protected GameObject gameObject;
    protected Transform transform;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    protected NodeData nodeData = (NodeData) null;
    private BehaviorTree owner = (BehaviorTree) null;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    protected int id = -1;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    protected string friendlyName = "";
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    protected bool instant = true;
    private int referenceID = -1;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
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
      this.Owner.StartTaskCoroutine(this, methodName);
    }

    protected Coroutine StartCoroutine(IEnumerator routine) => this.Owner.StartCoroutine(routine);

    protected Coroutine StartCoroutine(string methodName, object value)
    {
      return this.Owner.StartTaskCoroutine(this, methodName, value);
    }

    protected void StopCoroutine(string methodName) => this.Owner.StopTaskCoroutine(methodName);

    protected void StopCoroutine(IEnumerator routine) => this.Owner.StopCoroutine(routine);

    protected void StopAllCoroutines() => this.Owner.StopAllTaskCoroutines();

    public GameObject GameObject
    {
      set => this.gameObject = value;
    }

    public Transform Transform
    {
      set => this.transform = value;
    }

    protected T GetComponent<T>() where T : Component => this.gameObject.GetComponent<T>();

    protected Component GetComponent(System.Type type) => this.gameObject.GetComponent(type);

    protected GameObject GetDefaultGameObject(GameObject go)
    {
      return (UnityEngine.Object) go == (UnityEngine.Object) null ? this.gameObject : go;
    }

    public NodeData NodeData
    {
      get => this.nodeData;
      set => this.nodeData = value;
    }

    public BehaviorTree Owner
    {
      get => this.owner;
      set => this.owner = value;
    }

    public int Id
    {
      get => this.id;
      set => this.id = value;
    }

    public string FriendlyName
    {
      get => this.friendlyName;
      set => this.friendlyName = value;
    }

    public bool IsInstant
    {
      get => this.instant;
      set => this.instant = value;
    }

    public int ReferenceID
    {
      get => this.referenceID;
      set => this.referenceID = value;
    }

    public bool Disabled
    {
      get => this.disabled;
      set => this.disabled = value;
    }
  }
}
