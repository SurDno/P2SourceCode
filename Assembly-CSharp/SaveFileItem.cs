﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class SaveFileItem : MonoBehaviour
{
  public Action<SaveFileItem> OnPressed;

  public string File { get; set; }

  public void SetText(string text) => GetComponent<Text>().text = text;

  public void OnPress()
  {
    Action<SaveFileItem> onPressed = OnPressed;
    if (onPressed == null)
      return;
    onPressed(this);
  }
}
