using System.Collections;
using System.Collections.Generic;
using Google.Maps;
using Google.Maps.Coord;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VisibleFartsController : MonoBehaviour {

#region Public Fields

#endregion

#region Private Serializable Fields
    [SerializeField] private GameObject pfFartCloud;
    [SerializeField] private Transform worldContainer;
#endregion

#region Private Fields
    private List<FirebaseController.FartData> listActiveFartsData = new List<FirebaseController.FartData>();
    private List<GameObject> listFartsGO = new List<GameObject>();
#endregion

    private void OnEnable() {
        EventManager.Instance.StartListening(EventManager.Events.FartsListRefreshed, OnFartsListRefreshed);
    }
    private void OnDisable() {
        EventManager.Instance.StopListening(EventManager.Events.FartsListRefreshed, OnFartsListRefreshed);
    }

#region MonoBehaviour CallBacks
#endregion

#region Private Methods
    private void OnFartsListRefreshed() {

        var refreshedList = GameManager.Instance.listFarts;

        if(listActiveFartsData == refreshedList) {
            //do nothing
        } else {
            //if we have old farts on-screen, destroy them
            for (var i = 0; i < listActiveFartsData.Count; i++) {
                var fart = listActiveFartsData[i];
                if(!refreshedList.Contains(fart)) {
                    var fartTransformToDestroy = worldContainer.Find(fart.uniqueIDGeneratedByFirestore);
                    Destroy(fartTransformToDestroy.gameObject);
                    listActiveFartsData.RemoveAt(i);
                }
            }

            //If we don't have the fart on-screen, create it
            for (var i = 0; i < refreshedList.Count; i++) {
                var fart = refreshedList[i];
                if(!listActiveFartsData.Contains(fart)) {
                    double lat = double.Parse(fart.locationGeo.Split(',')[0]);
                    double lng = double.Parse(fart.locationGeo.Split(',')[1]);
                    var locationGeo = new LatLng(lat, lng);
                    var locationVec3 = MyLocationFollower.Instance.MapsService.Projection.FromLatLngToVector3(locationGeo);
                    var fartGO = Instantiate(pfFartCloud, locationVec3, Quaternion.identity, worldContainer);
                    fartGO.name = fart.uniqueIDGeneratedByFirestore;
                }
            }
            listActiveFartsData = refreshedList;
        }
    }
#endregion

#region Public Methods

#endregion
}