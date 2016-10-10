using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Run : MonoBehaviour {
	public GameObject dock;

	// Use this for initialization
	void Start () {
		// Data variables
		int amount_experiments = 0;

		// Experiment static values
		float hole_min = 0.1f, hole_max = maxSize (dock);
		float stepSize = 0.05f;
		int batch_size = 15;


		// Experiment values
		float I, deltaI, k = 0.2f;
		float correct_percent_batch = 0;

		int iteration_count = 5;
		int batch_count = 3 * iteration_count;
		ArrayList<bool> R = new ArrayList<bool> ();

		while (amount_experiments < batch_count && correct_percent_batch - 0.5f >= 0.1f) {
			// Run one iteration
			for (int i = 0; i < iteration_count; i++) {
				I = Random.Range (hole_min, hole_max);
				deltaI = k * I;
				createBlock (deltaI);
				R.Add(waitForExperiment(I, deltaI));
//				batch.Add (result);
				log ();
//				if (batch.Count > batch_count)
//					batch.RemoveFirst ();
				amount_experiments++;
			}

//			correct_percent = calculatePercentage (batch);
			if (correct_percent > 0.6f) {
				deltaI += stepSize;
			} else {
				deltaI -= stepSize;
			}
		}

	}

	bool waitForExperiment() {
		return false;
	}

//	float calculatePercentage(ArrayList<bool> batch) {
//		int sum = 0;
//		foreach (bool b in batch) {
//			if (b)
//				sum++;
//		}
//		return (float) sum / (float) batch.Count;
//	}

	void createBlock(float delta) {
		CreateBlock script = dock.AddComponent<CreateBlock>();
		script.x = delta;
		script.z = delta;
	}

	void log() {
	}

	float maxSize(GameObject gobj) {
		float x = gobj.transform.lossyScale.x;
		float z = gobj.transform.lossyScale.z;
		return x >= z ? x : z;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
