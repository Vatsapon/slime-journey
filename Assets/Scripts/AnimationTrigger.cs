using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTrigger : MonoBehaviour {

    private LevelManager levelManager;

    private void Start() {
        levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
    }

    public void Destroy() {
        Destroy(gameObject);
    }

    public void PlaySound(string name) {
        levelManager.GetComponent<AudioManager>().PlaySound(name);
    }
}
