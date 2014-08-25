using UnityEngine;
using System.Collections;
using Leap;

using System;
using System.IO;
using System.Net.Sockets;

public class GestureManager : MonoBehaviour {

	// Initialise Variables

	Controller controller;
	float main_max_intensity = 03.5f;
	float sub_max_intensity = 01.0f;	
	float max_dlIntensity = 0.53f;
	float max_dllowIntensity = 0.3f;
	int leftHandGrabCount = 0;
	int rightHandGrabCount = 0;


	[SerializeField] float main_lights_rate = 0.8f;
	[SerializeField] float second_lights_rate = 0.8f;

	[SerializeField] int numTurns = 100;


	GameObject main_light_1;
	GameObject main_light_2;
	GameObject main_light_3;

	GameObject sub_light_1;
	GameObject sub_light_2;

	GameObject directional_light;


	int lights1_REAL_intensity = 1;
	int lights2_REAL_intensity = 1;

	int rightCWCount = 0;
	int rightCCWCount = 0;
	int leftCWcount = 0;
	int leftCCWcount = 0;

	


	int mycounter = 0;

	Boolean socketReady = false;
	TcpClient mySocket;
	NetworkStream theStream;
	StreamWriter theWriter;
	StreamReader theReader;
	[SerializeField] String Host = "localhost"; // 192.168.1.138
	[SerializeField] Int32 Port = 10002;



	void Start () {

		// Tell the Leap API to enable gesture detection

		controller = new Controller();
		controller.EnableGesture (Gesture.GestureType.TYPE_CIRCLE);
//		controller.EnableGesture (Gesture.GestureType.TYPE_KEY_TAP);
//		controller.EnableGesture (Gesture.GestureType.TYPE_SCREEN_TAP);
//		controller.EnableGesture (Gesture.GestureType.TYPE_SWIPE);	

		// Tell the Unity API to search for these objects in our scene and assign them to variables

		main_light_1 = GameObject.Find("main_light_1");
		main_light_2 = GameObject.Find("main_light_2");
		main_light_3 = GameObject.Find("main_light_3");

		main_light_1.transform.light.intensity = main_max_intensity;
		main_light_2.transform.light.intensity = main_max_intensity;
		main_light_3.transform.light.intensity = main_max_intensity;


		sub_light_1 = GameObject.Find("sub_light_1");
		sub_light_2 = GameObject.Find("sub_light_2");

		sub_light_1.transform.light.intensity = sub_max_intensity;
		sub_light_2.transform.light.intensity = sub_max_intensity;

	
		directional_light = GameObject.Find("Directional light");


		setupSocket ();

		string command = String.Format ("{0},{1}", "$COMMAND,lights1", lights1_REAL_intensity);
		writeSocket (command);
		string command2 = String.Format ("{0},{1}", "$COMMAND,lights2", lights2_REAL_intensity);
		writeSocket (command2);

	}




	void Update () {


		/////
		// socket test code

//		mycounter++;
//
//		if (mycounter == 50) {
//			mycounter = 0;
//
//			setupSocket();
//			writeSocket("Hello from Unity");
//			closeSocket();
//
//
//		}



		/////////////////

		// Initialise the Leap Motion Frame and Gesture objects
			
		Frame frame = controller.Frame();
		GestureList gestures = frame.Gestures();

		// Initialise our music 


		int rightFingers_check = 0;
		int leftFingers_check = 0;
		bool leftHandGrab = false;
		bool rightHandGrab = false;

		// Detect Right or Left hand grabs, and count the number of fingers held out

		foreach (Hand hand in frame.Hands) {
			if(hand.IsRight) {
				rightFingers_check = countExtendedFingers(hand);
				rightHandGrab = detectGrab(hand);
			} else if(hand.IsLeft) {
				leftFingers_check =countExtendedFingers(hand);
				leftHandGrab = detectGrab(hand);
			}
		}


		if (leftHandGrab) {
			leftHandGrabManager();
		}

		if (rightHandGrab) {
			rightHandGrabManager();
		}



		// Handle other gestures (circles and swipes)

		for (int i = 0; i < gestures.Count; i++) {
			Gesture gesture = gestures [i];
			
			switch (gesture.Type) {
				case Gesture.GestureType.TYPE_CIRCLE:
		

					CircleGesture circle = new CircleGesture (gesture);
					float turns = circle.Progress;

					//Calculate clock direction using the angle between circle normal and pointable
					//Clockwise if angle is less than 90 degrees
					if (circle.Pointable.Direction.AngleTo (circle.Normal) <= System.Math.PI / 2) {
						if (turns >= 1) {
							turns = 0;
							if ((rightFingers_check < 3) && (rightFingers_check != 0)) {
								// nada
							} else if (rightFingers_check == 5) {
								rightCircleCWHandler();
							} else if (leftFingers_check == 5) {
								leftCircleCWHandler();
							}
						}

					} else { 
						// counter clock-wise

						if (turns >= 1) {
							turns = 0;
							if ((rightFingers_check < 3) && (rightFingers_check != 0))  {
								// nada
							} else if (rightFingers_check == 5) {
								rightCircleCCWHandler();
							} else if (leftFingers_check == 5) {
								leftCircleCCWHandler();
							}
						}
					}
					
					break;
					
				default:
					//				SafeWriteLine ("  Unknown gesture type.");
					break;
			}
		}
		
	}


	void onApplicationQuit() {

		closeSocket ();

	}






	bool detectGrab (Hand hand)  {
		// return whether or not the hand is clenched into a fist or 'grab' gesture
		
		if (hand.GrabStrength == 1.0f) {
			return true;
		} else {
			return false;
		}
	}
	
	
	int countExtendedFingers (Hand hand) {
		// Count the number of outstreched fingers on this hand
		
		int numberOfFingers = 0;
		
		foreach (Finger finger in hand.Fingers)  {
			if((finger.IsExtended)) numberOfFingers++;
		}
		
		return numberOfFingers;
	}
	

	
	void rightHandGrabManager () {
		
		// Wait until grab has been sustained for long enough
		
		rightHandGrabCount = rightHandGrabCount + 1;
		if (rightHandGrabCount >= 100) {
			rightHandGrabCount = 0;
			toggleBlinds();
		}
		
	}
	
	void leftHandGrabManager () {
		
		// Wait until grab has been sustained for long enough
		
		leftHandGrabCount = leftHandGrabCount + 1;
		if (leftHandGrabCount >= 100) {
			leftHandGrabCount = 0;
		}
		
	}


	void mainLightControl (bool lightsBrighter ) {

		if (lightsBrighter) {
			// Increase light brightness

			if (main_light_1.transform.light.intensity < main_max_intensity) {
				main_light_1.transform.light.intensity = main_light_1.transform.light.intensity + main_lights_rate;
				main_light_2.transform.light.intensity = main_light_2.transform.light.intensity + main_lights_rate;
				main_light_3.transform.light.intensity = main_light_3.transform.light.intensity + main_lights_rate;
			} else {
				main_light_1.transform.light.intensity = main_max_intensity;
				main_light_2.transform.light.intensity = main_max_intensity;
				main_light_3.transform.light.intensity = main_max_intensity;
			}


			if (directional_light.transform.light.intensity < max_dlIntensity) {
				directional_light.transform.light.intensity = directional_light.transform.light.intensity + 0.008f;
			} else {
				directional_light.transform.light.intensity = max_dlIntensity;
			}


		} else {
			// Decrease light brightness

			main_light_1.transform.light.intensity = main_light_1.transform.light.intensity - main_lights_rate;
			main_light_2.transform.light.intensity = main_light_2.transform.light.intensity - main_lights_rate;
			main_light_3.transform.light.intensity = main_light_3.transform.light.intensity - main_lights_rate;

			directional_light.transform.light.intensity = directional_light.transform.light.intensity - 0.0008f;
		}
	
	}


	void secondaryLightControl (bool lightsBrighter ) {
		
		if (lightsBrighter) {
			// Increase light brightness
			if (sub_light_1.transform.light.intensity < sub_max_intensity) {
				sub_light_1.transform.light.intensity = sub_light_1.transform.light.intensity + second_lights_rate;
				sub_light_2.transform.light.intensity = sub_light_1.transform.light.intensity;

			} else {
				sub_light_1.transform.light.intensity = sub_max_intensity;
				sub_light_2.transform.light.intensity = sub_max_intensity;
			}
			
		} else {
			// Decrease light brightness
			sub_light_1.transform.light.intensity = sub_light_1.transform.light.intensity - second_lights_rate;
			sub_light_2.transform.light.intensity = sub_light_1.transform.light.intensity;

			
		}
		
	}


	void rightCircleCWHandler() {
		// RIGHT HAND CLOCKWISE CIRCLE




		rightCWCount++;

		if (rightCWCount == numTurns) {

			mainLightControl(true);


			rightCWCount = 0;
			rightCCWCount = 0;
			leftCWcount = 0;
			leftCCWcount = 0;


			// Control Real Lights
			
			if (lights2_REAL_intensity == 0) {
				lights2_REAL_intensity = 45;
				
			} else if ((lights2_REAL_intensity == 1) || (lights2_REAL_intensity <= 10)) {
				lights2_REAL_intensity = 1;
			} else {
				lights2_REAL_intensity -=10;
			}
			
			string command = String.Format ("{0},{1}", "$COMMAND,lights2", lights2_REAL_intensity);
			writeSocket (command);


		}



	

		
	}
	
	
	void rightCircleCCWHandler() {
		// RIGHT HAND COUNTER CLOCKWISE CIRCLE




		rightCCWCount++;
		
		if (rightCCWCount == numTurns) {

			mainLightControl(false);
			
			rightCWCount = 0;
			rightCCWCount = 0;
			leftCWcount = 0;
			leftCCWcount = 0;


			if ((lights2_REAL_intensity >= 40) || (lights2_REAL_intensity == 0)) {
					lights2_REAL_intensity = 0;
	
			} else if (lights2_REAL_intensity == 1) {
					lights2_REAL_intensity = 5;
			} else {
					lights2_REAL_intensity += 10;
			}

			string command = String.Format ("{0},{1}", "$COMMAND,lights2", lights2_REAL_intensity);
			writeSocket (command);

		}
		
	}


	void leftCircleCWHandler() {
		// LEFT HAND CLOCKWISE CIRCLE


		leftCWcount++;
		
		if (leftCWcount == numTurns) {

			secondaryLightControl (true);
			
			rightCWCount = 0;
			rightCCWCount = 0;
			leftCWcount = 0;
			leftCCWcount = 0;


			if (lights1_REAL_intensity == 0) {
				lights1_REAL_intensity = 45;
					
			} else if ((lights1_REAL_intensity == 1) | (lights1_REAL_intensity <= 10)) {
				lights1_REAL_intensity = 1;
			} else {
				lights1_REAL_intensity -= 10;
			}

			string command = String.Format ("{0},{1}", "$COMMAND,lights1", lights1_REAL_intensity);
			writeSocket (command);




		}


	}


	void leftCircleCCWHandler() {
		// LEFT HAND COUNTER CLOCKWISE CIRCLE


		leftCCWcount++;
		
		if (leftCCWcount == numTurns) {

				secondaryLightControl (false);

			
				rightCWCount = 0;
				rightCCWCount = 0;
				leftCWcount = 0;
				leftCCWcount = 0;

				if ((lights1_REAL_intensity >= 40) || (lights1_REAL_intensity == 0)) {
						lights1_REAL_intensity = 0;
			
				} else if (lights1_REAL_intensity == 1) {
						lights1_REAL_intensity = 5;
				} else {
						lights1_REAL_intensity += 10;
				}

				string command = String.Format ("{0},{1}", "$COMMAND,lights1", lights1_REAL_intensity);
				writeSocket (command);
		}
		
	}







	void rightFistHandler() {		
	
		toggleBlinds ();
	
	
	}



	void toggleBlinds() {


		if (leftCurtainControl.toggle == false) {
			leftCurtainControl.toggle = true;
		}

		if (rightCurtainControl.toggle == false) {
			rightCurtainControl.toggle = true;
		}

		writeSocket("$COMMAND,curtains,1");
	}




	public void setupSocket() {
		try {
			mySocket = new TcpClient(Host, Port);
			theStream = mySocket.GetStream();
			theWriter = new StreamWriter(theStream);
			theReader = new StreamReader(theStream);
			socketReady = true;
		}
		catch (Exception e) {
			Debug.Log("Socket error: " + e);
		}
	}
	public void writeSocket(string theLine) {
		if (!socketReady)
			return;
		String foo = theLine;
		theWriter.Write(foo);
		theWriter.Flush();
	}

	public void closeSocket() {
		if (!socketReady)
			return;
		theWriter.Close();
		theReader.Close();
		mySocket.Close();
		socketReady = false;
	}



}    







