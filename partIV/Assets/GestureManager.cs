using UnityEngine;
using System.Collections;
using Leap;

public class GestureManager : MonoBehaviour {
	Controller controller;
	float max_plIntensity = 03.3f;
	float max_pl2Intensity = 01.5f;	
	float max_dlIntensity = 0.53f;
	
	void Start ()
	{
		controller = new Controller();
		controller.EnableGesture (Gesture.GestureType.TYPE_CIRCLE);
		controller.EnableGesture (Gesture.GestureType.TYPE_KEY_TAP);
		controller.EnableGesture (Gesture.GestureType.TYPE_SCREEN_TAP);
		controller.EnableGesture (Gesture.GestureType.TYPE_SWIPE);
	}
	
	void Update ()
	{
		GameObject point_light = GameObject.Find("point_light");
		GameObject point_light2 = GameObject.Find("point_light2");
		GameObject directional_light = GameObject.Find("Directional light");
		Frame frame = controller.Frame();
		GestureList gestures = frame.Gestures();
		int validLight = 0;
		
		for (int i = 0; i < gestures.Count; i++) {
			Gesture gesture = gestures [i];
			
			switch (gesture.Type) {
				
			case Gesture.GestureType.TYPE_CIRCLE:
				
				foreach (Hand hand in frame.Hands) {
					//					Debug.Log(hand.IsLeft ? "Left hand" : "Right hand");
					validLight = 0;
					if(hand.IsRight){
						foreach (Finger finger in hand.Fingers) {
							if((finger.IsExtended)) validLight++;
						}
					}
				}
				
				Debug.Log(validLight);
				//				Debug.Log("CHECK #1");
				CircleGesture circle = new CircleGesture (gesture);
				float turns = circle.Progress;
				//				Debug.Log(turns);
				
				//				Calculate clock direction using the angle between circle normal and pointable
				//				Clockwise if angle is less than 90 degrees
				if (circle.Pointable.Direction.AngleTo (circle.Normal) <= System.Math.PI / 2) {
					if (turns >= 1) {
						turns = 0;
						if ((validLight < 3) && (validLight != 0)) {
							if (point_light.transform.light.intensity < max_plIntensity) {
								point_light.transform.light.intensity = point_light.transform.light.intensity + 0.005f;
							}
							else {
								point_light.transform.light.intensity = max_plIntensity;
							}
							if (directional_light.transform.light.intensity < max_dlIntensity) {
								directional_light.transform.light.intensity = directional_light.transform.light.intensity + 0.005f;
							}
							else {
								directional_light.transform.light.intensity = max_dlIntensity;
							}
						} else if (validLight == 5) {
							if (point_light2.transform.light.intensity < max_pl2Intensity) {
								point_light2.transform.light.intensity = point_light2.transform.light.intensity + 0.005f;
							}
							else {
								point_light2.transform.light.intensity = max_pl2Intensity;
							}
						}
					}
					// counter clock-wise
				} else {
					if (turns >= 1) {
						turns = 0;
						if ((validLight < 3) && (validLight != 0)) {
							point_light.transform.light.intensity = point_light.transform.light.intensity - 0.005f;
							directional_light.transform.light.intensity = directional_light.transform.light.intensity - 0.005f;
						} else if (validLight == 5) {
							point_light2.transform.light.intensity = point_light2.transform.light.intensity - 0.005f;
						}
					}
				}
				
				break;
				
			case Gesture.GestureType.TYPE_SWIPE:
				SwipeGesture swipe = new SwipeGesture (gesture);
				//				SafeWriteLine ("  Swipe id: " + swipe.Id
				//				               + ", " + swipe.State
				//				               + ", position: " + swipe.Position
				//				               + ", direction: " + swipe.Direction
				//				               + ", speed: " + swipe.Speed);
				break;
				
			case Gesture.GestureType.TYPE_KEY_TAP:
				KeyTapGesture keytap = new KeyTapGesture (gesture);
				//				SafeWriteLine ("  Tap id: " + keytap.Id
				//				               + ", " + keytap.State
				//				               + ", position: " + keytap.Position
				//				               + ", direction: " + keytap.Direction);
				break;
				
			case Gesture.GestureType.TYPE_SCREEN_TAP:
				ScreenTapGesture screentap = new ScreenTapGesture (gesture);
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
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         