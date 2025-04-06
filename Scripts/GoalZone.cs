using System;
using UnityEngine;
using System.Collections;

public class GoalZone : MonoBehaviour
{
    public static event Action<int> OnGoalScored;

    public int side;
    // invoking the goal score event based on the side ( 0 : left and 1 : right )
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Football"))
        {
            Debug.Log("Goal on " + side + "side!!");
            OnGoalScored?.Invoke(side);
        }
    }
}
