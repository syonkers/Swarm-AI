/*Shaun Yonkers
 * 1650692
 * CMPT430
 * Lab 5: Swarm AI  
 * 
 * Boid script is used to handle each individual game object and where they are within the swarm along with managing health and damage.
 * 
 *  * ****** Credited to Brian Brookwell for implementation of portions of Boid ******
 * */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boid : MonoBehaviour {
	private GameObject enemyTarget;

	private const int CYCLES = 5;
	private int cycles;

	public int health = 5;

	public float MAX_DIRECTION_CHANGE = 180f * Mathf.Deg2Rad;

	public float ownCohesionWeight = 250f;
	public float ownCohesionRadius = 9f;

	public float ownAlignmentWeight = 40f;
	public float ownAlignmentRadius = 3f;

	public float ownSeparationWeight = 186f;
	public float ownSeparationRadius = 2.88f;

	public float enemyAttractionRadius = 49f;
	public float enemyAttractionWeight = 200f;
	public float enemyMeleeRange = 3f;

	public float normalSpeed = 1f;
	public float maxSpeed = 2f;

	public bool inCombat;
	public bool isDead;

	public List<GameObject> allyboids, enemies, attackRange;

	public Vector3 velocity, newVelocity;

	public Animator anim;
	public Rigidbody rb;


	// Use this for initialization
	void Awake () {
		cycles = CYCLES;
		anim = gameObject.GetComponent<Animator> ();
		rb = gameObject.GetComponent<Rigidbody> ();
		inCombat = false;
		isDead = false;
	}


	// Update is called once per frame
	void Update () {
		if (!isDead) {
			cycles--;
			if (cycles <= 0 && !inCombat) {
				cycles = CYCLES;
				Vector3 oldVelocity = GetComponent<Boid> ().velocity;
				newVelocity = oldVelocity + (Cohesion () * ownCohesionWeight) + (Alignment () * ownAlignmentWeight) + (Separation () * ownSeparationWeight) + (Attraction () * enemyAttractionWeight);
				newVelocity.y = 0;
				newVelocity = Vector3.RotateTowards (oldVelocity, Limit (newVelocity, maxSpeed), MAX_DIRECTION_CHANGE * Time.deltaTime, maxSpeed * Time.deltaTime);
			}
			// if there are people in our attack range we can set to be in combat and do the attack animation
			if (attackRange.Count > 0) {
				anim.SetBool ("isAttacking", true);
				newVelocity = Vector3.zero;
				inCombat = true;
			} else { //go back to roaming
				inCombat = false;
			}
			if (health <= 0) {
				isDead = true;
				this.gameObject.transform.position = new Vector3(20000, 0, 20000); // move far off the map to exit trigger regions
			}
			// find which enemies are within our range to attack
			foreach (GameObject boid in enemies) {
				float distance = Vector3.Distance (transform.position, boid.transform.position);
				if ((distance <= enemyMeleeRange) && (!attackRange.Contains (boid))) {
					attackRange.Add (boid);
				}
			}
		}
		// if they are dead and have falled a suffient amount off the map (time to remove from other lists), we can destroy the game object
		if (isDead)
		if (transform.position.y < -500)
			Destroy (gameObject);
	}

		

	void LateUpdate(){
		velocity = newVelocity;

		if (velocity != Vector3.zero) {
			transform.rotation = Quaternion.LookRotation (velocity);
			transform.position += velocity * Time.deltaTime;
			anim.SetBool ("isAttacking", false);
			anim.SetFloat ("Speed", velocity.magnitude);
		}
	}


	/***************Attack Function************************
	 * 
	 * Description: Attack function is called within the attack animation so as to pace out the attack times( 1 damage per animation ).
	 * 				The attack function finds the first enemy within the attack range list and deals one damage to it.
	 * 
	 * 
	 * *****************************************************/
	void Attack(){
		if (enemyTarget == null) {
			if (attackRange.Count > 0) {
				for (int i = 0; i < attackRange.Count; i++) {
					if (attackRange [i].gameObject.GetComponent<Boid> ().isDead == false) {
						enemyTarget = attackRange [i].gameObject;
					}
				}
			} else {
				return;
			}
		}
		if (enemyTarget != null) {
			enemyTarget.GetComponent<Boid> ().health--;
			if (enemyTarget.GetComponent<Boid> ().health <= 0)
				enemyTarget = null;
		}
		anim.SetBool ("isAttacking", false);
		
	}

	/***************Alignment Function************************
	 * 
	 * Description: Alignment is used to match the velocity of the boids neighbors so as to move
	 * 				as one unit
	 * 
	 * *****************************************************/

	Vector3 Alignment(){
		Vector3 alignment = Vector3.zero;
		Vector3 alignmentFinal;
		int neighbors = 0;
		foreach (GameObject boid in allyboids) {
			if (boid != this.gameObject && boid != null) {
				float distance = Vector3.Distance (transform.position, boid.transform.position);
				if (distance < ownAlignmentRadius && distance > 0) {
					alignment += boid.GetComponent<Boid> ().velocity;
					neighbors++;
				}
			}
		}
		if (neighbors > 0)
			alignmentFinal = Limit (alignment / neighbors, MAX_DIRECTION_CHANGE);
		else
			alignmentFinal = alignment;
		
		alignmentFinal.Normalize ();
		return alignmentFinal;
	}
		

	/***************Cohesion Function************************
	 * 
	 * Description: Cohesion is used to allow the boid to steer closer to other boids in the swarm
	 * 				and to find the center between it's neighbors
	 * 
	 * 
	 * *****************************************************/
	Vector3 Cohesion(){
		Vector3 cohesion = Vector3.zero;
		Vector3 cohesionFinal;
		int neighbors = 0;
		foreach (GameObject boid in allyboids) {
			if (boid != this.gameObject && boid != null) {
				float distance = Vector3.Distance (transform.position, boid.transform.position);
				if (distance < ownCohesionRadius && distance > 0) {
					cohesion += boid.transform.position;
					neighbors++;
				}
			}
		}
		if (neighbors > 0)
			cohesionFinal = Steer (cohesion / neighbors, false);
		else
			cohesionFinal = cohesion;
		
		cohesionFinal -= transform.position;
		cohesionFinal.Normalize ();
		return cohesionFinal;
	}

	/***************Seperation Function************************
	 * 
	 * Description: Seperation is used to prevent collision between two boids in the flock
	 * 				by steering away from any potential collisions
	 * 
	 * 
	 * *****************************************************/
	Vector3 Separation() {
		Vector3 seperation = Vector3.zero;
		Vector3 seperationFinal;
		int neighbors = 0;
		foreach (GameObject boid in allyboids){
			if (boid != this.gameObject && boid != null) {
				float distance = Vector3.Distance (transform.position, boid.transform.position);
				if (distance < ownSeparationRadius && distance > 0) {
					Vector3 position = transform.position - boid.transform.position;
					seperation += position;
					neighbors++;
				}
			}
		}
		if (neighbors > 0)
			seperationFinal = seperation / neighbors;
		else
			seperationFinal = seperation;
		seperationFinal.Normalize ();
		return seperationFinal;
	}

	/***************Attraction Function************************
	 * 
	 * Description: Attraction is used to allow each boid to move towards enemies within the enemy swarm
	 * 
	 * *****************************************************/
	Vector3 Attraction() {
		Vector3 attraction = Vector3.zero;
		Vector3 attractionFinal;
		int attractionCount = 0;

		foreach (GameObject boid in enemies) {
			if (boid != this.gameObject && boid != null){
				float distance = Vector3.Distance (transform.position, boid.transform.position);
				if (distance < enemyAttractionRadius) {
					attraction += boid.transform.position;
					attractionCount++;
				}
			}
		}

		if (attractionCount > 0)
			attractionFinal = Steer (attraction / attractionCount, false);
		else
			attractionFinal = attraction;
		
		attractionFinal -= transform.position;
		attractionFinal.Normalize ();
		return attractionFinal;
	}

	/***************Limit Function************************
	 * 
	 * Description: Limit is used to limit the magnitude of the vector specified by a specified maximum
	 * 
	 * Inputs: v - the vector which your are trying to limit
	 * 		   max - the maximum allowable limit for the vector
	 * 
	 * 
	 * *****************************************************/
	protected Vector3 Limit(Vector3 v, float max){
		return (v.magnitude > max) ? v.normalized * max : v;
	}

	/***************Steer Function************************
	 * 
	 * Description: Steer is used to calculate the vector in which to move the boid towards and look at the target
	 * 
	 * Inputs: target - the target in which we want to steer towards
	 * 		   slowDown - a boolean value if we are required to slow down before reaching the target
	 * *****************************************************/
	protected virtual Vector3 Steer(Vector3 target, bool slowDown){
		Vector3 steer = Vector3.zero;
		Vector3 targetDirection = target - transform.position;
		float targetDistance = targetDirection.magnitude;

		transform.LookAt (target);

		if (targetDistance > 0){
			targetDirection.Normalize ();
			if (slowDown && targetDistance < 100f * normalSpeed){
				targetDirection *= (maxSpeed * targetDistance / (100f * normalSpeed));
				targetDirection *= normalSpeed;
			} else {
				targetDirection *= maxSpeed;
			}

			steer = targetDirection - rb.velocity;
			steer = Limit (steer, MAX_DIRECTION_CHANGE * Time.deltaTime);
		}
		return steer;
	}
}
