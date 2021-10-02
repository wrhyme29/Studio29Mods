using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.Achievements;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.Debugger
{
    public class DebuggerTurnTakerController : TurnTakerController
    {
        public DebuggerTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {
        }
    }
}
