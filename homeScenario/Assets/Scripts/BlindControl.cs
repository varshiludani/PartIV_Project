// FILE: BlindControl.cs
// AUTHOR: Varshil Udani and Luke Harries
// DESCRIPTION: Script that makes use of a 3 state FSM to moves the window position from open to close and vice versa

using UnityEngine;
using System.Collections;

public class BlindControl : MonoBehaviour {

	// A signal that comes from the GestureManager.cs to indicate open or close
	public static bool blindDown;

	int mode;
	float blindVerticalPos;
	float blind_x;
	float blind_y;

	void Awake() {
		blind_x = -0.02969291f;
		blind_y = -0.1233926f;
	}

	void Start() {
		mode = 0;
		blindVerticalPos = 0.2220076f;
		blindDown = false;
	}
	
	// Update is called once per frame
	void Update () {

		//-- Mode 0: player off
		if(mode == 0) {   
			if(blindDown) {
				mode = 1;
			}
		}
		
		//-- Mode 1: Pull Down Blind
		else if(mode == 1) {
			if(blindDown) {

				blindVerticalPos -= Time.deltaTime * 0.0111965f;

				if(blindVerticalPos <= 0.1884181f) {

					blindVerticalPos = 0.1884181f;
					mode = 2;
				}
			} else mode = 3;
		}

		//-- Mode 2: Down
		else if(mode == 2) {
			if(!blindDown)
				mode = 3;
		}
		
		//-- Mode 3: stopping
		else {
			if(!blindDown) {

				blindVerticalPos += Time.deltaTime * 0.0111965f;

				if(blindVerticalPos >= 0.2220076f) {
					blindVerticalPos = 0.2220076f;
					mode = 2;
				}
			}
			else
				mode = 1;
		}
		
		//-- update objects
		this.transform.localPosition = new Vector3(blind_x, blind_y, blindVerticalPos);
			
	}
}
