using UnityEngine;
using System.Collections;

public class screen : MonoBehaviour {

    public float tic = 0.25f;
    float timer = 0.25f;
    int i = 0;
    public int supersize = 1;

    void Start() { timer = tic; }

    void Update()
    {
        if (timer <= 0) { ScreenCapture.CaptureScreenshot("C:\\Users\\Alvin\\Desktop\\droneslife\\A_Screenshot" + i++ + ".png", supersize); timer += tic; }
        else timer -= Time.deltaTime;

    }

}
