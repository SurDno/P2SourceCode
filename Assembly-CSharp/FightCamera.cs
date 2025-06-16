using Inspectors;
using UnityEngine;
using UnityEngine.PostProcessing;

public class FightCamera : MonoBehaviour
{
  private Camera camera;
  private PostProcessingBehaviour postProcessing;
  private KnifePostProcessEffect knifePostProcess;
  private float EulerK = 125f;
  private float EulerDump = 10f;
  private float currentEulerYOffset;
  private float currentEulerXOffset;
  private float velocityEulerY;
  private float velocityEulerX;
  private float initialFOV;
  private float currentFOVOffset;
  private float FOVK = 150f;
  private float FOVDump = 10f;
  private float FOVVelocity = 0.0f;
  private float currentVignetteOffset;
  private float vignetteK = 7f;
  private float vignetteDump = 2f;
  private float vignetteVelocity = 0.0f;
  private float currentBlackAndWhiteOffset;
  private float blackAndWhiteK = 3f;
  private float blackAndWhiteDump = 1.5f;
  private float BlackAndWhiteVelocity = 0.0f;
  private Vector3 localEulerAngles;
  private float playerPunchFOVAmplitude;
  private float playerPunchTimeLeft;
  private float playerPunchAmplitudeSpeed;
  private float playerPunchFOV;
  private VignetteModel.Settings vignetteSettings;

  public void PlayerPunchFOV(float amplitudeInDegrees, float timeOfMaximum)
  {
    this.playerPunchAmplitudeSpeed = amplitudeInDegrees / timeOfMaximum;
    this.playerPunchTimeLeft = timeOfMaximum;
    this.playerPunchFOV = 0.0f;
  }

  private void FixedUpdatePlayerPunchFOV()
  {
    this.playerPunchTimeLeft -= Time.fixedDeltaTime;
    if ((double) this.playerPunchTimeLeft > 0.0)
      this.playerPunchFOV += this.playerPunchAmplitudeSpeed * Time.fixedDeltaTime;
    else
      this.playerPunchFOV -= 2f * this.playerPunchAmplitudeSpeed * Time.fixedDeltaTime;
  }

  public void PushFOV(float strengthInDegrees) => this.currentFOVOffset = strengthInDegrees;

  public void PushVignette(float strengthFromZeroToOne)
  {
    this.currentVignetteOffset = strengthFromZeroToOne;
  }

  public void PushBlackAndWhite(float strengthFromZeroToOne)
  {
    this.currentBlackAndWhiteOffset = strengthFromZeroToOne;
  }

  public void PushDirectional(Vector3 direction, float strengthInDegrees)
  {
    direction.Normalize();
    this.currentEulerYOffset = strengthInDegrees * Vector3.Dot(direction, Vector3.right);
    this.currentEulerXOffset = strengthInDegrees * Vector3.Dot(direction, Vector3.forward);
  }

  public void PushKnifeStab(
    Vector3 speedInWorldSpace,
    Vector3 positionInWorldSpace,
    float strength)
  {
    this.knifePostProcess.AddScar(speedInWorldSpace, positionInWorldSpace, strength * 2f, (float) (0.75 + (double) strength * 1.25));
  }

  private void Start()
  {
    this.camera = this.GetComponent<Camera>();
    this.initialFOV = this.camera.fieldOfView;
    this.postProcessing = this.GetComponent<PostProcessingBehaviour>();
    this.knifePostProcess = this.GetComponent<KnifePostProcessEffect>();
  }

  private void LateUpdate()
  {
  }

  private void FixedUpdate()
  {
    this.FixedUpdatePlayerPunchFOV();
    this.UpdateCameraFOV();
    this.UpdateCameraVignette();
    this.UpdateCameraAngles();
    this.UpdateCameraBlackAndWhite();
    this.camera.fieldOfView = (float) ((double) this.initialFOV + (double) this.currentFOVOffset - ((double) this.playerPunchFOV > 0.0 ? (double) this.playerPunchFOV : 0.0));
  }

  [Inspected]
  public void TestVignettePush() => this.PushVignette(0.5f);

  [Inspected]
  public void TestFOVPush() => this.PushFOV(5f);

  [Inspected]
  public void TestDirectionalPush() => this.PushDirectional(new Vector3(1f, 1f, 0.0f), 30f);

  private void UpdateCameraFOV()
  {
    this.FOVVelocity += (float) (-(double) this.currentFOVOffset * (double) this.FOVK - (double) this.FOVVelocity * (double) this.FOVDump) * Time.fixedDeltaTime;
    this.currentFOVOffset += this.FOVVelocity * Time.fixedDeltaTime;
  }

  private void UpdateCameraVignette()
  {
    this.vignetteVelocity += (float) (-(double) this.currentVignetteOffset * (double) this.vignetteK - (double) this.vignetteVelocity * (double) this.vignetteDump) * Time.fixedDeltaTime;
    this.currentVignetteOffset += this.vignetteVelocity * Time.fixedDeltaTime;
    this.postProcessing.profile.vignette.settings = this.postProcessing.profile.vignette.settings with
    {
      color = Color.red,
      intensity = Mathf.Abs(this.currentVignetteOffset)
    };
  }

  private void UpdateCameraBlackAndWhite()
  {
    this.BlackAndWhiteVelocity += (float) (-(double) this.currentBlackAndWhiteOffset * (double) this.blackAndWhiteK - (double) this.BlackAndWhiteVelocity * (double) this.blackAndWhiteDump) * Time.fixedDeltaTime;
    this.currentBlackAndWhiteOffset += this.BlackAndWhiteVelocity * Time.fixedDeltaTime;
    ColorGradingModel.Settings settings = this.postProcessing.profile.colorGrading.settings;
    settings.basic.saturation = (float) (1.0 - 0.699999988079071 * (double) Mathf.Clamp01(this.currentBlackAndWhiteOffset));
    this.postProcessing.profile.colorGrading.settings = settings;
  }

  private void UpdateCameraAngles()
  {
    this.velocityEulerY += (float) (-(double) this.currentEulerYOffset * (double) this.EulerK - (double) this.velocityEulerY * (double) this.EulerDump) * Time.fixedDeltaTime;
    this.currentEulerYOffset += this.velocityEulerY * Time.fixedDeltaTime;
    this.velocityEulerX += (float) (-(double) this.currentEulerXOffset * (double) this.EulerK - (double) this.velocityEulerX * (double) this.EulerDump) * Time.fixedDeltaTime;
    this.currentEulerXOffset += this.velocityEulerX * Time.fixedDeltaTime;
    this.localEulerAngles = new Vector3(this.currentEulerXOffset, this.currentEulerYOffset, 0.0f);
  }
}
