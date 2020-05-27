using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayer : MonoBehaviour
{
    public bool finished = false;
    private UnityEngine.Video.VideoPlayer vp;
    
    // Start is called before the first frame update
    void Start() {
        vp = gameObject.GetComponent<UnityEngine.Video.VideoPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (vp.frame.Equals(vp.frameCount)) {
            finished = true;
        }
    }
}
