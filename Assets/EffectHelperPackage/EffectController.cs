using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class EffectController : MonoBehaviour
{
    [SerializeField] 
    protected List<ControllerWrapper> controllers;
     [SerializeField] 
    protected List<ControlEvent> CtrEvents;

   
   
    // Start is called before the first frame update
    void OnEnable()
    {
        
        if(CtrEvents.Count ==0){ 
            ControlEvent C_event;
            C_event.ID = "OnOff";
            C_event.is_testing = false;
            C_event.testbar = 0;
            CtrEvents.Add(C_event);
        }
    }

    // Update is called once per frame
    void Update()
    {   
        foreach(ControlEvent C_event in CtrEvents){
            if(C_event.is_testing){
                SetValue(C_event.testbar,C_event.ID);
                //Debug.Log("value:" + C_event.testbar);
            }
        }
       
    }

    private void TrySetTrigger(IController controller, string ID, float value) {
        try { 
            controller.SetTrigger(ID, value); 
        } catch (Exception e) { 
            // Debug.LogError(e.Message);
        }
    }

    private void TrySetTriggerFade(IController controller, string ID, float value, float duration) {
        try { 
            controller.SetTriggerFade(ID, value, duration); 
        } catch (Exception e) { 
            // Debug.LogError(e.Message);
        }
    }

    public void SetOn(string ID="OnOff") {
        foreach (var wrapper in controllers) {
            var controller = wrapper.GetController();
            if (controller != null) {
                TrySetTrigger(controller, ID, 1);
            }
        }
    }

    public void SetOff(string ID="OnOff") {
        foreach (var wrapper in controllers) {
            var controller = wrapper.GetController();
            if (controller != null) {
                TrySetTrigger(controller, ID, 0);
            }
        }
    }

      public void SetFadeOn(float duration, string ID="OnOff") {
        foreach (var wrapper in controllers) {
            var controller = wrapper.GetController();
            if (controller != null) {
                TrySetTriggerFade(controller, ID, 1, duration);
            }
        }
    }

    public void SetFadeOff(float duration, string ID="OnOff") {
        foreach (var wrapper in controllers) {
            var controller = wrapper.GetController();
            if (controller != null) {
                TrySetTriggerFade(controller, ID, 0, duration);
            }
        }
    }

    public void SetValue(float val, string ID="OnOff") {
        val = Mathf.Clamp(val, 0, 1);
        foreach (var wrapper in controllers) {
            var controller = wrapper.GetController();
            if (controller != null) {
                TrySetTrigger(controller, ID, val);
            }
        }
    }

    public void SetValueFade(float val, float duration, string ID="OnOff") {
        val = Mathf.Clamp(val, 0, 1);
        foreach (var wrapper in controllers) {
            var controller = wrapper.GetController();
            if (controller != null) {
                TrySetTriggerFade(controller, ID, val, duration);
            }
        }
    }
}
