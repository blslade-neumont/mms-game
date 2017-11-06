using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(TimeSlave))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] CameraController cameraController;
    [SerializeField] float moveSpeed = 4;
    [SerializeField] float jumpSpeed = 6;
    [SerializeField] float gravityConst = 9.81f;

    private CharacterController characterController;
    private TimeSlave slave;
    private float vspeed;

    void Start()
    {
        this.characterController = GetComponent<CharacterController>();
        this.slave = GetComponent<TimeSlave>();
    }

    private void Update()
    {
        if (this.slave == null) this.NormalUpdate();
    }
    void NormalUpdate()
    {
        var deltaTime = Time.deltaTime; //Ignoring time scale because it doesn't apply to the player

        var horiz = Input.GetAxis("Horizontal");
        var vert = Input.GetAxis("Vertical");
        var jump = Input.GetButton("Jump");

        var cam = cameraController.transform.forward;
        var forward = new Vector3(cam[0], 0, cam[2]);
        var sideways = Vector3.Cross(Vector3.up, forward);

        this.vspeed -= this.gravityConst * deltaTime;
        var moveVector = ((forward * vert) + (sideways * horiz)) * this.moveSpeed;
        if (this.characterController.isGrounded && jump) this.vspeed = this.jumpSpeed;
        moveVector += new Vector3(0, this.vspeed, 0);
        this.characterController.Move(moveVector * deltaTime);
        if (this.characterController.isGrounded) this.vspeed = 0;
    }

    private void OnDrawGizmos()
    {
        var cam = cameraController.transform.forward;
        var forward = new Vector3(cam[0], 0, cam[2]);
        var sideways = Vector3.Cross(Vector3.up, forward);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(this.transform.position, forward);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(this.transform.position, sideways);
    }

    private void OnDestroy()
    {
        var destroy = Destroy;
        if (destroy != null) destroy.Invoke(this, EventArgs.Empty);
    }
    public event EventHandler Destroy;
}
