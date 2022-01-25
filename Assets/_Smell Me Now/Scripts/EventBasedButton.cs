using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class EventBasedButton : MonoBehaviour {

#region Private Serializable Fields
    [SerializeField] EventManager.Events eventToBeTriggered;
    [SerializeField] private bool sendThisAsGameObject = false;
#endregion

#region Private Fields
    private Button button;
#endregion

#region MonoBehaviour CallBacks
    void Awake() {
        button = GetComponent<Button>();
        if(button == null) {
            Debug.LogError($"{name} is missing a component");
        }

        if(sendThisAsGameObject){
            button.onClick.AddListener(OnButtonClickedAndSendGO);
        }else{
            button.onClick.AddListener(OnButtonClicked);
        }
    }
#endregion

#region Private Methods
    private void OnButtonClicked() {
        EventManager.Instance.TriggerEvent(eventToBeTriggered);
    }

    private void OnButtonClickedAndSendGO(){
        EventManager.Instance.TriggerEventWithGOParam(eventToBeTriggered, this.gameObject);
    }
#endregion

}