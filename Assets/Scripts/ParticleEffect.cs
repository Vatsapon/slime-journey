using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete]
public class ParticleEffect : MonoBehaviour {

    public float destroyDelay = 1;
    private ParticleSystem particle;

    private float currentDestroyDuration;
    private float currentFadeDuration;

    private void Start() {
        particle = this.GetComponent<ParticleSystem>();
        currentFadeDuration = particle.startLifetime;
        currentDestroyDuration = destroyDelay;
    }
    
    private void Update() {
        if (currentDestroyDuration > 0) {
            currentDestroyDuration -= Time.deltaTime;
            return;
        }

        particle.emissionRate = 0;

        if (currentFadeDuration > 0) {
            currentFadeDuration -= Time.deltaTime;
        } else {
            Destroy(this.gameObject);
        }
    }
}
