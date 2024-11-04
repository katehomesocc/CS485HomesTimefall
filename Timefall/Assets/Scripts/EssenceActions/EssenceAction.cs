using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class EssenceAction : MonoBehaviour 
{

    public abstract List<BoardSpace> GetTargatableSpaces(List<BoardSpace> boardSpaces);

    public abstract bool CanTargetSpace(BoardSpace boardSpaces);

}
