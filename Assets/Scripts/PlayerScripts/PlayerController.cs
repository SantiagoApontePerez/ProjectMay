using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Status;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {

    #region Player Movement Variables
    [Header("Player Movement Variables")]
    [SerializeField][Range(000, 099)] private float MoveSpeedMultiplier = 5;
    [SerializeField][Range(000, 099)] private float JumpHeight = 60;
    [SerializeField]                  private float gravityValue = -9.81f;
    #endregion

    #region Player General Variables
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    #endregion

    #region Required Components
    [Header("Required Components")]
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
        StandardMove();
    }

    private void SetupPlayer() {
        cController = (cController is null) ? gameObject.AddComponent<CharacterController>() : cController;
        cameraTransform = Camera.main.transform;
    }

    #region Movement Functions

    void CheckGrounded() {
        groundedPlayer = cController.isGrounded;
        if(groundedPlayer && playerVelocity.y < 0) {
            playerVelocity.y = 0f;
        }
    }

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

    void OnJump() {
        playerVelocity.y += Mathf.Sqrt(JumpHeight * -3.0f * gravityValue);
    }
    #endregion
}
