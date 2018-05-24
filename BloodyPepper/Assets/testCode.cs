using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testCode : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StoryTest.AddTestScript(GetComponent<UnityEngine.UI.Text>());
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
