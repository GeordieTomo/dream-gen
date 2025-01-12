using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CollapseMenu : MonoBehaviour
{

    public Button collapseButton;
    public Button unfoldButton;

    public RectTransform panel;
    public float resizeAmount;
    public List<RectTransform> movePanels;

    public GameObject hideObject;

    public TabNavigation tabNavigation;

    public void Awake() {
        collapseButton.onClick.AddListener(Collapse);
        unfoldButton.onClick.AddListener(Unfold);
    }

    public void Collapse() {
        if (panel != null) panel.sizeDelta += new Vector2(0, -resizeAmount);
        if (movePanels.Any()) {
            for (int i = 0; i < movePanels.Count; i++) {
                movePanels[i].anchoredPosition += new Vector2(0, resizeAmount);
            }
        }
        if (hideObject != null) hideObject.SetActive(false);

        if (tabNavigation != null)
        {
            tabNavigation.FindSelectableUIElements();
        }
    }

    public void Unfold() {
        if (panel != null) panel.sizeDelta += new Vector2(0, resizeAmount);
        if (movePanels.Any()) {
            for (int i = 0; i < movePanels.Count; i++) {
                movePanels[i].anchoredPosition += new Vector2(0, -resizeAmount);
            }
        }
        if (hideObject != null) hideObject.SetActive(true);

        if (tabNavigation != null)
        {
            tabNavigation.FindSelectableUIElements();
        }
    }
}
