using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] public GameObject follow;
    [SerializeField] public float followDist = 8;
    [SerializeField] public float raiseDist = 3;
    private new Camera camera;

    void Start()
    {
        this.camera = GetComponent<Camera>();
    }

    void Update()
    {
        var followPos = this.follow.transform.position;
        var away = new Vector3(transform.position.x - followPos.x, 0, transform.position.z - followPos.z);
        var normalizedAway = away.normalized * this.followDist;
        this.transform.position = followPos + new Vector3(normalizedAway[0], raiseDist, normalizedAway[2]);
        this.transform.LookAt(this.follow.transform);
    }
}
