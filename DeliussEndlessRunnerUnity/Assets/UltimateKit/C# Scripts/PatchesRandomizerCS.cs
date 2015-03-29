/*
*	FUNCTION:
*	- This scirpt handles the creation and destruction of the environment patches.
*
*	USED BY:
*	This script is a part of the "Player" prefab.
*
*/
using UnityEngine;
using System.Collections;

public class PatchesRandomizerCS : MonoBehaviour {

	public GameObject[] patchesPrefabs;//patches that will be generated

	private GameObject goPreviousPatch;//the patch the the player passed
	private GameObject goCurrentPatch;//the patch the player is currently on
	private GameObject goNextPatch;//the next patch located immediatly after current patch
	private float fPatchDistance;//default displacement of patch
	private Transform tPlayer;//player transform
	
	private float fPreviousTotalDistance = 0.0f;//total displacement covered
	private int iCurrentPNum = 1;//number of patches generated
	
	//script references
	private InGameScriptCS hInGameScriptCS;
	private ElementsGeneratorCS hElementsGeneratorCS;
	private CheckPointsMainCS hCheckPointsMainCS;
	
	//get the current path length
	public float getCoveredDistance() { return fPreviousTotalDistance; } 
	
	void Start()
	{
		hInGameScriptCS = (InGameScriptCS)this.GetComponent(typeof(InGameScriptCS));
		hCheckPointsMainCS = (CheckPointsMainCS)GetComponent(typeof(CheckPointsMainCS));
		hElementsGeneratorCS = (ElementsGeneratorCS)this.GetComponent(typeof(ElementsGeneratorCS));
		
		iCurrentPNum = 1;
		fPreviousTotalDistance = 0.0f;
		fPatchDistance = hCheckPointsMainCS.getDefaultPathLength();
		
		instantiateStartPatch();	
		goPreviousPatch = goCurrentPatch;	
		
		tPlayer = GameObject.Find("Player").transform;
		hCheckPointsMainCS.setChildGroups();
		
		hCheckPointsMainCS.SetCurrentPatchCPs();
		hCheckPointsMainCS.SetNextPatchCPs();
	}
	
	void Update()
	{
		if(hInGameScriptCS.isGamePaused()==true)
			return;
		
		//destroy the patch if the Player has crossed it
		if(tPlayer.position.x>(iCurrentPNum*fPatchDistance)+100.0f)
		{
			Destroy(goPreviousPatch);
			iCurrentPNum++;
		}
	}//end of update
	
	/*
	*	FUNCTION: Create a new Patch after the player reaches goNextPatch
	*	CALLED BY:	CheckPointsMainCS.SetNextMidPointandRotation(...)
	*/
	public void createNewPatch()
	{
		goPreviousPatch = goCurrentPatch;
		goCurrentPatch = goNextPatch;
		
		instantiateNextPatch();	
		hCheckPointsMainCS.setChildGroups();
		
		fPreviousTotalDistance += CheckPointsMainCS.fPathLength;
		
		hElementsGeneratorCS.generateElements();	//generate obstacles on created patch
	}
	
	private void instantiateNextPatch()
	{	
		goNextPatch = (GameObject)Instantiate((GameObject)patchesPrefabs[UnityEngine.Random.Range(0,patchesPrefabs.Length)], new Vector3(fPatchDistance*(iCurrentPNum+1),0,0), new Quaternion());
	}
	
	/*
	*	FUNCTION: Instantiate the first patch on start of the game.
	*	CALLED BY: Start()
	*/
	private void instantiateStartPatch()
	{
		goCurrentPatch = (GameObject)Instantiate((GameObject)patchesPrefabs[UnityEngine.Random.Range(0,patchesPrefabs.Length)], new Vector3(0,0,0), new Quaternion());
		goNextPatch = (GameObject)Instantiate((GameObject)patchesPrefabs[UnityEngine.Random.Range(0,patchesPrefabs.Length)], new Vector3(fPatchDistance,0,0), new Quaternion());
	}
	
	public GameObject getCurrentPatch() { return goCurrentPatch; }
	public GameObject getNextPatch() { return goNextPatch; }
}
