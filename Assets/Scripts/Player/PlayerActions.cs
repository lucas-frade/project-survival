using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerActions : NetworkBehaviour
{
    [Header("Client controlled")]
    public Transform weapon;

    [Command]
    private void CmdMoveWeapon(Quaternion rotation, Vector2 position)
    {
        weapon.rotation = rotation;
        weapon.position = position;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        CmdMoveWeapon(Quaternion.Euler(0f, 0f, angle - 90), (Vector2)transform.position + ((direction * 999f).normalized * 0.4f) + new Vector2(0f, 0.5f));
    }
}
