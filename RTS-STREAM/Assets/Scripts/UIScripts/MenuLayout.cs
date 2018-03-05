using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLayout : MonoBehaviour
{

    public MenuCard menuCardPrefab;

    public static MenuLayout singleton;

    void Start()
    {
        if (singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        singleton = this;
    }

    public void AddChildren(Sprite sprite, string text, System.Action buttonAction)
    {
        var menuCard = Instantiate(menuCardPrefab, transform);

        menuCard.image.sprite = sprite;
        menuCard.text.text = text;
        menuCard.button.onClick.RemoveAllListeners();
        menuCard.button.onClick.AddListener(buttonAction.Invoke);
    }
	public void DestroyAllChildren ()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
