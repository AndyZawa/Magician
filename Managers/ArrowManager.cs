using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    public static GameObject arrow;
    public static Vector3 offset;

    private void Awake()
    {
        arrow = Resources.Load( GameConsts.ARROW_INDICATOR_PREFAB_PATH ) as GameObject;
    }

    public static GameObject SpawnArrow( Vector3 slotPosition, Vector3 rotation )
    {
        GameObject newArrow =  Instantiate(arrow, slotPosition + offset, Quaternion.Euler( rotation ) );
        return newArrow;
    }

}
