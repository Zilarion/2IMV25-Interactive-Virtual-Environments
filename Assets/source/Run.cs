using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Run : MonoBehaviour {
	public GameObject dock;

	bool runningExperiment = false;

	// Use this for initialization
	void Start () {
		// Data variables
		int amount_experiments = 0;

		// Experiment static values
		float hole_min = 0.1f, hole_max = maxSize (dock);
		float stepSize = 0.05f;
		int batch_size = 15;
		int step_count = 5;

		// Experiment values
		float I = 0.0f, deltaI = 0.0f, k = 0.2f;
		float correct_percent_batch = 0.0f;;
		float correct_percent;

		List<bool> R = new List<bool> ();

		while (amount_experiments < batch_size && correct_percent_batch - 0.5f >= 0.1f) {
			// Run one iteration
			for (int i = 0; i < step_count; i++) {
				// Generate experiment values
				I = Random.Range (hole_min, hole_max);
				deltaI = k * I;
				createBlock (deltaI);
				// Do experiment
				bool result = waitForExperiment(I, deltaI);
				R.Add (result);
				log ();
				amount_experiments++;
			}

			correct_percent = calculatePercentage (R, step_count);
			if (correct_percent > 0.6f) {
				deltaI += stepSize;
			} else {
				deltaI -= stepSize;
			}
			correct_percent_batch = calculatePercentage (R, batch_size);
		}
	}

	bool waitForExperiment(float I, float deltaI) {
		return false;
	}

	float calculatePercentage(List<bool> list, int last_amount_elements) {
		int sum = 0;
		int size = list.Count;
		int count = 0;
		int start = Mathf.Max (0, size - last_amount_elements); // -1 ?

		for (int i = start; i < size; i++) {
			count++;
			if (list [i])
				sum++;
		}
		return (float) sum / (float) count;
	}

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

	void experimentComplete(bool choice) {
		
	}

	// Update is called once per frame
	void Update () {
		if (runningExperiment) {
			if (Input.GetKeyDown ("up"))
				experimentComplete (true);
			if (Input.GetKeyDown ("down"))
				experimentComplete (false);
		}
	}
}
 