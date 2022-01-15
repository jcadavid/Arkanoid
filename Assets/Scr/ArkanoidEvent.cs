
public static class ArkanoidEvent
{
    public delegate void BallDeadZoneAction(Ball ball);
    public static BallDeadZoneAction OnBallReachDeadZoneEvent;
    public delegate void BlockDestroyedAction(int blockID);
    public static BlockDestroyedAction OnBlockDestroyedEvent;

    public delegate void PowerUpScoreAction(int score);
    public static PowerUpScoreAction OnPowerUpScoreEvent;
    public delegate void PowerUpChangeBallSpeed(float velocity, int seconds);
    public static PowerUpChangeBallSpeed OnPowerUpChangeBallSpeedEvent;
    public delegate void PowerUpChangeScalePaddle(float scale);
    public static PowerUpChangeScalePaddle OnPowerUpChangeScalePaddleEvent;
}
