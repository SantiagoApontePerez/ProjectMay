using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Status;
using System;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    #region Player Movement Variables
    [Header("Player Movement Variables")]
    [SerializeField][Range(00, 100)] private float MoveSpeedMultiplier = 5;
    [SerializeField][Range(50, 999)] private float JumpSpeedMultiplier = 60;
    #endregion

    #region Required Components
    [Header("Required Components")]
    [SerializeField] private InputReader _inputReader;
    public Rigidbody rb;
    #endregion

    private void Awake() {
        SetupPlayer();
    }

    private void OnEnable() {
        //_inputReader.LClick += DebugPrint;
        //_inputReader.RClick += DebugPrint;
        _inputReader.PJump  += OnJump;

    }

    private void OnDisable() {
        //_inputReader.LClick -= DebugPrint;
        //_inputReader.RClick -= DebugPrint;
        _inputReader.PJump  -= OnJump;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StandardMove();
    }

    private void SetupPlayer() {
        rb = (rb is null) ? GetComponent<Rigidbody>() : rb;
    }

    #region Movement Functions
    void StandardMove() {
        Vector3 pMovV3 = new Vector3(_inputReader.MovAxis.x, 0, _inputReader.MovAxis.y);
        transform.Translate(MoveSpeedMultiplier * Time.deltaTime * pMovV3);
    }

    void OnJump() {
        if(JumpStatus.IsGrounded(transform) && !JumpStatus.IsJumping(rb) && !JumpStatus.IsFalling(rb)) {
            rb.AddForce(rb.velocity.x, JumpSpeedMultiplier, rb.velocity.y);
            Debug.Log("Jump Initiated");
        }
        return;
    }
    #endregion
}
