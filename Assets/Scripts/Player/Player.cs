using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    GameObject instructionUI;

	// Use this for initialization
	void Start () {
        instructionUI = GameObject.FindGameObjectWithTag("Instructions");

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(instructionUI != null)
            {
                bool active = instructionUI.activeInHierarchy ? false : true;
                instructionUI.SetActive(active);
            }
        }
    }
}
