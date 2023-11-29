using System.Collections;
using System.Collections.Generic;
using UnityEngine;

  [System.Serializable]
 
    public struct FloatParaSet{
       
        public string para;
         [Tooltip("default min")]
        public float val1;
         [Tooltip("default max")]
        public float val2;
    }

     [System.Serializable]
    public struct ColorParaSet{
        public string para;
        [ColorUsage(true, true)] 
        public Color color1;
        [ColorUsage(true, true)] 
        public Color color2;
    }

     [System.Serializable]
     public struct BoolParaSet{
        public string para;
        [Range(0, 1)] 
        public float threshold;
     }

     public struct MatInstSet{
        public string Obj_name;
        public Material Mat;
      
     }

     public enum LightState{
      
     }
    