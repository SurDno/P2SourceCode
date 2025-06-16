using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ParadoxNotion.Services
{
  public class MessageRouter : 
    MonoBehaviour,
    IPointerEnterHandler,
    IEventSystemHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerClickHandler,
    IDragHandler,
    IScrollHandler,
    IUpdateSelectedHandler,
    ISelectHandler,
    IDeselectHandler,
    IMoveHandler,
    ISubmitHandler
  {
    private Dictionary<string, List<object>> listeners = new Dictionary<string, List<object>>(StringComparer.OrdinalIgnoreCase);

    public void OnPointerEnter(PointerEventData eventData)
    {
      Dispatch(nameof (OnPointerEnter), (object) eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      Dispatch(nameof (OnPointerExit), (object) eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
      Dispatch(nameof (OnPointerDown), (object) eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
      Dispatch(nameof (OnPointerUp), (object) eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
      Dispatch(nameof (OnPointerClick), (object) eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
      Dispatch(nameof (OnDrag), (object) eventData);
    }

    public void OnDrop(BaseEventData eventData)
    {
      Dispatch(nameof (OnDrop), (object) eventData);
    }

    public void OnScroll(PointerEventData eventData)
    {
      Dispatch(nameof (OnScroll), (object) eventData);
    }

    public void OnUpdateSelected(BaseEventData eventData)
    {
      Dispatch(nameof (OnUpdateSelected), (object) eventData);
    }

    public void OnSelect(BaseEventData eventData)
    {
      Dispatch(nameof (OnSelect), (object) eventData);
    }

    public void OnDeselect(BaseEventData eventData)
    {
      Dispatch(nameof (OnDeselect), (object) eventData);
    }

    public void OnMove(AxisEventData eventData)
    {
      Dispatch(nameof (OnMove), (object) eventData);
    }

    public void OnSubmit(BaseEventData eventData)
    {
      Dispatch(nameof (OnSubmit), (object) eventData);
    }

    private void OnAnimatorIK(int layerIndex)
    {
      Dispatch(nameof (OnAnimatorIK), layerIndex);
    }

    private void OnBecameInvisible() => Dispatch(nameof (OnBecameInvisible), null);

    private void OnBecameVisible() => Dispatch(nameof (OnBecameVisible), null);

    private void OnCollisionEnter(Collision collisionInfo)
    {
      Dispatch(nameof (OnCollisionEnter), (object) collisionInfo);
    }

    private void OnCollisionExit(Collision collisionInfo)
    {
      Dispatch(nameof (OnCollisionExit), (object) collisionInfo);
    }

    private void OnCollisionStay(Collision collisionInfo)
    {
      Dispatch(nameof (OnCollisionStay), (object) collisionInfo);
    }

    private void OnTriggerEnter(Collider other)
    {
      Dispatch(nameof (OnTriggerEnter), (object) other);
    }

    private void OnTriggerExit(Collider other)
    {
      Dispatch(nameof (OnTriggerExit), (object) other);
    }

    private void OnTriggerStay(Collider other)
    {
      Dispatch(nameof (OnTriggerStay), (object) other);
    }

    private void OnMouseDown() => Dispatch(nameof (OnMouseDown), null);

    private void OnMouseDrag() => Dispatch(nameof (OnMouseDrag), null);

    private void OnMouseEnter() => Dispatch(nameof (OnMouseEnter), null);

    private void OnMouseExit() => Dispatch(nameof (OnMouseExit), null);

    private void OnMouseOver() => Dispatch(nameof (OnMouseOver), null);

    private void OnMouseUp() => Dispatch(nameof (OnMouseUp), null);

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
      Dispatch(nameof (OnControllerColliderHit), (object) hit);
    }

    private void OnParticleCollision(GameObject other)
    {
      Dispatch(nameof (OnParticleCollision), (object) other);
    }

    public void OnCustomEvent(EventData eventData)
    {
      Dispatch(nameof (OnCustomEvent), eventData);
    }

    public void Register(object target, params string[] messages)
    {
      if (target == null)
        return;
      for (int index = 0; index < messages.Length; ++index)
      {
        if (target.GetType().RTGetMethod(messages[index]) == null)
        {
          Debug.LogError((object) string.Format("Type '{0}' does not implement a method named '{1}', for the registered event to use.", target.GetType().FriendlyName(), messages[index]));
        }
        else
        {
          List<object> objectList = null;
          if (!listeners.TryGetValue(messages[index], out objectList))
          {
            objectList = new List<object>();
            listeners[messages[index]] = objectList;
          }
          if (!objectList.Contains(target))
            objectList.Add(target);
        }
      }
    }

    public void RegisterCallback(string message, Action callback)
    {
      Internal_RegisterCallback(message, callback);
    }

    public void RegisterCallback<T>(string message, Action<T> callback)
    {
      Internal_RegisterCallback(message, callback);
    }

    private void Internal_RegisterCallback(string message, Delegate callback)
    {
      List<object> objectList = null;
      if (!listeners.TryGetValue(message, out objectList))
      {
        objectList = new List<object>();
        listeners[message] = objectList;
      }
      if (objectList.Contains(callback))
        return;
      objectList.Add(callback);
    }

    public void UnRegister(object target)
    {
      if (target == null)
        return;
      foreach (string key in listeners.Keys)
      {
        foreach (object obj in listeners[key].ToArray())
        {
          if (obj == target)
            listeners[key].Remove(target);
          else if (obj is Delegate && (obj as Delegate).Target == target)
            listeners[key].Remove(obj);
        }
      }
    }

    public void UnRegister(object target, params string[] messages)
    {
      if (target == null || messages == null)
        return;
      for (int index = 0; index < messages.Length; ++index)
      {
        string message = messages[index];
        if (listeners.ContainsKey(message))
        {
          foreach (object obj in listeners[message].ToArray())
          {
            if (obj == target)
              listeners[message].Remove(target);
            else if (obj is Delegate && (obj as Delegate).Target == target)
              listeners[message].Remove(obj);
          }
        }
      }
    }

    public void Dispatch(string message, object arg)
    {
      List<object> objectList;
      if (!listeners.TryGetValue(message, out objectList))
        return;
      for (int index = 0; index < objectList.Count; ++index)
      {
        object del = objectList[index];
        if (del != null)
        {
          MethodInfo methodInfo = !(del is Delegate) ? del.GetType().RTGetMethod(message) : (del as Delegate).RTGetDelegateMethodInfo();
          if (!(methodInfo == null))
          {
            object[] objArray;
            if (methodInfo.GetParameters().Length != 1)
              objArray = null;
            else
              objArray = new object[1]{ arg };
            object[] parameters = objArray;
            if (del is Delegate)
              (del as Delegate).DynamicInvoke(parameters);
            else if (methodInfo.ReturnType == typeof (IEnumerator))
              BlueprintManager.current.StartCoroutine((IEnumerator) methodInfo.Invoke(del, parameters));
            else
              methodInfo.Invoke(del, parameters);
          }
        }
      }
    }
  }
}
