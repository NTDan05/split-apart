using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{

    [SerializeField] private bool _paired = false;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;

    public bool Active { get; private set; } = false;

    private void Start()
    {
        GetComponent<SpriteRenderer>().color = inactiveColor;
    }

    private void Update()
    {
        if (Active)
        {
            if (!_paired) Destroy(gameObject);
            else GetComponent<SpriteRenderer>().color = activeColor;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = inactiveColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Active = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Active = false;
        }
    }
}
