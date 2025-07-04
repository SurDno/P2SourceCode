﻿using System;
using UnityEngine;

namespace FirstPersonController
{
  [Serializable]
  public class MouseLook
  {
    public bool clampVerticalRotation = true;
    public bool lockCursor = true;
    private Quaternion m_CameraTargetRot;
    private Quaternion m_CharacterTargetRot;
    private bool m_cursorIsLocked = true;
    public float MaximumX = 90f;
    public float MinimumX = -90f;
    public bool smooth;
    public float smoothTime = 5f;
    public float XSensitivity = 2f;
    public float YSensitivity = 2f;

    public void Init(Transform character, Transform camera)
    {
      m_CharacterTargetRot = character.localRotation;
      m_CameraTargetRot = camera.localRotation;
    }

    public void LookRotation(Transform character, Transform camera)
    {
      float y = Input.GetAxis("MouseX") * XSensitivity;
      float num = Input.GetAxis("MouseY") * YSensitivity;
      m_CharacterTargetRot *= Quaternion.Euler(0.0f, y, 0.0f);
      m_CameraTargetRot *= Quaternion.Euler(-num, 0.0f, 0.0f);
      if (clampVerticalRotation)
        m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);
      if (smooth)
      {
        character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot, smoothTime * Time.deltaTime);
        camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot, smoothTime * Time.deltaTime);
      }
      else
      {
        character.localRotation = m_CharacterTargetRot;
        camera.localRotation = m_CameraTargetRot;
      }
      UpdateCursorLock();
    }

    public void SetCursorLock(bool value)
    {
      lockCursor = value;
      if (lockCursor)
        return;
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
    }

    public void UpdateCursorLock()
    {
      if (!lockCursor)
        return;
      InternalLockUpdate();
    }

    private void InternalLockUpdate()
    {
      if (Input.GetKeyUp(KeyCode.Escape))
        m_cursorIsLocked = false;
      else if (Input.GetMouseButtonUp(0))
        m_cursorIsLocked = true;
      if (m_cursorIsLocked)
      {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
      }
      else
      {
        if (m_cursorIsLocked)
          return;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
      }
    }

    private Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
      q.x /= q.w;
      q.y /= q.w;
      q.z /= q.w;
      q.w = 1f;
      float num = Mathf.Clamp(114.59156f * Mathf.Atan(q.x), MinimumX, MaximumX);
      q.x = Mathf.Tan((float) Math.PI / 360f * num);
      return q;
    }
  }
}
