using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBroadcaster : MonoBehaviour
{
    public EffectBroadcaster broadCaster;
    public KeyCode onKey = KeyCode.Q; // Key to turn on the VFX
    public KeyCode offKey = KeyCode.W; // Key to turn off the VFX
    public KeyCode fadeonKey = KeyCode.E; // Key to turn on the VFX
    public KeyCode fadeoffKey = KeyCode.R; // Key to turn off the VFX
    public float delay;
    // Start is called before the first frame update
    void Start()
    {
       if(broadCaster == null){
            broadCaster =FindObjectOfType<EffectBroadcaster>();
       }
    }

    // Update is called once per frame
    void Update()
    {
         if (Input.GetKeyDown(onKey))
        {
            broadCaster.IntergrateCall("On");
        }

        // Check if the 'off' key is pressed
        if (Input.GetKeyDown(offKey))
        {
            broadCaster.IntergrateCall("Off");
        }
        if (Input.GetKeyDown(fadeonKey))
        {
           broadCaster.SeqeunceCall("FadeOn",delay,0,5);
        }
        if (Input.GetKeyDown(fadeoffKey))
        {
            broadCaster.SeqeunceCall("FadeOff",delay,0,5);
        }
    }
}
