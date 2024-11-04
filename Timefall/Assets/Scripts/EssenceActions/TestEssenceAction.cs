using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEssenceAction : EssenceAction
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool CanTargetSpace(BoardSpace boardSpaces)
    {
        Debug.Log("TestEssenceAction: CanTargetSpace");
        return true;
    }

    public override List<BoardSpace> GetTargatableSpaces(List<BoardSpace> spaces)
    {
        Debug.Log("TestEssenceAction: GetTargatableSpaces");
        return spaces;
    }
}
