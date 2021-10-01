using System.Collections;
using System.Collections.Generic;

public static class Data {
    public enum Events {
        OnTableLoaded,
        OnGameStarted,
        OnGamePaused,
        OnGameUnpaused,
        OnCorrectElementSelected,
        OnGameOver,
        OnTableReady,
        OnElementSelected,
        OnTimeRanOut,
        OnDrawerReady,
        OnElementDrawed,
        OnGameManagerReady,
        OnTimerStarted,
        OnHammerHitGround,
        OnHammerThrown,
    }

    internal static float locationRefreshInterval = 5f;
    internal static float fartListRefreshIntervalSecs = 5f;
}