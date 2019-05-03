using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Osc;

public class OscManager : MonoBehaviour {

    const string OSC_PATH_COLOR = "/color";

    public void OnRecieve(OscPort.Capsule capsule)
    {
        var msg = capsule.message;

        if (msg.path == OSC_PATH_COLOR)
        {
            float r = ((float)msg.data[0]);
            float g = ((float)msg.data[1]);
            float b = ((float)msg.data[2]);

            Debug.Log("Test " + r + " " + g + " " + b);
        }
    }

}
