/*Shaun Yonkers
 * 1650692
 * CMPT430
 * Lab 5: Swarm AI  
 * 
 * The trigger script is used to include and exclude units from the ally and enemy lists contained in boid script using a sphere collider.
 * */
using UnityEngine;
using System.Collections;

public class TriggerScript : MonoBehaviour {

	public Boid boidScript;
	// Use this for initialization
	void Start () {
		boidScript = transform.parent.gameObject.GetComponent<Boid>();
	}

	// add enemies or allies to the corresponding list if within the sphere collider
	void OnTriggerEnter(Collider col){
		if (col.tag == "Radius"){
			if (col.transform.parent.gameObject.tag != this.transform.parent.gameObject.tag) {
				boidScript.enemies.Add (col.transform.parent.gameObject);
			} else if (col.transform.parent.gameObject.tag == this.gameObject.transform.parent.gameObject.tag) {
				boidScript.allyboids.Add (col.gameObject.transform.parent.gameObject);
			}
		}
	}

	// remove enemies and allies from the corresponding list(s) when exiting the sphere collider
	void OnTriggerExit(Collider col){
		if (col.tag == "Radius") {
			if (col.transform.parent.gameObject.tag != this.transform.parent.gameObject.tag) {
				boidScript.enemies.Remove (col.transform.parent.gameObject);
				if (boidScript.attackRange.Contains(col.transform.parent.gameObject))
					boidScript.attackRange.Remove(col.transform.parent.gameObject);
			} else if (col.gameObject.transform.parent.gameObject.tag == this.gameObject.transform.parent.gameObject.tag) {
				boidScript.allyboids.Remove (col.gameObject.transform.parent.gameObject);
			} 
		}
	}
}
