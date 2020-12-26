using Studio29.TheTamer;
using Handelabra.Sentinels.Engine.Model;
using NUnit.Framework;

namespace Studio29Tests
{
    [SetUpFixture]
    public class Setup
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            ModHelper.AddAssembly("Studio29", typeof(TheTamerCharacterCardController).Assembly);
        }
    }
}
