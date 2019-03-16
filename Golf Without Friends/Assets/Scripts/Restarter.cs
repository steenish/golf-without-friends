using UnityEngine;
using UnityEngine.SceneManagement;

public class Restarter : MonoBehaviour {	
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
