using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRoom : MonoBehaviour
{   
    public EffectBroadcaster broadcaster;
     public KeyCode onKey = KeyCode.Q; // Key to turn on the VFX
    public KeyCode offKey = KeyCode.W; // Key to turn off the VFX
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
         if (Input.GetKeyDown(onKey))
        {
           broadcaster.IntergrateCall("break");
        }

        // Check if the 'off' key is pressed
        if (Input.GetKeyDown(offKey))
        {
             broadcaster.IntergrateCall("compose");
        }
    }
}
