// FILE: TVanim.cs
// AUTHOR: Varshil Udani and Luke Harries
// DESCRIPTION: Script toggles the TV animator FSM

using UnityEngine;
using System.Collections;

public class TVanim : MonoBehaviour {

	// A signal that comes from the GestureManager.cs to indicate TV on or off
	public static bool set_tvOn;
	
	Animator anim; // Our 4 state FSM for TV

	void Start () {
		anim = GetComponent<Animator>();
		set_tvOn = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (set_tvOn == false) {
			anim.SetBool ("tvOn", false);
		} 
		else if (set_tvOn == true) {
			anim.SetBool("tvOn", true);
		}
	}
}

