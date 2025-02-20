using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class Enemy : MonoBehaviour
{
    [SerializeField] private playerMovement player;
    [SerializeField] private Rigidbody2D rig;
    private float moveSpeed = 10f;
    private float health = 100f;
    private float hitForce2 = 2f;
    private float hitForce = 5f;
    private bool canMove = true;
    public TMP_Text killsMessage;
    private int score = 0;
   
    // Update is called once per frame
    void Update()
    {
        if (health > 0 && canMove)
        {
            rig.MovePosition(transform.position + ((player.transform.position - transform.position).normalized * moveSpeed * Time.deltaTime));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (health > 0)
        {
            if (collision.transform.CompareTag("Player"))
            {
                player.TakeDamage((player.transform.position - transform.position).normalized);
            }
        }
    }
    
    public void TakeDamage()
    {
        if (health > 0)
        {
            health -= 50f;

            UnityEngine.Debug.Log("Enemy Health = " + health);
            
            if (health <= 0)
            {
                score += 1;
                killsMessage.text = "Kills: " + score;
                rig.AddForce((transform.position - player.transform.position).normalized * hitForce, ForceMode2D.Impulse);
                health = 0;
                StartCoroutine(DoRespawn());
            }
            else
            {

                StartCoroutine(DoHit());
                rig.AddForce((transform.position - player.transform.position).normalized * hitForce2, ForceMode2D.Impulse);
            }
        }
    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (health > 0)
        {
            if (collision.transform.CompareTag("Sword"))
            {
                health -= 50f;
                rig.AddForce((transform.position - player.transform.position).normalized * hitForce, ForceMode2D.Impulse);

                if (health <= 0)
                {
                    health = 0;
                    StartCoroutine(DoRespawn());
                }
            }
        }
    }*/

    IEnumerator DoRespawn()
    {
        yield return new WaitForSeconds(3);
        health = 100;
    }

    IEnumerator DoHit()
    {
        canMove = false;
        yield return new WaitForSeconds(1);
        canMove = true;
    }
}
