using System.Diagnostics;
using UnityEngine;

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
        if (_transform == null)
          _transform = base.transform;
        return _transform;
      }
    }

    public Collider CachedCollider
    {
      [DebuggerStepThrough, DebuggerNonUserCode] get
      {
        if (_collider == null)
          _collider = GetComponent<Collider>();
        return _collider;
      }
    }

    public Rigidbody CachedRigidBody
    {
      [DebuggerStepThrough, DebuggerNonUserCode] get
      {
        if (_rigidBody == null)
          _rigidBody = GetComponent<Rigidbody>();
        return _rigidBody;
      }
    }

    public GameObject CachedGameObject
    {
      [DebuggerStepThrough, DebuggerNonUserCode] get
      {
        if (_gameObject == null)
          _gameObject = base.gameObject;
        return _gameObject;
      }
    }

    public new Transform transform => CachedTransform;

    public Collider collider => CachedCollider;

    public Rigidbody rigidbody => CachedRigidBody;

    public new GameObject gameObject => CachedGameObject;

    [DebuggerNonUserCode]
    [DebuggerStepThrough]
    protected void AssertNotNull(object value, string fieldName = null)
    {
      SRDebugUtil.AssertNotNull(value, fieldName, this);
    }

    [DebuggerNonUserCode]
    [DebuggerStepThrough]
    protected void Assert(bool condition, string message = null)
    {
      SRDebugUtil.Assert(condition, message, this);
    }

    [Conditional("UNITY_EDITOR")]
    [DebuggerNonUserCode]
    [DebuggerStepThrough]
    protected void EditorAssertNotNull(object value, string fieldName = null)
    {
      AssertNotNull(value, fieldName);
    }

    [Conditional("UNITY_EDITOR")]
    [DebuggerNonUserCode]
    [DebuggerStepThrough]
    protected void EditorAssert(bool condition, string message = null)
    {
      Assert(condition, message);
    }
  }
}
