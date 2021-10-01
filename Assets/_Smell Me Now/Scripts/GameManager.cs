using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private static GameManager instance;
    public static GameManager Instance { get => instance; set => instance = value; }

    [SerializeField] private GameObject pfFirebaseController;

    private FirebaseController firebaseController;

    public enum GameStates {
        Initializing,
        Loading,
        Playing
    }

    public GameStates gameState = GameStates.Initializing;
    internal List<FirebaseController.FartData> listFarts = new List<FirebaseController.FartData>();

    void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            Application.targetFrameRate = 60;

        } else {
            Destroy(this);
        }

    }

    private void Start() {
        firebaseController = Instantiate(pfFirebaseController).GetComponent<FirebaseController>();

        if(firebaseController == null) {
            Debug.LogError("Gamemanager coudln't instantiate a Firebase Controller");
        }

        EventManager.Instance.TriggerEvent(EventManager.Events.OnGameManagerReady);
        gameState = GameStates.Playing;
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            EventManager.Instance.TriggerEvent(EventManager.Events.OnLevelStarted);
        }
    }

    public void SendFartDataToFirebase(FirebaseController.FartData fartData) {
        firebaseController.SendFartDataToFirebase(fartData);
    }

}