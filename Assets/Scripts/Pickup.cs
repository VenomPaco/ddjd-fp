using System;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class Pickup : MonoBehaviour
{

    private String _labelText;
    private StarterAssetsInputs _input;

    private List<GameObject> _items;
    // Start is called before the first frame update
    void Start()
    {
        _items = new List<GameObject>();
        _input = GetComponent<StarterAssetsInputs>();
        _labelText = "Hit E to pick up";
    }

    // Update is called once per frame
    void Update()
    {
        if (_input.pickup)
        {
                if (_items.Count > 0)
                {
                    if (_items[0] != null)
                    {
                        Destroy(_items[0]);
                        Debug.Log("Destroy");
                    }
                    _items.RemoveAt(0);
                }
        }
    }
    
    private void OnGUI()
    {
        if (_items.Count > 0)
        {
            GUI.Box(new Rect(140,Screen.height-50,Screen.width-300,120),(_labelText));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            _items.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            _items.Remove(other.gameObject);
        }
    }
}
