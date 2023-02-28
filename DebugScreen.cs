using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;


public class DebugScreen : MonoBehaviour
{
    GameObject Player;
    Transform PlayerTransform;
    public TextMeshProUGUI leftSideText;
    int[] frames;
    int framesBehind = 0;
    int maxFramesBehind = 10;
    public int avgFrames = 0;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player");
        PlayerTransform = Player.transform;
        frames = new int[maxFramesBehind];
    }

    // Update is called once per frame
    void Update()
    {
        string leftSideString = "";

        float current = (int)(1f / Time.unscaledDeltaTime);
        if(framesBehind < maxFramesBehind) {
            frames[framesBehind] = (int) current;
        } else {
            avgFrames = (frames.Sum() / frames.Count());
            framesBehind = 0;
        }
        leftSideString += avgFrames + "FPS\n";
        framesBehind ++;


        
        
        float x = PlayerTransform.position.x;
        float y = PlayerTransform.position.y;
        float z = PlayerTransform.position.z;
        leftSideString += "x: " + String.Format("{0:0.00}", x) + ", y: " + String.Format("{0:0.00}", y) + ", z: " + String.Format("{0:0.00}", z) + "\n";
        leftSideString += "Chunk x: " + MeshUpdater.playerChunkX + ", Chunk z: " + MeshUpdater.playerChunkZ;
        leftSideText.SetText(leftSideString);
    }
}
