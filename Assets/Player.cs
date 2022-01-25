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
    [SerializeField] private GameObject pfFartParticleSystem, pfFartGreaterRadiusParticleSystem;
    [SerializeField] private Transform worldContainer;
#endregion

#region Private Fields
    private static Player instance;
    private bool isHigherRadiusActivated = false;
#endregion

    private void OnEnable() {
        EventManager.Instance.StartListening(EventManager.Events.ButtonFartPressed, OnButtonFartPressed);
        EventManager.Instance.StartListening(EventManager.Events.ButtonRadiusPressed, OnButtonRadiusPressed);
        EventManager.Instance.StartListening(EventManager.Events.ButtonNumBeansNormalPressed, OnButtonNumBeansPressed);
    }
    private void OnDisable() {
        EventManager.Instance.StopListening(EventManager.Events.ButtonFartPressed, OnButtonFartPressed);
        EventManager.Instance.StopListening(EventManager.Events.ButtonRadiusPressed, OnButtonRadiusPressed);
        EventManager.Instance.StopListening(EventManager.Events.ButtonNumBeansNormalPressed, OnButtonNumBeansPressed);
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

        if(InventoryController.Instance.numBoostBeansRaw > 0){

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

            if(isHigherRadiusActivated){
                Instantiate(pfFartGreaterRadiusParticleSystem, worldContainer);
            }else{
                Instantiate(pfFartParticleSystem, worldContainer);
            }

            InventoryController.Instance.RemoveFarts(1);
        }else{
            AnnouncementsController.Instance.ShowAnnouncement("You are out of Beans", 2f);
        }
    }

    private void OnButtonRadiusPressed(){
        int currentCost = GameManager.Instance.GetCurrentHigherRadiusCost();
        int numBeansCurrency = InventoryController.Instance.numBeansCurrency;

        if(currentCost <= numBeansCurrency){
            InventoryController.Instance.RemoveBeansCurrency(currentCost);
            GameManager.Instance.SetCurrentHigherRadiusCost(currentCost * 2);
            isHigherRadiusActivated = true;
        }else{
            AnnouncementsController.Instance.ShowAnnouncement("Not enough beans", 2f);
            StoreController.Instance.ShowStore();
        }

    }

    private void OnButtonNumBeansPressed(){
        InventoryController.Instance.ShowInventory();
    }
    #endregion

    #region Public Methods

    #endregion
}