using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] Room[] rooms;
    public Room currentRoom;
    int roomNum = -1;
    [SerializeField] bool noStartingRoom = false;
    [SerializeField] bool hasEndingRoom = false;
    [SerializeField] GameObject endVisuals;

    private void Start()
    {
        if (noStartingRoom) return;
        NewRoom();
    }

    public void NewRoom()
    {
        roomNum++;
        currentRoom = rooms[roomNum];
        currentRoom.OpenDoor();
    }

    public void OnEnemyKilled(EnemyHealth e)
    {
        currentRoom.enemies.Remove(e);
        currentRoom.AlertAllEnemies();

        // check if all enemies dead
        if (currentRoom.enemies.Count <= 0) 
        {
            // check if no rooms left
            if (currentRoom == rooms[rooms.Length-1])
            {
                if (hasEndingRoom) EndMission();
            }
            else NewRoom();
        }
    }

    private void EndMission()
    {
        endVisuals.SetActive(true);
    }
}
