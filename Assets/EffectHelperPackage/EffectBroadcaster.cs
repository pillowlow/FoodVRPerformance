using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EffectBroadcaster : MonoBehaviour
{   
    Dictionary<string,bool> Check_Flags = new();
    List<EffectReciever> Receivers = new() ;
    // Start is called before the first frame update

    // singleton
   

    void OnEnable()
    {   
        
    }
    void Start(){
        //DeBugRecievers();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    // register
    public void RegisterReciever(EffectReciever reciever){
        EffectReciever tocheck = Receivers.Find(tocheck=>tocheck.gameObject.name == reciever.gameObject.name);
        if(tocheck ==null){
            Receivers.Add(reciever);
            Debug.Log(reciever.gameObject.name+ "  Registered");
        }
    }

    public void DeBugRecievers(){
        Debug.Log("Recievers: "+Receivers.Count);
    }

    public void SingleCall(string action_name, int ID){
        //register check
        if(!Check_Flags.ContainsKey(action_name)){
            Check_Flags.Add(action_name,false);
        }
        // if is acting return
        if(Check_Flags[action_name]){
            Debug.Log(action_name+ " is busy");
            return;
        }
        //start
         Check_Flags[action_name] = true;

        BroadcastCall call;
        call.callIndex = ID;
        call.action_name = action_name;
        foreach(EffectReciever reciever in Receivers){
            reciever.Receive(call);
        }
        // end set to false
        Check_Flags[action_name] = false;
    }

    public void IntergrateCall(string action_name){
         
         //register check
        if(!Check_Flags.ContainsKey(action_name)){
            Check_Flags.Add(action_name,false);
        }
        // if is acting return
        if(Check_Flags[action_name]){
            Debug.Log(action_name+ " is busy");
            return;
        }
        //start
         Check_Flags[action_name] = true;

        BroadcastCall call;
        call.callIndex = 0;
        call.action_name = action_name;
        foreach(EffectReciever reciever in Receivers){
            reciever.AllReceive(call);
        }
        // end set to false
        Check_Flags[action_name] = false;
        Debug.Log("Intergrate Call  "+ action_name+ "  end ");
    }

    public void SeqeunceCall(string action_name,float delay,int start = 0,int end=5){
        //register check
        if(!Check_Flags.ContainsKey(action_name)){
            Check_Flags.Add(action_name,false);
        }
        if(!Check_Flags[action_name]){
            Check_Flags[action_name] = true;
            Debug.Log($"{action_name} triggered");
             StartCoroutine(SequenceSet(action_name,delay,start,end));
        }
       
    }

    
    // sending on event in sequence
    private IEnumerator SequenceSet(string action_name,float delay,int start=0,int end=5){
         
        BroadcastCall call;
        Debug.Log("Sequence Call  "+ action_name+ "  start ");
        for(int i =start;i<end+1;i++){
            float iterationStartTime = Time.time;
            call.callIndex = i;
            call.action_name = action_name;
            foreach(EffectReciever reciever in Receivers){
                reciever.Receive(call);
                
            }
            
            if(delay<0.01){
                yield return null;
            }
            else{yield return new WaitForSeconds(delay);}
            Debug.Log($"Iteration {i} took {Time.time - iterationStartTime} seconds");
        }
        // set to false
        Check_Flags[action_name] = false;
         Debug.Log("Sequence Call  "+ action_name+ "  end ");
    }
    // sending off event in sequence

}
