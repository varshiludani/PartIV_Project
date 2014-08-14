using UnityEngine;
using System.Collections;
using Leap;



public class GestureManager : MonoBehaviour {

	// Initialise Variables

	Controller controller;
	public AudioClip music;
	float max_plIntensity = 03.3f;
	float max_pl2Intensity = 01.5f;	
	float max_dlIntensity = 0.53f;
	int leftHandGrabCount = 0;
	int rightHandGrabCount = 0;
	bool isTVplaying = false;

	GameObject point_light;
	GameObject point_light2;
	GameObject directional_light;



	void Start () {

		// Tell the Leap API to enable gesture detection

		controller = new Controller();
		controller.EnableGesture (Gesture.GestureType.TYPE_CIRCLE);
		controller.EnableGesture (Gesture.GestureType.TYPE_KEY_TAP);
		controller.EnableGesture (Gesture.GestureType.TYPE_SCREEN_TAP);
		controller.EnableGesture (Gesture.GestureType.TYPE_SWIPE);	

		// Tell the Unity API to search for these objects in our scene and assign them to variables

		point_light = GameObject.Find("point_light");
		point_light2 = GameObject.Find("point_light2");
		directional_light = GameObject.Find("Directional light");

	}

	void Update () {

		// Initialise the Leap Motion Frame and Gesture objects
			
		Frame frame = controller.Frame();
		GestureList gestures = frame.Gestures();

		// Initialise our music 

		audio.clip = music;

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
							mainLightControl(true);
						} else if (rightFingers_check == 5) {
							secondaryLightControl(true);
						} else if (leftFingers_check == 5) {
							increaseAudioVolume(true);
						}
					}

				} else { 
					// counter clock-wise

					if (turns >= 1) {
						turns = 0;
						if ((rightFingers_check < 3) && (rightFingers_check != 0))  {
							mainLightControl(false);
						} else if (rightFingers_check == 5) {
							secondaryLightControl(false);
						} else if (leftFingers_check == 5) {
							increaseAudioVolume(false);
						}
					}
				}
				
				break;
				
			case Gesture.GestureType.TYPE_SWIPE:
				SwipeGesture swipe = new SwipeGesture (gesture);

				//Classify swipe as either horizontal or vertical
				bool isHorizontal = calculateSwipeDirection(swipe);

				if(isHorizontal) {
					// Horizontal

					if(swipe.Direction.x > 0) {
						WindowControl.windowOpen = false;
					} else {
						WindowControl.windowOpen = true;
					}	

				} else  { 
					// Vertical

					if(swipe.Direction.y > 0) {
						BlindControl.blindDown = false;
					} else {
						BlindControl.blindDown = true;
					}                  

				}

				break;
				
			case Gesture.GestureType.TYPE_KEY_TAP:
				KeyTapGesture keytap = new KeyTapGesture (gesture);
				Debug.Log("Key Tap");
				//				SafeWriteLine ("  Tap id: " + keytap.Id
				//				               + ", " + keytap.State
				//				               + ", position: " + keytap.Position
				//				               + ", direction: " + keytap.Direction);
				break;
				
			case Gesture.GestureType.TYPE_SCREEN_TAP:
				ScreenTapGesture screentap = new ScreenTapGesture (gesture);
				Debug.Log("Screen Tap");
				//				SafeWriteLine ("  Tap id: " + screentap.Id
				//				               + ", " + screentap.State
				//				               + ", position: " + screentap.Position
				//				               + ", direction: " + screentap.Direction);
				break;
				
			default:
				//				SafeWriteLine ("  Unknown gesture type.");
				break;
			}
		}
		
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
	
	

	void toggleTV() {
		
		// Check if TV is already on (by default it is not)
		
		if (!isTVplaying)
		{
			TVanim.set_tvOn = true;
			isTVplaying = true;
		}
		else 
		{
			TVanim.set_tvOn = false;
			isTVplaying = false;
		}
		
	}
	
	
	void toggleMusic () {
		
		// Check if music is already playing (by default it is not)
		
		if (!audio.isPlaying) {
			// Start music and initalise Record Player animation
			audio.Play();
			RecordPlayer.recordPlayerActive = true;
		} else {
			// Stop music and animation
			audio.Pause();
			RecordPlayer.recordPlayerActive = false;
			
		}
		
	}
	
	
	void rightHandGrabManager () {
		
		// Wait until grab has been sustained for long enough
		
		rightHandGrabCount = rightHandGrabCount + 1;
		if (rightHandGrabCount >= 100) {
			rightHandGrabCount = 0;
			
			toggleTV();
		}
		
	}
	
	void leftHandGrabManager () {
		
		// Wait until grab has been sustained for long enough
		
		leftHandGrabCount = leftHandGrabCount + 1;
		if (leftHandGrabCount >= 100) {
			leftHandGrabCount = 0;
			toggleMusic();
		}
		
	}


	void mainLightControl (bool lightsBrighter ) {

		if (lightsBrighter) {
			// Increase light brightness

			if (point_light.transform.light.intensity < max_plIntensity)
			{
				point_light.transform.light.intensity = point_light.transform.light.intensity + 0.005f;
			}
			else
			{
				point_light.transform.light.intensity = max_plIntensity;
			}
			if (directional_light.transform.light.intensity < max_dlIntensity)
			{
				directional_light.transform.light.intensity = directional_light.transform.light.intensity + 0.005f;
			}
			else
			{
				directional_light.transform.light.intensity = max_dlIntensity;
			}


		} else {
			// Decrease light brightness

			point_light.transform.light.intensity = point_light.transform.light.intensity - 0.005f;
			directional_light.transform.light.intensity = directional_light.transform.light.intensity - 0.005f;

		}
	
	}


	void secondaryLightControl (bool lightsBrighter ) {
		
		if (lightsBrighter) {
			// Increase light brightness
			if (point_light2.transform.light.intensity < max_pl2Intensity)
			{
				point_light2.transform.light.intensity = point_light2.transform.light.intensity + 0.005f;
			}
			else
			{
				point_light2.transform.light.intensity = max_pl2Intensity;
			}
			
		} else {
			// Decrease light brightness
			point_light2.transform.light.intensity = point_light2.transform.light.intensity - 0.005f;

			
		}
		
	}


	void increaseAudioVolume (bool increase ) {

		if (audio.isPlaying) {
			if (increase) {
				// Increase volume
				audio.volume = audio.volume + 0.001f;
			
			} else {
				// Decrease volume
			
				audio.volume = audio.volume - 0.001f;

			}
		}


	}


	bool calculateSwipeDirection(SwipeGesture swipe) {

		if ((System.Math.Abs (swipe.Direction.x)) > (System.Math.Abs (swipe.Direction.y))) {
			// Swipe is Horizontal
			return true;
		} else {
			// Swipe is Vertical
			return false;
		}

	}



}