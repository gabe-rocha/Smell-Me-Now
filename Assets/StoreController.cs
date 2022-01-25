using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreController : MonoBehaviour
{
    
    public static StoreController Instance;

    [SerializeField] private GameObject storeGO;

    private float initialStoreScreenXPosition = 9999;


    private void OnEnable(){
        EventManager.Instance.StartListening(EventManager.Events.ButtonCloseStorePressed, OnButtonCloseStorePressed);
    }
    private void OnDisable(){
        EventManager.Instance.StopListening(EventManager.Events.ButtonCloseStorePressed, OnButtonCloseStorePressed);
    }

    private void Awake(){
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start(){
        initialStoreScreenXPosition = storeGO.transform.position.x;
    }

    public void ShowStore(){
        storeGO.LeanMoveLocalX(0f, 0.5f).setEaseOutBack();
    }

    private void HideStore(){
        storeGO.LeanMoveLocalX(initialStoreScreenXPosition, 0.5f).setEaseInBack();
    }

    private void OnButtonCloseStorePressed(){
        HideStore();
    }
}
