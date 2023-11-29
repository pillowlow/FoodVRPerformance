using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestVFX_broadcast : MonoBehaviour
{   
    public string eventname;
    public float delay;
    public int start=0;
    public int end=10;
    public VFX_Broadcaster Source; // Assign this in the inspector
    public KeyCode onKey = KeyCode.Q; // Key to turn on the VFX
    public KeyCode offKey = KeyCode.W; 
    public KeyCode SQNonKey = KeyCode.E; // Key to turn on the VFX
    public KeyCode SQNoffKey = KeyCode.R; 
    bool isOn = false;
    // Start is called before the first frame update
    void Start()    
    {
        
    }

    // Update is called once per frame
    void Update()
    {
          
        if (Input.GetKeyDown(onKey))
        {
            if (Source != null)
                Source.SingleOn(eventname); // Call the On function
            else
                Debug.LogError("VFX_BroadCaster reference not set on TestVFX_broadcast.");
        }

        
        if (Input.GetKeyDown(offKey))
        {
            if (Source != null)
                Source.SingleOff(eventname); // Call the Off function
            else
                Debug.LogError("VFX_BroadCaster reference not set on TestVFX_broadcast.");
        }

        if (Input.GetKeyDown(SQNonKey))
        {
            if (Source != null)
                StartCoroutine(Source.SequenceOn(eventname,delay,start,end));
                 // Call the Off function
            else
                Debug.LogError("VFX_BroadCaster reference not set on TestVFX_broadcast.");
        }

        if (Input.GetKeyDown(SQNoffKey))
        {
            if (Source != null)
                StartCoroutine(Source.SequenceOff(eventname,delay,start,end));
                 // Call the Off function
            else
                Debug.LogError("VFX_BroadCaster reference not set on TestVFX_broadcast.");
        }
    }

    public void testOn(){
         if(isOn){
            StartCoroutine(Source.SequenceOff(eventname,delay,start,end));
            isOn = false;
         }
         else{
             StartCoroutine(Source.SequenceOn(eventname,delay,start,end));
              isOn = true;
         }
        
    }
}
