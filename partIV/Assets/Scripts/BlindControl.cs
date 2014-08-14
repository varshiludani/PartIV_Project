using UnityEngine;
using System.Collections;


// Blind Z pos down = 0.1884181
// Blind Z pos up = 0.2220076

public class BlindControl : MonoBehaviour {

	public static bool blindDown;


	int mode;
	float blindVerticalPos;
	float blind_x;
	float blind_y;


	void Awake()
	{
		blind_x = -0.02969291f;
		blind_y = -0.1233926f;
	}

	void Start()
	{
		mode = 0;
		blindVerticalPos = 0.2220076f;
		blindDown = false;
	}
	
	// Update is called once per frame
	void Update () {

		//-- Mode 0: player off
		if(mode == 0)
		{   
			if(blindDown)
			{
				mode = 1;
				Debug.Log("Blind Down");
			}


		}
		//-- Mode 1: Pull Down Blind
		else if(mode == 1)
		{
			if(blindDown)
			{
				Debug.Log("Lowering");

				blindVerticalPos -= Time.deltaTime * 0.0111965f;

				if(blindVerticalPos <= 0.1884181f)
				{
					Debug.Log("Down");

					blindVerticalPos = 0.1884181f;
					mode = 2;
				}

			}
			else
				mode = 3;
			}

		//-- Mode 2: Down
		else if(mode == 2)
		{
			if(!blindDown)
				mode = 3;
		}
		//-- Mode 3: stopping
		else
		{
			if(!blindDown)
			{
				Debug.Log("Raising");

				blindVerticalPos += Time.deltaTime * 0.0111965f;

				if(blindVerticalPos >= 0.2220076f)
				{
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
