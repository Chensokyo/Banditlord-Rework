using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Character;
using TaleWorlds.SaveSystem;
using static TaleWorlds.CampaignSystem.SettlementComponent;

namespace BProperties
{
    public class BProperties
    {

        [SaveableField(1)]
        public string settlementID;

        [SaveableField(2)]
        public MobileParty militia;

        [SaveableField(3)]
        public MobileParty nobles;

        [SaveableField(4)]
        public bool paid;

        public BProperties(string settlementID, MobileParty militia, MobileParty nobles, bool paid = false)
        {
            this.settlementID = settlementID;
            this.militia = militia;
            this.nobles = nobles;
            this.paid = false;
        }

        public Settlement getSelf()
        {
            return Settlement.Find(this.settlementID);
        }

        public int getCost
        {
            get
            {
                float totalCost = 0;
                totalCost += this.militia.Party.NumberOfMenWithoutHorse * 10 + this.militia.Party.NumberOfMenWithHorse * 50;
                totalCost += this.nobles.Party.NumberOfAllMembers * 50;
                totalCost += (int)Math.Ceiling(getProsperity * 0.3);

                // 10% discount for player if they own the settlement
                if (getSelf().OwnerClan == Clan.PlayerClan)
                {
                    totalCost *= 0.9f;
                }

                return (int)Math.Ceiling(totalCost);
            }
        }

        public int getProsperity
        {
            get
            {
                return (int)Math.Ceiling(getSelf().Town.Prosperity);
            }
        }

    }
}