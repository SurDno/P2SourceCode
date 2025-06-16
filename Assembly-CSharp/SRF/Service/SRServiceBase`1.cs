namespace SRF.Service
{
  public abstract class SRServiceBase<T> : SRMonoBehaviour where T : class
  {
    protected virtual void Awake() => SRServiceManager.RegisterService<T>(this);

    protected virtual void OnDestroy() => SRServiceManager.UnRegisterService<T>();
  }
}
