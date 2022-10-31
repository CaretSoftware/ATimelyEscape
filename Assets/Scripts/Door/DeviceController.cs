using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DeviceController : MonoBehaviour
{
    protected Device device;
    public void SetDevice(Door door)
    {
        this.device = door;
    }
}
