using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{   
    [System.Serializable]
    public struct Lightsocket{
        
        public Material Mat;
        public List<FloatParaSet> flaotParaList;
        public List<ColorParaSet> colorParaList;

    }
    [System.Serializable]
    public struct Trigger{
        public string name;
        public List<Lightsocket> SocketList;
        
    }
    
    [SerializeField]
    protected List<Trigger> TriggerList;

    protected Dictionary<string,bool>FadeFlags = new();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
