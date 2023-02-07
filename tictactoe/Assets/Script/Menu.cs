using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public ChessManager manager;
    public CinemachineVirtualCamera vcam1;
    public CinemachineVirtualCamera vcam2;
    public CinemachineVirtualCamera vcam3;
    public TextMeshPro wintip;
    public TextMeshPro turntip;
    public GameObject turnBluetank;
    public GameObject turnRedtank;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //menu
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.collider.gameObject.name == "TankBlue")
                {
                    manager._gameState = ChessManager.GameState.blueTurn;
                    manager.isPlayerBlue = true;
                    vcam3.Priority = 99;
                    vcam1.Priority = 0;
                    vcam2.Priority = 0;
                }
                if (hit.collider.gameObject.name == "TankRed")
                {
                    manager._gameState = ChessManager.GameState.blueTurn;
                    manager.isPlayerBlue = false;
                    vcam3.Priority = 99;
                    vcam1.Priority = 0;
                    vcam2.Priority = 0;
                }


                if (hit.collider.gameObject.name == "onePlayerButton")
                {
                    manager.BoardReset();
                    manager._gamemode = ChessManager.GameMode.onePlayer;
                    vcam3.Priority = 0;
                    vcam1.Priority = 0;
                    vcam2.Priority = 99;
                }
                if (hit.collider.gameObject.name == "twoPlayerButton")
                {
                    manager.BoardReset();
                    manager._gamemode = ChessManager.GameMode.twoPlayer;
                    manager._gameState = ChessManager.GameState.blueTurn;
                    vcam3.Priority = 99;
                    vcam1.Priority = 0;
                    vcam2.Priority = 0;
                }
            }
        }

        //restart
        if (Input.GetKey(KeyCode.R)&&manager._gameState!=ChessManager.GameState.setup)
        {
            manager.StopAllCoroutines();
            manager._gameState = ChessManager.GameState.blueTurn;
            manager.BoardReset();
            menuReset();
            
        }


        //back to menu
        if (Input.GetKey(KeyCode.Escape))
        {
            manager.BoardReset();
            manager._gameState = ChessManager.GameState.setup;
            menuReset();
            vcam3.Priority = 0;
            vcam1.Priority = 99;
            vcam2.Priority = 0;
        }

        //win tip
        if (manager._gameState == ChessManager.GameState.blueWin)
        {
            wintip.text = "Blue Win!";
            turntip.text = "";
            wintip.color = Color.blue;
            turnRedtank.SetActive(false);
            turnBluetank.SetActive(false);
        }
        else if (manager._gameState == ChessManager.GameState.redWin)
        {
            wintip.text = "Red Win!";
            wintip.color = Color.red;
            turntip.text = "";
            turnRedtank.SetActive(false);
            turnBluetank.SetActive(false);
        }
        else if (manager._gameState == ChessManager.GameState.draw)
        {
            wintip.text = "Draw";
            wintip.color = Color.yellow;
            turntip.text = "";
            turnRedtank.SetActive(false);
            turnBluetank.SetActive(false);
        }
        else
        {
            wintip.text = "";

            //turntip
            if (manager._gameState == ChessManager.GameState.blueTurn)
            {
                turntip.text = "Blue Turn";
                turntip.color = Color.blue;
                turnBluetank.SetActive(true);
                turnRedtank.SetActive(false);
            }
            else if (manager._gameState == ChessManager.GameState.redTurn)
            {
                turntip.text = "                    Red Turn";
                turntip.color = Color.red;
                turnBluetank.SetActive(false);
                turnRedtank.SetActive(true);
            }
        }

        
            
    }
    void menuReset()
    {
        wintip.text = "";
        turntip.text = "";
    }
}
