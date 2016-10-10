using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Run : MonoBehaviour {
	public GameObject dock, block;

	// Data variables
	bool runningExperiment = false;
	bool correctResult = false;
	int amount_experiments = 0;

	// Experiment static values
	float hole_min, hole_max, stepSize;
	int batch_size, step_count;

	// Experiment values
	float I, deltaI, k;
	float correct_percent_batch;
	float correct_percent;

	int stepI = 0;

	// Data
	List<bool> R;

	// Initialize base values
	void Start () {
		R = new List<bool> ();
		hole_min = 0.1f;
		hole_max = maxSize (dock);
		stepSize = 0.05f;
		batch_size = 15;
		step_count = 5;

		I = 0.0f;
		deltaI = 0.0f;
		k = 0.2f;

		startExperiment ();
	}

	void startExperiment() {
		runningExperiment = true;
		// Generate whether we will oversize the block or not
		bool oversized = Random.Range (0.0f, 1.0f) >= 0.5f;
		correctResult = !oversized;

		// Generate experiment values
		I = Random.Range (hole_min, hole_max);
		deltaI = I * k;

		// Create gap with value I
		createGap (I);

		// Create block with difference delta I
		float blockSize = oversized ? I + deltaI : I - deltaI;
		block.transform.localScale = new Vector3(blockSize, block.transform.localScale.y, block.transform.localScale.z);
		print ("Starting experiment #" + amount_experiments + " values: " + I + " / " + deltaI + " / " + k + " expected: " + correctResult);
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

	void createGap(float delta) {
		print ("Creating new block with delta: " + delta);
		// Kill all the children
		foreach (Transform child in dock.transform) {
			Destroy(child.gameObject);
		}
		// Destroy the script
		CreateBlock old_script = dock.GetComponent<CreateBlock>();
		if (old_script != null) {
			Destroy (old_script);
		}

		// Add a new one :)
		CreateBlock script = dock.AddComponent<CreateBlock>();
		script.x = delta;
		script.z = 0.15f; // Keep static, only vary x
	}

	void log() {
	}

	float maxSize(GameObject gobj) {
		float x = gobj.transform.lossyScale.x;
		float z = gobj.transform.lossyScale.z;
		return x >= z ? x : z;
	}

	void experimentComplete(bool choice) {
		// Add result
		R.Add (correctResult == choice);
		log ();

		// Increase counters
		stepI++;
		amount_experiments++;

		// Step checks
		if (stepI < step_count) {
			startExperiment ();
		} else {
			stepI = 0;

			// Calculate new percentage
			correct_percent = calculatePercentage (R, step_count);
			print ("Step done: " + correct_percent);
			if (correct_percent > 0.5f) {
				print ("Decreasing k");
				k -= stepSize;
			} else {
				print ("Increasing k");
				k += stepSize;
			}
			correct_percent_batch = calculatePercentage (R, batch_size);

			// Termination check
			if (amount_experiments > batch_size && correct_percent_batch >= 0.4f && correct_percent_batch <= 0.6f) {
				print ("All done! Correctness percentage last batch: " + correct_percent_batch + " / k: " + k);
			} else {
				// Not done yet, new experiment!
				startExperiment ();
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (runningExperiment) {
			if (Input.GetKeyDown ("up")) {
				experimentComplete (true);
			}
			if (Input.GetKeyDown ("down")) {
				experimentComplete (false);
			}
		}
	}
}
 