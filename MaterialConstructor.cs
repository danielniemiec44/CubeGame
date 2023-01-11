using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Windows;

public class MaterialConstructor : MonoBehaviour
{
public Material[] materials;

public Texture2D[] texturePreview;

    void Start() {
        materials = new Material[2];
        texturePreview = new Texture2D[2];
        StartCoroutine(LoadTextureFromCache("grass_block", 0));
        StartCoroutine(LoadTextureFromCache("dirt", 1));
    }

    IEnumerator LoadTextureFromCache(string blockName, int index) {
        if(blockName == "grass_block") {
            var www1 = UnityWebRequestTexture.GetTexture("file:///C:/Users/danie/AppData/Roaming/.cubegame/block/" + blockName + "_side.png");
            var www2 = UnityWebRequestTexture.GetTexture("file:///C:/Users/danie/AppData/Roaming/.cubegame/block/" + blockName + "_top.png");
                
            yield return www1.SendWebRequest();
            yield return www2.SendWebRequest();
            
            if(www1.result == UnityWebRequest.Result.Success && www2.result == UnityWebRequest.Result.Success) {
                Texture2D texture1 = DownloadHandlerTexture.GetContent(www1);
                Texture2D texture2 = DownloadHandlerTexture.GetContent(www2);
                Texture2D texture1Rotated = rotateTexture(texture1, false);
                Texture2D texture1Rotated2 = rotateTexture(texture1, true);
                Texture2D texture1Rotated3 = rotateTexture(texture1Rotated, false);
                Texture2D texture2Colored = new Texture2D(texture2.width, texture2.height);

                Texture2D generatedTexture = new Texture2D(texture1.width * 4, texture1.height * 3, UnityEngine.Experimental.Rendering.DefaultFormat.LDR, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
                generatedTexture.filterMode = FilterMode.Trilinear;

                for(int x = 0; x < texture2.width; x++){
                    for(int y = 0; y < texture2.height; y++){
                        //texture2Colored.SetPixel(x, y, Color.Lerp(texture2.GetPixel(x, y), greenColor, Mathf.PingPong(Time.time, 1)));
                        Color color = texture2.GetPixel(x, y);
                        float h;
                        float s;
                        float v;
                        Color.RGBToHSV(color, out h, out s, out v);
                        texture2Colored.SetPixel(x, y, Color.HSVToRGB(78.0f / 360, 55.0f / 100, v));
                    }
                }
                texture2Colored.Apply(false);
                
                Graphics.CopyTexture(texture1, 0, 0, 0, 0, texture1.width, texture1.height, generatedTexture, 0, 0, texture1.width, texture1.height * 2);
                Graphics.CopyTexture(texture1Rotated, 0, 0, 0, 0, texture1Rotated.width, texture1Rotated.height, generatedTexture, 0, 0, 0, texture1Rotated.height);
                Graphics.CopyTexture(texture1Rotated3, 0, 0, 0, 0, texture1Rotated3.width, texture1Rotated3.height, generatedTexture, 0, 0, texture1Rotated3.width, 0);
                Graphics.CopyTexture(texture1Rotated2, 0, 0, 0, 0, texture1Rotated2.width, texture1Rotated2.height, generatedTexture, 0, 0, texture1Rotated2.width * 2, texture1Rotated2.height);
                
                Graphics.CopyTexture(texture2Colored, 0, 0, 0, 0, texture2Colored.width, texture2Colored.height, generatedTexture, 0, 0, texture2Colored.width, texture2Colored.height);
                Graphics.CopyTexture(texture2Colored, 0, 0, 0, 0, texture2Colored.width, texture2Colored.height, generatedTexture, 0, 0, texture2Colored.width * 3, texture2Colored.height);


                materials[index] = new Material(Shader.Find("StandardCullOFF"));
                materials[index].SetTexture("_MainTex", generatedTexture);
                texturePreview[index] = generatedTexture;
            }
        } else {
            var www1 = UnityWebRequestTexture.GetTexture("file:///C:/Users/danie/AppData/Roaming/.cubegame/block/" + blockName + ".png");
   
            yield return www1.SendWebRequest();
            
            if(www1.result == UnityWebRequest.Result.Success) {
                Texture2D texture1 = DownloadHandlerTexture.GetContent(www1);
                Texture2D texture1Rotated = rotateTexture(texture1, false);
                Texture2D texture1Rotated2 = rotateTexture(texture1, true);
                Texture2D texture1Rotated3 = rotateTexture(texture1Rotated, false);

                Texture2D generatedTexture = new Texture2D(texture1.width * 4, texture1.height * 3, UnityEngine.Experimental.Rendering.DefaultFormat.LDR, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
                generatedTexture.filterMode = FilterMode.Trilinear;
                
                Graphics.CopyTexture(texture1, 0, 0, 0, 0, texture1.width, texture1.height, generatedTexture, 0, 0, texture1.width, texture1.height * 2);
                Graphics.CopyTexture(texture1Rotated, 0, 0, 0, 0, texture1Rotated.width, texture1Rotated.height, generatedTexture, 0, 0, 0, texture1Rotated.height);
                Graphics.CopyTexture(texture1Rotated3, 0, 0, 0, 0, texture1Rotated3.width, texture1Rotated3.height, generatedTexture, 0, 0, texture1Rotated3.width, 0);
                Graphics.CopyTexture(texture1Rotated2, 0, 0, 0, 0, texture1Rotated2.width, texture1Rotated2.height, generatedTexture, 0, 0, texture1Rotated2.width * 2, texture1Rotated2.height);
                
                Graphics.CopyTexture(texture1, 0, 0, 0, 0, texture1.width, texture1.height, generatedTexture, 0, 0, texture1.width, texture1.height);
                Graphics.CopyTexture(texture1, 0, 0, 0, 0, texture1.width, texture1.height, generatedTexture, 0, 0, texture1.width * 3, texture1.height);


                materials[index] = new Material(Shader.Find("StandardCullOFF"));
                materials[index].SetTexture("_MainTex", generatedTexture);
                texturePreview[index] = generatedTexture;
            }
        }
    }


    Texture2D rotateTexture(Texture2D originalTexture, bool clockwise)
     {
         Color32[] original = originalTexture.GetPixels32();
         Color32[] rotated = new Color32[original.Length];
         int w = originalTexture.width;
         int h = originalTexture.height;
 
         int iRotated, iOriginal;
 
         for (int j = 0; j < h; ++j)
         {
             for (int i = 0; i < w; ++i)
             {
                 iRotated = (i + 1) * h - j - 1;
                 iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                 rotated[iRotated] = original[iOriginal];
             }
         }
 
         Texture2D rotatedTexture = new Texture2D(h, w);
         rotatedTexture.SetPixels32(rotated);
         rotatedTexture.Apply();
         return rotatedTexture;
     }


}
