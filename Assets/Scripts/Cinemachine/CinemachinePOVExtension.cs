using UnityEngine;
using Cinemachine;

public class CinemachinePOVExtension : CinemachineExtension {

    #region InputReader Instance
    [Header("Input Reader Required")]
    [SerializeField] private InputReader _inputReader;
    #endregion

    #region Clamps
    [Header("Clamps")]
    [SerializeField] private float clampAngle = 80f;
    [SerializeField] private float horizontalSpeed = 10f;
    [SerializeField] private float verticalSpeed = 10f;
    #endregion

    #region Variables
    private Vector3 startingRotation;
    #endregion

    protected override void Awake() {
        base.Awake();
    }
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime) {
        if (vcam.Follow) {
            if(stage == CinemachineCore.Stage.Aim) {
                if(startingRotation == null) {
                    startingRotation = transform.localRotation.eulerAngles;
                }
                Vector2 deltaInput = _inputReader.GetMouseDelta();
                startingRotation.x += deltaInput.x * verticalSpeed   * Time.deltaTime;
                startingRotation.y += deltaInput.y * horizontalSpeed * Time.deltaTime;
                startingRotation.y = Mathf.Clamp(startingRotation.y, -clampAngle, clampAngle);
                state.RawOrientation = Quaternion.Euler(-startingRotation.y, startingRotation.x, 0f);
            }
        }
    }
}
