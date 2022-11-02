using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Device
{
    [SerializeField] private OpeningDirection direction;
    [Tooltip("Time it takes from activation until door is completly open")]
    [SerializeField] private float timeToOpen = 3f;
    [Tooltip("How high the door go.")]
    [SerializeField] private float leangthOfOpening = 1f;
    [SerializeField] private Transform doorOne;
    [SerializeField] private Transform doorTwo;

    private bool turnedOn;
    private Vector3 lockPositionOne;
    private Vector3 unlockPositionOne;
    private Vector3 lockPositionTwo;
    private Vector3 unlockPositionTwo;

    private float timer;

    private bool OD_running;

    private void Start()
    {
        lockPositionOne = doorOne.position;
        lockPositionTwo = doorTwo.position;
        if (direction == OpeningDirection.up)
        {
            unlockPositionOne = lockPositionOne + (Vector3.up * leangthOfOpening);
        }
        else if (direction == OpeningDirection.side )
        {
            unlockPositionOne = lockPositionOne + (doorOne.rotation * Vector3.right * leangthOfOpening);
        }else if(direction == OpeningDirection.twoSide)
        {
            unlockPositionOne = lockPositionOne + (doorOne.rotation * Vector3.right * leangthOfOpening);
            unlockPositionTwo = lockPositionTwo + (doorTwo.rotation * Vector3.left * leangthOfOpening);
        }
        controller.SetDevice(this);
    }

    public override void TurnedOn(bool turnedOn)
    {
        if (this.turnedOn != turnedOn)
        {
            this.turnedOn = turnedOn;
            if (!OD_running)
            {
                StopAllCoroutines();
                StartCoroutine(OpenDoor());
            }
        }


    }

    private IEnumerator OpenDoor()
    {
        OD_running = true;
        do
        {
            if (turnedOn)
            {
                timer += Time.deltaTime / timeToOpen;
            }
            else
            {
                timer -= Time.deltaTime / timeToOpen;
            }
            doorOne.position = Vector3.Lerp(lockPositionOne, unlockPositionOne, timer);
            if(direction == OpeningDirection.twoSide)
            {
                doorTwo.position = Vector3.Lerp(lockPositionTwo, unlockPositionTwo, timer);
            }
            yield return null;
        } while (timer > 0 && timer < 1);



        if (timer >= 1)
        {
            timer = 1;
        }
        else
        {
            timer = 0;
        }
        OD_running = false;
    }
    [Serializable]
    private enum OpeningDirection
    {
        up,
        side,
        twoSide
    }
}
