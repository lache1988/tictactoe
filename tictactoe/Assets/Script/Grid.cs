using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public GameObject TankBlue;
    public GameObject TankRed;
    public int gridX;
    public int gridY;

    public enum GridState
    {
        IsEmpty,IsBlue,IsRed
    }

    public GridState _gridstate;
    
    void Start()
    {
    }

    void Update()
    {
        SetTank();
    }

    public void Reset()
    {
        this._gridstate = GridState.IsEmpty;
    }

    public void SetState(bool isBlue)
    {
        if (isBlue)
            _gridstate = GridState.IsBlue;
        else
            _gridstate = GridState.IsRed;
    }

    void SetTank()
    {
        if(_gridstate == GridState.IsEmpty)
                {
                    TankBlue.SetActive(false);
                    TankRed.SetActive(false);
                }
                else if (_gridstate == GridState.IsBlue)
                {
                    TankBlue.SetActive(true);
                    TankRed.SetActive(false);
                }
                else if (_gridstate == GridState.IsRed)
                {
                    TankBlue.SetActive(false);
                    TankRed.SetActive(true);
        }
    }
}
