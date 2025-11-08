using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    [SerializeField] GameObject popupObject;
    [SerializeField] bool destroyOnClose = false;
    [SerializeField] bool persistent = false;

    void Start()
    {
        popupObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            popupObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !persistent)
        {
            popupObject.SetActive(false);
            if (destroyOnClose)
            {
                Destroy(gameObject);
            }
        }
    }
}
