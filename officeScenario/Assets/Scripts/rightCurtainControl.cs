using UnityEngine;
using System.Collections;

public class rightCurtainControl : MonoBehaviour {

	public static bool toggle = false;
	
	
	int mode;
	float curtainScale;
	float scale_y;
	float scale_z;
	
	
	void Awake()
	{
		curtainScale = 0.3f;
		scale_y = 1.0f;
		scale_z = 1.6f;
	}
	
	// Use this for initialization
	void Start () {
		
		curtainScale = 0.3f;
		mode = 0;
		toggle = false;
		
	}
	
	// Update is called once per frame
	void Update () {
		
		
		//-- Mode 0: Curtain Open
		if(mode == 0)
		{   
			if(toggle)
			{
				mode = 1;
				Debug.Log("Close Right Curtain");
			}
			
			
		}
		//-- Mode 1: Close Curtain
		else if(mode == 1)
		{
			Debug.Log("Closing");
			
			curtainScale += Time.deltaTime * 0.05f;
			
			if(curtainScale >= 1.0f)
			{
				Debug.Log("Closed");
				
				curtainScale = 1.0f;
				mode = 2;
				toggle = false;
			}
			
		}
		
		//-- Mode 2: Closed
		else if(mode == 2)
		{
			if(toggle)
				mode = 3;
		}
		//-- Mode 3: Opening
		else
		{
			Debug.Log("Opening");
			
			curtainScale -= Time.deltaTime * 0.05f;
			
			if(curtainScale <= 0.3f)
			{
				curtainScale = 0.3f;
				mode = 0;
				toggle = false;
				
			}
		}
		
		//-- update objects
		
		
		this.transform.localScale = new Vector3 (-curtainScale, scale_y, scale_z);
		
		
		
		
		
	}
}
