using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FactionType { faction_1, faction_2 }

public class RtsEntity : MonoBehaviour
{
    public string entityName;
    public int maxHealth;
    public int health;
    public int price;
    public Sprite preview;
    public string description;
    public FactionType faction;
    public bool isSelectable = true;

    public Renderer[] renderers;    

    void Start()
    {
        SetColor();
    }
    public void SetColor()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = faction == FactionType.faction_1 ? Color.blue : Color.red;
        }
    }

    public void CheckHealth()
    {
        if (health > maxHealth)
            health = maxHealth;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

}
