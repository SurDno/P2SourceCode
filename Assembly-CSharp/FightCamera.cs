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
  private float FOVVelocity;
  private float currentVignetteOffset;
  private float vignetteK = 7f;
  private float vignetteDump = 2f;
  private float vignetteVelocity;
  private float currentBlackAndWhiteOffset;
  private float blackAndWhiteK = 3f;
  private float blackAndWhiteDump = 1.5f;
  private float BlackAndWhiteVelocity;
  private Vector3 localEulerAngles;
  private float playerPunchFOVAmplitude;
  private float playerPunchTimeLeft;
  private float playerPunchAmplitudeSpeed;
  private float playerPunchFOV;
  private VignetteModel.Settings vignetteSettings;

  public void PlayerPunchFOV(float amplitudeInDegrees, float timeOfMaximum)
  {
    playerPunchAmplitudeSpeed = amplitudeInDegrees / timeOfMaximum;
    playerPunchTimeLeft = timeOfMaximum;
    playerPunchFOV = 0.0f;
  }

  private void FixedUpdatePlayerPunchFOV()
  {
    playerPunchTimeLeft -= Time.fixedDeltaTime;
    if (playerPunchTimeLeft > 0.0)
      playerPunchFOV += playerPunchAmplitudeSpeed * Time.fixedDeltaTime;
    else
      playerPunchFOV -= 2f * playerPunchAmplitudeSpeed * Time.fixedDeltaTime;
  }

  public void PushFOV(float strengthInDegrees) => currentFOVOffset = strengthInDegrees;

  public void PushVignette(float strengthFromZeroToOne)
  {
    currentVignetteOffset = strengthFromZeroToOne;
  }

  public void PushBlackAndWhite(float strengthFromZeroToOne)
  {
    currentBlackAndWhiteOffset = strengthFromZeroToOne;
  }

  public void PushDirectional(Vector3 direction, float strengthInDegrees)
  {
    direction.Normalize();
    currentEulerYOffset = strengthInDegrees * Vector3.Dot(direction, Vector3.right);
    currentEulerXOffset = strengthInDegrees * Vector3.Dot(direction, Vector3.forward);
  }

  public void PushKnifeStab(
    Vector3 speedInWorldSpace,
    Vector3 positionInWorldSpace,
    float strength)
  {
    knifePostProcess.AddScar(speedInWorldSpace, positionInWorldSpace, strength * 2f, (float) (0.75 + strength * 1.25));
  }

  private void Start()
  {
    camera = GetComponent<Camera>();
    initialFOV = camera.fieldOfView;
    postProcessing = GetComponent<PostProcessingBehaviour>();
    knifePostProcess = GetComponent<KnifePostProcessEffect>();
  }

  private void LateUpdate()
  {
  }

  private void FixedUpdate()
  {
    FixedUpdatePlayerPunchFOV();
    UpdateCameraFOV();
    UpdateCameraVignette();
    UpdateCameraAngles();
    UpdateCameraBlackAndWhite();
    camera.fieldOfView = (float) (initialFOV + (double) currentFOVOffset - (playerPunchFOV > 0.0 ? playerPunchFOV : 0.0));
  }

  [Inspected]
  public void TestVignettePush() => PushVignette(0.5f);

  [Inspected]
  public void TestFOVPush() => PushFOV(5f);

  [Inspected]
  public void TestDirectionalPush() => PushDirectional(new Vector3(1f, 1f, 0.0f), 30f);

  private void UpdateCameraFOV()
  {
    FOVVelocity += (float) (-(double) currentFOVOffset * FOVK - FOVVelocity * (double) FOVDump) * Time.fixedDeltaTime;
    currentFOVOffset += FOVVelocity * Time.fixedDeltaTime;
  }

  private void UpdateCameraVignette()
  {
    vignetteVelocity += (float) (-(double) currentVignetteOffset * vignetteK - vignetteVelocity * (double) vignetteDump) * Time.fixedDeltaTime;
    currentVignetteOffset += vignetteVelocity * Time.fixedDeltaTime;
    postProcessing.profile.vignette.settings = postProcessing.profile.vignette.settings with
    {
      color = Color.red,
      intensity = Mathf.Abs(currentVignetteOffset)
    };
  }

  private void UpdateCameraBlackAndWhite()
  {
    BlackAndWhiteVelocity += (float) (-(double) currentBlackAndWhiteOffset * blackAndWhiteK - BlackAndWhiteVelocity * (double) blackAndWhiteDump) * Time.fixedDeltaTime;
    currentBlackAndWhiteOffset += BlackAndWhiteVelocity * Time.fixedDeltaTime;
    ColorGradingModel.Settings settings = postProcessing.profile.colorGrading.settings;
    settings.basic.saturation = (float) (1.0 - 0.699999988079071 * Mathf.Clamp01(currentBlackAndWhiteOffset));
    postProcessing.profile.colorGrading.settings = settings;
  }

  private void UpdateCameraAngles()
  {
    velocityEulerY += (float) (-(double) currentEulerYOffset * EulerK - velocityEulerY * (double) EulerDump) * Time.fixedDeltaTime;
    currentEulerYOffset += velocityEulerY * Time.fixedDeltaTime;
    velocityEulerX += (float) (-(double) currentEulerXOffset * EulerK - velocityEulerX * (double) EulerDump) * Time.fixedDeltaTime;
    currentEulerXOffset += velocityEulerX * Time.fixedDeltaTime;
    localEulerAngles = new Vector3(currentEulerXOffset, currentEulerYOffset, 0.0f);
  }
}
