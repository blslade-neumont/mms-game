using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DoorHinge : MonoBehaviour
{
    [SerializeField] public float closedRotation = 0;
    [SerializeField] public float openRotation = 90;
    [SerializeField] public float timeToOpen = 1.5f;
    [SerializeField] public Button[] buttons;

    private TimeSlave slave;
    private float openAmount;

    void Start()
    {
        this.slave = this.GetComponent<TimeSlave>();
        if (this.slave) this.slave.canCreatePhantom = false;
    }

    void Update()
    {
        if (this.slave == null) this.NormalUpdate();
    }
    void NormalUpdate()
    {
        var deltaTime = Time.deltaTime * (this.slave == null ? 1 : this.slave.time.timeScale);
        var shouldOpen = (buttons.Length == 0 || buttons.All(btn => btn.isPressed));
        if (shouldOpen) openAmount += deltaTime / this.timeToOpen;
        else openAmount -= deltaTime / timeToOpen;
        openAmount = Mathf.Clamp(openAmount, 0, 1);
        var oldEuler = this.transform.eulerAngles;
        this.transform.eulerAngles = new Vector3(oldEuler.x, Mathf.Lerp(closedRotation, openRotation, openAmount), oldEuler.z);
    }
}
