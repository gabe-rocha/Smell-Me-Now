using System.Collections;
using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif
using Google.Maps;
using Google.Maps.Coord;
using Google.Maps.Examples.Shared;

/// <summary>
/// Example showing how to have the player's real-world GPS location reflected by on-screen
/// movements.
/// </summary>
/// <remarks>
/// Uses <see cref="ErrorHandling"/> component to display any errors encountered by the
/// <see cref="MapsService"/> component when loading geometry.
/// </remarks>
[RequireComponent(typeof(MapsService), typeof(ErrorHandling))]
public class MyLocationFollower : MonoBehaviour {

  public static MyLocationFollower Instance { get => instance; private set { instance = value; } }
  private static MyLocationFollower instance;

  [SerializeField] private float mockMovementSpeed = 2f;
  [SerializeField] private float mockRotationSpeed = 30f;
  [SerializeField] private float testHeading = 45;
  [SerializeField] private float curLat, curLng;
  [SerializeField] private GameObject playerGO;
  [SerializeField] private GameObject playerModel;
  [SerializeField] private Animator animator;

  // Dialog for location services request.
#if PLATFORM_ANDROID
  GameObject dialog = null;
#endif

  /// <summary>
  /// The maps service.
  /// </summary>
  internal MapsService MapsService;

  /// <summary>
  /// The player's last recorded real-world location and the the current floating origin.
  /// </summary>
  private LatLng PreviousLocation;
  private Vector3 mockPlayerOriginalLocation;
  internal LatLng currentGeoLocation;
  internal Vector3 currentVec3Location;

  private void Awake() {
    if(instance == null) {
      instance = this;
    } else {
      Destroy(this);
    }
  }

  /// <summary>Start following player's real-world location.</summary>
  private void Start() {
    GetPermissions();
    StartCoroutine(Follow());
  }

  /// <summary>
  /// Follow player's real-world location, as derived from the device's GPS signal.
  /// </summary>
  private IEnumerator Follow() {

#if !UNITY_EDITOR

    // If location is allowed by the user, start the location service and compass, otherwise abort
    // the coroutine.
    // #if PLATFORM_IOS
    // The location permissions request in IOS does not seem to get invoked until it is called for
    // in the code. It happens at runtime so if the code is not trying to access the location
    // right away, it will not pop up the permissions dialog.
    Input.location.Start();
    // #endif

    while (!Input.location.isEnabledByUser) {
      Debug.Log("Waiting for location services to become enabled..");
      yield return new WaitForSeconds(1f);
    }
    Debug.Log("Location services is enabled.");

    // #if !PLATFORM_IOS
    // Input.location.Start();
    // #endif

    Input.compass.enabled = true;

    // Wait for the location service to start.
    while (true) {
      if(Input.location.status == LocationServiceStatus.Initializing) {
        // Starting, just wait.
        yield return new WaitForSeconds(1f);
      } else if(Input.location.status == LocationServiceStatus.Failed) {
        // Failed, abort the coroutine.
        Debug.LogError("Location Services failed to start.");

        yield break;
      } else if(Input.location.status == LocationServiceStatus.Running) {
        // Started, continue the coroutine.
        break;
      }
    }
    PreviousLocation = new LatLng(Input.location.lastData.latitude, Input.location.lastData.longitude);
    MapsService = GetComponent<MapsService>();
    MapsService.InitFloatingOrigin(PreviousLocation);
    MapsService.LoadMap(ExampleDefaults.DefaultBounds, ExampleDefaults.DefaultGameObjectOptions);

#else    

    //UNITY EDITOR
    // Get the MapsService component and load it at the device location.
    mockPlayerOriginalLocation = new Vector3(curLat, 0, curLng);

    PreviousLocation = new LatLng(mockPlayerOriginalLocation.x, mockPlayerOriginalLocation.z);
    MapsService = GetComponent<MapsService>();
    MapsService.InitFloatingOrigin(PreviousLocation);
    MapsService.LoadMap(ExampleDefaults.DefaultBounds, ExampleDefaults.DefaultGameObjectOptions);

    yield return null;
#endif
  }

  /// <summary>
  /// Moves the camera and refreshes the map as the player moves.
  /// </summary>
  private void Update() {
    if(MapsService == null) {
      return;
    }

#if !UNITY_EDITOR

    // // Get the current map location.
    // LatLng currentLocation = new LatLng(Input.location.lastData.latitude, Input.location.lastData.longitude);
    // Vector3 currentWorldLocation = MapsService.Projection.FromLatLngToVector3(currentLocation);

    // mockPlayerOriginalLocation = new Vector3(curLat, 0, curLng);

    // Get the current map location.
    currentGeoLocation = new LatLng(Input.location.lastData.latitude, Input.location.lastData.longitude);
    currentVec3Location = MapsService.Projection.FromLatLngToVector3(currentGeoLocation);

    // Move the Player to the current map location.
    Vector3 targetPlayerPosition = new Vector3(currentVec3Location.x, playerGO.transform.position.y, currentVec3Location.z);
    playerGO.transform.position = Vector3.Lerp(playerGO.transform.position, targetPlayerPosition, Time.deltaTime);

    bool isMoving = Vector3.Distance(playerGO.transform.position, targetPlayerPosition) > 2f;

    //Rotate
    if(!isMoving) {
      // testHeading += Input.GetAxis("Horizontal") * mockRotationSpeed * Time.deltaTime;
      playerModel.transform.rotation = Quaternion.Euler(playerModel.transform.up * Input.compass.trueHeading);
    } else {
      playerModel.transform.forward = targetPlayerPosition - playerModel.transform.position;
    }

    //Animate
    animator.SetBool("Walking", isMoving);

    // Only move the map location if the device has moved more than 2 meters.
    if(Vector3.Distance(Vector3.zero, currentVec3Location) > 2) {
      MapsService.MoveFloatingOrigin(currentGeoLocation, new [] { playerGO.gameObject });
      MapsService.LoadMap(ExampleDefaults.DefaultBounds, ExampleDefaults.DefaultGameObjectOptions);
      PreviousLocation = currentGeoLocation;
    }
#else
    //UNITY EDITOR

    mockPlayerOriginalLocation = new Vector3(curLat, 0, curLng);

    // Get the current map location.
    currentGeoLocation = new LatLng(mockPlayerOriginalLocation.x, mockPlayerOriginalLocation.z);
    currentVec3Location = MapsService.Projection.FromLatLngToVector3(currentGeoLocation);

    // Move the Player to the current map location.
    Vector3 targetPlayerPosition = new Vector3(currentVec3Location.x, playerGO.transform.position.y, currentVec3Location.z);
    playerGO.transform.position = Vector3.Lerp(playerGO.transform.position, targetPlayerPosition, Time.deltaTime);

    bool isMoving = Vector3.Distance(playerGO.transform.position, targetPlayerPosition) > 2f;

    //Rotate
    if(!isMoving) {
      testHeading += Input.GetAxis("Horizontal") * mockRotationSpeed * Time.deltaTime;
      playerModel.transform.rotation = Quaternion.Euler(playerModel.transform.up * testHeading);
    } else {
      playerModel.transform.forward = targetPlayerPosition - playerModel.transform.position;
    }

    //Animate
    animator.SetBool("Walking", isMoving);

    // Only move the map location if the device has moved more than 2 meters.
    if(Vector3.Distance(Vector3.zero, currentVec3Location) > 2) {
      MapsService.MoveFloatingOrigin(currentGeoLocation, new [] { playerGO.gameObject });
      MapsService.LoadMap(ExampleDefaults.DefaultBounds, ExampleDefaults.DefaultGameObjectOptions);
      PreviousLocation = currentGeoLocation;
    }
#endif
  }

  private void GetPermissions() {
#if PLATFORM_ANDROID
    if(!Permission.HasUserAuthorizedPermission(Permission.FineLocation)) {
      Permission.RequestUserPermission(Permission.FineLocation);
      dialog = new GameObject();
    }
#endif // Android
  }

  void OnGUI() {
#if PLATFORM_ANDROID
    if(!Permission.HasUserAuthorizedPermission(Permission.FineLocation)) {
      // The user denied permission to use location information.
      // Display a message explaining why you need it with Yes/No buttons.
      // If the user says yes then present the request again
      // Display a dialog here.
      dialog.AddComponent<PermissionsRationaleDialog>();
      return;
    } else if(dialog != null) {
      Destroy(dialog);
    }
#endif
  }
}