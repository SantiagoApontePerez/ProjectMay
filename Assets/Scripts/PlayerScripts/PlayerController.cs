using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Status;
using System;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {

    #region Player Variables *Changeable*
    #region Movement
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
    #region Torch
    [Header("Torch Variables")]
    [Tooltip("Battery current time in seconds")]
    [SerializeField]                    private float torchBatteryTimer;
    [Tooltip("Battery max time in seconds")]
    [SerializeField]                    private float torchBatteryCapacity = 100;
    [Tooltip("How quickly the torch battery deplets")]
    [SerializeField][Range(0.0f, 9.9f)] private float torchDepletionRate = 1;
    [Tooltip("How quickly the torch battery recharges")]
    [SerializeField][Range(0.0f, 9.9f)] private float torchRechargeRate = 1;
    #endregion
    #endregion

    #region Player Debug Variables
    [Header("Debug Values")]
    [SerializeField] private Vector3 playerVelocity;
    [SerializeField] private float velocityY = 0.0f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool torchToggle = false;
    [SerializeField] private bool canToggleTorch = true;
    #endregion

    #region Player Movement Variables *Unchangeable*
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

    [Header("UI Elements")]
    [SerializeField] private TMP_Text hudBatteryPercent;
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
        SetUpVars();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrounded();
        BStandardMovement();
        UpdateTorchSys();
        UpdateHUD();
    }

    private void SetupPlayer() {
        cController = (cController is null) ? gameObject.AddComponent<CharacterController>() : cController;
        cameraTransform = Camera.main.transform;
    }

    private void SetUpVars() {
        #region Battery Managment
        torchBatteryTimer = torchBatteryCapacity;
        #endregion
    }

    #region Movement System
    void CheckGrounded() {
        if(cController.isGrounded) {
            velocityY = 0f;
            isGrounded = cController.isGrounded; //Debug
        }
    }

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
        if (canToggleTorch == true) {
            if (torchToggle == false) {
                torchToggle = true;
            }else if(torchToggle == true) {
                torchToggle = false;
            }
        }
        Debug.Log("Toggle is " + torchToggle);
    }

    void UpdateTorchActive() {
        torchLight.SetActive(torchToggle);
    }

    void UpdateTorchSys() {
        TorchRotation();
        TorchBattery();
    }

    void TorchRotation() {
        Quaternion rotation = torchLight.transform.rotation;
        rotation.eulerAngles = cameraTransform.eulerAngles;

        torchLight.transform.rotation = rotation;
    }

    void TorchBattery() {
        UpdateTorchActive();
        //If Torch is On, Take away battery and if the torch reaches 0%
        if(torchToggle is true) {
            torchBatteryTimer -= Time.deltaTime * torchDepletionRate;
            torchBatteryTimer = Mathf.Clamp(torchBatteryTimer, 0, torchBatteryCapacity);
            if (torchBatteryTimer <= 0) {
                torchToggle = false;
            }
        }
        //If Torch is off, Recharge battery and stop at 100%
        else if(torchToggle is false) {
            torchBatteryTimer += Time.deltaTime * torchRechargeRate;
            torchBatteryTimer = Mathf.Clamp(torchBatteryTimer, 0, torchBatteryCapacity);
        }
        //Player can only toggle when the battery is bigger than 1
        canToggleTorch = torchBatteryTimer >= 1 ? true : false;
    }
    #endregion

    #region HUD
    /* !!MUST BE PLACED INTO GAME MANAGER!! */
    void UpdateHUD() {
        UpdateUIBattery();
    }

    public float UIBatteryPercentCalc { get { return torchBatteryTimer / torchBatteryCapacity * 100; } }

    public void UpdateUIBattery() {
        hudBatteryPercent.text = UIBatteryPercentCalc.ToString("#") + "%";
    }
    #endregion
}
