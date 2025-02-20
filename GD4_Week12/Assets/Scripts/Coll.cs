using UnityEngine;

public class Coll : MonoBehaviour
{
    [SerializeField] Enemy enemy;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            enemy.TakeDamage();
            gameObject.SetActive(false);
        }
    }
}
