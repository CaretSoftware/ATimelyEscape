using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private class PlayerSave
    {
        public float[] position;

        public PlayerSave(Transform transform)
        {
            position = new float[3];

            position[0] = transform.position.x;
            position[1] = transform.position.y;
            position[2] = transform.position.z;
        }

    }

}
