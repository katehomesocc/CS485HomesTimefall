using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRequest
{
    public Card actionCard;

    public bool doBoard = false;
    public bool doHand = false;
    public bool doDiscard = false;

    public bool isBot = false;

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

        return string.Format("doBoard:{0}, doHand:{1}, doDiscard:{2},\npotentialBoardTargets:{3}, potentialHandTargets:{4}, potentialDiscardedTargets:{5},\nactiveBoardTargets:{6}, activeHandTargets:{7}, activeDiscardedTargets:{8}",
        doBoard, doHand, doDiscard, potentialBoard,potentialHand,potentialDiscard,activeBoard,activeHand,activeDiscard);
    }
}
