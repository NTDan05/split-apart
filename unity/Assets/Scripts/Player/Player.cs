using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject big;
    [SerializeField] GameObject small1;
    [SerializeField] GameObject small2;
    [SerializeField] GameObject splitDirectionIndicator;
    [SerializeField] GameObject combinePositionIndicator1;
    [SerializeField] GameObject combinePositionIndicator2;
    [SerializeField] GameObject combineRestrictionIndicator1;
    [SerializeField] GameObject combineRestrictionIndicator2;
    [SerializeField] LayerMask spaceChecksLayers;

    private Rigidbody2D bigRb;
    private Rigidbody2D small1Rb;
    private Rigidbody2D small2Rb;
    private bool isBig = true;
    private GameObject selected;

    // constants
    private const float SMALL_SIZE = 0.7f;
    private const float BIG_SIZE = 1f;
    private const float DIFF = (BIG_SIZE - SMALL_SIZE) / 2;
    private static readonly Vector2[] combineSpaceCheckOffsets =
    {
        new Vector2(-DIFF,  DIFF), new Vector2(0f,  DIFF), new Vector2(DIFF,  DIFF),
        new Vector2(-DIFF,  0f),   new Vector2(0f,  0f),   new Vector2(DIFF,  0f),
        new Vector2(-DIFF, -DIFF), new Vector2(0f, -DIFF), new Vector2(DIFF, -DIFF)
    };

    public float launchSpeed = 15f;
    public float splitCooldown = 0.5f;
    private float currentSplitCooldown = 0f;
    // Start is called before the first frame update
    void Start()
    {
        bigRb = big.GetComponent<Rigidbody2D>();
        small1Rb = small1.GetComponent<Rigidbody2D>();
        small2Rb = small2.GetComponent<Rigidbody2D>();

        big.GetComponent<PlayerMovement>().SetKeybinds(new KeyCode[] { Keybinds.leftKey1, Keybinds.leftKey2 },
                                                     new KeyCode[] { Keybinds.rightKey1, Keybinds.rightKey2 },
                                                     new KeyCode[] { Keybinds.jumpKey1, Keybinds.jumpKey2 });
        small1.GetComponent<PlayerMovement>().SetKeybinds(new KeyCode[] { Keybinds.leftKey1 },
                                                         new KeyCode[] { Keybinds.rightKey1 },
                                                         new KeyCode[] { Keybinds.jumpKey1 });
        small2.GetComponent<PlayerMovement>().SetKeybinds(new KeyCode[] { Keybinds.leftKey2 },
                                                         new KeyCode[] { Keybinds.rightKey2 },
                                                         new KeyCode[] { Keybinds.jumpKey2 });
    }

    // Update is called once per frame
    void Update()
    {
        if (isBig)
        {
            // update cooldown
            currentSplitCooldown -= Time.deltaTime;

            if (currentSplitCooldown > 0) return;
            
            splitDirectionIndicator.SetActive(Input.GetKey(Keybinds.aimKey));

            // aiming
            if (Input.GetKey(Keybinds.aimKey))
            {
                float angle = GetAngleToMouse(big.transform.position);
                splitDirectionIndicator.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);

                // splitting
                if (Input.GetKeyDown(Keybinds.splitKey))
                {
                    Split(big.transform.position, angle);
                }
            }
        }
        else
        {
            // select position for combining
            // don't select both at once
            if (Input.GetKey(Keybinds.selectKey1) ^ Input.GetKey(Keybinds.selectKey2)) // XOR: only one pressed
            {
                bool key1 = Input.GetKey(Keybinds.selectKey1);

                combinePositionIndicator1.SetActive(key1);
                combinePositionIndicator2.SetActive(!key1);

                combineRestrictionIndicator1.SetActive(!HasEnoughSpace(small1.transform.position) && key1);
                combineRestrictionIndicator2.SetActive(!HasEnoughSpace(small2.transform.position) && !key1);

                selected = key1 ? small1 : small2;
            }
            else
            {
                // neither pressed or both pressed
                combinePositionIndicator1.SetActive(false);
                combinePositionIndicator2.SetActive(false);
                combineRestrictionIndicator1.SetActive(false);
                combineRestrictionIndicator2.SetActive(false);
                selected = null;
            }

            // combine
            if (Input.GetKeyDown(Keybinds.combineKey) && selected != null && HasEnoughSpace(selected.transform.position))
            {
                Combine(selected);
            }
        }
    }

    void Split(Vector2 position, float angle)
    {
        // calculate offset
        Vector2 offset = AngleToSquareOffset(angle, 0.5f);
        small1.transform.position = position + offset;
        small2.transform.position = position - offset;

        // split
        big.SetActive(false);
        small1.SetActive(true);
        combinePositionIndicator1.SetActive(false);
        small2.SetActive(true);
        combinePositionIndicator2.SetActive(false);

        // velocity boost
        small1Rb.velocity = launchSpeed * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        small2Rb.velocity = -launchSpeed * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        // update state
        isBig = false;
    }

    void Combine(GameObject selected)
    {
        // move to position
        big.transform.position = selected.transform.position;

        // combine
        big.SetActive(true);

        // update velocity
        bigRb.velocity = selected.GetComponent<Rigidbody2D>().velocity;

        splitDirectionIndicator.SetActive(false);
        small1.SetActive(false);
        small2.SetActive(false);

        // update state
        isBig = true;
        currentSplitCooldown = splitCooldown;
    }

    // Convert an angle to the corresponding point on the perimeter of a square
    // Assumes the square is centered at the origin
    Vector2 AngleToSquareOffset(float rads, float sideLength)
    {
        // normalize angle
        rads = Mathf.Repeat(rads, 2 * Mathf.PI);

        // top
        if (rads >= Mathf.PI / 4 && rads < Mathf.PI * 3 / 4)
            return new Vector2(sideLength / 2 / Mathf.Tan(rads), sideLength / 2);

        // left
        if (rads >= Mathf.PI * 3 / 4 && rads < Mathf.PI * 5 / 4)
            return new Vector2(-sideLength / 2, -sideLength / 2 * Mathf.Tan(rads));

        // bottom
        if (rads >= Mathf.PI * 5 / 4 && rads < Mathf.PI * 7 / 4)
            return new Vector2(-sideLength / 2 / Mathf.Tan(rads), -sideLength / 2);

        // right
        return new Vector2(sideLength / 2, sideLength / 2 * Mathf.Tan(rads));
    }

    // Get the angle from a given position to the mouse position
    float GetAngleToMouse(Vector2 position)
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mouseWorldPos - position;
        float rads = Mathf.Atan2(direction.y, direction.x);

        return rads;
    }

    // checks space around a position to see if there is any obstacles
    // checks 9 times to account for all expansion directions
    bool HasEnoughSpace(Vector2 position)
    {
        // check 
        foreach (Vector2 offset in combineSpaceCheckOffsets)
        {
            Vector2 checkPosition = position + offset;
            if (!Physics2D.OverlapBox(checkPosition, new Vector2(BIG_SIZE - 0.1f, BIG_SIZE - 0.1f), 0, spaceChecksLayers))
            {
                return true;
            }
        }

        return false;
    }

    public void SetPosition(Vector2 position)
    {
        big.transform.position = position;
        small1.transform.position = position + new Vector2(0.5f, 0);
        small2.transform.position = position - new Vector2(0.5f, 0);
    }
}
