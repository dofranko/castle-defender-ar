using UnityEngine;
using System.Collections;

public class twiddleSettings {

    // time of the experiment. It'll increase by 'step', until it reaches 'maxTime'
    public static float timelimit = 3f;
    public static float maxTime = 30;
    public static float step = 0.25f;
    public static float increaseTimeLimit() { timelimit += step; timelimit = (timelimit >= maxTime ? maxTime : timelimit); return timelimit;  }
     
}
