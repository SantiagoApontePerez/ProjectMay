using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu (fileName = "Input Reader", menuName = "InputReader/Input")]
public class InputReader : ScriptableObject, ThisPlay.IPlayerActions {

    #region Reader Setup
    private ThisPlay _controls;

    private void OnEnable() {
        SetUpControlls();
        _controls.Player.Enable();
    }

    private void OnDisable() {
        _controls.Player.Enable();
    }

    private void SetUpControlls() {
        _controls = (_controls is null) ? new ThisPlay() : _controls;
        _controls.Player.SetCallbacks(this);
    }
    #endregion

    #region IPlayerActions *In Game*

    public event Action LClick = delegate { };
    public event Action RClick = delegate { };
    public event Action PJump  = delegate { };
    public Vector2 MovAxis { get; private set; }
    public Vector2 camAxis { get; private set; }
    public bool DidJump { get; private set; }

    public void OnJump(InputAction.CallbackContext context) {
        if (context.performed) {
            PJump?.Invoke();
            DidJump = true;
        }
        if (context.canceled) {
            DidJump = false;
        }
    }

    public void OnMouseLeft(InputAction.CallbackContext context) {
        LClick?.Invoke();
    }

    public void OnMouseRight(InputAction.CallbackContext context) {
        RClick?.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context) {
        this.MovAxis = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context) {
        this.camAxis = context.ReadValue<Vector2>();
    }

    public Vector2 GetMouseDelta() {
        return _controls.Player.Look.ReadValue<Vector2>();
    }
    #endregion
}
