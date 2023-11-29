using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestVFX_switch : MonoBehaviour
{   
    public VFX_Switch vfxSwitch; // Assign this in the inspector
    public KeyCode onKey = KeyCode.O; // Key to turn on the VFX
    public KeyCode offKey = KeyCode.P; // Key to turn off the VFX

    void Update()
    {
        // Check if the 'on' key is pressed
        if (Input.GetKeyDown(onKey))
        {
            if (vfxSwitch != null)
                vfxSwitch.SetOn(); // Call the On function
            else
                Debug.LogError("VFX_Switch reference not set on VFX_SwitchTester.");
        }

        // Check if the 'off' key is pressed
        if (Input.GetKeyDown(offKey))
        {
            if (vfxSwitch != null)
                vfxSwitch.SetOff(); // Call the Off function
            else
                Debug.LogError("VFX_Switch reference not set on VFX_SwitchTester.");
        }
    }
}
