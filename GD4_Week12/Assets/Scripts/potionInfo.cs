using UnityEngine;

[CreateAssetMenu(menuName = "Potion Information", fileName = "New Potion Info")]
public class potionInfo : ScriptableObject
{
	public float addHealth;
	public float addMana;
	public string message;
}
