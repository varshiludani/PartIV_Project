// FILE: GestureManager.cs
// AUTHOR: Varshil Udani and Luke Harries
// DESCRIPTION: An interface between Leap Motion gestures and Unity 3D objects

using UnityEngine;
using System.Collections;
using Leap;

public class GestureManager : MonoBehaviour {

	// Initialise Variables

	Controller controller;
	public AudioClip music;

	//max light intensities
	float max_plIntensity = 03.3f;
	float max_pl2Intensity = 01.5f;	
	float max_dlIntensity = 0.53f;
	
	//counter to measure how long the user needs to perform the grab gesture
	int leftHandGrabCount = 0;
	int rightHandGrabCount = 0;
	bool isTVplaying = false;

	GameObject point_light;
	GameObject point_light2;
	GameObject directional_light;


	// Called when the program first starts
	void Start () {

		// Tell the Leap API to enable gesture detection
		controller = new Controller();
		controller.EnableGesture (Gesture.GestureType.TYPE_CIRCLE);
		controller.EnableGesture (Gesture.GestureType.TYPE_SWIPE);	

		// Tell the Unity API to search for these objects in our scene and assign them to variables
		point_light = GameObject.Find("point_light");
		point_light2 = GameObject.Find("point_light2");
		directional_light = GameObject.Find("Directional light");
	}

	// Called every rendering time
	void Update () {

		// Initialise the Leap Motion Frame and Gesture objects
		Frame frame = controller.Frame();
		GestureList gestures = frame.Gestures();

		// Initialise our music
		audio.clip = music;

		int rightFingers_check = 0; //counter to count the fingers
		int leftFingers_check = 0;
		bool leftHandGrab = false;
		bool rightHandGrab = false;

		// For the number of hands in the frame do the following
		foreach (Hand hand in frame.Hands) {
			if(hand.IsRight) {
				// count the number of fingers extended for right hand
				rightFingers_check = countExtendedFingers(hand);
				// Detect if the user is performing the grab gesture with right hand
				rightHandGrab = detectGrab(hand);
			} else if(hand.IsLeft) {
				leftFingers_check =countExtendedFingers(hand);
				leftHandGrab = detectGrab(hand);
			}
		}

		// If left hand grab is detected then call its manager function
		if (leftHandGrab) {
			leftHandGrabManager();
		}

		// If right hand is detected then call its manager function
		if (rightHandGrab) {
			rightHandGrabManager();
		}

		// Handle other gestures (circles and swipes)
		for (int i = 0; i < gestures.Count; i++) {
			Gesture gesture = gestures [i];
			// Switch statement to consider all gesture implementations in the array
			switch (gesture.Type) {
			case Gesture.GestureType.TYPE_CIRCLE:
	
				// If basic circle gesture is detected then initiate it using the Leap API
				CircleGesture circle = new CircleGesture (gesture);
				float turns = circle.Progress;

				//Calculate clock direction using the angle between circle normal and pointable
				//Clockwise if angle is less than 90 degrees
				if (circle.Pointable.Direction.AngleTo (circle.Normal) <= System.Math.PI / 2) {
					if (turns >= 1) {
						turns = 0;
						
						// If fingers extented (right hand) is less than 3 then room lights intensity increase.
						// else if all fingers are extended (right hand) then lamp light intensity increase.
						// else if all finger are extended (left hand) then Audio player volume increase.
						if ((rightFingers_check < 3) && (rightFingers_check != 0)) {
							secondaryLightControl(true);
						} else if (rightFingers_check == 5) {
							mainLightControl(true);
						} else if (leftFingers_check == 5) {
							increaseAudioVolume(true);
						}
					}

				// ANTICLOCKWISE circle gesture -> Opposite of above
				} else {
					if (turns >= 1) {
						turns = 0;
						if ((rightFingers_check < 3) && (rightFingers_check != 0))  {
							secondaryLightControl(false);
						} else if (rightFingers_check == 5) {
							mainLightControl(false);
						} else if (leftFingers_check == 5) {
							increaseAudioVolume(false);
						}
					}
				}	
				break;
			
			// Swipe gestures will control the windows and blinds. Extensive use of hand vectors is implemented
			case Gesture.GestureType.TYPE_SWIPE:
				
				// If basic swipe gesture is detected then initiate it using the Leap API
				SwipeGesture swipe = new SwipeGesture (gesture);

				//Classify swipe as either horizontal or vertical
				bool isHorizontal = calculateSwipeDirection(swipe);

				if(isHorizontal) {
					// Horizontal  - Window control
					if(swipe.Direction.x > 0) {
						WindowControl.windowOpen = false;
					} else {
						WindowControl.windowOpen = true;
					}	

				} else  { 
					// Vertical - Blinds Control
					if(swipe.Direction.y > 0) {
						BlindControl.blindDown = false;
					} else {
						BlindControl.blindDown = true;
					}                  
				}
				break;
				
			default:
				// Debug.Log("Unknown gesture type");
				break;
			}
		}	
	}
	
	// Function that returns true if grab strength is 1
	bool detectGrab (Hand hand)  {
		// return whether or not the hand is clenched into a fist or 'grab' gesture
		if (hand.GrabStrength == 1.0f) {
			return true;
		} else {
			return false;
		}
	}
	
	// Function that returns the number of outstreched fingers on input hand
	int countExtendedFingers (Hand hand) {

		int numberOfFingers = 0;
		
		foreach (Finger finger in hand.Fingers)  {
			if((finger.IsExtended)) numberOfFingers++;
		}
		return numberOfFingers;
	}
	
	// Function that checks if TV is already on (by default it is not). Toggle the TV status
	void toggleTV() {
		
		if (!isTVplaying) {
			TVanim.set_tvOn = true;
			isTVplaying = true;
		} else {
			TVanim.set_tvOn = false;
			isTVplaying = false;
		}
	}
	
	// Function that checks if music is already on (by default it is not). Toggle the music status
	void toggleMusic () {
		
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
	
	// Function that waits until grab has been sustained for long enough. If yes toggle the TV
	void rightHandGrabManager () {
		
		rightHandGrabCount = rightHandGrabCount + 1;
		if (rightHandGrabCount >= 100) {
			rightHandGrabCount = 0;
			
			toggleTV();
		}
		
	}
	
	// Function that waits until grab has been sustained for long enough. If yes toggle the Music
	void leftHandGrabManager () {
		
		leftHandGrabCount = leftHandGrabCount + 1;
		if (leftHandGrabCount >= 100) {
			leftHandGrabCount = 0;
			toggleMusic();
		}
		
	}
	
	// Room Light Control Funtion
	void mainLightControl (bool lightsBrighter ) {

		if (lightsBrighter) {
			// Increase light brightness
			if (point_light.transform.light.intensity < max_plIntensity) {
				point_light.transform.light.intensity = point_light.transform.light.intensity + 0.0009f;
			} else {
				point_light.transform.light.intensity = max_plIntensity;
			}
			
			if (directional_light.transform.light.intensity < max_dlIntensity) {
				directional_light.transform.light.intensity = directional_light.transform.light.intensity + 0.0009f;
			} else {
				directional_light.transform.light.intensity = max_dlIntensity;
			}

		} else {
			// Decrease light brightness
			point_light.transform.light.intensity = point_light.transform.light.intensity - 0.0009f;
			directional_light.transform.light.intensity = directional_light.transform.light.intensity - 0.0009f;
		}
	}

	// Lamp Light control function
	void secondaryLightControl (bool lightsBrighter ) {
		
		if (lightsBrighter) {
			// Increase light brightness
			if (point_light2.transform.light.intensity < max_pl2Intensity) {
				point_light2.transform.light.intensity = point_light2.transform.light.intensity + 0.01f;
			} else {
				point_light2.transform.light.intensity = max_pl2Intensity;
			}
			
		} else {
			// Decrease light brightness
			point_light2.transform.light.intensity = point_light2.transform.light.intensity - 0.01f;
		}
	}

	// Audio voloume control
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

	// Check sif the swipe is horizontal or vertical based on the x and y swipe vectors
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