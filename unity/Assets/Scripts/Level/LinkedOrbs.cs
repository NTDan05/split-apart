using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkedOrbs : MonoBehaviour
{
    [SerializeField] private Orb[] _orbs = new Orb[2];
    [SerializeField] private LineRenderer _lineRenderer;

    private void Start()
    {
        if (_orbs.Length != 2)
        {
            Debug.LogError("LinkedOrbs requires exactly 2 orbs.");
            return;
        }

        if (_lineRenderer == null)
        {
            _lineRenderer = gameObject.GetComponent<LineRenderer>();
        }

        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, _orbs[0].transform.position);
        _lineRenderer.SetPosition(1, _orbs[1].transform.position);
    }

    private void Update()
    {
        if (AllOrbsActive())
        {
            Destroy(gameObject);
        }
    }
    
    private bool AllOrbsActive()
    {
        foreach (Orb orb in _orbs)
        {
            if (!orb.Active) return false;
        }
        return true;
    }
}
