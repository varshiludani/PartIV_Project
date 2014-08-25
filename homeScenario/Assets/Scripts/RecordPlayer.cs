// FILE: RecordPlayer.cs
// AUTHOR: Varshil Udani and Luke Harries
// DESCRIPTION: Script that animates record player animations using 3 state FSM. 
//              Makes use of the original recordPlayer script from Asset Store.

using UnityEngine;
using System.Collections;

public class RecordPlayer : MonoBehaviour {

    // A signal that comes from the GestureManager.cs to indicate On or Off
    public static bool recordPlayerActive;

    GameObject disc;
    GameObject arm;

    int mode;
    float armAngle;
    float discAngle;
    float discSpeed;


void Awake() {
    disc = gameObject.transform.Find("teller").gameObject;
    arm = gameObject.transform.Find("arm").gameObject;
}

void Start() {
    mode = 0;
    armAngle = 0.0f;
    discAngle = 0.0f;
    discSpeed = 0.0f;
	recordPlayerActive = false;
}

void Update() {
    //-- Mode 0: player off
    if(mode == 0) {   
        if(recordPlayerActive == true)
            mode = 1;
    }
    
    //-- Mode 1: activation
    else if(mode == 1) {
        if(recordPlayerActive == true) {
            armAngle += Time.deltaTime * 30.0f;
            
            if(armAngle >= 30.0f) {
                armAngle = 30.0f;
                mode = 2;
            }
            
            discAngle += Time.deltaTime * discSpeed;
            discSpeed += Time.deltaTime * 80.0f;
        }
        else mode = 3;
    }
    
    //-- Mode 2: running
    else if(mode == 2) {
        if(recordPlayerActive == true)
            discAngle += Time.deltaTime * discSpeed;
        else mode = 3;
    }
    
    //-- Mode 3: stopping
    else {
        if(recordPlayerActive == false) {
            armAngle -= Time.deltaTime * 30.0f;
            
            if(armAngle <= 0.0f)
                armAngle = 0.0f;

            discAngle += Time.deltaTime * discSpeed;
            discSpeed -= Time.deltaTime * 80.0f;
            
            if(discSpeed <= 0.0f)
                discSpeed = 0.0f;

            if((discSpeed == 0.0f) && (armAngle == 0.0f))
                mode = 0;
        }
        else mode = 1;
    }

    //-- update objects
    arm.transform.localEulerAngles = new Vector3(0.0f, armAngle, 0.0f);
    disc.transform.localEulerAngles = new Vector3(0.0f, discAngle, 0.0f);
}

}
