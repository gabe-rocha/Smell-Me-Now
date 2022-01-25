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

    public enum Items{
        boostBeansRaw,
        boostBeansCan    
    }

    internal static float locationRefreshInterval = 5f;
    internal static float fartListRefreshIntervalSecs = 5f;
    internal static int defaultNumBeansCurrency = 200;
    internal static int defaultBoostNumBeansRaw = 3;
    internal static int defaultNumFarts = 20;
    internal static int defaultBeansRadiusCost = 150;
    internal static int maxBeansRadiusCost = 900;
    internal static int defaultBoostNumBeansCanNormal = 2;
    internal static int BoostBeansNormalAddAmount = 1;
    internal static int BoostBeansCanAddAmount = 5;
}