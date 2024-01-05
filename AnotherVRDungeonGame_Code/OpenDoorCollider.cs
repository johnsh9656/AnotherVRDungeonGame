using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        FindObjectOfType<RoomManager>().NewRoom();
    }
}
