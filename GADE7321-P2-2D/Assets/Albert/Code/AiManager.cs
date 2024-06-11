using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AiManager : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public TMP_Text healthText;
    public float respawnCooldown = 5f;
    public GameObject respawnPoint;
    public float respawnHeight = 10f;
    public float moveSpeed = 5f;
    public int maxMovement = 10;
    public bool aiTurn = false;
    public int respawnAmounts;
    public int respawnAmountsLeft;
    public TMP_Text respawnAmountText;
    public Button endTurnButton;
    private Rigidbody2D rb;
    private GameObject player;
    private Vector3 movePosition;
    private bool isRespawning = false;

    [SerializeField] private BulletManager bulletManager;
    [SerializeField] private AIAimMechanics aiAimMechanics;
    [SerializeField] private TurnBasedManager turnManager;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player1");
        currentHealth = maxHealth;
        respawnAmountsLeft = respawnAmounts;
        UpdateHealthUI();

        Button btn = endTurnButton.GetComponent<Button>();
        btn.onClick.AddListener(EndTurn);

        TurnBasedManager.OnTurnChanged += OnTurnChanged;
    }

    private void Update()
    {
        if (aiTurn)
        {
            AIDecisionMaking();
        }
        UpdateHealthUI();
    }

    private void OnTurnChanged(int playerIndex, GameObject newPlayer)
    {
        aiTurn = (newPlayer == gameObject);
        
    }

    void AIDecisionMaking()
    {
        if (aiAimMechanics == null) return;

        GameState currentState = new GameState
        {
            aiPosition = transform.position,
            playerPosition = player.transform.position,
            aiHealth = currentHealth,
            playerHealth = player.GetComponent<Player1>().currentHealth,
            aiTurn = aiTurn,
            aiRespawns = respawnAmountsLeft
        };

        Vector2 bestMove = GetBestMove(currentState, 3);
        MoveToPosition(bestMove);
    }

    void MoveToPosition(Vector2 position)
    {
        Vector2 direction = (position - (Vector2)transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        aiAimMechanics.AIUpdate();
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
                if (beta <= alpha) break;
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
                if (beta <= alpha) break;
            }
            return minEval;
        }
    }

    int EvaluateGameState(GameState state)
    {
        int playerHealth = Mathf.Clamp(state.playerHealth, 0, maxHealth);
        int aiHealth = Mathf.Clamp(state.aiHealth, 0, maxHealth);
        float distance = Vector2.Distance(state.aiPosition, state.playerPosition);
        float maxDistance = 10f; 

        // Weights for the utility function components
        float w1 = 1f; // Health weight
        float w2 = 0.5f; // Positional advantage weight
        float w3 = 1.5f; // Remaining respawns weight
        float w4 = -0.3f; // Distance to enemy weight (negative because closer is riskier)

        // Simple positional advantage example: favor positions closer to the player
        float positionalAdvantage = Mathf.Max(0, maxDistance - distance);

        return (int)(w1 * aiHealth + w2 * positionalAdvantage + w3 * state.aiRespawns + w4 * distance);
    }

    List<Vector2> GenerateAIMoves(GameState state)
    {
        List<Vector2> moves = new List<Vector2>
        {
            state.aiPosition + Vector2.left,
            state.aiPosition + Vector2.right,
            state.aiPosition + Vector2.up,
            state.aiPosition + Vector2.down
        };
        return moves;
    }

    GameState ApplyMove(GameState state, Vector2 move)
    {
        return new GameState
        {
            aiPosition = move,
            playerPosition = state.playerPosition,
            aiHealth = state.aiHealth,
            playerHealth = state.playerHealth,
            aiTurn = !state.aiTurn,
            aiRespawns = state.aiRespawns
        };
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            BulletHit(bulletManager.bulletTypes[bulletManager.GetCurrentBulletIndex()].damage);
            Destroy(other.gameObject);
        }
    }

    public int BulletHit(int damage)
    {
        if (!isRespawning)
        {
            currentHealth -= damage;
            UpdateHealthUI();
            if (currentHealth <= 0) Die();
        }
        return currentHealth;
    }

    void Die()
    {
        if (!isRespawning) StartCoroutine(RespawnCooldown());
    }

    IEnumerator RespawnCooldown()
    {
        isRespawning = true;
        yield return new WaitForSeconds(respawnCooldown);

        if (respawnAmountsLeft > 0)
        {
            transform.position = respawnPoint.transform.position;
            currentHealth = maxHealth;
            respawnAmountsLeft--;
            UpdateRespawnText();
        }
        else
        {
            Destroy(gameObject);
        }
        isRespawning = false;
    }

    void UpdateHealthUI()
    {
        healthText.text = currentHealth.ToString();
    }

    void UpdateRespawnText()
    {
        respawnAmountText.text = $"Respawn {gameObject.name}: {respawnAmountsLeft}";
    }

    public void EndTurn()
    {
        aiTurn = false;
        turnManager.EndTurn();
    }
}

public struct GameState
{
    public Vector2 aiPosition;
    public Vector2 playerPosition;
    public int aiHealth;
    public int playerHealth;
    public bool aiTurn;
    public int aiRespawns;
}
