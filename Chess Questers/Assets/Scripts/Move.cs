using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{

    public readonly int x;
    public readonly int y;
    public readonly bool IsAttack;
    public readonly bool IsJump;

    public Move(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Move(int x, int y, bool isAttack, bool isJump)
    {
        this.x = x;
        this.y = y;
        IsAttack = isAttack;
        IsJump = isJump;
    }
    


}
