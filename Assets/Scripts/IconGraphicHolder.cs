using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class IconGraphicHolder : Singleton<IconGraphicHolder>
{ 
    float currentX = 0;
    public GenericDictionary<string,Texture2D> dict = new GenericDictionary<string, Texture2D>();

    public void Wipe(){
        currentX = 0;
        dict.Clear();
    }
    
    public void MakeIcon(Character c)
    {

        CharacterGraphic model = CharacterBuilder.inst.GenerateGraphic(c);


        foreach (var item in model.allRenderers)
        {item.gameObject.layer = 8;}
        model.Orginize(c);
        model.cam.transform.localPosition =  model.camPositionDict[c.species];
       
        RenderTexture texture = new RenderTexture(250,250,16);
        texture.Create();
        model.cam.targetTexture = texture;   
       
        model.transform.SetParent(transform);
        model.transform.localPosition = new Vector3(currentX,0,0);
        currentX +=15;
       

        StartCoroutine(q());
        IEnumerator q(){
            yield return new WaitForEndOfFrame();
            Texture2D t = toTexture2D(model.cam.activeTexture) ;
            dict.Add(c.ID,t);
            Destroy(model.gameObject);
           
        }
    }

    

    public Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D dest = new Texture2D(rTex.width, rTex.height, TextureFormat.RGBA32, false);
        dest.Apply(false);
        Graphics.CopyTexture(rTex, dest);
        return dest;
    }
}