using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Timers;

public class GameManager : MonoBehaviour
{
  public ConsoleManager consoleManager;
  public BoardManager boardManager;
  public OptionsForLaunch gameOptions;
  public BoardBuilder boardBuilder;
  public Abalone abalone;

  bool gameStarted = false;

  public TextMeshProUGUI whiteTurnsUI;
  public TextMeshProUGUI blackTurnsUI;
  public TextMeshProUGUI whiteLostPiecesUI;
  public TextMeshProUGUI blackLostPiecesUI;
  public TextMeshProUGUI whiteTimeUI;
  public TextMeshProUGUI blacktimeUI;
  public TextMeshProUGUI totalTimer;

  float totalMilisecondsPassed;

  float blackMilisecondsPassed;

  float whiteMilisecondsPassed;

  float blackTotalTime;
  float whiteTotalTime;
  public int blackLostPieces;
  public int whiteLostPieces;
  public int whiteTurnsLeft;
  public int blackTurnsLeft;



  public static Turn currentTurn;

  public Agent agent;
  private void startTimer()
  {
    consoleManager.sendMessageToConsole("Starting!");
  }

  // private void FixedUpdate()
  // {
  //   if (gameStarted)
  //   {
  //     updateTotalTimer();
  //     if (currentTurn == Turn.BLACK)
  //     {
  //       updateBlackTimer();
  //     }
  //     else
  //     {
  //       updateWhiteTimer();
  //     }

  //     if (gameOptions.isBlackAnAgent() && currentTurn == Turn.BLACK)
  //     {
  //       agentTurn();
  //     }
  //     if (gameOptions.isWhiteAnAgent() && currentTurn == Turn.WHITE)
  //     {
  //       agentTurn();
  //     }
  //   }
  // }

  public void agentTurn()
  {
    State currentState = boardManager.convertBoardToState();
    agent.setState(currentState);
    State newState = agent.turn(currentState);

    Node[,] newBoard = boardManager.convertStateToBoard(newState);
    boardBuilder.generateAllNeighbors(newBoard);
    Abalone.boardState = newBoard;
    abalone.updateUIBoard();
    cycleTurn();
  }

  public void updateTotalTimer()
  {
    totalMilisecondsPassed += Time.deltaTime;
    totalTimer.SetText(formatMiliseconds(totalMilisecondsPassed));
  }

  public void updateBlackTimer()
  {
    blackMilisecondsPassed += Time.deltaTime;
    blacktimeUI.SetText(formatMiliseconds(blackMilisecondsPassed));
    blackTotalTime = blackMilisecondsPassed;
  }

  public void updateWhiteTimer()
  {
    whiteMilisecondsPassed += Time.deltaTime;
    whiteTimeUI.SetText(formatMiliseconds(whiteMilisecondsPassed));
    whiteTotalTime = whiteMilisecondsPassed;
  }

  public string formatMiliseconds(float miliseconds)
  {
    string min = Mathf.Floor(miliseconds / 60).ToString("00");
    float secFloat = miliseconds % 60;
    string sec = Mathf.Floor(miliseconds % 60).ToString("00");
    string mil = Mathf.Floor((miliseconds * 100) % 100).ToString("00");
    string timeString = string.Format("{0}:{1}:{2}", min, sec, mil);
    return timeString;
  }

  public void startGame()
  {

    //Set up board
    currentTurn = Turn.BLACK;
    boardManager.referenceAllBoardTiles();
    InputScript.deselectAllTiles();
    switch (gameOptions.GetLayout())
    {
      case OptionsForLaunch.Layout.Default:
        consoleManager.sendMessageToConsole("Setting layout to default");
        boardManager.setBoardToDefaultLayout();
        break;

      case OptionsForLaunch.Layout.Belgian:
        consoleManager.sendMessageToConsole("Setting layout to Belgian Daisy");
        boardManager.setBoardToBelgianLayout();
        break;

      case OptionsForLaunch.Layout.German:
        consoleManager.sendMessageToConsole("Setting layout to German Daisy");
        boardManager.setBoardToGermanLayout();
        break;
    }
    abalone.generateBoard();
    abalone.updateGameStateBoard();
    startTimer();


    if (gameOptions.getTurnLimit() == 0)
    {
      whiteTurnsUI.SetText("N/A");
      blackTurnsUI.SetText("N/A");
    }
    else
    {
      whiteTurnsUI.SetText(gameOptions.getTurnLimit().ToString());
      blackTurnsUI.SetText(gameOptions.getTurnLimit().ToString());
    }
    totalMilisecondsPassed = 0;
    blackMilisecondsPassed = 0;
    whiteMilisecondsPassed = 0;
    whiteLostPiecesUI.SetText("0");
    blackLostPiecesUI.SetText("0");
    whiteLostPieces = 0;
    blackLostPieces = 0;
    whiteTotalTime = 0;
    blackTotalTime = 0;
    gameStarted = true;
    whiteTimeUI.SetText("00:00:00");
    blacktimeUI.SetText("00:00:00");
    if (gameOptions.isBlackAnAgent())
    {
      agentTurn();
    }




  }

  public static Turn getCurrentTurn()
  {
    return currentTurn;
  }

  public static void setCurrentTurn(Turn newTurn)
  {
    currentTurn = newTurn;
  }

  public void cycleTurn()
  {
    cycleTurnsRemaining(currentTurn);

    if (currentTurn == Turn.BLACK)
    {
      currentTurn = Turn.WHITE;
    }
    else
    {
      currentTurn = Turn.BLACK;
    }
    consoleManager.sendMessageToConsole("Current turn: " + GameManager.getCurrentTurn().ToString());
    checkForWinCondition();
    checkForAgentTurns();
  }

  public void checkForAgentTurns()
  {
    if (gameOptions.isBlackAnAgent() || gameOptions.isWhiteAnAgent())
    {
      agentTurn();
    }
  }

  public void checkForWinCondition()
  {
    if (whiteLostPieces >= 6 || blackLostPieces >= 6)
    {
      showEndGame(currentTurn);
    }
  }

  public void showEndGame(Turn winningPlayer)
  {

  }

  public int getTurnAsInt()
  {
    if (currentTurn == Turn.BLACK)
    {
      return 1;
    }
    return 2;
  }

  public void addLostPiece()
  {
    consoleManager.sendMessageToConsole("Adding lost Piece");
    switch (currentTurn)
    {
      case Turn.BLACK:
        whiteLostPieces++;
        whiteLostPiecesUI.SetText(whiteLostPieces.ToString());
        break;
      case Turn.WHITE:
        blackLostPieces++;
        blackLostPiecesUI.SetText(blackLostPieces.ToString());
        break;
    }
  }

  public void cycleTurnsRemaining(Turn player)
  {
    if (!blackTurnsUI.text.Equals("N/A"))
    {
      switch (player)
      {
        case Turn.BLACK:
          blackTurnsLeft--;
          blackTurnsUI.SetText(blackTurnsLeft.ToString());
          break;
        case Turn.WHITE:
          whiteTurnsLeft--;
          whiteTurnsUI.SetText(whiteTurnsLeft.ToString());
          break;
      }
    }

  }
  public void getTime()
  {

  }

}


public enum Turn
{
  BLACK,
  WHITE
}
