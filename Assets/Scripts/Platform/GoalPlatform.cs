using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalPlatform : MonoBehaviour {

    public float nextLevelCD;
    public string nextLevelName;
    bool ending;

	// Use this for initialization
	void Start () {
        ending = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !ending)
        {
            StartCoroutine(NextLevelTimer(nextLevelCD));
            ending = true;
        }
    }

    void NextLevel()
    {
        SceneManager.LoadScene(nextLevelName);   
    }

    IEnumerator NextLevelTimer(float time)
    {
        yield return new WaitForSeconds(time);
        NextLevel();
    }
}
