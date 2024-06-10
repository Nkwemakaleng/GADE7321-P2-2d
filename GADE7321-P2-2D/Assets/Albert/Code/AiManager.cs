using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public bool aiTurn = false;
    private int remainingMovement;
    Rigidbody2D rb;
    public AIAimMechanics aiAimMechanics;
    private GameObject player;
    public int respawnAmounts;
    public int respawnAmountsLeft;
    public TMP_Text respawnAmountText;
    [SerializeField] private BulletManager bulletManager;
    [SerializeField] private TurnBasedManager turnManager;

    void Start()
    {
        respawnAmountsLeft = respawnAmounts;
        if (aiAimMechanics == null)
        {
            Debug.LogWarning("AIAimMechanics component not found.");
        }
        else
        {
            Debug.Log("AIAimMechanics component found.");
        }
        TurnBasedManager.OnTurnChanged += OnTurnChanged;
        remainingMovement = maxMovement;
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        UpdateHealthUI();
        player = GameObject.Find("Player1"); // Find the player GameObject
    }

    void Update()
    {
        if (aiTurn)
        {
            AIDecisionMaking();
        }
        UpdateHealthUI();
    }

    private void OnTurnChanged(int playerIndex, GameObject newPlayer)
    {
        if (newPlayer == gameObject)
        {
            Debug.Log("AI's Turn");
            aiTurn = true;
        }
        else
        {
            aiTurn = false;
        }
    }

    void AIDecisionMaking()
    {
        if (aiAimMechanics == null)
        {
            Debug.LogError("AIAimMechanics component is missing.");
            return;
        }

        // Ensure the AI has the AIAimMechanics component attached
        if (aiAimMechanics != null)
        {
            GameState currentState = new GameState
            {
                aiPosition = transform.position,
                playerPosition = player.transform.position,
                aiHealth = currentHealth,
                playerHealth = player.GetComponent<Player1>().currentHealth,
                aiTurn = aiTurn
            };

            Vector2 bestMove = GetBestMove(currentState, 3);
            Vector2 direction = (bestMove - (Vector2)transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
            remainingMovement -= Mathf.Abs((int)(direction.magnitude));
            if (remainingMovement <= 0)
            {
                aiAimMechanics.AIUpdate(); // AI aims and shoots
                aiTurn = false;
            }
        }
        else
        {
            Debug.LogError("AIAimMechanics component is missing on AI.");
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            BulletHit(bulletManager.bulletTypes[bulletManager.GetCurrentBulletIndex()].damage); // Apply the damage from the bullet
            Destroy(other.gameObject); // Optionally destroy the bullet on hit); 
            Debug.Log("Hit");
        }
    }

    public int BulletHit(int damage)
    {
        if (!isRespawning)
        {
            currentHealth -= damage; // Decrease player's health by the damage amount
            UpdateHealthUI(); // Update health UI
            if (currentHealth < 0 || currentHealth == 0)
            {
                Die(); // Die if health reaches zero
            }
        }
        return currentHealth;
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

        if (currentHealth < 0 || currentHealth == 0)
        {
            Reset();
        }
        isRespawning = false;
    }

    void Reset()
    {
        if (currentHealth == 0)
        {
            if (respawnAmountsLeft > 0)
            {
                transform.position = respawnPoint.transform.position;
                transform.rotation = Quaternion.identity;
                respawnAmountsLeft -= 1;
                UpdateRespawnText();
            }
            else
            {
                Debug.Log("Not enough Lives Left");
                DestroyThisObject();
            }
        }
        else
        {
            if (respawnAmountsLeft > 0)
            {
                Vector3 respawnPos = new Vector3(transform.position.x, respawnHeight, transform.position.z);
                transform.position = respawnPos;
                transform.rotation = Quaternion.identity;
                respawnAmountsLeft -= 1;
                UpdateRespawnText();
            }
            else
            {
                Debug.Log("No enough respawns left for automatic respawn");
                DestroyThisObject();
            }
        }
        currentHealth = maxHealth; // Reset player's health
        UpdateHealthUI(); // Update health UI
        UpdateRespawnText();
    }

    void UpdateHealthUI()
    {
        healthText.text = "Health: " + currentHealth.ToString();
    }

    void UpdateRespawnText()
    {
        respawnAmountText.text = "Respawn " + this.gameObject.name + " :" + Mathf.Max(respawnAmountsLeft, 0).ToString();
    }

    void DestroyThisObject()
    {
        Destroy(this.gameObject);
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
