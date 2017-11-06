using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum TimeCrystalType
{
    Pause,
    Rewind,
    Record
}

[RequireComponent(typeof(TimeSlave))]
public class TimeCrystal : MonoBehaviour
{
    [SerializeField] public GameObject crystalRootObject;
    [SerializeField] public TimeCrystalType type;

    private TimeSlave slave;

    void Start()
    {
        slave = this.GetComponent<TimeSlave>();
        slave.canCreatePhantom = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var time = slave.time;
            switch (this.type)
            {
            case TimeCrystalType.Pause:
                time.pauseCrystals++;
                break;

            case TimeCrystalType.Rewind:
                time.rewindCrystals++;
                break;

            case TimeCrystalType.Record:
                time.recordCrystals++;
                break;

            default:
                throw new NotImplementedException();
            }
            this.slave.time.RemoveState(this.slave);
            Destroy(crystalRootObject);
        }
    }
}
