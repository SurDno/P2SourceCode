using InputServices;
using System;
using UnityEngine;

public class GlyphSelectable : MonoBehaviour
{
  [SerializeField]
  private GameObject _selectedFrame;
  private RectTransform objectRect;
  private RectTransform imageRect;

  public bool IsSelected { get; private set; }

  public void SetSelected(bool selected)
  {
    this._selectedFrame.SetActive(selected);
    if (selected)
    {
      this._selectedFrame.transform.position = this.transform.position;
      this.imageRect.sizeDelta = (double) this.objectRect.sizeDelta.x > 100.0 ? new Vector2(120f, 120f) : new Vector2(110f, 100f);
    }
    this.IsSelected = selected;
  }

  private void OnEnable()
  {
    InputService.Instance.onJoystickUsedChanged += new Action<bool>(this.OnJoystick);
    this.objectRect = this.GetComponent<RectTransform>();
    this.imageRect = this.GetComponent<RectTransform>();
    this.SetSelected(false);
  }

  private void OnDisable()
  {
    InputService.Instance.onJoystickUsedChanged -= new Action<bool>(this.OnJoystick);
  }

  private void OnJoystick(bool joystick)
  {
    if (joystick)
    {
      if (!this.IsSelected)
        return;
      this.SetSelected(true);
    }
    else
      this.SetSelected(false);
  }
}
