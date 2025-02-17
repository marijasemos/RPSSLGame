﻿using RPSSL.GameService.Interfaces;
using RPSSL.GameService.Models.Enums;
using static RPSSL.GameService.Helpers.MatchEvaluator;

namespace RPSSL.GameService.Strategies;

/// <summary>
/// Game strategy implementation for two players, providing player-specific results.
/// </summary>
public class TwoPlayerStrategy : IGameStrategy
{
  private readonly string _playerOneId;
  private readonly string _playerTwoId;

  public TwoPlayerStrategy(string playerOneId, string playerTwoId)
  {
    _playerOneId = playerOneId;
    _playerTwoId = playerTwoId;
  }

  /// <inheritdoc />
  public GameResultEnum GetResult(ChoiceEnum playerOneChoice, ChoiceEnum playerTwoChoice)
  {
    if (playerOneChoice == playerTwoChoice)
    {
      return GameResultEnum.Tie;
    }

    return IsWinningMove(playerOneChoice, playerTwoChoice) ? GameResultEnum.Win : GameResultEnum.Lose;
  }
}