using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool RetrieveJumpInput()
    {
        return Input.GetButtonDown("Jump");
    }

    public bool RetrieveJumpDown()
    {
        return Input.GetButton("Jump");
    }

    public float RetrieveMoveInput()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public float RetrieveVerticalLookInput()
    {
        return Input.GetAxisRaw("Vertical");
    }

    public bool RetrieveAttackInput()
    {
        return Input.GetKeyDown(KeyCode.Mouse0);
    }

    public bool RetrieveShiftInput()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }

    public bool RetrieveJInput()
    {
        return Input.GetKeyDown(KeyCode.J);
    }
}
