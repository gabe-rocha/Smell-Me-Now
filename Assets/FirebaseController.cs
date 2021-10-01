using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseController : MonoBehaviour {

#region Public Fields
    public struct FartData {
        public string uniqueIDGeneratedByFirestore;
        public string username;
        public string locationGeo;
        public float range;
        public long fartTimeInTicks;
        public long ticksToLive;
    }
#endregion

#region Private Serializable Fields

#endregion

#region Private Fields
    private FirebaseApp app;
    private bool firebaseIsUp = false;
    private FirebaseFirestore db;
    private bool refreshingFarts = false;

#endregion

#region MonoBehaviour CallBacks
    void Awake() {
        db = FirebaseFirestore.DefaultInstance;

    }

    void Start() {
        CheckAndFixDependencies();
    }

    private void CheckAndFixDependencies() {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if(dependencyStatus == Firebase.DependencyStatus.Available) {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                firebaseIsUp = true;
                Debug.Log("Firebase is Up");

            } else {
                UnityEngine.Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));

                // Firebase Unity SDK is not safe to use here.
                firebaseIsUp = false;
            }
        });
    }

    void Update() {
        if(firebaseIsUp && !refreshingFarts) {
            StartCoroutine(RefreshFartsList());
            refreshingFarts = true;
        }
    }
#endregion

#region Private Methods

    //this runs every 5 seconds
    private IEnumerator RefreshFartsList() {
        while (true) {
            int lat = int.Parse(MyLocationFollower.Instance.currentGeoLocation.Lat.ToString().Split('.')[0]);
            int lng = int.Parse(MyLocationFollower.Instance.currentGeoLocation.Lng.ToString().Split('.')[0]);
            GetFartsFromFirebase(lat, lng);
            yield return new WaitForSeconds(Data.fartListRefreshIntervalSecs);
        }
    }
#endregion

#region Public Methods
    public void SendFartDataToFirebase(FartData fartData) {

        string[] locationGeo = fartData.locationGeo.Split(',');
        string lat = locationGeo[0].Split('.')[0];
        string lng = locationGeo[1].Split('.')[0];

        CollectionReference colRef = db.Collection($"Lat{lat}").Document($"Lng{lng}").Collection("Farts");

        Dictionary<string, object> fart = new Dictionary<string, object> { { "username", fartData.username },
            { "locationGeo", fartData.locationGeo },
            { "range", fartData.range },
            { "ticksToLive", fartData.ticksToLive },
            { "fartTimeInTicks", fartData.fartTimeInTicks }
        };

        colRef.AddAsync(fart).ContinueWithOnMainThread(task => {
            DocumentReference addedDocRef = task.Result;
            Debug.Log($"Added document with ID: {addedDocRef.Id}");
            EventManager.Instance.TriggerEvent(EventManager.Events.FartDataSentToFirebase);
        });
    }

    public void GetFartsFromFirebase(int lat, int lng) {
        Query allFartsQuery = db.Collection($"Lat{lat}").Document($"Lng{lng}").Collection("Farts");

        allFartsQuery.GetSnapshotAsync().ContinueWithOnMainThread(task => {
            QuerySnapshot allFartsFromLatLng = task.Result;
            Debug.Log($"Number of Farts at {lat}, {lng}: {allFartsFromLatLng.Count}");

            List<FartData> listFarts = new List<FartData>();
            foreach (DocumentSnapshot fart in allFartsFromLatLng.Documents) {
                Dictionary<string, object> dicFart = fart.ToDictionary();

                FartData fartData = new FartData() {
                    uniqueIDGeneratedByFirestore = fart.Id,
                    username = dicFart["username"].ToString(),
                    locationGeo = dicFart["locationGeo"].ToString(),
                    range = int.Parse(dicFart["range"].ToString()),
                    ticksToLive = long.Parse(dicFart["ticksToLive"].ToString()),
                    fartTimeInTicks = long.Parse(dicFart["fartTimeInTicks"].ToString())
                };

                //Make sure old farts are removed from the database
                long timeNowInTicks = DateTime.UtcNow.Ticks;
                if(timeNowInTicks > fartData.fartTimeInTicks + fartData.ticksToLive) {
                    Debug.Log($"Old Fart!");
                    DocumentReference oldFartRef = db.Collection($"Lat{lat}").Document($"Lng{lng}").Collection("Farts").Document(fart.Id);
                    oldFartRef.DeleteAsync();
                } else {
                    Debug.Log($"Still Fresh!");
                    listFarts.Add(fartData);
                }
            }
            GameManager.Instance.listFarts = listFarts;
            EventManager.Instance.TriggerEvent(EventManager.Events.FartsListRefreshed);

        });
    }
#endregion
}