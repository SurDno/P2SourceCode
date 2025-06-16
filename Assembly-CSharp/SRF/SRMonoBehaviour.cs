using System.Diagnostics;

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
        if ((Object) _transform == (Object) null)
          _transform = base.transform;
        return _transform;
      }
    }

    public Collider CachedCollider
    {
      [DebuggerStepThrough, DebuggerNonUserCode] get
      {
        if ((Object) _collider == (Object) null)
          _collider = this.GetComponent<Collider>();
        return _collider;
      }
    }

    public Rigidbody CachedRigidBody
    {
      [DebuggerStepThrough, DebuggerNonUserCode] get
      {
        if ((Object) _rigidBody == (Object) null)
          _rigidBody = this.GetComponent<Rigidbody>();
        return _rigidBody;
      }
    }

    public GameObject CachedGameObject
    {
      [DebuggerStepThrough, DebuggerNonUserCode] get
      {
        if ((Object) _gameObject == (Object) null)
          _gameObject = base.gameObject;
        return _gameObject;
      }
    }

    public Transform transform => CachedTransform;

    public Collider collider => CachedCollider;

    public Rigidbody rigidbody => CachedRigidBody;

    public GameObject gameObject => CachedGameObject;

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
