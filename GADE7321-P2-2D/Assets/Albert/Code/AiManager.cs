using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class AiManager : MonoBehaviour
{

    public int maxHealth = 100;
    public int currentHealth;
    public TMP_Text healthText;
    public float respawnCooldown = 5f;
    public GameObject respawnPoint;
    public float respawnHeight = 10f;
    private bool isRespawning = false;
    public float moveSpeed = 5f;
    public int maxMovement = 10;
    public bool aiTurn = true;
    private int remainingMovement;
    Rigidbody2D rb;
    private AIAimMechanics aiAimMechanics;

    void Start()
    {
        aiAimMechanics = GetComponent<AIAimMechanics>();
        remainingMovement = maxMovement;
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    void Update()
    {
        if (aiTurn)
        {
            AIDecisionMaking();
        }
        UpdateHealthUI();
    }

    void AIDecisionMaking()
    {
        
        GameState currentState = new GameState
        {
            aiPosition = transform.position,
            playerPosition = GameObject.Find("Player1").transform.position,
            aiHealth = currentHealth,
            playerHealth = GameObject.Find("Player1").GetComponent<Player1>().currentHealth,
            aiTurn = aiTurn
        };

        Vector2 bestMove = GetBestMove(currentState, 3);
        Vector3 movement = new Vector3(bestMove.x, bestMove.y, 0) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);
        remainingMovement -= Mathf.Abs((int)(movement.magnitude));
        if (remainingMovement <= 0)
        {

            aiAimMechanics.AIUpdate(); // AI aims and shoots
            aiTurn = false;
        }
    }

    Vector2 GetBestMove(GameState state, int depth)
    {
        Vector2 bestMove = state.aiPosition;
        int bestValue = int.MinValue;

        foreach (var move in GenerateAIMoves(state))
        {
            GameState newState = ApplyMove(state, move);
            int moveValue = Minimax(newState, depth - 1, false, int.MinValue, int.MaxValue);
            if (moveValue > bestValue)
            {
                bestValue = moveValue;
                bestMove = move;
            }
        }

        return bestMove;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            BulletHit(20);
        }
    }

    void BulletHit(int damage)
    {
        if (!isRespawning)
        {
            currentHealth -= damage;
            UpdateHealthUI();
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        if (!isRespawning)
        {
            StartCoroutine(RespawnCooldown());
        }
    }

    IEnumerator RespawnCooldown()
    {
        isRespawning = true;
        yield return new WaitForSeconds(respawnCooldown);

        if (currentHealth <= 0)
        {
            Reset();
        }

        isRespawning = false;
    }

    void Reset()
    {
        if (!isRespawning)
        {
            if (currentHealth <= 0)
            {
                transform.position = respawnPoint.transform.position;
                transform.rotation = Quaternion.identity;
            }
            else
            {
                Vector3 respawnPos = new Vector3(transform.position.x, respawnHeight, transform.position.z);
                transform.position = respawnPos;
                transform.rotation = Quaternion.identity;
            }
            currentHealth = maxHealth;
            UpdateHealthUI();
        }
    }

    void UpdateHealthUI()
    {
        healthText.text = currentHealth.ToString();
    }

    int Minimax(GameState state, int depth, bool maximizingPlayer, int alpha, int beta)
    {
        if (depth == 0 || state.aiHealth <= 0 || state.playerHealth <= 0)
        {
            return EvaluateGameState(state);
        }

        if (maximizingPlayer)
        {
            int maxEval = int.MinValue;
            foreach (var move in GenerateAIMoves(state))
            {
                GameState newState = ApplyMove(state, move);
                int eval = Minimax(newState, depth - 1, false, alpha, beta);
                maxEval = Mathf.Max(maxEval, eval);
                alpha = Mathf.Max(alpha, eval);
                if (beta <= alpha)
                {
                    break;
                }
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            foreach (var move in GenerateAIMoves(state))
            {
                GameState newState = ApplyMove(state, move);
                int eval = Minimax(newState, depth - 1, true, alpha, beta);
                minEval = Mathf.Min(minEval, eval);
                beta = Mathf.Min(beta, eval);
                if (beta <= alpha)
                {
                    break;
                }
            }
            return minEval;
        }
    }

    int EvaluateGameState(GameState state)
    {
        return state.aiHealth - state.playerHealth;
    }

    List<Vector2> GenerateAIMoves(GameState state)
    {
        List<Vector2> moves = new List<Vector2>();
        moves.Add(state.aiPosition + Vector2.left);
        moves.Add(state.aiPosition + Vector2.right);
        return moves;
    }

    GameState ApplyMove(GameState state, Vector2 move)
    {
        GameState newState = new GameState
        {
            aiPosition = move,
            playerPosition = state.playerPosition,
            aiHealth = state.aiHealth,
            playerHealth = state.playerHealth,
            aiTurn = !state.aiTurn
        };
        return newState;
    }
}
