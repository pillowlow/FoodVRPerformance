using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using System.Linq;


/// <summary>
/// This class provide a fast and general control socket for the on/off control of single compound VFX instance containing VFX Graph, Shaders and Lights Effect
/// </summary>



public enum SocketStatus{
        FADE_FLOAT,
        BOOL,
        SINGLE_FLOAT,
        SINGLE_COLOR,
        FADE_COLOR,
}
public class VFX_Switch : MonoBehaviour

{   
    [System.Serializable]
    public struct ControlSocket
    {   
        // show all the time
        public bool is_Gradient;
        public bool is_VFX;
        public bool is_Mat;
        public bool is_Light;
        public SocketStatus status;
        // show if not (is_Light == true && is_Mat, is_VFX ==false)
        public string para_name;
        
        // show only if the status is FADE_COLOR || FADE_FLOAT
        public float duration;
        // show only if the status is FADE_FLOAT || SINGLE_FLOAT
        public float off;   
        public float on;
        // show only if is_Gradient
        public float start;   
        public float end;
        public float percent;
        // show only if the status is  SINGLE_COLOR || FADE_COLOR
        [ColorUsage(true, true)] 
        public Color hdrColorOn;
        [ColorUsage(true, true)] 
        public Color hdrColorOff;
    }
    [SerializeField]
    //show if 
    protected List<ControlSocket> SocketList = new();

    [SerializeField]
    protected List<VisualEffect> VFXList = new ();
    [SerializeField]
    protected List<Material> MatList = new();
    [SerializeField]
    protected List<Light> LightList = new();
    protected Dictionary<string,bool> BoolFlags = new();
    // for mats 
    protected List<GameObject> ObjTree =new();
    protected List<Material> InstMatList = new();



    // Start is called before the first frame update
    void Start()
    {   
        // get all child
        TraAddObjTree(this.gameObject);
        //Debug.Log("Objtree:" + ObjTree.Count);
        // cloning materials
        FillMatInstance();
        Debug.Log("InstMat:" + InstMatList.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetOn(){
        
        foreach(ControlSocket socket in SocketList){
            if(!socket.is_Gradient){
            Debug.Log("ON");
            // single 
            if(socket.status == SocketStatus.BOOL){    
                // set VFX
                    if(socket.is_VFX){
                       VisualEffect VFX = VFXList.Find(vfx => vfx.HasBool(socket.para_name));
                       if(VFX!=null){
                            VFX.SetBool(socket.para_name,true);
                       }
                       else{
                            Debug.Log("cant find VFX with "+ socket.para_name + " para");
                       }
                       
                    }
                    // set Mat
                    if(socket.is_Mat){
                        Material Mat = InstMatList.Find(Mat => Mat.HasFloat(socket.para_name));
                        if(Mat!=null){
                            Mat.SetFloat(socket.para_name,1);
                        }
                        else{
                            Debug.Log("cant find Mat with "+ socket.para_name + " para");
                       }
                        
                    }
                    //set Light
                    if(socket.is_Light){
                        foreach(Light light in LightList){
                            light.intensity = socket.on;
                        }
                    }
            }
            //SINGLE_FLOAT
            if(socket.status == SocketStatus.SINGLE_FLOAT){
                if(socket.is_VFX){
                    VisualEffect VFX = VFXList.Find(vfx => vfx.HasFloat(socket.para_name));
                    if(VFX!=null){
                        VFX.SetFloat(socket.para_name,socket.on);
                    }
                    else{
                        Debug.Log("cant find VFX with "+ socket.para_name + " para");
                    }

                }
                // set Mat
                if(socket.is_Mat){
                    Material Mat = InstMatList.Find(Mat => Mat.HasFloat(socket.para_name));
                    if(Mat!=null){
                        Mat.SetFloat(socket.para_name,socket.on);
                    }
                    else{
                        Debug.Log("cant find Mat with "+ socket.para_name + " para");
                    }
                    
                }
                //set Light
                if(socket.is_Light){
                    foreach(Light light in LightList){
                        light.intensity = socket.on;
                    }
                }
            }
            //Fade
            if(socket.status == SocketStatus.FADE_FLOAT){
                if(socket.is_VFX){
                    VisualEffect VFX = VFXList.Find(vfx => vfx.HasFloat(socket.para_name));
                    if(VFX!=null){
                        StartCoroutine(FadeVFX(socket.para_name,VFX,socket.off,socket.on,socket.duration));
                    }
                    else{
                        Debug.Log("cant find VFX with "+ socket.para_name + " para");
                    }

                }
                // set Mat
                if(socket.is_Mat){
                    Material Mat = InstMatList.Find(Mat => Mat.HasFloat(socket.para_name));
                    if(Mat!=null){
                        StartCoroutine(FadeMat(socket.para_name,Mat,socket.off,socket.on,socket.duration));
                    }
                    else{
                        Debug.Log("cant find Mat with "+ socket.para_name + " para");
                    }
                    
                }
                //set Light
                if(socket.is_Light){
                    foreach(Light light in LightList){
                        StartCoroutine(FadeLight(light,socket.off,socket.on,socket.duration));
                    }
                }
            }
            //SINGLE_COLOR
            if(socket.status == SocketStatus.SINGLE_COLOR){
                if(socket.is_VFX){
                    VisualEffect VFX = VFXList.Find(vfx => vfx.GetVector4(socket.para_name)!=null);
                    if(VFX!=null){
                        VFX.SetVector4(socket.para_name,HdrColor2Vector4(socket.hdrColorOn));
                    }
                    else{
                        Debug.Log("cant find VFX with "+ socket.para_name + " para");
                    }

                }
                // set Mat
                if(socket.is_Mat){
                    Material Mat = InstMatList.Find(Mat => Mat.HasColor(socket.para_name));
                    if(Mat!=null){
                        Mat.SetColor(socket.para_name,socket.hdrColorOn);
                    }
                    else{
                        Debug.Log("cant find Mat with "+ socket.para_name + " para");
                    }
                    
                }
                //set Light
                if(socket.is_Light){
                    foreach(Light light in LightList){
                        light.color = socket.hdrColorOn;
                    }
                }
            }
            //FADE_COLOR
             if(socket.status == SocketStatus.FADE_COLOR){
                if(socket.is_VFX){
                    VisualEffect VFX = VFXList.Find(vfx => vfx.GetVector4(socket.para_name)!=null);
                    if(VFX!=null){
                        StartCoroutine(FadeVFXColor(socket.para_name,VFX,socket.hdrColorOff,socket.hdrColorOn,socket.duration));
                    }
                    else{
                        Debug.Log("cant find VFX with "+ socket.para_name + " para");
                    }

                }
                // set Mat
                if(socket.is_Mat){
                    Material Mat = InstMatList.Find(Mat => Mat.HasColor(socket.para_name));
                    if(Mat!=null){
                        StartCoroutine(FadeMatColor(socket.para_name,Mat,socket.hdrColorOff,socket.hdrColorOn,socket.duration));
                    }
                    else{
                        Debug.Log("cant find Mat with "+ socket.para_name + " para");
                    }
                    
                }
                //set Light
                if(socket.is_Light){
                    foreach(Light light in LightList){
                        StartCoroutine(FadeLightColor(light,socket.hdrColorOff,socket.hdrColorOn,socket.duration));
                    }
                }
            }
        }
        }
        return;
    }
    // set off
    public void SetOff(){
        Debug.Log("OFF");
        foreach(ControlSocket socket in SocketList){
            // single 
            if(!socket.is_Gradient){
            if(socket.status == SocketStatus.BOOL){    
                // set VFX
                    if(socket.is_VFX){
                       VisualEffect VFX = VFXList.Find(vfx => vfx.HasBool(socket.para_name));
                       if(VFX!=null){
                            VFX.SetBool(socket.para_name,false);
                       }
                       else{
                            Debug.Log("cant find VFX with "+ socket.para_name + " para");
                       }
                       
                    }
                    // set Mat
                    if(socket.is_Mat){
                        Material Mat = InstMatList.Find(Mat => Mat.HasFloat(socket.para_name));
                        if(Mat!=null){
                            Mat.SetFloat(socket.para_name,0);
                        }
                        else{
                            Debug.Log("cant find Mat with "+ socket.para_name + " para");
                       }
                        
                    }
                    //set Light
                    if(socket.is_Light){
                        foreach(Light light in LightList){
                            light.intensity = socket.off;
                        }
                    }
            }
            //SINGLE_FLOAT
            if(socket.status == SocketStatus.SINGLE_FLOAT){
                if(socket.is_VFX){
                    VisualEffect VFX = VFXList.Find(vfx => vfx.HasFloat(socket.para_name));
                    if(VFX!=null){
                        VFX.SetFloat(socket.para_name,socket.off);
                    }
                    else{
                        Debug.Log("cant find VFX with "+ socket.para_name + " para");
                    }

                }
                // set Mat
                if(socket.is_Mat){
                    Material Mat = InstMatList.Find(Mat => Mat.HasFloat(socket.para_name));
                    if(Mat!=null){
                        Mat.SetFloat(socket.para_name,socket.off);
                    }
                    else{
                        Debug.Log("cant find Mat with "+ socket.para_name + " para");
                    }
                    
                }
                //set Light
                if(socket.is_Light){
                    foreach(Light light in LightList){
                        light.intensity = socket.off;
                    }
                }
            }
            //Fade_FLOAT
            if(socket.status == SocketStatus.FADE_FLOAT){
                if(socket.is_VFX){
                    VisualEffect VFX = VFXList.Find(vfx => vfx.HasFloat(socket.para_name));
                    if(VFX!=null){
                        StartCoroutine(FadeVFX(socket.para_name,VFX,socket.on,socket.off,socket.duration));
                    }
                    else{
                        Debug.Log("cant find VFX with "+ socket.para_name + " para");
                    }

                }
                // set Mat
                if(socket.is_Mat){
                    Material Mat = InstMatList.Find(Mat => Mat.HasFloat(socket.para_name));
                    if(Mat!=null){
                        StartCoroutine(FadeMat(socket.para_name,Mat,socket.on,socket.off,socket.duration));
                    }
                    else{
                        Debug.Log("cant find Mat with "+ socket.para_name + " para");
                    }
                    
                }
                //set Light
                if(socket.is_Light){
                    foreach(Light light in LightList){
                        StartCoroutine(FadeLight(light,socket.on,socket.off,socket.duration));
                    }
                }
            }
            //SINGLE_COLOR
            if(socket.status == SocketStatus.SINGLE_COLOR){
                if(socket.is_VFX){
                    VisualEffect VFX = VFXList.Find(vfx => vfx.GetVector4(socket.para_name)!=null);
                    if(VFX!=null){
                        VFX.SetVector4(socket.para_name,HdrColor2Vector4(socket.hdrColorOff));
                    }
                    else{
                        Debug.Log("cant find VFX with "+ socket.para_name + " para");
                    }

                }
                // set Mat
                if(socket.is_Mat){
                    Material Mat = InstMatList.Find(Mat => Mat.HasColor(socket.para_name));
                    if(Mat!=null){
                        Mat.SetColor(socket.para_name,socket.hdrColorOff);
                    }
                    else{
                        Debug.Log("cant find Mat with "+ socket.para_name + " para");
                    }
                    
                }
                //set Light
                if(socket.is_Light){
                    foreach(Light light in LightList){
                        light.color = socket.hdrColorOff;
                    }
                }
            }
            //FADE_COLOR
             if(socket.status == SocketStatus.FADE_COLOR){
                if(socket.is_VFX){
                    VisualEffect VFX = VFXList.Find(vfx => vfx.GetVector4(socket.para_name)!=null);
                    if(VFX!=null){
                        StartCoroutine(FadeVFXColor(socket.para_name,VFX,socket.hdrColorOn,socket.hdrColorOff,socket.duration));
                    }
                    else{
                        Debug.Log("cant find VFX with "+ socket.para_name + " para");
                    }

                }
                // set Mat
                if(socket.is_Mat){
                    Material Mat = InstMatList.Find(Mat => Mat.HasColor(socket.para_name));
                    if(Mat!=null){
                        StartCoroutine(FadeMatColor(socket.para_name,Mat,socket.hdrColorOn,socket.hdrColorOff,socket.duration));
                    }
                    else{
                        Debug.Log("cant find Mat with "+ socket.para_name + " para");
                    }
                    
                }
                //set Light
                if(socket.is_Light){
                    foreach(Light light in LightList){
                        StartCoroutine(FadeLightColor(light,socket.hdrColorOn,socket.hdrColorOff,socket.duration));
                    }
                }
            }
            }
        }
        return;
    }



    public void SetPercent(float percent){
        foreach(ControlSocket socket in SocketList){
            // single 
            if(socket.is_Gradient){
            float target = Mathf.Lerp(socket.start,socket.end,percent);
            Color target_color = Color.Lerp(socket.hdrColorOn,socket.hdrColorOff,percent);
            //SINGLE_FLOAT
            if(socket.status == SocketStatus.SINGLE_FLOAT){
                if(socket.is_VFX){
                    VisualEffect VFX = VFXList.Find(vfx => vfx.HasFloat(socket.para_name));
                    if(VFX!=null){
                        VFX.SetFloat(socket.para_name,target);
                    }
                    else{
                        Debug.Log("cant find VFX with "+ socket.para_name + " para");
                    }

                }
                // set Mat
                if(socket.is_Mat){
                    Material Mat = InstMatList.Find(Mat => Mat.HasFloat(socket.para_name));
                    if(Mat!=null){
                        Mat.SetFloat(socket.para_name,target);
                    }
                    else{
                        Debug.Log("cant find Mat with "+ socket.para_name + " para");
                    }
                    
                }
                //set Light
                if(socket.is_Light){
                    foreach(Light light in LightList){
                        light.intensity = target;
                    }
                }
            }
            //Fade_FLOAT
            if(socket.status == SocketStatus.FADE_FLOAT){
                if(socket.is_VFX){
                    VisualEffect VFX = VFXList.Find(vfx => vfx.HasFloat(socket.para_name));
                    if(VFX!=null){
                        StartCoroutine(FadeVFX(socket.para_name,VFX,VFX.GetFloat(socket.para_name),target,socket.duration));
                    }
                    else{
                        Debug.Log("cant find VFX with "+ socket.para_name + " para");
                    }

                }
                // set Mat
                if(socket.is_Mat){
                    Material Mat = InstMatList.Find(Mat => Mat.HasFloat(socket.para_name));
                    if(Mat!=null){
                        StartCoroutine(FadeMat(socket.para_name,Mat,Mat.GetFloat(socket.para_name),target,socket.duration));
                    }
                    else{
                        Debug.Log("cant find Mat with "+ socket.para_name + " para");
                    }
                    
                }
                //set Light
                if(socket.is_Light){
                    foreach(Light light in LightList){
                        StartCoroutine(FadeLight(light,light.intensity,target,socket.duration));
                    }
                }
            }
            //SINGLE_COLOR
            if(socket.status == SocketStatus.SINGLE_COLOR){
                if(socket.is_VFX){
                    VisualEffect VFX = VFXList.Find(vfx => vfx.GetVector4(socket.para_name)!=null);
                    if(VFX!=null){
                        VFX.SetVector4(socket.para_name,HdrColor2Vector4(target_color));
                    }
                    else{
                        Debug.Log("cant find VFX with "+ socket.para_name + " para");
                    }

                }
                // set Mat
                if(socket.is_Mat){
                    Material Mat = InstMatList.Find(Mat => Mat.HasColor(socket.para_name));
                    if(Mat!=null){
                        Mat.SetColor(socket.para_name,target_color);
                    }
                    else{
                        Debug.Log("cant find Mat with "+ socket.para_name + " para");
                    }
                    
                }
                //set Light
                if(socket.is_Light){
                    foreach(Light light in LightList){
                        light.color = target_color;
                    }
                }
            }
            //FADE_COLOR
             if(socket.status == SocketStatus.FADE_COLOR){
                if(socket.is_VFX){
                    VisualEffect VFX = VFXList.Find(vfx => vfx.GetVector4(socket.para_name)!=null);
                    if(VFX!=null){
                        StartCoroutine(FadeVFXColor(socket.para_name,VFX,VFX.GetVector4(socket.para_name),target_color,socket.duration));
                    }
                    else{
                        Debug.Log("cant find VFX with "+ socket.para_name + " para");
                    }

                }
                // set Mat
                if(socket.is_Mat){
                    Material Mat = InstMatList.Find(Mat => Mat.HasColor(socket.para_name));
                    if(Mat!=null){
                        StartCoroutine(FadeMatColor(socket.para_name,Mat,Mat.GetColor(socket.para_name),target_color,socket.duration));
                    }
                    else{
                        Debug.Log("cant find Mat with "+ socket.para_name + " para");
                    }
                    
                }
                //set Light
                if(socket.is_Light){
                    foreach(Light light in LightList){
                        StartCoroutine(FadeLightColor(light,light.color,target_color,socket.duration));
                    }
                }
            }
            }
        }
        return;
    }




    // set to value





    IEnumerator FadeVFX(string para, VisualEffect vfx,float start,float end,float duration){
        
        string key = "VFX" + vfx.gameObject.name + vfx.name + para;
         Debug.Log("start fade " + key);
        // if first time, register
        if(!BoolFlags.ContainsKey(key)){
            BoolFlags.Add(key,false);
        }
        // if is fading
        if(BoolFlags[key]){
            yield break;
        }
        // fade processing 
        BoolFlags[key]= true;
        //start fade
        float timer = 0.0f;
        while(timer <= duration){
            vfx.SetFloat(para,Mathf.Lerp(start,end,timer/duration));
            timer+=Time.deltaTime;
            yield return null;
        }
        vfx.SetFloat(para, end);
        BoolFlags[key]= false;
        Debug.Log("end fade " + key);
        yield return null;

    }
    IEnumerator FadeMat(string para, Material mat,float start,float end,float duration){
        
        string key = "Mat" + mat.name + para;
        Debug.Log("start fade " + key);
        // if first time, register
        if(!BoolFlags.ContainsKey(key)){
            BoolFlags.Add(key,false);
        }
        // if is fading
        if(BoolFlags[key]){
            yield break;
        }
        // fade processing 
        BoolFlags[key]= true;
        //start fade
        float timer = 0.0f;
        while(timer <= duration){
            mat.SetFloat(para,Mathf.Lerp(start,end,timer/duration));
            timer+=Time.deltaTime;
            yield return null;
        }
        mat.SetFloat(para, end);
        BoolFlags[key]= false;
        Debug.Log("end fade " + key);
        yield return null;

    }
    IEnumerator FadeLight(Light light,float start,float end,float duration){
        string key = "Light" + light.gameObject.name;
        Debug.Log("start fade " + key);
        // if first
        if(!BoolFlags.ContainsKey(key)){
            BoolFlags.Add(key,false);
        }
        // if is fading
        if(BoolFlags[key]){
            yield break;
        }
        // fade processing 
        BoolFlags[key]= true;
        float timer = 0.0f;
        while(timer <= duration){
            light.intensity = Mathf.Lerp(start,end,timer/duration);
            timer+=Time.deltaTime;
            yield return null;
        }
        light.intensity = end;
        Debug.Log("end fade " + key);
        BoolFlags[key]= false;
        yield return null;
    }

    IEnumerator FadeVFXColor(string colorParam, VisualEffect vfx, Color startColor, Color endColor, float duration)
    {
        string key = "VFXColor" + vfx.name + colorParam;
        Debug.Log("Start Vfx color fade: " + key);

        // If first time, register
        if (!BoolFlags.ContainsKey(key))
        {
            BoolFlags.Add(key, false);
        }
        // If is fading
        if (BoolFlags[key])
        {
            yield break;
        }
        // Fade processing
        BoolFlags[key] = true;

        // Start fade
        float timer = 0.0f;
        while (timer < duration)
        {
            Color currentColor = Color.Lerp(startColor, endColor, timer / duration);
            vfx.SetVector4(colorParam, HdrColor2Vector4(currentColor));
            timer += Time.deltaTime;
            yield return null;
        }
         // Ensure the final color is set to 'endColor'
        vfx.SetVector4(colorParam, endColor);

        BoolFlags[key] = false;
        Debug.Log("End vfx color fade: " + key);
    }

    IEnumerator FadeMatColor(string colorParam, Material mat, Color startColor, Color endColor, float duration)
    {
        string key = "MatColor" + mat.name + colorParam;
        Debug.Log("Start mat color fade: " + key);

        // If first time, register
        if (!BoolFlags.ContainsKey(key))
        {
            BoolFlags.Add(key, false);
        }
        // If is fading
        if (BoolFlags[key])
        {
            yield break;
        }
        // Fade processing
        BoolFlags[key] = true;

        // Start fade
        float timer = 0.0f;
        while (timer < duration)
        {
            Color currentColor = Color.Lerp(startColor, endColor, timer / duration);
            mat.SetColor(colorParam, currentColor);
            timer += Time.deltaTime;
            yield return null;
        }
         // Ensure the final color is set to 'endColor'
        mat.SetColor(colorParam, endColor);

        BoolFlags[key] = false;
        Debug.Log("End mat color fade: " + key);
    }

    IEnumerator FadeLightColor( Light light, Color startColor, Color endColor, float duration)
    {
        string key = "LightColor" + light.gameObject.name ;
        Debug.Log("Start light color fade: " + key);

        // If first time, register
        if (!BoolFlags.ContainsKey(key))
        {
            BoolFlags.Add(key, false);
        }
        // If is fading
        if (BoolFlags[key])
        {
            yield break;
        }
        // Fade processing
        BoolFlags[key] = true;

        // Start fade
        float timer = 0.0f;
        while (timer < duration)
        {
            Color currentColor = Color.Lerp(startColor, endColor, timer / duration);
            light.color= currentColor;
            timer += Time.deltaTime;
            yield return null;
        }
         // Ensure the final color is set to 'endColor'
        light.color = endColor;

        BoolFlags[key] = false;
        Debug.Log("End light color fade: " + key);
    }

    Vector4 HdrColor2Vector4(Color hdrColor)
    {
        Vector4 colorVector = new Vector4(hdrColor.r, hdrColor.g, hdrColor.b, hdrColor.a);
        return colorVector;
    }


    void TraAddObjTree(GameObject Obj){
        if(!ObjTree.Contains(Obj)){
            ObjTree.Add(Obj);
        }
        foreach(Transform child in Obj.transform){
            TraAddObjTree(child.gameObject);
        }
    }

    void FillMatInstance(){
        foreach(GameObject obj in ObjTree){
            Renderer renderer = obj.GetComponent<Renderer>();
            if(renderer!=null){
                // applying clone 
                Material[] newMaterials = renderer.materials;

                for (int i = 0; i < newMaterials.Length; i++)
                {
                    Material originalMat = newMaterials[i];

                    foreach (Material mat in MatList)
                    {
                        // Check if the material names match up to the length of the name in MatList
                        if (originalMat.name.StartsWith(mat.name))
                        {
                            // Clone the original material and perform other operations
                            Material InsMat = new Material(originalMat);
                            newMaterials[i] = InsMat;
                            InstMatList.Add(InsMat);

                            // Optional: Log the matching material name
                            Debug.Log("Matching material name: " + mat.name);
                            break; // Exit the loop once a match is found
                        }
                    }
                }

                renderer.materials = newMaterials;
            }
            
        }
    }
}

