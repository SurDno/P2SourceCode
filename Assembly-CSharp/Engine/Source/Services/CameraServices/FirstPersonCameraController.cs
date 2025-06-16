using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services.Simulations;
using Engine.Source.Commons;
using Engine.Source.Settings;
using Engine.Source.Settings.External;
using InputServices;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Services.CameraServices
{
  public class FirstPersonCameraController : ICameraController
  {
    private float xRotationAngle;
    private float xPrev = 0.0f;
    private float yPrev = 0.0f;
    private Vector3 currentVelocity;
    private Vector3 currentPosition;
    private Simulation simulation;
    [Inspected]
    private IEntity player;

    public void Initialise()
    {
      this.simulation = ServiceLocator.GetService<Simulation>();
      this.currentPosition = Vector3.zero;
      this.currentVelocity = Vector3.zero;
      this.Update((IEntity) null, (GameObject) null);
    }

    public void Shutdown()
    {
      this.Update((IEntity) null, (GameObject) null);
      this.simulation = (Simulation) null;
    }

    public void Update(IEntity target, GameObject gameObjectTarget)
    {
      this.player = this.simulation.Player;
      if (this.player == null)
        return;
      IEntityView player = (IEntityView) this.player;
      if ((Object) player.GameObject == (Object) null)
        return;
      InputGameSetting instance = InstanceByRequest<InputGameSetting>.Instance;
      PivotPlayer component = player.GameObject.GetComponent<PivotPlayer>();
      if ((Object) component != (Object) null)
      {
        Transform transform = player.GameObject.transform;
        if (ExternalSettingsInstance<ExternalInputSettings>.Instance.SmoothMove)
        {
          Vector3 position = transform.position;
          if ((double) (position - this.currentPosition).magnitude > 1.0)
          {
            this.currentPosition = position;
            this.currentVelocity = Vector3.zero;
          }
          else
            this.currentPosition = Vector3.SmoothDamp(this.currentPosition, position, ref this.currentVelocity, ExternalSettingsInstance<ExternalInputSettings>.Instance.SmoothMoveTime);
          component.MoveRoot.transform.position = this.currentPosition;
        }
        Quaternion rotation = transform.rotation;
        float y = 0.0f;
        float num1 = 0.0f;
        if (!CursorService.Instance.Visible && !CursorService.Instance.Free)
        {
          float t = InputService.Instance.JoystickUsed ? instance.JoystickSensitivity.Value : instance.MouseSensitivity.Value;
          if (!InputService.Instance.JoystickUsed)
            t = Mathf.Lerp(ExternalSettingsInstance<ExternalInputSettings>.Instance.MinMouseSensitivity, ExternalSettingsInstance<ExternalInputSettings>.Instance.MaxMouseSensitivity, t);
          float num2 = t;
          float num3 = t;
          float axis1 = InputService.Instance.GetAxis("RightStickX");
          float num4 = (float) ((double) (this.xPrev + axis1) * (double) Time.deltaTime * (double) ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickSensitivity * (double) instance.JoystickSensitivity.Value * 2.0);
          this.xPrev = axis1;
          float axis2 = InputService.Instance.GetAxis("RightStickY");
          float num5 = (float) ((double) (this.yPrev + axis2) * (double) Time.deltaTime * (double) ExternalSettingsInstance<ExternalInputSettings>.Instance.JoystickSensitivity * (double) instance.JoystickSensitivity.Value * 2.0);
          this.yPrev = axis2;
          Vector2 vector2;
          vector2.x = Input.GetAxisRaw("MouseX");
          vector2.y = Input.GetAxisRaw("MouseY");
          y = vector2.x * num2 + num4;
          num1 = vector2.y * num3 - num5;
          if (ExternalSettingsInstance<ExternalInputSettings>.Instance.UseArrow)
          {
            float num6 = 2f;
            y += (float) ((Input.GetKey(KeyCode.LeftArrow) ? -(double) num6 : 0.0) + (Input.GetKey(KeyCode.RightArrow) ? (double) num6 : 0.0));
            num1 += (float) ((Input.GetKey(KeyCode.UpArrow) ? (double) num6 : 0.0) + (Input.GetKey(KeyCode.DownArrow) ? -(double) num6 : 0.0));
          }
        }
        Quaternion quaternion = rotation * Quaternion.Euler(0.0f, y, 0.0f);
        transform.localRotation = quaternion;
        if (InputService.Instance.JoystickUsed ? instance.JoystickInvert.Value : instance.MouseInvert.Value)
          this.xRotationAngle += num1;
        else
          this.xRotationAngle -= num1;
        this.xRotationAngle = Mathf.Clamp(this.xRotationAngle, -60f, 60f);
        component.CameraControllingBone.localEulerAngles = new Vector3(this.xRotationAngle, 0.0f, 0.0f);
        Transform cameraTransform = GameCamera.Instance.CameraTransform;
        cameraTransform.transform.position = component.AnimatedCameraBone.position;
        cameraTransform.transform.rotation = component.AnimatedCameraBone.rotation;
      }
      else
      {
        Vector3 position = player.GameObject.transform.position;
        Quaternion rotation = player.GameObject.transform.rotation;
        Transform cameraTransform = GameCamera.Instance.CameraTransform;
        cameraTransform.transform.localPosition = position;
        cameraTransform.transform.localRotation = rotation;
      }
    }
  }
}
