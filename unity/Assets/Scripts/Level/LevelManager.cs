using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Transform spawn;
    [SerializeField] Player player;
    [SerializeField] Transform orbs; // parent object of orbs
    [SerializeField] Finish[] finish;
    [SerializeField] FinishUI finishUI;
    private bool orbsCollected = false;
    public float PlayTime { get; private set; } // in seconds
    public bool IsPlaying { get; private set; } = true;

    void Start()
    {
        // gameplay
        Physics2D.gravity = new Vector2(0f, -30f);
        player.SetPosition(spawn.position);
        foreach (Finish f in finish)
        {
            f.ResetFinish();
        }

        // UI
        finishUI.SetActive(false);
    }

    void Update()
    {
        if (!IsPlaying)
            return;

        PlayTime += Time.deltaTime;

        // enable finishes when all orbs are collected
        if (!orbsCollected && orbs.childCount == 0)
        {
            foreach (Finish f in finish)
            {
                f.EnableFinish();
            }
            orbsCollected = true;
        }

        // check if all finishes are reached
        bool finished = true;
        foreach (Finish f in finish)
        {
            if (!f.ReachedByPlayer)
            {
                finished = false;
                break;
            }
        }

        // all finishes reached
        if (finished)
        {
            FinishLevel();
        }
    }

    void FinishLevel()
    {
        finishUI.SetActive(true);
        finishUI.UpdateTimer(PlayTime);
        IsPlaying = false;
    }
}
