using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    public bool ReachedByPlayer { get; private set; }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ReachedByPlayer = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ReachedByPlayer = false;
        }
    }

    public void ResetFinish()
    {
        ReachedByPlayer = false;
        gameObject.SetActive(false);
    }

    public void EnableFinish()
    {
        gameObject.SetActive(true);
    }
}
