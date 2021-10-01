using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocationServiceController : MonoBehaviour {

#region Public Fields
    public static LocationServiceController Instance { get => instance; private set { instance = value; } }
    private static LocationServiceController instance;
#endregion

#region Private Serializable Fields
    [SerializeField] private float mockMovementSpeed = 0.25f;
#endregion

#region Private Fields
    private Vector2 mockPlayerOriginalLocation;
#endregion

#region MonoBehaviour CallBacks

    private void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(this.gameObject);
        }
    }

    void Start() {
        mockPlayerOriginalLocation = new Vector2(30.223584429128632f, -95.50366895511574f);

        StartCoroutine(RefreshUserLocation());
    }

    private void Update() {
#if UNITY_EDITOR
        mockPlayerOriginalLocation += new Vector2(Input.GetAxis("Horizontal") * mockMovementSpeed * Time.deltaTime, Input.GetAxis("Vertical") * mockMovementSpeed * Time.deltaTime);
#endif
    }

#endregion

#region Private Methods
    private IEnumerator RefreshUserLocation() {

#if UNITY_EDITOR
        Debug.Log("Unity Editor, location not enabled. Will use mock data");
        EventManager.Instance.TriggerEvent(EventManager.Events.LocationServiceInitialized);
        yield break;
#endif 
        // First, check if user has location service enabled
        if(!Input.location.isEnabledByUser) {
            EventManager.Instance.TriggerEvent(EventManager.Events.LocationNotEnabledByPlayer);
            Debug.Log("Location Not Enabled By Player");
            yield break;
        }

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if(maxWait < 1) {
            print("Timed out");
            EventManager.Instance.TriggerEvent(EventManager.Events.LocationServiceDidntInitialize);
            yield break;
        }

        // Connection has failed
        if(Input.location.status == LocationServiceStatus.Failed) {
            EventManager.Instance.TriggerEvent(EventManager.Events.LocationServiceFailed);
            print("Unable to determine device location");
            yield break;
        } else {
            // Access granted and location value could be retrieved
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
            EventManager.Instance.TriggerEvent(EventManager.Events.LocationServiceInitialized);
        }
    }
#endregion

#region Public Methods
    public Vector2 GetLatLng() {

#if UNITY_EDITOR
        return mockPlayerOriginalLocation;
#endif
        return new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
    }
#endregion
}