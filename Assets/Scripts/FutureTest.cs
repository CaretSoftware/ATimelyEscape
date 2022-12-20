using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem { 
public class FutureTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
                new FailStateEvent().Invoke();
                Debug.Log("55");
        }
    }
}
}