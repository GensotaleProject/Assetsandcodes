using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScript : MonoBehaviour
{
    [Range(0, 30)] public float rotatesPerSecond;
    public bool counterclockwise;
    public float timeRemaining = float.MaxValue;
    Transform thisTrans;

    public enum Axis { X, Y, Z };

    void Awake()
    {
        thisTrans = transform;
    }
    
    void Update()
    {
        RotateObject(thisTrans, rotatesPerSecond, counterclockwise);   
    }

    public static void RotateObject(Transform _object, float _rotatesPerSecond, bool _counterClockwise)
    {
        _object.Rotate(0, 0, (_rotatesPerSecond * 360 * GameMaster.gameMaster.frameTime * GameMaster.gameMaster.timeScale) * ((_counterClockwise) ? 1 : -1));
    }

    public static void RotateObject(Transform _object, float _rotatesPerSecond, bool _counterClockwise, Axis _axis)
    {
        switch (_axis)
        {
            case Axis.X:
                _object.Rotate((_rotatesPerSecond * 360 * GameMaster.gameMaster.frameTime * GameMaster.gameMaster.timeScale) * ((_counterClockwise) ? 1 : -1), 0, 0);
                break;

            case Axis.Y:
                _object.Rotate(0, (_rotatesPerSecond * 360 * GameMaster.gameMaster.frameTime * GameMaster.gameMaster.timeScale) * ((_counterClockwise) ? 1 : -1), 0);
                break;

            case Axis.Z:
                _object.Rotate(0, 0, (_rotatesPerSecond * 360 * GameMaster.gameMaster.frameTime * GameMaster.gameMaster.timeScale) * ((_counterClockwise) ? 1 : -1));
                break;
        }
    }
}
