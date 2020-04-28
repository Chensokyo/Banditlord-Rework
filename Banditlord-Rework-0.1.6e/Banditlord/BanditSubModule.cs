using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BanditSubModule
{
    class BanditSubModule : MBSubModuleBase
    {

        public static readonly string ModuleName = "BanditTroops";

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
        }
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (!(game.GameType is Campaign))
                return;
            CampaignGameStarter gameInitializer = (CampaignGameStarter)gameStarterObject;

            gameInitializer.AddBehavior(new BanditTroops.BanditTroops());
        }
    }
}
