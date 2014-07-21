using UnityEngine;
using System.Collections;

public class TVanim : MonoBehaviour {

	public static bool set_tvOn;
	
	Animator anim;
	// Use this for initialization
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

