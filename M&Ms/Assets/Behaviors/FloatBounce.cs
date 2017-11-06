using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatBounce : MonoBehaviour
{
    [SerializeField] public float bounceMax = 4;
    [SerializeField] public float bounceMin = 1;
    [SerializeField] public float cycleDuration = 2;
    [SerializeField] public float rotationAdd = 3;

    private float currentTime = 0;
    private TimeSlave slave;

    void Start()
    {
        this.slave = this.GetComponent<TimeSlave>();
        if (this.slave) this.currentTime = this.slave.time.currentTime;
    }

    void Update()
    {
        if (this.slave == null) this.updatePosition(Time.deltaTime);
        else this.currentTime = this.slave.time.currentTime;
    }
    void NormalUpdate()
    {
        this.updatePosition(Time.deltaTime * this.slave.time.timeScale);
    }

    private void updatePosition(float timeDelta)
    {
        this.currentTime += timeDelta;
        var cycleTime = ((this.currentTime % this.cycleDuration) / this.cycleDuration) * (2 * Mathf.PI);
        var posY = bounceMin + (bounceMax - bounceMin) * ((Mathf.Cos(cycleTime) + 1) / 2);
        this.transform.position = new Vector3(this.transform.position.x, posY, this.transform.position.z);
        this.transform.Rotate(Vector3.left, rotationAdd * timeDelta);
    }
}
