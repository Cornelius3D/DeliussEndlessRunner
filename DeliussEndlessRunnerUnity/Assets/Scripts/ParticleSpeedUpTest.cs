using UnityEngine;
using System.Collections;

public class ParticleSpeedUpTest : MonoBehaviour {
	
	private ParticleSystem ps;
	
	void Awake ()
	{
		ps = GetComponent<ParticleSystem>();
	}
	// Update is called once per frame
	void Update () {
		//ps.transform.localPosition = new Vector3(0, 0, 0);
		//ps.Simulate(0.005f, true);
		//ps.Play(true);	
	}
}
