using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

#region Public Fields
    public static Player Instance { get => instance; set => instance = value; }
#endregion

#region Private Serializable Fields
    [SerializeField] private GameObject pfFartParticleSystem;
    [SerializeField] private Transform worldContainer;
#endregion

#region Private Fields
    private static Player instance;
#endregion

    private void OnEnable() {
        EventManager.Instance.StartListening(EventManager.Events.ButtonFartPressed, OnButtonFartPressed);
    }
    private void OnDisable() {
        EventManager.Instance.StopListening(EventManager.Events.ButtonFartPressed, OnButtonFartPressed);
    }

#region MonoBehaviour CallBacks

    private void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(this);
        }
    }
#endregion

#region Private Methods
    private void OnButtonFartPressed() {
        // var curLocationVec3 = MyLocationFollower.Instance.currentVec3Location;
        // string locationVec3 = $"{curLocationVec3.x},{curLocationVec3.y},{curLocationVec3.z}";

        var curGeoLocation = MyLocationFollower.Instance.currentGeoLocation;
        string locationGeo = $"{curGeoLocation.Lat},{curGeoLocation.Lng}";

        FirebaseController.FartData fartData = new FirebaseController.FartData() {
            username = "gabe",
            locationGeo = locationGeo,
            range = 50,
            fartTimeInTicks = DateTime.UtcNow.Ticks,
            ticksToLive = TimeSpan.FromSeconds(600).Ticks
        };
        GameManager.Instance.SendFartDataToFirebase(fartData);
        Debug.Log($"Farted at {locationGeo}");
        Instantiate(pfFartParticleSystem, worldContainer);
    }
#endregion

#region Public Methods

#endregion
}