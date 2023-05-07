using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;

    #region Variables
    [SerializeField][Range(0, 10)] private float MovSpeedMultiplier;
    #endregion

    private void OnEnable() {
        //_inputReader.LClick += DebugPrint;
        //_inputReader.RClick += DebugPrint;
        //_inputReader.PJump  += DebugPrint;

    }

    private void OnDisable() {
        //_inputReader.LClick -= DebugPrint;
        //_inputReader.RClick -= DebugPrint;
        //_inputReader.PJump  -= DebugPrint;
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

    void StandardMove() {
        Vector3 pMovV3 = new Vector3(_inputReader.MovAxis.x, 0, _inputReader.MovAxis.y);
        transform.Translate(MovSpeedMultiplier * Time.deltaTime * pMovV3);
    }
}
