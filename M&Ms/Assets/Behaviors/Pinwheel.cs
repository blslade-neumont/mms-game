using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinwheel : MonoBehaviour
{
    [SerializeField] public float rotationSpeed = 4;

    private TimeSlave slave;

    void Start()
    {
        slave = GetComponent<TimeSlave>();
    }

    void Update()
    {
        if (this.slave == null) this.NormalUpdate();
    }
    void NormalUpdate()
    {
        var timeDelta = Time.deltaTime * (this.slave == null ? 1 : this.slave.time.timeScale);
        var oldEuler = this.transform.eulerAngles;
        this.transform.eulerAngles = new Vector3(oldEuler.x, oldEuler.y, oldEuler.z + (timeDelta * rotationSpeed));
    }
}
