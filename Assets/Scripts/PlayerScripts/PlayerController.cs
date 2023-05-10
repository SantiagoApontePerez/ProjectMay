using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Status;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {

    #region Player Movement Variables
    [Header("Player Movement Variables")]
    [Tooltip("Default 5")]
    [SerializeField][Range(000f, 099f)] private float MoveSpeedMultiplier = 5;
    [Tooltip("Default 60")]
    [SerializeField][Range(000f, 099f)] private float jumpHeight = 60;
    [SerializeField]                    private float gravityValue = -9.81f;
    [SerializeField][Range(0.0f, 0.5f)] private float moveSmoothTime = 0.3f;
    #endregion

    #region Player General Variables
    private Vector3 playerVelocity;
    [SerializeField] private float velocityY = 0.0f;
    private bool    groundedPlayer;
    #endregion

    #region Player Movement Variables
    Vector2 currentDir    = Vector2.zero;
    Vector2 currentDirVel = Vector2.zero;
    #endregion

    #region Required Components
    [Header("Required Components")]
    [Tooltip("Please place a valid input reader")]
    [SerializeField] private InputReader _inputReader;
    public CharacterController cController;
    #endregion

    #region Required Transforms
    [Header("Required Transforms")]
    public Transform cameraTransform;
    #endregion

    private void Awake() {
        SetupPlayer();
    }

    private void OnEnable() {
        //_inputReader.LClick += DebugPrint;
        //_inputReader.RClick += DebugPrint;
        //_inputReader.PJump  += OnJump;

    }

    private void OnDisable() {
        //_inputReader.LClick -= DebugPrint;
        //_inputReader.RClick -= DebugPrint;
        //_inputReader.PJump  -= OnJump;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrounded();
        BStandardMovement();
    }

    private void SetupPlayer() {
        cController = (cController is null) ? gameObject.AddComponent<CharacterController>() : cController;
        cameraTransform = Camera.main.transform;
    }

    #region Movement System
    void CheckGrounded() {
        if(cController.isGrounded) {
            velocityY = 0f;
        }
    }

    #region Unused Movement Function
    void StandardMove() {
        Vector3 pMovV3 = new Vector3(_inputReader.MovAxis.x, 0f, _inputReader.MovAxis.y);
        pMovV3 = cameraTransform.forward * pMovV3.z + cameraTransform.right * pMovV3.x;
        pMovV3.y = 0f;
        cController.Move(pMovV3.normalized * Time.deltaTime * MoveSpeedMultiplier);

        if(_inputReader.DidJump && groundedPlayer) {
            OnJump();
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        cController.Move(playerVelocity * Time.deltaTime);
    }
    #endregion

    void BStandardMovement() {
        Vector2 targetDir = new Vector2(_inputReader.MovAxis.x, _inputReader.MovAxis.y);
        targetDir.Normalize();

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVel, moveSmoothTime);

        CheckGrounded();

        velocityY += gravityValue * Time.deltaTime;

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * MoveSpeedMultiplier + Vector3.up * velocityY;
        velocity = cameraTransform.forward * velocity.z + cameraTransform.right * velocity.x;

        cController.Move(velocity * Time.deltaTime);

        if (_inputReader.DidJump && cController.isGrounded && !JumpStatus.IsFallingCC(cController)) {
            OnJump();
        }

        cController.Move(JumpCalc() * Time.deltaTime);
    }

    #region Jump System
    void OnJump() {
        velocityY += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    }

    Vector3 JumpCalc() {
        return new Vector3(0, velocityY, 0);
    }
    #endregion
    #endregion
}
