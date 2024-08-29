using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerCamera : NetworkBehaviour
{
    private void LateUpdate()
    {
        if (!isLocalPlayer) return;
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(transform.position.x, transform.position.y, -10f), Time.deltaTime * 3f);
    }
}
