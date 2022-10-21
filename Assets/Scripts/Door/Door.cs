using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    [SerializeField] private AllButtonPressed controller;
    [SerializeField] private float timeToOpen = 3f;
    [SerializeField] private float hightOfOpenDoor = 1f; 

    private bool turnedOn;
    private Vector3 lockPosition;
    private Vector3 unlockPosition;

    private float timer;

    private bool OD_running;

    private void Start()
    {
        lockPosition = transform.position;
        unlockPosition = lockPosition + (Vector3.up * hightOfOpenDoor);
        controller.SetDoor(this);
    }

    public void TurnedOn(bool turnedOn)
    {
        if (this.turnedOn != turnedOn)
        {
            this.turnedOn = turnedOn;
            if (!OD_running)
            {
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
            transform.position = Vector3.Lerp(lockPosition, unlockPosition, timer);
            yield return null;
        }while (timer > 0 || timer < 1);

        OD_running = false;

        if (timer >= 1)
        {
            timer = 1;
        }
        else
        {
            timer = 0;
        }
    }
}
