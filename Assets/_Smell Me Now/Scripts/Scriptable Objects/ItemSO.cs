using UnityEngine;


[CreateAssetMenu(menuName = "Smell Me Now/Items", fileName = "Item")]
public class ItemSO : ScriptableObject
{
    public Data.Items itemType;
    public string itemName;
    public Sprite spriteItem;
    public bool isStackable = true;
    public string itemDescription;
}
