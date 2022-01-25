using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryButton : MonoBehaviour
{

    [HideInInspector] public bool isSelected = false;

    [SerializeField] private TextMeshProUGUI textItemAmount, textItemName, textItemDescription;
    [SerializeField] private Image itemImage, selectedImage;

    [HideInInspector] public ItemSO thisItem;
    [HideInInspector] public int itemAmount;


    private void OnEnable(){
        // EventManager.Instance.StartListening(EventManager.Events.InventoryContentsChanged, OnInventoryContentsChanged);
    }
    private void OnDisable(){
        // EventManager.Instance.StopListening(EventManager.Events.InventoryContentsChanged, OnInventoryContentsChanged);
    }


    public void Initialize(ItemSO item, int amount){
        thisItem = item;
        itemAmount = amount;
        itemImage.sprite = thisItem.spriteItem;
        textItemAmount.text = $"x{amount.ToString()}";        
        selectedImage.enabled = false;
        isSelected = false;
    }

    public void SetSelected(bool _isSelected){

        if(_isSelected){
            //Select It
            //update item description text
            textItemName.text = thisItem.itemName;
            textItemAmount.text = $"x{itemAmount}";
            textItemDescription.text = thisItem.itemDescription;
        }

        selectedImage.enabled = _isSelected;
        isSelected = _isSelected;
    }

    public void UpdateAmount(int newAmount){
        itemAmount = newAmount;

        if(itemAmount == 0){
            Destroy(this.gameObject);
        }else{
            textItemAmount.text = $"x{itemAmount}";
        }     
    }
}
