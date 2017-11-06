using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] public bool isPressed = false;
    private int entitiesPressing = 0;

    void Start()
    {
    }

    void Update()
    {
        this.isPressed = entitiesPressing > 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var otherPlayer = other.gameObject.GetComponent<PlayerController>();
            otherPlayer.Destroyed += OtherPlayer_Destroy;
            entitiesPressing++;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var otherPlayer = other.gameObject.GetComponent<PlayerController>();
            otherPlayer.Destroyed -= OtherPlayer_Destroy;
            entitiesPressing--;
        }
    }

    private void OtherPlayer_Destroy(object sender, System.EventArgs e)
    {
        entitiesPressing--;
        ((PlayerController)sender).Destroyed -= OtherPlayer_Destroy;
    }
}
