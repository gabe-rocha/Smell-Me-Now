using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textNumBeansCurrency;
    [SerializeField] private TextMeshProUGUI textNumBeansNormal;
    [SerializeField] private TextMeshProUGUI textBeansRadiusCost;

    private void OnEnable()
    {
        EventManager.Instance.StartListening(EventManager.Events.InventoryContentsChanged, OnInventoryContentsChanged);
        EventManager.Instance.StartListening(EventManager.Events.ButtonOpenStorePressed, OnButtonOpenStorePressed);
    }

    private void OnDisable()
    {
        EventManager.Instance.StopListening(EventManager.Events.InventoryContentsChanged, OnInventoryContentsChanged);
        EventManager.Instance.StopListening(EventManager.Events.ButtonOpenStorePressed, OnButtonOpenStorePressed);
    }
    // Start is called before the first frame update
    void Start()
    {
        RefreshUI();
    }

    private void OnInventoryContentsChanged(){
        RefreshUI();
    }

    private void RefreshUI(){
        textNumBeansCurrency.text = $"x{InventoryController.Instance.numBeansCurrency.ToString()}";
        textNumBeansNormal.text = $"x{InventoryController.Instance.numFarts.ToString()}";
        textBeansRadiusCost.text = $"{Data.defaultBeansRadiusCost.ToString()}x";
    }

    private void OnButtonOpenStorePressed(){
        StoreController.Instance.ShowStore();
    }
}
