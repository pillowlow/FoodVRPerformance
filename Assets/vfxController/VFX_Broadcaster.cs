using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.PoseDetection;
using UnityEngine;

public class VFX_Broadcaster : MonoBehaviour
{     
    Dictionary<string,bool> SQN_Flags =  new();
    Dictionary<string,bool> State_Flags =  new();
     VFX_Receiver[] Receivers ;
    // Start is called before the first frame update
    void Start()
    {
        Receivers = FindObjectsOfType<VFX_Receiver>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // BroadCast one shot

    public void SingleOn(string event_name){
        //register or set  on 
        if(!State_Flags.ContainsKey(event_name)){
            State_Flags.Add(event_name,false);
        }
        // if is true break
        if(State_Flags[event_name]){
            Debug.Log(event_name+ " is already off");
            return;
        }
        BroadCastEvent curevent;
        curevent.name = event_name;
        curevent.mode = EventMode.SINGLE_ON;
        curevent.index = 0;
        foreach(VFX_Receiver reciever in Receivers){
            reciever.Receive(curevent);
        }
        State_Flags[event_name] = true;
    }
    public void SingleOff(string event_name){
        //register or set  on 
        if(!State_Flags.ContainsKey(event_name)){
            State_Flags.Add(event_name,true);
        }
        // if is false break
        if(!State_Flags[event_name]){
            Debug.Log(event_name+ " is already off");
            return;
        }
        BroadCastEvent curevent;
        curevent.name = event_name;
        curevent.mode = EventMode.SINGLE_OFF;
        curevent.index = 0;
        foreach(VFX_Receiver reciever in Receivers){
            reciever.Receive(curevent);
        }
        State_Flags[event_name] = false;
    }
    // sending on event in sequence
    public IEnumerator SequenceOn(string event_name,float delay,int start=0,int end=10){
        // check state
        if(!State_Flags.ContainsKey(event_name)){
            State_Flags.Add(event_name,false);
        }
        // if is on already break
        if(State_Flags[event_name]){
            yield break;
        }

        // check is sequence
        // if is first start
        if(!SQN_Flags.ContainsKey(event_name)){
            SQN_Flags.Add(event_name,false);
        }
        // if is seqencing break
        if(SQN_Flags[event_name]){
            yield break;
        }

        // set start
        SQN_Flags[event_name] = true;
        BroadCastEvent curevent;
        curevent.name = event_name;
        curevent.mode = EventMode.SQN_ON;

        for(int i =start;i<end+1;i++){
            foreach(VFX_Receiver reciever in Receivers){
                
                curevent.index = i;
                reciever.Receive(curevent);
                yield return new WaitForSeconds(delay);;
            }
        }
        // set to false
        SQN_Flags[event_name] = false;
        State_Flags[event_name] = true;
    }
    // sending off event in sequence
     public IEnumerator SequenceOff(string event_name,float delay,int start=0,int end=10){
         // check state
        if(!State_Flags.ContainsKey(event_name)){
            State_Flags.Add(event_name,true);
        }
        // if is off already break
        if(!State_Flags[event_name]){
            yield break;
        }

        // check is sequence
        if(!SQN_Flags.ContainsKey(event_name)){
            SQN_Flags.Add(event_name,false);
        }
        // if is seqencing break
        if(SQN_Flags[event_name]){
            yield break;
        }

        // set start
        SQN_Flags[event_name] = true;
        BroadCastEvent curevent;
        curevent.name = event_name;
        curevent.mode = EventMode.SQN_OFF;

        for(int i =start;i<end+1;i++){
            foreach(VFX_Receiver reciever in Receivers){
                
                curevent.index = i;
                reciever.Receive(curevent);
                yield return new WaitForSeconds(delay);;
                
            }
        }
        SQN_Flags[event_name] = false;
        State_Flags[event_name] = false;
    }
}
