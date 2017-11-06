using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum TimeState
{
    Normal,
    Paused,
    Rewinding,
    Recording,
    RewindingRecording,
    PlaybackRecording
}

public class TimeController : MonoBehaviour
{
    [SerializeField] public Text HUDText;

    [SerializeField] public int pauseCrystals = 0;
    [SerializeField] public int pauseCrystalsRequired = 3;
    [SerializeField] public int pauseDuration = 5;

    [SerializeField] public int rewindCrystals = 0;
    [SerializeField] public int rewindCrystalsRequired = 3;
    [SerializeField] public int rewindDuration = 5;

    [SerializeField] public int recordCrystals = 0;
    [SerializeField] public int recordCrystalsRequired = 3;
    [SerializeField] public int recordDuration = 5;
    [SerializeField] public int recordRewindSpeed = 5;

    [SerializeField] public bool ignoreCrystalCount = false;

    [SerializeField] public TimeState timeState = TimeState.Normal;
    [SerializeField] public float timeStateDuration = 0;

    [SerializeField] public float timeBetweenGC = 3;
    [SerializeField] public float historyBuffer = 2;

    public float currentTime = 0;
    private float timeBeforeGC = 3;

    void Start()
    {
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        if (this.timeState != TimeState.Normal)
        {
            this.timeStateDuration -= Time.deltaTime;
            if (this.timeStateDuration <= 0) deltaTime += this.timeStateDuration;
        }
        currentTime += deltaTime * this.timeScale;

        var prevState = this.timeState;
        if (this.timeStateDuration <= 0 && this.timeState != TimeState.Normal)
        {
            if (this.timeState == TimeState.Recording)
            {
                this.timeStateDuration = this.recordDuration / this.recordRewindSpeed;
                this.timeState = TimeState.RewindingRecording;
            }
            else if (this.timeState == TimeState.RewindingRecording)
            {
                this.timeStateDuration = this.recordDuration;
                this.timeState = TimeState.PlaybackRecording;
            }
            else
            {
                if (this.timeState == TimeState.PlaybackRecording) this.clearRecordingStorage();
                this.timeStateDuration = 0;
                this.timeState = TimeState.Normal;
            }
        }
        if (this.timeState == TimeState.Normal)
        {
            if (Input.GetButton("TriggerPause") && ((this.pauseCrystals >= this.pauseCrystalsRequired) || this.ignoreCrystalCount))
            {
                this.pauseCrystals = Mathf.Max(0, this.pauseCrystals - this.pauseCrystalsRequired);
                this.timeState = TimeState.Paused;
                this.timeStateDuration = this.pauseDuration;
            }
            else if (Input.GetButton("TriggerRewind") && ((this.rewindCrystals >= this.rewindCrystalsRequired) || this.ignoreCrystalCount))
            {
                this.rewindCrystals = Mathf.Max(0, this.rewindCrystals - this.rewindCrystalsRequired);
                this.timeState = TimeState.Rewinding;
                this.timeStateDuration = this.rewindDuration;
            }
            else if (Input.GetButton("TriggerRecord") && ((this.recordCrystals >= this.recordCrystalsRequired) || this.ignoreCrystalCount))
            {
                this.recordCrystals = Mathf.Max(0, this.recordCrystals - this.recordCrystalsRequired);
                this.timeState = TimeState.Recording;
                this.timeStateDuration = this.recordDuration;
            }
        }
        if (prevState != this.timeState)
        {
            foreach (var slave in slaves)
            {
                slave.TriggerTimeStateChanged();
            }
        }

        this.timeBeforeGC -= Time.deltaTime;
        if (this.timeBeforeGC <= 0)
        {
            this.timeBeforeGC = this.timeBetweenGC;
            var deleteBefore = this.currentTime - (Mathf.Max(this.rewindDuration, this.recordDuration) + this.historyBuffer);
            foreach (var state in storage.Values)
            {
                state.DeleteHistory(deleteBefore);
            }
        }

        if (this.HUDText)
        {
            var secondsLeft = Mathf.Ceil(this.timeStateDuration);
            switch (this.timeState)
            {
            case TimeState.Normal:
                if (this.ignoreCrystalCount)
                {
                    this.HUDText.text = "Pause (1)\r\n" +
                                        "Rewind (2)\r\n" +
                                        "Record (3)";
                }
                else
                {
                    this.HUDText.text = "Pause (1): " + this.pauseCrystals + " of " + this.pauseCrystalsRequired + " crystals\r\n" +
                                        "Rewind (2): " + this.rewindCrystals + " of " + this.rewindCrystalsRequired + " crystals\r\n" +
                                        "Record (3): " + this.recordCrystals + " of " + this.recordCrystalsRequired + " crystals";
                }
                break;

            case TimeState.Paused:
                this.HUDText.text = "Paused! " + secondsLeft + " seconds left.";
                break;

            case TimeState.Rewinding:
                this.HUDText.text = "Rewinding! " + secondsLeft + " seconds left.";
                break;

            case TimeState.Recording:
                this.HUDText.text = "Recording! " + secondsLeft + " seconds left.";
                break;

            case TimeState.RewindingRecording:
                this.HUDText.text = "Rewinding...";
                break;

            case TimeState.PlaybackRecording:
                this.HUDText.text = "Playing back recording! " + secondsLeft + " seconds left.";
                break;
            }
        }
    }

    private List<TimeSlave> slaves = new List<TimeSlave>();
    private Dictionary<TimeSlave, QuantumState> storage = new Dictionary<TimeSlave, QuantumState>();
    private QuantumState getQuantumState(TimeSlave slave)
    {
        if (storage.ContainsKey(slave)) return storage[slave];
        this.slaves.Add(slave);
        return this.storage[slave] = new QuantumState(this, slave);
    }
    private void removeQuantumState(TimeSlave slave)
    {
        if (!storage.ContainsKey(slave)) return;
        storage.Remove(slave);
        slaves.Remove(slave);
    }
    private void clearRecordingStorage()
    {
        foreach (var val in storage.Values)
        {
            val.recordingStorage.Clear();
        }
    }

    public void RecordState(TimeSlave slave, bool recording = false)
    {
        var state = this.getQuantumState(slave);
        state.RecordState(recording);
    }
    public void FetchState(TimeSlave slave, bool recording = false)
    {
        var state = this.getQuantumState(slave.recordingPhantomFor ?? slave);
        state.FetchState(recording, slave);
    }
    public void RemoveState(TimeSlave slave)
    {
        this.removeQuantumState(slave);
    }

    public float timeScale
    {
        get
        {
            switch (this.timeState)
            {
            case TimeState.Normal:
            case TimeState.Recording:
            case TimeState.PlaybackRecording:
                return 1;

            case TimeState.Paused:
                return 0;

            case TimeState.Rewinding:
                return -1;

            case TimeState.RewindingRecording:
                return -this.recordRewindSpeed;

            default:
                throw new NotImplementedException();
            }
        }
    }
}
