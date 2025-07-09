using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using static Command;

public class CommandItem : MonoBehaviour {
    
    [Header("Setting")]
    public CommandType type;
    public Functions function;
    public int amount;

    [Header("Component")]
    public GameObject selectFrame;
    public Image fadeImage;
    public Image panelImage;
    public GameObject canvasParent;
    public Sprite iconSprite;
    public GameObject itemHoldPrefab;
    public TextMeshProUGUI amountText;

    [HideInInspector]
    public Functions parentFunction;

    private LevelManager levelManager;

    private void Start() {
        levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
    }

    private void Update() {
        amountText.text = amount.ToString();

        if (amount <= 0) {
            fadeImage.enabled = true;
            selectFrame.SetActive(false);
        } else {
            fadeImage.enabled = false;
            selectFrame.SetActive(true);
        }
    }

    public GameObject CreateNewPrefab() {
        GameObject itemHolding = Instantiate(itemHoldPrefab, transform.position, itemHoldPrefab.transform.rotation);
        itemHolding.transform.SetParent(canvasParent.transform);
        itemHolding.transform.SetSiblingIndex(itemHolding.transform.GetSiblingIndex() - 5);

        if (iconSprite != null) {
            Image iconImage = itemHolding.transform.Find("Icon").GetComponent<Image>();
            iconImage.sprite = iconSprite;
        }

        Image panelImage = itemHolding.transform.Find("Panel").GetComponent<Image>();
        panelImage.color = this.panelImage.color;

        itemHolding.tag = "Untagged";
        itemHolding.transform.position = transform.position;
        itemHolding.GetComponent<Command>().SetCommandType(type);
        itemHolding.GetComponent<Command>().SetFunctions(function);

        return itemHolding;
    }

    public void OnClick() {

        if (levelManager.IsRunning()) {
            return;
        }

        if (levelManager.GetSelectedFunction() != null && amount > 0) {
            Functions function = levelManager.GetSelectedFunction();

            int emptySlot = -1;

            for (int i = 0; i < function.GetCommandList().Count; i++) {
                if (function.GetCommandList()[i].GetCommandType() == CommandType.NONE) {
                    emptySlot = i;
                    break;
                }
            }

            if (emptySlot != -1) {
                function.GetCommandList()[emptySlot].SetCommandType(type);
                function.GetCommandList()[emptySlot].SetFunctions(this.function);

                GameObject item = CreateNewPrefab();
                item.transform.position = transform.position;
                item.GetComponent<Command>().parentFunction = function;
                function.GetCommandList()[emptySlot].currentItemObject = item;
                amount--;
            }
        }
    }
}
