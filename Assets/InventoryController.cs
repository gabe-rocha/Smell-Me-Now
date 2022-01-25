using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryController : MonoBehaviour
{

    public static InventoryController Instance;

    [SerializeField] private GameObject inventoryGO;
    [SerializeField] private GameObject scrollViewContent;
    [SerializeField] private GameObject inventoryButtonTemplate;
    [SerializeField] private List<ItemSO> listItemsAvailable;
    [SerializeField] private TextMeshProUGUI textBeanCurrency;


    [HideInInspector] public int numBeansCurrency = 0;
    [HideInInspector] public int numBoostBeansRaw = 0;
    [HideInInspector] public int numBoostBeansCanNormal = 0;
    [HideInInspector] public int numFarts = 0;


    private float initialInventoryScreenXPosition = 9999;
    private List<InventoryButton> listInventoryButtons = new List<InventoryButton>();

    private void OnEnable()
    {
        EventManager.Instance.StartListening(EventManager.Events.ButtonOpenInventoryPressed, OnButtonOpenInventoryPressed);
        EventManager.Instance.StartListening(EventManager.Events.ButtonOpenStorePressed, OnButtonOpenStorePressed);
        EventManager.Instance.StartListening(EventManager.Events.ButtonCloseInventoryPressed, OnButtonCloseInventoryPressed);
        EventManager.Instance.StartListening(EventManager.Events.ButtonUseInventoryItemPressed, OnButtonUseInventoryItemPressed);
        EventManager.Instance.StartListeningWithGOParam(EventManager.Events.ButtonInventoryButtonPressed, OnButtonInventoryButtonPressed);
    }

    private void OnDisable()
    {
        EventManager.Instance.StopListening(EventManager.Events.ButtonOpenInventoryPressed, OnButtonOpenInventoryPressed);
        EventManager.Instance.StopListening(EventManager.Events.ButtonOpenStorePressed, OnButtonOpenStorePressed);
        EventManager.Instance.StopListening(EventManager.Events.ButtonCloseInventoryPressed, OnButtonCloseInventoryPressed);
        EventManager.Instance.StopListening(EventManager.Events.ButtonUseInventoryItemPressed, OnButtonUseInventoryItemPressed);
        EventManager.Instance.StopListeningWithGOParam(EventManager.Events.ButtonInventoryButtonPressed, OnButtonInventoryButtonPressed);
    }


    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        initialInventoryScreenXPosition = inventoryGO.transform.position.x;
        LoadInventoryContents();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadInventoryContents(){
        listInventoryButtons.Clear();
        numBeansCurrency = PlayerPrefs.GetInt("numBeansCurrency", Data.defaultNumBeansCurrency);
        textBeanCurrency.text = numBeansCurrency.ToString();

        numFarts = PlayerPrefs.GetInt("numFarts", Data.defaultNumFarts);
        numBoostBeansRaw = PlayerPrefs.GetInt("numBoostBeansRaw", Data.defaultBoostNumBeansRaw);
        numBoostBeansCanNormal = PlayerPrefs.GetInt("numBoostBeansCanNormal", Data.defaultBoostNumBeansCanNormal);

        foreach(ItemSO item in listItemsAvailable){
            Data.Items itemType = item.itemType;

            switch(itemType){
                case Data.Items.boostBeansRaw:
                    var numBoostBeansRaw = PlayerPrefs.GetInt("numBoostBeansRaw", 10);
                    if(numBoostBeansRaw > 0){
                        AddButtonToContent(item, numBoostBeansRaw);
                    }

                break;
                case Data.Items.boostBeansCan:
                    var numBoostBeansCan = PlayerPrefs.GetInt("numBoostBeansCanNormal", 3);
                    if(numBoostBeansCan > 0){
                        AddButtonToContent(item, numBoostBeansCan);
                    }
                break;

            }
        }

        EventManager.Instance.TriggerEvent(EventManager.Events.InventoryContentsChanged);
    }


    public void SaveInventoryContents(){
        PlayerPrefs.SetInt("numBeansCurrency", numBeansCurrency);
        PlayerPrefs.SetInt("numBoostBeansRaw", numBoostBeansRaw);
        PlayerPrefs.SetInt("numFarts", numFarts);
        PlayerPrefs.SetInt("numBoostBeansCanNormal", numBoostBeansCanNormal);

        EventManager.Instance.TriggerEvent(EventManager.Events.InventoryContentsChanged);
    }

    public void RemoveBeansCurrency(int amount){
        numBeansCurrency -= amount;
        if(numBeansCurrency < 0) numBeansCurrency = 0;

        SaveInventoryContents();
    }

    public void RemoveFarts(int amount){
        numFarts -= amount;
        if(numFarts < 0) numFarts = 0;

        SaveInventoryContents();
    }

    public void RemoveBeansCanNormal(int amount){
        numBoostBeansCanNormal -= amount;
        if(numBoostBeansCanNormal < 0) numBoostBeansCanNormal = 0;

        SaveInventoryContents();
    }

    private void OnButtonOpenInventoryPressed(){
        ShowInventory();
    }

    public void ShowInventory(){
        inventoryGO.LeanMoveLocalX(0f, 0.5f).setEaseOutBack();

        //deselect all buttons
        foreach( InventoryButton ib in listInventoryButtons){
            ib.SetSelected(false);
        }
        //Select 1st Button
        if(listInventoryButtons.Count > 0){
            listInventoryButtons[0].SetSelected(true);
        }

    }

    private void OnButtonCloseInventoryPressed(){
        HideInventory();
    }

    private void HideInventory(){
        inventoryGO.LeanMoveLocalX(initialInventoryScreenXPosition, 0.5f).setEaseInBack();
    }

    private void AddButtonToContent(ItemSO item, int amount){
        var buttonGO = Instantiate(inventoryButtonTemplate, scrollViewContent.transform);
        buttonGO.SetActive(true);

        InventoryButton invButton = buttonGO.GetComponent<InventoryButton>();
        invButton.Initialize(item, amount);
        listInventoryButtons.Add(invButton);
    }

    private void OnButtonInventoryButtonPressed(GameObject buttonPressedGO){

        InventoryButton invButton = buttonPressedGO.GetComponent<InventoryButton>();
        if(invButton.isSelected){
            //try using the item
            //todo
        }
        else{
            //deselect all buttons
            foreach( InventoryButton ib in listInventoryButtons){
                ib.SetSelected(false);
            }
            
            //select pressed button
            invButton.SetSelected(true);
        }
    }

    public void OnButtonUseInventoryItemPressed(){
        
        Debug.Log("1");

        InventoryButton selectedButton = null;
        foreach( InventoryButton ib in listInventoryButtons){
            if(ib.isSelected){
                selectedButton = ib;
            }
        }

        Debug.Log("2");

        if(selectedButton == null){
            Debug.Log($"No Button Selected");
            return;
        }

        ItemSO item = selectedButton.thisItem;
        switch(item.itemType){
            case Data.Items.boostBeansRaw:
                if(numBoostBeansRaw > 0){
                    numBoostBeansRaw -= 1;
                    numFarts += Data.BoostBeansNormalAddAmount;
                    selectedButton.UpdateAmount(numBoostBeansRaw);
                    if(numBoostBeansRaw == 0){
                        listInventoryButtons.Remove(selectedButton);
                    }
                    SaveInventoryContents();
                }else{
                    StoreController.Instance.ShowStore();
                }
            break;

            case Data.Items.boostBeansCan:
                if(numBoostBeansCanNormal > 0){
                    numBoostBeansCanNormal -= 1;
                    numFarts += Data.BoostBeansCanAddAmount;
                    selectedButton.UpdateAmount(numBoostBeansCanNormal);
                    if(numBoostBeansCanNormal == 0){
                        listInventoryButtons.Remove(selectedButton);
                    }
                    SaveInventoryContents();
                }else{
                    StoreController.Instance.ShowStore();
                }
            break;
        }

        EventManager.Instance.TriggerEvent(EventManager.Events.InventoryContentsChanged);
    }

    private void OnButtonOpenStorePressed(){
        HideInventory();
        StoreController.Instance.ShowStore();
    }
}
