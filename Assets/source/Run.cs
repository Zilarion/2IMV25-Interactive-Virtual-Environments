using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.UI;

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

	// Logging
	Stopwatch sw;
	System.IO.StreamWriter writer;

	string logFile;

	// Initialize base values
	void Start () {
		R = new List<bool> ();
		hole_min = 0.1f;
		hole_max = maxSize (dock);
		batch_size = 15;
		step_count = 5;

		I = 0.0f;
		deltaI = 0.0f;
		k = 0.08f;
		stepSize = k;

		sw = new Stopwatch ();
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
		//print ("Starting experiment #" + amount_experiments + " values: " + I + " / " + deltaI + " / " + k + " expected: " + correctResult);
		sw.Start();
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

	float maxSize(GameObject gobj) {
		float x = gobj.transform.lossyScale.x;
		float z = gobj.transform.lossyScale.z;
		return x >= z ? x : z;
	}

	void experimentComplete(bool choice) {
		// Measure the time
		sw.Stop ();
		double time_ms = sw.ElapsedMilliseconds;
		sw.Reset();

		// Add result
		bool correct = correctResult == choice;
		R.Add (correct);
		{
			if (writer != null) {
				// Log results:
				string results = k + "," + I + "," + deltaI + "," + correct + "," + correctResult + "," + time_ms;
				writer.WriteLine (results);
			}
		}

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
			stepSize /= 2.0f;
			if (correct_percent > 0.5f) {
				k -= stepSize;
			} else {
				k += stepSize;
			}
			correct_percent_batch = calculatePercentage (R, batch_size);

			// Termination check
			if (amount_experiments > batch_size && correct_percent_batch >= 0.4f && correct_percent_batch <= 0.6f) {
				//print ("All done! Correctness percentage last batch: " + correct_percent_batch + " / k: " + k);
				writer.Flush ();
				writer.Close ();
				writer = null;
				//TODO make nices quit method
				UnityEngine.Application.Quit();
				((UnityEngine.Application)(null)).ToString();
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
			// This must be an else if to prevent issues with both buttons pressed in same tick
			else if (Input.GetKeyDown ("down")) {
				experimentComplete (false);
			}
		} else {
			if (Input.GetKeyDown (KeyCode.Return)) {
				GameObject inputFieldGo = GameObject.Find("InputField");
				InputField inputFieldCo = inputFieldGo.GetComponent<InputField>();
				string playerName = inputFieldCo.text;
				if (playerName != null && playerName.Length != 0) {

					startExperiment ();

					logFile = "data/ " + playerName + ".csv";
					inputFieldGo.SetActive (false);

					writer = new System.IO.StreamWriter (logFile, true);
					writer.WriteLine ("k, I, deltaI, result, shouldFit, time");
				}
			}
		}
	}
}
 