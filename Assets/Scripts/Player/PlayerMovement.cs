using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Server controlled")]
    public float velocityChangeMod;
    public float velocityMod;
    public float sprintingMod;
    public Vector2 inputVelocity;
    public bool pressingShift;
    public Rigidbody2D rigidBody;
    public PlayerAnimations animations;

    [Command]
    private void CmdMoveInput(Vector2 input, bool pressingShift)
    {
        inputVelocity = input;
        this.pressingShift = pressingShift;
    }

    private void FixedUpdate()
    {
        if (!isServer) return;
        Vector2 velocity = Vector2.ClampMagnitude(inputVelocity, 1f) * velocityMod * (pressingShift ? 1.5f : 1f);
        rigidBody.velocity = Vector2.Lerp(rigidBody.velocity, velocity, Time.deltaTime * velocityChangeMod);

        if (velocity.x < 0f) transform.localRotation = new Quaternion(0f, 180f, 0f, 0f);
        else if (velocity.x > 0f) transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);

        if (velocity != Vector2.zero) animations.ServerChangeAnimation(pressingShift ? AnimationState.Run : AnimationState.Walk);
        else animations.ServerChangeAnimation(AnimationState.Idle);
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        CmdMoveInput(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), Input.GetKey(KeyCode.LeftShift));
    }
}
