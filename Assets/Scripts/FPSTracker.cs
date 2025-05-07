using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSTracker : MonoBehaviour
{
    // Start is called before the first frame update
    private int firstSecondFrames = 0;
    private int secondSecondFrames = 0;
    private int thirdSecondFrames = 0;
    private int fourthSecondFrames = 0;
    private int fifthSecondFrames = 0;
    public int fps = 0;
    public float timer = 1;
    void Start()
    {
        
    }
    //just tracks fps cause I wanted it
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0)
        {
			firstSecondFrames++;
		}
        else
        {
            fps = (int)Mathf.Round((firstSecondFrames+secondSecondFrames+thirdSecondFrames+fourthSecondFrames+fifthSecondFrames)/5.0f);
            //Debug.Log(fps);
            fifthSecondFrames = fourthSecondFrames;
            fourthSecondFrames = thirdSecondFrames;
            thirdSecondFrames = secondSecondFrames;
            secondSecondFrames = firstSecondFrames;
            firstSecondFrames = 0;
            timer = 1;
        }
    }
}
