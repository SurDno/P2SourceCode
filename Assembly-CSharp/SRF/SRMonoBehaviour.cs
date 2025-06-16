// Decompiled with JetBrains decompiler
// Type: SRF.SRMonoBehaviour
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Diagnostics;
using UnityEngine;

#nullable disable
namespace SRF
{
  public abstract class SRMonoBehaviour : MonoBehaviour
  {
    private Collider _collider;
    private Transform _transform;
    private Rigidbody _rigidBody;
    private GameObject _gameObject;

    public Transform CachedTransform
    {
      [DebuggerStepThrough, DebuggerNonUserCode] get
      {
        if ((Object) this._transform == (Object) null)
          this._transform = base.transform;
        return this._transform;
      }
    }

    public Collider CachedCollider
    {
      [DebuggerStepThrough, DebuggerNonUserCode] get
      {
        if ((Object) this._collider == (Object) null)
          this._collider = this.GetComponent<Collider>();
        return this._collider;
      }
    }

    public Rigidbody CachedRigidBody
    {
      [DebuggerStepThrough, DebuggerNonUserCode] get
      {
        if ((Object) this._rigidBody == (Object) null)
          this._rigidBody = this.GetComponent<Rigidbody>();
        return this._rigidBody;
      }
    }

    public GameObject CachedGameObject
    {
      [DebuggerStepThrough, DebuggerNonUserCode] get
      {
        if ((Object) this._gameObject == (Object) null)
          this._gameObject = base.gameObject;
        return this._gameObject;
      }
    }

    public new Transform transform => this.CachedTransform;

    public Collider collider => this.CachedCollider;

    public Rigidbody rigidbody => this.CachedRigidBody;

    public new GameObject gameObject => this.CachedGameObject;

    [DebuggerNonUserCode]
    [DebuggerStepThrough]
    protected void AssertNotNull(object value, string fieldName = null)
    {
      SRDebugUtil.AssertNotNull(value, fieldName, (MonoBehaviour) this);
    }

    [DebuggerNonUserCode]
    [DebuggerStepThrough]
    protected void Assert(bool condition, string message = null)
    {
      SRDebugUtil.Assert(condition, message, (MonoBehaviour) this);
    }

    [Conditional("UNITY_EDITOR")]
    [DebuggerNonUserCode]
    [DebuggerStepThrough]
    protected void EditorAssertNotNull(object value, string fieldName = null)
    {
      this.AssertNotNull(value, fieldName);
    }

    [Conditional("UNITY_EDITOR")]
    [DebuggerNonUserCode]
    [DebuggerStepThrough]
    protected void EditorAssert(bool condition, string message = null)
    {
      this.Assert(condition, message);
    }
  }
}
