using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum Layer
    {
        Monster = 6,
        Ground = 7,
        Block = 8,
    }

    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        Game,
    }
    
    public enum Sound
    {
        BGM,
        Effect,
        MaxCount,
    }

    public enum UIEvent
    {
        Click, 
        Drag,

    }

    public enum MouseEvent
    {
        Press, 
        Click,
    }

    public enum CameraMode
    {
        QuaterView,
    }
}
