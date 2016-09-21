using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using AC;

public class PlayerScript : MonoBehaviour {


	private Player Pyerus;
	public bool isGround;
	public bool isDJump;
	public bool isJump;
	private int health;
	private int maxHealth;


	public Text healthText;
//	public Text groundedText;
//	public Text jumpingText;
//	public Text doubleJumpText;

	void Start ()
	{
		Pyerus = KickStarter.player;
	}
	
	// Update is called once per frame
	void Update () 
	{
		health = Pyerus.GetComponent <Status> ().currentHealth;
		maxHealth = Pyerus.GetComponent <Status> ().maxHealth;
		healthText.text = health + "/" + maxHealth;
		
//		if (Pyerus.GetComponent<Player> ().IsGrounded() == true) {
//			isGround = true;
//			//groundedText.text = "On Ground:" + isGround;
//		}
//		else {
//			isGround = false;
//			//groundedText.text = "On Ground:" + isGround;
//		} 
//
//		if (Pyerus.GetComponent<Player> ().isDoubleJump == true) {
//			isDJump = true;
//			//doubleJumpText.text = "Double Jumping?:" + isDJump;
//		}
//		else {
//			isDJump = false;
//			//doubleJumpText.text = "Double Jumping?:" + isDJump;
//		} 
//
//		if (Pyerus.GetComponent<Player> ().isJumping == true) {
//			isJump = true;
//			//jumpingText.text = "Jumping?:" + isJump;
//		}
//		else {
//			isJump = false;
//			//jumpingText.text = "Jumping?:" + isJump;
//		} 

	}
}
