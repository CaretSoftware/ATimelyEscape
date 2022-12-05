using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeSceneManager : MonoBehaviour
{
    private static int[] _room0 = new[] { 0, 1, 2 };
    private static int[] _room1 = new[] { 1, 2, 3 };
    private static int[] _room2 = new[] { 2, 3, 4 };
    private static int[] _room3 = new[] { 3, 4, 5 };
    private static int[] _room4 = new[] { 4, 5, 6 };
    private static int[] _room5 = new[] { 5, 6, 7 };
    private static int[] _room6 = new[] { 6, 7 };
    private static int[] _room7 = new[] { 7 };

    private static int[][] _rooms = new int[][] {
        _room0,    // 0 cageRoom 
        new int[] { 1, 2, 3 },    // 1 Incubator
        new int[] { 2, 3, 4 },    // 3 
        new int[] { 3, 4, 5 },    // 4 
        new int[] { 4, 5, 6 },    // 5 
        new int[] { 5, 6, 7 },    // 6 
        new int[] { 6, 7 },    // 7 
    };
    
    private int _currentRoom = 0;
    
    private void Awake() {
        
    }
}
