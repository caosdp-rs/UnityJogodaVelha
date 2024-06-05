using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public Button[] buttons;  // Array de botões
    public Text messageText;  // Texto de mensagem para mostrar quem venceu ou empate
    private string playerTurn = "X";
    private int moveCount = 0;
    private bool isGameActive = true;
    public int difficulty = 2; // 1: fácil, 2: médio, 3: difícil

    public void ButtonClick(Button button)
    {
        if (button.GetComponentInChildren<Text>().text == "" && isGameActive)
        {
            MakeMove(button);
            if (isGameActive && playerTurn == "O")
            {
                StartCoroutine(ComputerMove());
            }
        }
    }

    private void MakeMove(Button button)
    {
        button.GetComponentInChildren<Text>().text = playerTurn;
        moveCount++;
        if (CheckWin())
        {
            messageText.text = playerTurn + " Wins!";
            isGameActive = false;
            StartCoroutine(ResetAfterDelay());
        }
        else if (moveCount >= 9)
        {
            messageText.text = "It's a Draw!";
            isGameActive = false;
            StartCoroutine(ResetAfterDelay());
        }
        else
        {
            playerTurn = (playerTurn == "X") ? "O" : "X";
        }
    }

    private IEnumerator ComputerMove()
    {
        yield return new WaitForSeconds(1);  // Atraso para a jogada do computador
        int move;
        if (difficulty == 1) // Fácil
        {
            move = GetRandomMove();
        }
        else if (difficulty == 2) // Médio
        {
            move = Random.Range(0, 2) == 0 ? GetRandomMove() : GetBestMove();
        }
        else // Difícil
        {
            move = GetBestMove();
        }
        MakeMove(buttons[move]);
    }

    private int GetRandomMove()
    {
        List<int> availableMoves = new List<int>();
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].GetComponentInChildren<Text>().text == "")
            {
                availableMoves.Add(i);
            }
        }
        if (availableMoves.Count > 0)
        {
            int index = Random.Range(0, availableMoves.Count);
            return availableMoves[index];
        }
        return -1;
    }

    private int GetBestMove()
    {
        int bestScore = int.MinValue;
        int move = -1;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].GetComponentInChildren<Text>().text == "")
            {
                buttons[i].GetComponentInChildren<Text>().text = "O";
                int score = Minimax(buttons, 0, false, difficulty == 3 ? int.MaxValue : 2); // Limita a profundidade
                buttons[i].GetComponentInChildren<Text>().text = "";
                if (score > bestScore)
                {
                    bestScore = score;
                    move = i;
                }
            }
        }
        return move;
    }

    private int Minimax(Button[] newButtons, int depth, bool isMaximizing, int maxDepth)
    {
        if (CheckWinCondition(newButtons, "O"))
            return 10 - depth;
        if (CheckWinCondition(newButtons, "X"))
            return depth - 10;
        if (IsDraw(newButtons))
            return 0;

        if (depth >= maxDepth) // Limita a profundidade
            return 0;

        if (isMaximizing)
        {
            int bestScore = int.MinValue;
            for (int i = 0; i < newButtons.Length; i++)
            {
                if (newButtons[i].GetComponentInChildren<Text>().text == "")
                {
                    newButtons[i].GetComponentInChildren<Text>().text = "O";
                    int score = Minimax(newButtons, depth + 1, false, maxDepth);
                    newButtons[i].GetComponentInChildren<Text>().text = "";
                    bestScore = Mathf.Max(score, bestScore);
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;
            for (int i = 0; i < newButtons.Length; i++)
            {
                if (newButtons[i].GetComponentInChildren<Text>().text == "")
                {
                    newButtons[i].GetComponentInChildren<Text>().text = "X";
                    int score = Minimax(newButtons, depth + 1, true, maxDepth);
                    newButtons[i].GetComponentInChildren<Text>().text = "";
                    bestScore = Mathf.Min(score, bestScore);
                }
            }
            return bestScore;
        }
    }

    private bool CheckWinCondition(Button[] checkButtons, string player)
    {
        return (checkButtons[0].GetComponentInChildren<Text>().text == player &&
                checkButtons[1].GetComponentInChildren<Text>().text == player &&
                checkButtons[2].GetComponentInChildren<Text>().text == player) ||
               (checkButtons[3].GetComponentInChildren<Text>().text == player &&
                checkButtons[4].GetComponentInChildren<Text>().text == player &&
                checkButtons[5].GetComponentInChildren<Text>().text == player) ||
               (checkButtons[6].GetComponentInChildren<Text>().text == player &&
                checkButtons[7].GetComponentInChildren<Text>().text == player &&
                checkButtons[8].GetComponentInChildren<Text>().text == player) ||
               (checkButtons[0].GetComponentInChildren<Text>().text == player &&
                checkButtons[3].GetComponentInChildren<Text>().text == player &&
                checkButtons[6].GetComponentInChildren<Text>().text == player) ||
               (checkButtons[1].GetComponentInChildren<Text>().text == player &&
                checkButtons[4].GetComponentInChildren<Text>().text == player &&
                checkButtons[7].GetComponentInChildren<Text>().text == player) ||
               (checkButtons[2].GetComponentInChildren<Text>().text == player &&
                checkButtons[5].GetComponentInChildren<Text>().text == player &&
                checkButtons[8].GetComponentInChildren<Text>().text == player) ||
               (checkButtons[0].GetComponentInChildren<Text>().text == player &&
                checkButtons[4].GetComponentInChildren<Text>().text == player &&
                checkButtons[8].GetComponentInChildren<Text>().text == player) ||
               (checkButtons[2].GetComponentInChildren<Text>().text == player &&
                checkButtons[4].GetComponentInChildren<Text>().text == player &&
                checkButtons[6].GetComponentInChildren<Text>().text == player);
    }

    private bool IsDraw(Button[] checkButtons)
    {
        foreach (Button button in checkButtons)
        {
            if (button.GetComponentInChildren<Text>().text == "")
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckWin()
    {
        return CheckWinCondition(buttons, playerTurn);
    }

    private IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(2);  // Pausa a execução por 2 segundos
        ResetGame();
    }

    private void ResetGame()
    {
        foreach (Button button in buttons)
        {
            button.GetComponentInChildren<Text>().text = "";
        }
        moveCount = 0;
        playerTurn = "X";
        messageText.text = "";  // Limpa a mensagem de vitória
        isGameActive = true;    // Reativa o jogo
    }
}
