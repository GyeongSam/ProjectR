using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum Condition
    {
        None,
        GetKey,
    }

    public enum PlayerColor
    {
        Red,
        Orange,
        Yellow,
        Green,
        Blue,
        Purple,
        None,
    }

    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right,
        Forward,
        Back,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    public static GameObject doors;
    public static GameObject[] Mirror;
}
