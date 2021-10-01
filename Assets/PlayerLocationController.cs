using System.Collections;
using System.Collections.Generic;
using Google.Maps;
using Google.Maps.Coord;
using Google.Maps.Event;
using Google.Maps.Examples.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MapsService))]
public class PlayerLocationController : MonoBehaviour {

#region Public Fields
#endregion

#region Private Serializable Fields
    [SerializeField] private float offsetToRefreshMap = 5;
#endregion

#region Private Fields
    private Vector2 playerLocation, previousPlayerLocation;
    private LatLng playerLatLng;
    private MapsService mapsService;
#endregion

    private void OnEnable() {
        EventManager.Instance.StartListening(EventManager.Events.LocationServiceInitialized, OnLocationServiceInitialized);
    }
    private void OnDisable() {
        EventManager.Instance.StopListening(EventManager.Events.LocationServiceInitialized, OnLocationServiceInitialized);
    }

#region MonoBehaviour CallBacks

    /// <summary>
    /// Use <see cref="MapsService"/> to load geometry.
    /// </summary>
    private void OnLocationServiceInitialized() {

        StartCoroutine(RefreshUserLocationPeriodically());

        // Get required MapsService component on this GameObject.
        mapsService = GetComponent<MapsService>();
        // Set real-world location to load.
        mapsService.InitFloatingOrigin(playerLatLng);
        // Register a listener to be notified when the map is loaded.
        mapsService.Events.MapEvents.Loaded.AddListener(OnLoaded);
        // Load map with default options.
        mapsService.LoadMap(ExampleDefaults.DefaultBounds, ExampleDefaults.DefaultGameObjectOptions);

    }

    /// <summary>
    /// Example of OnLoaded event listener.
    /// </summary>
    /// <remarks>
    /// The communication between the game and the MapsSDK is done through APIs and event listeners.
    /// </remarks>
    public void OnLoaded(MapLoadedArgs args) {
        // The Map is loaded - you can start/resume gameplay from that point.
        // The new geometry is added under the GameObject that has MapsService as a component.

        Debug.Log("Map Loaded");
        EventManager.Instance.TriggerEvent(EventManager.Events.MapLoaded);
    }

    private void Update() {

    }

#endregion

#region Private Methods
    private IEnumerator RefreshUserLocationPeriodically() {

        while (true) {
            playerLocation = LocationServiceController.Instance.GetLatLng();
            playerLatLng = new LatLng(playerLocation.x, playerLocation.y);

            if(mapsService != null) {
                var offset = Vector2.Distance(playerLocation, previousPlayerLocation);
                Debug.Log($"Offset: {offset}");
                if(offset >= offsetToRefreshMap) {
                    previousPlayerLocation = playerLocation;
                    Camera.main.transform.position += mapsService.MoveFloatingOrigin(playerLatLng);
                }
            }

            // #if UNITY_EDITOR
            yield return new WaitForSeconds(0.1f);
            // #endif
            // yield return new WaitForSeconds(Data.locationRefreshInterval);
        }
    }

#endregion

#region Public Methods

#endregion
}