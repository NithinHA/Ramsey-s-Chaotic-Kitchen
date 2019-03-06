using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
	new public string name = "New Item";
	public Sprite icon = null;
	public float time_to_prepare;       // for food_item, utensils and dishes
	public Item served_in;              // only for dishes
	public Item[] ingredients;			// only for dishes
}
