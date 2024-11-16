using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRequest
{
    public List<BoardSpace> potentialBoardTargets = new List<BoardSpace>();
    public List<CardDisplay> potentialHandTargets = new List<CardDisplay>();
    public List<Card> potentialDiscardedTargets = new List<Card>();

    public List<BoardSpace> activeBoardTargets = new List<BoardSpace>();
    public List<CardDisplay> activeHandTargets = new List<CardDisplay>();
    public List<Card> activeDiscardedTargets = new List<Card>();

    public BoardSpace boardTarget;
    public CardDisplay handTarget;
    public Card discardedTarget;

    public Player player;

    public void ClearPossibleTargets()
    {
        potentialBoardTargets.Clear();
        potentialHandTargets.Clear();
        potentialDiscardedTargets.Clear();
    }

    public override string ToString()
    {
        string potentialBoard = potentialBoardTargets == null ? "null" : potentialBoardTargets.Count.ToString();
        string potentialHand = potentialHandTargets == null ? "null" : potentialHandTargets.Count.ToString();
        string potentialDiscard = potentialDiscardedTargets == null ? "null" : potentialDiscardedTargets.Count.ToString();
        string activeBoard = activeBoardTargets == null ? "null" : activeBoardTargets.Count.ToString();
        string activeHand = activeHandTargets == null ? "null" : activeHandTargets.Count.ToString();
        string activeDiscard = activeDiscardedTargets == null ? "null" : activeDiscardedTargets.Count.ToString();

        return string.Format("potentialBoardTargets:{0}, potentialHandTargets:{1}, potentialDiscardedTargets:{2}, activeBoardTargets:{3}, activeHandTargets:{4}, activeDiscardedTargets:{5}",
        potentialBoard,potentialHand,potentialDiscard,activeBoard,activeHand,activeDiscard);
    }
}
