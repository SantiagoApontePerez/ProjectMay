using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Status;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {

    #region Player Movement Variables *Changeable*
    [Header("Player Movement Variables")]
    [Tooltip("Default 5")]
    [SerializeField][Range(000f, 099f)] private float MoveSpeedMultiplier = 5;
    [Tooltip("Default 3")]
    [SerializeField][Range(000f, 010f)] private float jumpHeight = 1;
    [Tooltip("Gravity default 9.81m/s")]
    [SerializeField]                    private float gravityValue = -9.81f;
    [Tooltip("Movement smoothing *linear*")]
    [SerializeField][Range(0.0f, 0.5f)] private float moveSmoothTime = 0.1f;
    #endregion

    #region Player Debug Variables
    [Header("Debug Values")]
    [SerializeField] private Vector3 playerVelocity;
    [SerializeField] private float velocityY = 0.0f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool torchToggle = false;
    #endregion

    #region Player Movement Variables
    Vector2 currentDir    = Vector2.zero;
    Vector2 currentDirVel = Vector2.zero;
    #endregion

    #region Required Components
    [Header("Required Components")]
    [Tooltip("Please place a valid input reader")]
    [SerializeField] private InputReader _inputReader;
    [Tooltip("Please place a valid character controller")]
    [SerializeField] private CharacterController cController;
    #endregion

    #region Required Transforms
    [Header("Required Transforms")]
    [Tooltip("This is automatically fetched on runtime")]
    public Transform cameraTransform;
    #endregion

    #region Required GameObjects
    [Header("Required GameObjects")]
    [SerializeField] private GameObject torchLight;
    #endregion

    private void Awake() {
        SetupPlayer();
    }

    private void OnEnable() {
        //_inputReader.LClick += DebugPrint;
        //_inputReader.RClick += DebugPrint;
        //_inputReader.PJump  += OnJump;
        _inputReader.TogTorch += ToggleTorch;

    }

    private void OnDisable() {
        //_inputReader.LClick -= DebugPrint;
        //_inputReader.RClick -= DebugPrint;
        //_inputReader.PJump  -= OnJump;
        _inputReader.TogTorch -= ToggleTorch;
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
            isGrounded = cController.isGrounded; //Debug
        }
    }

    #region Unused Movement Function
    void StandardMove() {
        Vector3 pMovV3 = new Vector3(_inputReader.MovAxis.x, 0f, _inputReader.MovAxis.y);
        pMovV3 = cameraTransform.forward * pMovV3.z + cameraTransform.right * pMovV3.x;
        pMovV3.y = 0f;
        cController.Move(pMovV3.normalized * Time.deltaTime * MoveSpeedMultiplier);

        if(_inputReader.DidJump && isGrounded) {
            AddJumpVel();
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
            AddJumpVel();
        }

        cController.Move(JumpCalc() * Time.deltaTime);
    }

    #region Jump System
    void AddJumpVel() {
        velocityY += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    }

    Vector3 JumpCalc() {
        return new Vector3(0, velocityY, 0);
    }
    #endregion
    #endregion

    #region Torch System
    void ToggleTorch() {
        if(torchToggle == false) {
            torchToggle = true;
            UpdateTorch();
            return;
        }
        else if(torchToggle == true) {
            torchToggle = false;
            UpdateTorch();
            return;
        }

        Debug.Log("Toggle is " + torchToggle);
    }

    void UpdateTorch() {
        torchLight.SetActive(torchToggle);
    }

    #endregion
}
