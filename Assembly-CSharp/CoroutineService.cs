using System;
using System.Collections;
using UnityEngine;

public class CoroutineService : MonoBehaviour
{
  private static CoroutineService instance;

  public static CoroutineService Instance
  {
    get
    {
      if (instance == null)
        instance = new GameObject(typeof (CoroutineService).Name).AddComponent<CoroutineService>();
      return instance;
    }
  }

  public void Route(IEnumerator coroutine) => StartCoroutine(coroutine);

  public void WaitSecond(float delay, Action action)
  {
    StartCoroutine(WaitSecondRoute(delay, action));
  }

  public void WaitFrame(int count, Action action)
  {
    StartCoroutine(WaitFrameRoute(count, action));
  }

  public void WaitFrame(Action action) => StartCoroutine(WaitFrameRoute(1, action));

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
    yield return new WaitForSeconds(delay);
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
