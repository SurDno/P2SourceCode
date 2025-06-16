using System;
using System.Collections;

public class CoroutineService : MonoBehaviour
{
  private static CoroutineService instance;

  public static CoroutineService Instance
  {
    get
    {
      if ((UnityEngine.Object) instance == (UnityEngine.Object) null)
        instance = new GameObject(typeof (CoroutineService).Name).AddComponent<CoroutineService>();
      return instance;
    }
  }

  public void Route(IEnumerator coroutine) => this.StartCoroutine(coroutine);

  public void WaitSecond(float delay, Action action)
  {
    this.StartCoroutine(WaitSecondRoute(delay, action));
  }

  public void WaitFrame(int count, Action action)
  {
    this.StartCoroutine(WaitFrameRoute(count, action));
  }

  public void WaitFrame(Action action) => this.StartCoroutine(WaitFrameRoute(1, action));

  private IEnumerator WaitFrameRoute(int count, Action action)
  {
    for (int index = 0; index < count; ++index)
      yield return null;
    try
    {
      action();
    }
    catch (Exception ex)
    {
      Debug.LogException(ex);
    }
  }

  private IEnumerator WaitSecondRoute(float delay, Action action)
  {
    yield return (object) new WaitForSeconds(delay);
    try
    {
      action();
    }
    catch (Exception ex)
    {
      Debug.LogException(ex);
    }
  }
}
