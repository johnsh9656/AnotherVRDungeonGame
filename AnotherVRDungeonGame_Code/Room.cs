using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<EnemyHealth> enemies = new List<EnemyHealth>();
    public Animator doorAnim;
    public Animator doorAnim2;

    private void Awake()
    {
        // disable all enemies
        foreach (EnemyHealth e in enemies)
        {
            e.gameObject.SetActive(false);
        }
    }

    public void OpenDoor()
    {
        if (doorAnim)
        {
            doorAnim.SetTrigger("Open");
            if (doorAnim2) doorAnim2.SetTrigger("Open2");
        }

        foreach (EnemyHealth e in enemies)
        {
            e.gameObject.SetActive(true);
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {
        //AlertAllEnemies();
        Destroy(GetComponent<Collider>());
    }*/

    public void AlertAllEnemies()
    {
        foreach (EnemyHealth e in enemies)
        {
            e.GetComponent<EnemyAI>().AlertOfPlayer();
        }
    }
}
