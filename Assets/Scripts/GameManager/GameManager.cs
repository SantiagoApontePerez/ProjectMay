using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager {
    
    // Singleton Game Manager
    private static readonly GameManager sInstance = new GameManager();

    static GameManager() { }
    private GameManager() { }

    public static GameManager Instance { get { return sInstance; } }

    private bool mGamePaused = false;

    public bool IsPaused { get { return mGamePaused; } }

    public void Quit() {
        Application.Quit();
    }
}
