using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Studio29.Debugger
{
    public class DebuggerTurnTakerController : TurnTakerController
    {
        public DebuggerTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {
        }
    }
}
