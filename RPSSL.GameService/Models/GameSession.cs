using RPSSL.GameService.Models.Enums;

namespace RPSSL.GameService.Models;

public class GameSession
{
  public string GameCode { get; set; }
  public GameStatusEnum Status { get; set; }
  public string PlayerOneId { get; set; }
  public string PlayerTwoId { get; set; }
  public int? PlayerOneChoice { get; set; }
  public int? PlayerTwoChoice { get; set; }
}
