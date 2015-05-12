
/*
*	FUNCTION:
*	- This script controls the Enemy (Police Car) based on the player’s movement.
*	- It controls the enemy's animatations and it's behavior if the player stumbles.
*
*	USED BY:
*	This script is a part of the “Enemy” prefab.
*
*/

using UnityEngine;
using System.Collections;

public class EnemyControllerCS : MonoBehaviour {
	
	// Roman - tweak player's height offset
	[Range(0, 5)]
	public float enemyVerticalPosOffset = 1.5f;
	
	// Roman - tweak how far away the enemy is from the player
	[Range(0, 30)]
	public float enemyDistFromPlayerOffset = 1.0f;
	
	// Roman - tweak the horizontal position of the enemy (how far to the lef or how far to the right)
	[Range(-10, 10)]
	public float enemyHorizontalOffset = 0.0f;
	
	[Range(50, 150)]
	public float enemyFadeOffDist;
	
	// Roman - The death position to animate to when the player dies
	public Transform deathPosition;
	
	private Transform tEnemy;	//enemy transform
	private Transform tPlayer;//player transform
	
	private int iEnemyState = 0;
	private float fDeathRotation = 0.0f;
	private float fCosLerp = 0.0f;	//used for Lerp
	
	//script references
	private InGameScriptCS hInGameScriptCS;
	private ControllerScriptCS hControllerScriptCS;
	private SoundManagerCS hSoundManagerCS;
	
	//enemy logic
	private float fEnemyPosition = 0.0f;
	private float fEnemyPositionX = -5;
	private float fEnemyPositionY = 0;
	private float fStumbleStartTime;
	private float fChaseTime = 5;
	
	void Start()
	{	
		tPlayer = GameObject.Find("Player").transform;
		tEnemy = this.transform;
		
		hInGameScriptCS = (InGameScriptCS)GameObject.Find("Player").GetComponent(typeof(InGameScriptCS));
		hControllerScriptCS = (ControllerScriptCS)GameObject.Find("Player").GetComponent(typeof(ControllerScriptCS));
		hSoundManagerCS = (SoundManagerCS)GameObject.Find("SoundManager").GetComponent(typeof(SoundManagerCS));
	}
	
	/*
	*	FUNCTION: Starting the chasing sequence
	*	CALLED BY: ControllerScript.launchGame()
	*/
	public void launchEnemy()
	{
		iEnemyState = 2;
		print ("enemy launched");
	}
	
	void FixedUpdate ()
	{
		if(hInGameScriptCS.isGamePaused()==true)
			return;
		
		// Roman - Hell hound chases player if player is not dead
		if (iEnemyState < 3)
		{				
			tEnemy.position = new Vector3(Mathf.Lerp(tEnemy.position.x, (tPlayer.position.x - fEnemyPosition - enemyDistFromPlayerOffset), Time.deltaTime*10), 
				tEnemy.position.y + enemyVerticalPosOffset, tEnemy.position.z + enemyHorizontalOffset);
		
		
			
			if (!hControllerScriptCS.isInAir())//follow the player in y-axis if he's not jumping (cars cant jump)
				tEnemy.position = new Vector3(tEnemy.position.x, Mathf.Lerp(tEnemy.position.y, tPlayer.position.y + fEnemyPositionY, Time.deltaTime*8),
					tEnemy.position.z);			
		}
		//ignore y-axis rotation and horizontal movement in idle and death state
		if (iEnemyState < 4)
		{
			tEnemy.position = new Vector3(tEnemy.position.x, tEnemy.position.y,	Mathf.Lerp(tEnemy.position.z, tPlayer.position.z, Time.deltaTime*10));
			tEnemy.localEulerAngles = new Vector3(tEnemy.localEulerAngles.x,-hControllerScriptCS.getCurrentPlayerRotation(), tEnemy.localEulerAngles.z);
		}
		
		// Roman - this is where the enemy lerps out of the screen
		if (iEnemyState == 1)//hide the chasing character
		{
			fCosLerp += (Time.deltaTime/10);
			fEnemyPosition = Mathf.Lerp(fEnemyPosition, fEnemyPositionX + enemyFadeOffDist, Mathf.Cos(fCosLerp)/1000);
			
			if (fCosLerp >= 0.7f)
			{
				fCosLerp = 0.0f;
				iEnemyState = 0;
				
				hSoundManagerCS.stopSound(SoundManagerCS.EnemySounds.Siren);
			}
		}
		// Roman - enemy comes back into view
		else if (iEnemyState == 2)//show the chasing character
		{
			hSoundManagerCS.playSound(SoundManagerCS.EnemySounds.Siren);
			
			fCosLerp += (Time.deltaTime/4);
			fEnemyPosition = Mathf.Lerp(fEnemyPosition, fEnemyPositionX, Mathf.Cos(fCosLerp));
			
			if (fCosLerp >= 1.5f)
			{
				fCosLerp = 0.0f;
				iEnemyState = 3;
			}
		}
		else if (iEnemyState == 3)//wait for 'fChaseTime' after showing character
		{
			if ( (Time.time - fStumbleStartTime)%60 >= fChaseTime)
				iEnemyState = 1;
		}
		
		//DEATH SEQUENCE
		else if (iEnemyState == 4)//on death
		{	
			print ("death");
			//hSoundManagerCS.playSound(SoundManagerCS.EnemySounds.TiresSqueal);
			//hSoundManagerCS.stopSound(SoundManagerCS.EnemySounds.Siren);
			
			// works
			Vector3 deathPos = new Vector3(deathPosition.position.x, transform.position.y, deathPosition.position.z);
			transform.position = Vector3.Lerp (transform.position, deathPos, Time.deltaTime * 10f);
			transform.eulerAngles = deathPosition.eulerAngles;
		}
		
		//print (iEnemyState);
	}//end of Update
	
	/*
	*	FUNCTION: Animate enemy
	*	RETURNS:	'true' if the enemy was already chasing player
	*				'false' if the enemy was not chasing the player
	*	CALLED BY: ControllerScript.processStumble()
	*/
	public bool processStumble()
	{
		if (isEnemyActive())//if enemy is already chasing player
		{
			iEnemyState = 0;		
			return true;
		}
		else
		{
			fStumbleStartTime = Time.time;
			iEnemyState = 2;		
			return false;
		}
	}
	
	public void playDeathAnimation() { iEnemyState = 4; }
	public void hideEnemy() { iEnemyState = 1; }
	
	/*
	*	FUNCTION: Check if the enemy is chasing the player
	*/
	public bool isEnemyActive()
	{
		if (iEnemyState == 2 || iEnemyState == 3)
			return true;
		else
			return false;
	}
}
