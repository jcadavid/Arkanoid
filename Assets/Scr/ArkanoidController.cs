using System.Collections.Generic;
using UnityEngine;

public class ArkanoidController : MonoBehaviour
{
    private const string BALL_PREFAB_PATH = "Prefabs/Ball";
    private const string POWERUP_PREFAB_PATH = "Prefabs/PowerUp";
    private readonly Vector2 BALL_INIT_POSITION = new Vector2(0, -0.86f);

    [SerializeField]
    private GridController _gridController;

    [Space(20)]
    [SerializeField]
    private List<LevelData> _levels = new List<LevelData>();

    private int _currentLevel = 0;

    private Ball _ballPrefab = null;
    private List<Ball> _balls = new List<Ball>();
    private List<PowerUp> _powerups = new List<PowerUp>();
    private int _totalScore = 0;

    bool _multipleballsPU = false;
    [SerializeField]
    float TimeBalls = 5;
    float maxTimeBalls = 2;

    [SerializeField]
    private Paddle _paddle;

    private void Start()
    {
        ArkanoidEvent.OnBallReachDeadZoneEvent += OnBallReachDeadZone;
        ArkanoidEvent.OnBlockDestroyedEvent += OnBlockDestroyed;
        ArkanoidEvent.OnPowerUpScoreEvent += OnPowerUpScoreEvent;
        ArkanoidEvent.OnPowerUpChangeScalePaddleEvent += OnPowerUpChangeScalePaddleEvent;
        ArkanoidEvent.OnPowerUpChangeBallSpeedEvent += OnPowerUpChangeBallSpeedEvent;
        ArkanoidEvent.OnPowerUpAddMoreBallsEvent += OnPowerUpAddMoreBallsEvents;
        ArkanoidEvent.OnPowerUpDestroy += OnPowerUpDestroy;
    }

    private void OnPowerUpAddMoreBallsEvents()
    {
        _multipleballsPU = true;
        TimeBalls = Time.time + maxTimeBalls;
        Debug.Log("Actual Time" + Time.time + "Calculated: " + TimeBalls);

        if (_balls.Count == 1)
        {
            _balls.Add(CreateBallAt(BALL_INIT_POSITION));
            _balls.Add(CreateBallAt(BALL_INIT_POSITION));
        }
        else if (_balls.Count < 3)
        {
            _balls.Add(CreateBallAt(BALL_INIT_POSITION));

        }

    }

    private void OnDestroy()
    {
        ArkanoidEvent.OnBallReachDeadZoneEvent -= OnBallReachDeadZone;
        ArkanoidEvent.OnBlockDestroyedEvent -= OnBlockDestroyed;
        ArkanoidEvent.OnPowerUpScoreEvent -= OnPowerUpScoreEvent;
        ArkanoidEvent.OnPowerUpChangeScalePaddleEvent -= OnPowerUpChangeScalePaddleEvent;
        ArkanoidEvent.OnPowerUpChangeBallSpeedEvent -= OnPowerUpChangeBallSpeedEvent;
        ArkanoidEvent.OnPowerUpAddMoreBallsEvent -= OnPowerUpAddMoreBallsEvents;
        ArkanoidEvent.OnPowerUpDestroy -= OnPowerUpDestroy;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _totalScore = 0;
            ArkanoidEvent.OnScoreUpdatedEvent?.Invoke(0, _totalScore);
            _paddle.resetHorizontalScale(1f);
            _multipleballsPU = false;
            ClearPowerUps();
            InitGame();
        }

        if (_multipleballsPU)
        {
            if (Time.time > TimeBalls)
            {
                Debug.Log("Has elapsed: " + Time.time);
                if (_balls.Count == 1)
                {
                    _multipleballsPU = false;
                }
                else if (_balls.Count > 1)
                {
                    Ball deletedBall = _balls[_balls.Count - 1];
                    _balls.Remove(deletedBall);
                    Destroy(deletedBall.gameObject);
                    Destroy(deletedBall);
                }
            }
        }
    }

    private void ClearPowerUps()
    {
        for (int i = _powerups.Count - 1; i >= 0; i--)
        {
            PowerUp destroyPowerup = _powerups[i];
            DestroyPowerUp(destroyPowerup);
        }

        _powerups.Clear();
    }

    private void DestroyPowerUp(PowerUp powerup)
    {
        if (powerup != null)
        {
            _powerups.Remove(powerup);
            powerup.gameObject.SetActive(false);
            Destroy(powerup.gameObject);
            Destroy(powerup);
        }
    }

    private void InitGame()
    {
        _currentLevel = 0;
        _totalScore = 0;

        _gridController.BuildGrid(_levels[0]);
        ArkanoidEvent.OnGameStartEvent?.Invoke();
        ArkanoidEvent.OnScoreUpdatedEvent?.Invoke(0, _totalScore);
        SetInitialBall();
    }

    private void SetInitialBall()
    {
        ClearBalls();

        Ball ball = CreateBallAt(BALL_INIT_POSITION);
        ball.Init();
        _balls.Add(ball);
    }

    private Ball CreateBallAt(Vector2 position)
    {
        if (_ballPrefab == null)
        {
            _ballPrefab = Resources.Load<Ball>(BALL_PREFAB_PATH);
        }

        return Instantiate(_ballPrefab, position, Quaternion.identity);
    }

    private void ClearBalls()
    {
        for (int i = _balls.Count - 1; i >= 0; i--)
        {
            Ball destroyBall = _balls[i];
            destroyBall.gameObject.SetActive(false);
            Destroy(destroyBall.gameObject);
            Destroy(destroyBall);
        }

        _balls.Clear();
    }

    private void OnBallReachDeadZone(Ball ball)
    {
        ball.Hide();
        _balls.Remove(ball);
        Destroy(ball.gameObject);

        CheckGameOver();
    }

    private void CheckGameOver()
    {
        if (_balls.Count == 0)
        {
            //Game over
            ClearBalls();
            ClearPowerUps();

            Debug.Log("Game Over: LOSE!!!");
            ArkanoidEvent.OnGameOverEvent?.Invoke();
        }
    }



    private void OnBlockDestroyed(int blockId)
    {
        BlockTile blockDestroyed = _gridController.GetBlockBy(blockId);

        if (blockDestroyed != null)
        {
            _totalScore += blockDestroyed.Score;

            ArkanoidEvent.OnScoreUpdatedEvent?.Invoke(blockDestroyed.Score, _totalScore);
            float randomValue = Random.value;
            Vector2 blockDestroyedPosition = blockDestroyed.GetComponent<Transform>().position;
            if (randomValue <= 1)
            {
                _powerups.Add(SpawnPowerUp(blockDestroyedPosition));
            }
        }

        if (_gridController.GetBlocksActive() == 0)
        {
            _currentLevel++;
            if (_currentLevel >= _levels.Count)
            {
                ClearBalls();
                Debug.LogError("Game Over: WIN!!!!");
            }
            else
            {
                SetInitialBall();
                _gridController.BuildGrid(_levels[_currentLevel]);
            }

        }
    }

    private PowerUp SpawnPowerUp(Vector2 position)
    {
        return Instantiate(Resources.Load<PowerUp>(POWERUP_PREFAB_PATH), position, Quaternion.identity);
    }

    private void OnPowerUpChangeBallSpeedEvent(float velocity, int seconds)
    {
        foreach (Ball ball in _balls)
        {
            ball.changeVelocity(velocity, 2);
        }
    }

    private void OnPowerUpChangeScalePaddleEvent(float scale)
    {
        _paddle.changeHorizontalScale(scale);
    }

    private void OnPowerUpScoreEvent(int score)
    {
        _totalScore += score;
        ArkanoidEvent.OnScoreUpdatedEvent?.Invoke(score, _totalScore);
    }

    private void OnPowerUpDestroy(PowerUp destroy)
    {
        DestroyPowerUp(destroy);
    }
}