using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TouchCameraController : MonoBehaviour {

#region Public Fields

#endregion

#region Private Serializable Fields
    [SerializeField] private float cameraSensitivity = 6f;
#endregion

#region Private Fields

    Vector3 lastTouchPosition = Vector3.zero;
    Vector3 currentTouchPosition, dragDelta;
#endregion

#region MonoBehaviour CallBacks
    void Awake() {
        //component = GetComponent<Component>();
        //if(component == null) {
        //Debug.LogError($"{name} is missing a component");
        //}

    }

    void Start() {

    }

    void Update() {

        if(GameManager.Instance.gameState != GameManager.GameStates.Playing) {
            return;
        }

        if(Input.touchCount > 0) {
            if(Input.touches[0].phase == TouchPhase.Began) {
                lastTouchPosition = Input.touches[0].position;
            } else if(Input.touches[0].phase == TouchPhase.Moved) {

                currentTouchPosition = Input.touches[0].position;
                dragDelta = currentTouchPosition - lastTouchPosition;

                Camera.main.transform.RotateAround(transform.parent.position, Vector3.up, dragDelta.x * cameraSensitivity * Time.deltaTime);

                lastTouchPosition = currentTouchPosition;
            }
        }
    }
#endregion

#region Private Methods

#endregion

#region Public Methods

#endregion
}