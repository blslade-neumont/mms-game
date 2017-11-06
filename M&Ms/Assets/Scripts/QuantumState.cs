using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public struct FiniteState
{
    public FiniteState(float time, TimeSlave slave)
    {
        this.time = time;
        this.position = slave.transform.position;
        this.rotation = slave.transform.rotation;
        this.isActive = slave.isActive;
        this.extra = slave.extraState;
    }

    public float time;
    public Vector3 position;
    public Quaternion rotation;
    public bool isActive;
    public object extra;
}

public class QuantumState
{
    public QuantumState(TimeController time, TimeSlave slave)
    {
        this.time = time;
        this.slave = slave;
    }

    private TimeController time;
    private TimeSlave slave;

    public List<FiniteState> storage = new List<FiniteState>();
    public List<FiniteState> recordingStorage = new List<FiniteState>();

    private int indexOfFirst(List<FiniteState> storage, Predicate<float> predicate)
    {
        return indexOfFirst(storage, (FiniteState state) => predicate(state.time));
    }
    private int indexOfFirst(List<FiniteState> storage, Predicate<FiniteState> predicate)
    {
        for (var q = 0; q < storage.Count; q++)
        {
            if (predicate(storage[q])) return q;
        }
        return -1;
        //var min = 0;
        //var max = storage.Count;
        //while (max > min + 1)
        //{
        //    var check = min + ((max - min) / 2);
        //    if (predicate(storage[check])) max = check + 1;
        //    else min = check + 1;
        //}
        //if (min >= max) return -1;
        //return max - 1;
    }
    private void deleteAfter(List<FiniteState> storage, float time)
    {
        if (storage.Count == 0 || storage[storage.Count - 1].time < time) return;
        var idx = this.indexOfFirst(storage, (t) => t >= time);
        if (idx == -1) return;
        storage.RemoveRange(idx, storage.Count - idx);
    }
    private void deleteBefore(List<FiniteState> storage, float time)
    {
        if (storage.Count == 0 || storage[storage.Count - 1].time < time) return;
        var idx = this.indexOfFirst(storage, (t) => t >= time);
        if (idx == -1) storage.Clear();
        else if (idx != 0) storage.RemoveRange(0, idx);
    }

    public void RecordState(bool recording = false)
    {
        var storage = (recording ? this.recordingStorage : this.storage);
        var currentTime = this.time.currentTime;
        this.deleteAfter(storage, currentTime);
        storage.Add(new FiniteState(currentTime, slave));
    }
    public void FetchState(bool recording = false, TimeSlave applyTo = null)
    {
        var storage = (recording ? this.recordingStorage : this.storage);
        var currentTime = this.time.currentTime;
        if (storage.Count == 0) return;
        var idx = this.indexOfFirst(storage, t => t >= currentTime);
        if (idx > 0) idx--;
        if (idx == -1) idx = storage.Count - 1;
        var state = storage[idx];
        if (applyTo == null) applyTo = slave;
        applyTo.transform.position = state.position;
        applyTo.transform.rotation = state.rotation;
        applyTo.isActive = state.isActive;
        applyTo.extraState = state.extra;
    }

    public void DeleteHistory(float deleteBefore)
    {
        this.deleteBefore(storage, deleteBefore);
        this.deleteBefore(recordingStorage, deleteBefore);
    }
}
