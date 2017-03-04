/*Shaun Yonkers
 * 1650692
 * CMPT430
 * Lab 5: Swarm AI  
 * 
 * Swarm script is used to generate the two swarms, at set locations, withing a set radius, using the prefab and color desired.
 * 
 *  * ****** Credited to Brian Brookwell for implementation of portions of Swarm ******
 * */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Swarm{

	public int count;
	public float spawnRadius;
	public List<GameObject> boids;

	private GameObject groundColor;

	/***************Swarm Function************************
	 * 
	 * Description: Generate a swarm of gameobjects using the inputs requested
	 * 
	 * Inputs: Position - the center position of where the swarm will be generated
	 * 		   Radius - the radius around position in which to randomly spawm the gameobjects
	 * 		   Count - the number of gameobjects in the swarm that will be spawned
	 * 		   Prefab - the prefab which will be used to instantiate as a gameobject
	 * 		   TeamColor - the color displayed under each unit to distinguish it from one team to another
	 * 
	 * *****************************************************/

	public Swarm( Vector3 position, float radius, int count, GameObject prefab, Color teamColor){
		boids = new List<GameObject> ();

		GameObject temp;

		for (int i = 0; i < count; i++) {
			temp = (GameObject)GameObject.Instantiate (prefab);
			groundColor = temp.transform.Find ("GroundFX").gameObject;
			groundColor.GetComponent<Light>().color = teamColor;

			Vector3 pos = Random.insideUnitCircle * radius;
			pos.x += position.x;
			pos.z = pos.y + position.z;
			pos.y = 0;
			temp.transform.position = new Vector3 (pos.x + 4f * (int)(i/4), pos.y, pos.z + 3f * (i % 4));

			temp.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			boids.Add (temp);
		}
	}

	/***************Set Tag Function************************
	 * 
	 * Description: set the tag of each boid within the boids list to the tag specified in swarm controller
	 * 
	 * Inputs: tag: the string that is provided within the swarm controller script to designate the tag name
	 * 
	 * *****************************************************/
	public void setTag(string tag){
		foreach (GameObject boid in boids) {
			boid.tag = tag;
		}
	}
}
