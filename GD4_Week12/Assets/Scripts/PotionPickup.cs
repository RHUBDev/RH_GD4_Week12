using UnityEngine;

public class PotionPickup : MonoBehaviour
{
    public potionInfo potion;
    private playerMovement player;

    void Start()
    {
        Debug.Log("PotionStart");
        player = GameObject.FindWithTag("Player").GetComponent<playerMovement>();
    }

    public void OnTriggerEnter2D()
    {
        Debug.Log("Trig");
        player.Pickup(potion);
        Destroy(gameObject);
    }
}
