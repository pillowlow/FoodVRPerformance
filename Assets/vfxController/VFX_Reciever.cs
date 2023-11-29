using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class VFX_Receiver : MonoBehaviour
{   
    [SerializeField]
    protected List<string > Eventnames = new ();

    [SerializeField]
    protected VFX_Switch mySwitch;
    public int index;

    void Start()
    {   
        if (mySwitch == null)
        {
            mySwitch = GetComponent<VFX_Switch>();
        }
    }

    public bool Receive(BroadCastEvent eventToReceive)
    {
        string curEvent = Eventnames.Find(eventItem => eventItem == eventToReceive.name);
        
        if (curEvent != null) // Assuming name is a valid identifier for an existing event
        {
            switch (eventToReceive.mode)
            {
                case EventMode.SINGLE_ON:
                    mySwitch.SetOn();
                    break;
                case EventMode.SINGLE_OFF:
                    mySwitch.SetOff();
                    break;
                case EventMode.SQN_ON:
                    // Replace 'someCondition' with your actual condition
                    if (index == eventToReceive.index) 
                    {
                        mySwitch.SetOn();
                    }
                    break;
                case EventMode.SQN_OFF:
                    // Replace 'someCondition' with your actual condition
                    if (index == eventToReceive.index)
                    {
                        mySwitch.SetOff();
                    }
                    break;
            }
            return true;
        }
        return false;
    }
}


public enum EventMode
{
    SINGLE_ON,
    SINGLE_OFF,
    SQN_ON,
    SQN_OFF
}

public struct BroadCastEvent
{
    public string name;
    public int index;
    public EventMode mode;
}
