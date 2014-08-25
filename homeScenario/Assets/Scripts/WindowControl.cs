// FILE: WindowControl.cs
// AUTHOR: Varshil Udani and Luke Harries
// DESCRIPTION: Script that makes use of a 3 state FSM to moves the window position from open to close and vice versa

using UnityEngine;
using System.Collections;

public class WindowControl : MonoBehaviour {

	// A signal that comes from the GestureManager.cs to indicate open or close
	public static bool windowOpen;
	
	int mode; // A 3 state FSM
	float window_x_pos;
	float window_y;
	float window_z;
	
	
	void Awake() {
		window_y = -0.1224926f;
		window_z =  0.1793067f;
	}
	
	void Start() {
		mode = 0;
		window_x_pos = -0.02961853f;
		windowOpen = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		//-- Mode 0: window closed
		if(mode == 0) {   
			
			if(windowOpen) {
				mode = 1;
			}
		}
		
		//-- Mode 1: open window
		else if(mode == 1) {
			if(windowOpen) {

				//Keep opening the window until position reached and keep the position there
				//Change mode to Window Opened
				window_x_pos += Time.deltaTime * 0.00780997133333f;
				
				if(window_x_pos >= -0.006188616f) {
					window_x_pos = -0.006188616f;
					mode = 2;
				}
			} else mode = 3;
		}
		
		//-- Mode 2: Open
		else if(mode == 2) {
			if(!windowOpen)
				mode = 3;
		}
		
		//-- Mode 3: close window
		else {
			if(!windowOpen) {

				window_x_pos -= Time.deltaTime * 0.00780997133333f;
				
				if(window_x_pos <= -0.02961853f) {
					window_x_pos = -0.02961853f;
					mode = 2;
				}
			} else mode = 1;
		}
		
		//-- update objects
		this.transform.localPosition = new Vector3(window_x_pos, window_y, window_z);
		
	}
}
