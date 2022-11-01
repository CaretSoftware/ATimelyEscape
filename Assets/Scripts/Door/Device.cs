using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Device : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The AllButonPressed instence responsable for opening the door")]
    [SerializeField] protected DeviceController controller;
    public virtual void TurnedOn(bool turnedOn)
    {

    }
}
