using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectReciever : MonoBehaviour

{     [SerializeField ]
    protected  EffectBroadcaster broadcaster;
     [SerializeField ]
     protected EffectController controller;
    [SerializeField ]
    protected List<SingleAction> sActions;
    [SerializeField ]
    protected List<FadeAction> fActions;
    

    
    void OnEnable()
    {   
        if( broadcaster ==null){
            broadcaster = FindObjectOfType<EffectBroadcaster>();
        }
        if(controller == null){
            controller = this.GetComponent<EffectController>();
        }
        
        broadcaster.RegisterReciever(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Receive(BroadcastCall call){
        // set single act
        foreach(SingleAction sact in sActions){
            if(sact.ID == call.callIndex && call.action_name == sact.action_name){ // if index and call correct
                foreach(string trig_name in sact.trigger_names){
                    controller.SetValue(sact.target,trig_name);
                    //Debug.Log("call "+trig_name +"vale "+sact.target);
                }
            }
        }
        // set fade act
        foreach(FadeAction fact in fActions){
            if(fact.ID == call.callIndex && call.action_name == fact.action_name){ // if index and call correct
                foreach(string trig_name in fact.trigger_names){
                    controller.SetValueFade(fact.target,fact.duration,trig_name);
                    //Debug.Log("call "+trig_name +"vale "+fact.target);
                }
            }
        }
    }

    public void AllReceive(BroadcastCall call){
         foreach(SingleAction sact in sActions){
            if(call.action_name == sact.action_name){ // if index and call correct
                foreach(string trig_name in sact.trigger_names){
                    controller.SetValue(sact.target,trig_name);
                }
            }
        }
        // set fade act
        foreach(FadeAction fact in fActions){
            if(call.action_name == fact.action_name){ // if index and call correct
                foreach(string trig_name in fact.trigger_names){
                    controller.SetValueFade(fact.target,fact.duration,trig_name);
                }
            }
        }
    }
}
