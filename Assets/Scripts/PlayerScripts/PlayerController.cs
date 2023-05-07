using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;

    private void OnEnable() {
        _inputReader.LClick += DebugPrint;
        _inputReader.RClick += DebugPrint;
        _inputReader.PJump  += DebugPrint;

    }

    private void OnDisable() {
        _inputReader.LClick -= DebugPrint;
        _inputReader.RClick -= DebugPrint;
        _inputReader.PJump  -= DebugPrint;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DebugPrint() {
        Debug.Log("Button Registered");
        if(_inputReader.MovAxis.x > 0 || _inputReader.MovAxis.y > 0) {
            Debug.Log("Analog Sensed");
        }
    }
}
