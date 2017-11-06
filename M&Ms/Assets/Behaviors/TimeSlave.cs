using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeSlave : MonoBehaviour
{
    [SerializeField] public TimeController time;
    [SerializeField] public TimeSlave recordingPhantomFor = null;
    [SerializeField] public bool ignorePause = false;
    [SerializeField] public bool ignoreRewind = false;

    [SerializeField] public bool isActive = true;
    [SerializeField] public object extraState = null;

    void Start()
    {
    }

    void Update()
    {
        if (this.recordingPhantomFor != null)
        {
            if (time.timeState != TimeState.PlaybackRecording)
            {
                this.gameObject.SendMessage("DestroyPhantom", SendMessageOptions.DontRequireReceiver);
                Destroy(this.gameObject);
                return;
            }

            this.time.FetchState(this, true);
            this.gameObject.SendMessage("PlaybackUpdate", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            switch (time.timeState)
            {
            case TimeState.Normal:
            case TimeState.PlaybackRecording:
                this.gameObject.SendMessage("NormalUpdate", SendMessageOptions.DontRequireReceiver);
                this.time.RecordState(this);
                break;
            
            case TimeState.Paused:
                if (this.ignorePause) goto case TimeState.Normal;
                else goto case TimeState.RewindingRecording;

            case TimeState.Rewinding:
                if (this.ignoreRewind) goto case TimeState.Normal;
                else goto case TimeState.RewindingRecording;

            case TimeState.Recording:
                this.gameObject.SendMessage("NormalUpdate", SendMessageOptions.DontRequireReceiver);
                this.time.RecordState(this, false);
                this.time.RecordState(this, true);
                break;

            case TimeState.RewindingRecording:
                this.time.FetchState(this);
                this.gameObject.SendMessage("PlaybackUpdate", SendMessageOptions.DontRequireReceiver);
                break;
            }
        }
    }

    public void TriggerTimeStateChanged()
    {
        this.gameObject.SendMessage("TimeStateChanged", SendMessageOptions.DontRequireReceiver);
        if (this.time.timeState == TimeState.PlaybackRecording)
        {
            var clone = Instantiate(this.gameObject);
            var phantomTimeSlave = clone.GetComponent<TimeSlave>();
            phantomTimeSlave.recordingPhantomFor = this;
            clone.SendMessage("CreatePhantom", SendMessageOptions.DontRequireReceiver);
        }
    }
}
