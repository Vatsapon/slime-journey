using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Command;

public class Item : MonoBehaviour {

    public CommandType type;
    public Functions function;
    public GameObject collectParticle;

    [Space(10)]
    public SpriteRenderer frontIcon;
    public SpriteRenderer backIcon;

    private LevelManager levelManager;

    private void Start() {
        levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        frontIcon.sprite = levelManager.FindCommandItemOfType(type, function).iconSprite;
        backIcon.sprite = levelManager.FindCommandItemOfType(type, function).iconSprite;
    }

    private void Collect() {

        levelManager.GetComponent<AudioManager>().PlaySound("Item_Collected");
        CommandItem commandItem = levelManager.FindCommandItemOfType(type, function);
        commandItem.amount++;

        Instantiate(collectParticle, transform.position, collectParticle.transform.rotation);

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") || other.CompareTag("Clone")) {
            Collect();
        }
    }
}
