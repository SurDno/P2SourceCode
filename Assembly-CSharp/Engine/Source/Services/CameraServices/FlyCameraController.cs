// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.CameraServices.FlyCameraController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Settings;
using Engine.Source.Settings.External;
using Engine.Source.Utility;
using InputServices;
using UnityEngine;

#nullable disable
namespace Engine.Source.Services.CameraServices
{
  public class FlyCameraController : ICameraController
  {
    private Vector3 rotation = Vector3.zero;
    private float xPrev = 0.0f;
    private float yPrev = 0.0f;

    public void Initialise()
    {
    }

    public void Shutdown()
    {
    }

    public void Update(IEntity target, GameObject gameObjectTarget)
    {
      if (CursorService.Instance.Visible || CursorService.Instance.Free)
        return;
      Transform cameraTransform = GameCamera.Instance.CameraTransform;
      InputGameSetting instance = InstanceByRequest<InputGameSetting>.Instance;
      float num1 = Mathf.Lerp(ExternalSettingsInstance<ExternalInputSettings>.Instance.MinMouseSensitivity, ExternalSettingsInstance<ExternalInputSettings>.Instance.MaxMouseSensitivity, instance.MouseSensitivity.Value);
      float num2 = num1;
      float num3 = instance.MouseInvert.Value ? -num1 : num1;
      float axis1 = InputService.Instance.GetAxis("RightStickX");
      float num4 = (this.xPrev + axis1) * Time.deltaTime * ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickSensitivity;
      this.xPrev = axis1;
      float axis2 = InputService.Instance.GetAxis("RightStickY");
      float num5 = (this.yPrev + axis2) * Time.deltaTime * ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickSensitivity;
      this.yPrev = axis2;
      Vector2 vector2;
      vector2.x = Input.GetAxisRaw("MouseX");
      vector2.y = Input.GetAxisRaw("MouseY");
      this.rotation.x += vector2.x * num2 * ExternalSettingsInstance<ExternalInputSettings>.Instance.FlySensitivity * Time.deltaTime + num4;
      this.rotation.y += vector2.y * num3 * ExternalSettingsInstance<ExternalInputSettings>.Instance.FlySensitivity * Time.deltaTime - num5;
      this.rotation.y = Mathf.Clamp(this.rotation.y, -90f, 90f);
      if (ExternalSettingsInstance<ExternalInputSettings>.Instance.UseArrow)
      {
        float num6 = 2f;
        this.rotation.x += (float) ((Input.GetKey(KeyCode.LeftArrow) ? -(double) num6 : 0.0) + (Input.GetKey(KeyCode.RightArrow) ? (double) num6 : 0.0));
        this.rotation.y += (float) ((Input.GetKey(KeyCode.UpArrow) ? (double) num6 : 0.0) + (Input.GetKey(KeyCode.DownArrow) ? -(double) num6 : 0.0));
      }
      cameraTransform.rotation = Quaternion.AngleAxis(this.rotation.x, Vector3.up);
      cameraTransform.rotation *= Quaternion.AngleAxis(this.rotation.y, Vector3.left);
      bool flag1 = Input.GetKey(KeyCode.W) || InputService.Instance.GetButton("Up LeftStick", false);
      bool flag2 = Input.GetKey(KeyCode.S) || InputService.Instance.GetButton("Down LeftStick", false);
      float num7 = (float) ((flag1 ? 1.0 : 0.0) + (flag2 ? -1.0 : 0.0));
      bool flag3 = Input.GetKey(KeyCode.D) || InputService.Instance.GetButton("Right LeftStick", false);
      bool flag4 = Input.GetKey(KeyCode.A) || InputService.Instance.GetButton("Left LeftStick", false);
      float num8 = (float) ((flag3 ? 1.0 : 0.0) + (flag4 ? -1.0 : 0.0));
      float num9 = !InputUtility.CheckModificators(KeyModifficator.Shift) ? ExternalSettingsInstance<ExternalInputSettings>.Instance.FlyWalkSpeed : ExternalSettingsInstance<ExternalInputSettings>.Instance.FlyRunSpeed;
      cameraTransform.position += cameraTransform.forward * num9 * num7 * Time.deltaTime;
      cameraTransform.position += cameraTransform.right * num9 * num8 * Time.deltaTime;
      if (Input.GetKey(KeyCode.Q))
        cameraTransform.position += cameraTransform.up * num9 * Time.deltaTime;
      if (!Input.GetKey(KeyCode.E))
        return;
      cameraTransform.position -= cameraTransform.up * num9 * Time.deltaTime;
    }
  }
}
