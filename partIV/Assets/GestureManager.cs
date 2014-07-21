using UnityEngine;
using System.Collections;
using Leap;

public class GestureManager : MonoBehaviour {
	Controller controller;
	public AudioClip music;
	float max_plIntensity = 03.3f;
	float max_pl2Intensity = 01.5f;	
	float max_dlIntensity = 0.53f;
	int grabCount = 0;
	int grabCount2 = 0;
	bool isTVplaying = false;
	
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
		audio.clip = music;

		int rightFingers_check = 0;
		int leftFingers_check = 0;
		bool grab = false;
		bool grab2 = false;

		foreach (Hand hand in frame.Hands)
		{
			//Debug.Log(hand.IsLeft ? "Left hand" : "Right hand");
			rightFingers_check = 0;
			leftFingers_check = 0;

			if(hand.IsRight)
			{
				foreach (Finger finger in hand.Fingers) 
				{
					if((finger.IsExtended)) rightFingers_check++;
				}
				if (hand.GrabStrength == 1.0f)
				{
					grab2 = true;
					break;
				}
				else grab2 = false;
			}
			else if(hand.IsLeft)
			{
				foreach (Finger finger in hand.Fingers)
				{
					if((finger.IsExtended)) leftFingers_check++;
				}
				if (hand.GrabStrength == 1.0f)
				{
					grab = true;
					break;
				}
				else grab = false;
			}
		}

		if (grab)
		{
			grabCount = grabCount + 1;
			if (grabCount >= 100)
			{
				grabCount = 0;
				if (!audio.isPlaying)
				{
					audio.Play();
					RecordPlayer.recordPlayerActive = true;
				}
				else 
				{
					audio.Pause();
					RecordPlayer.recordPlayerActive = false;
				}
			}
		}

		if (grab2)
		{
			grabCount2 = grabCount2 + 1;
			if (grabCount2 >= 100)
			{
				grabCount2 = 0;
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
		}
		
		for (int i = 0; i < gestures.Count; i++)
		{
			Gesture gesture = gestures [i];
			
			switch (gesture.Type)
			{
			case Gesture.GestureType.TYPE_CIRCLE:
				//Debug.Log(rightFingers_check);
				//Debug.Log("CHECK #1");
				CircleGesture circle = new CircleGesture (gesture);
				float turns = circle.Progress;
				//Debug.Log(turns);
				//Calculate clock direction using the angle between circle normal and pointable
				//Clockwise if angle is less than 90 degrees
				if (circle.Pointable.Direction.AngleTo (circle.Normal) <= System.Math.PI / 2)
				{
					if (turns >= 1)
					{
						turns = 0;
						if ((rightFingers_check < 3) && (rightFingers_check != 0))
						{
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
						}
						else if (rightFingers_check == 5)
						{
							if (point_light2.transform.light.intensity < max_pl2Intensity)
							{
								point_light2.transform.light.intensity = point_light2.transform.light.intensity + 0.005f;
							}
							else
							{
								point_light2.transform.light.intensity = max_pl2Intensity;
							}
						}
						else if (leftFingers_check == 5)
						{
							if (audio.isPlaying)
							{
								audio.volume = audio.volume + 0.001f;
							}
						}
					}
					// counter clock-wise
				}
				else
				{
					if (turns >= 1)
					{
						turns = 0;
						if ((rightFingers_check < 3) && (rightFingers_check != 0)) 
						{
							point_light.transform.light.intensity = point_light.transform.light.intensity - 0.005f;
							directional_light.transform.light.intensity = directional_light.transform.light.intensity - 0.005f;
						} 
						else if (rightFingers_check == 5)
						{
							point_light2.transform.light.intensity = point_light2.transform.light.intensity - 0.005f;
						}
						else if (leftFingers_check == 5)
						{
							audio.volume = audio.volume - 0.001f;
						}
					}
				}
				
				break;
				
			case Gesture.GestureType.TYPE_SWIPE:
				SwipeGesture swipe = new SwipeGesture (gesture);
				//Classify swipe as either horizontal or vertical
				bool isHorizontal = false;
				//Debug.Log(swipe.Direction.x);
				if ((System.Math.Abs(swipe.Direction.x)) > (System.Math.Abs(swipe.Direction.y)))
				{
					isHorizontal = true;
				}
				//Classify as right-left or up-down
				if(isHorizontal)
				{
					if(swipe.Direction.x > 0)
					{
						Debug.Log("Right");
//						TVanim.set_tvOn = true;
					}
					else
					{
						Debug.Log("Left");
//						TVanim.set_tvOn = false;
					}	
				}
				else 
				{ //vertical
					if(swipe.Direction.y > 0)
					{
						Debug.Log("Up");
					}
					else
					{
						Debug.Log("Down");
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
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         