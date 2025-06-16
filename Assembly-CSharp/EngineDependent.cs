using Cofe.Meta;
using Engine.Common.Services;
using Engine.Source.Commons;

public abstract class EngineDependent : MonoBehaviour
{
  private bool waiting;

  protected bool Connected { get; private set; }

  protected virtual void OnEnable()
  {
    if (InstanceByRequest<EngineApplication>.Instance.IsInitialized)
      Connect();
    else
      BeginWaiting();
  }

  private void Disconnect()
  {
    if (!Connected)
      return;
    Connected = false;
    OnDisconnectFromEngine();
  }

  private void Connect()
  {
    if (Connected)
      return;
    Connected = true;
    MetaService.GetContainer(GetType()).GetHandler(FromLocatorAttribute.Id).Compute(this, null);
    EndWaiting();
    OnConnectToEngine();
  }

  private void EndWaiting()
  {
    if (!waiting)
      return;
    waiting = false;
    InstanceByRequest<EngineApplication>.Instance.OnInitialized -= Connect;
  }

  private void BeginWaiting()
  {
    if (waiting)
      return;
    waiting = true;
    InstanceByRequest<EngineApplication>.Instance.OnInitialized += Connect;
  }

  protected virtual void OnDisable()
  {
    EndWaiting();
    Disconnect();
  }

  protected abstract void OnConnectToEngine();

  protected abstract void OnDisconnectFromEngine();
}
