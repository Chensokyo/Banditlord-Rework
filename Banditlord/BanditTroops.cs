using System;
using System.Configuration;
using System.Collections.Generic;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.SaveSystem;
using TaleWorlds.Library;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BanditTroops
{
    public struct UnitDetail
    {
        public string unitID;
        public int unitCount;

        public UnitDetail(string _unitID, int _unitCount)
        {
            unitID = _unitID;
            unitCount = _unitCount;
        }
    }
    public struct BuyUnitsOption
    {
        public int cost;
        public string optionID;
        public UnitDetail[] unitList;

        public override string ToString()
        {
            string dialog = "Pay " + cost + " denars for ";
            for (int i = 0; i < unitList.Length; i++)
            {

                string unitName = unitList[i].unitID;
                if (BanditTroops.prettyNames.ContainsKey(unitName))
                {
                    unitName = BanditTroops.prettyNames[unitName];
                }

                dialog += unitList[i].unitCount + " " + unitName + (unitList[i].unitCount > 1 ? "s" : "");
                if (i != unitList.Length - 1)
                {
                    dialog += ", ";
                }
            }

            return dialog;
        }

        public BuyUnitsOption(int _cost, string _optionID, UnitDetail[] _unitList)
        {
            cost = _cost;
            optionID = _optionID;
            unitList = _unitList;
        }
    }

    public class BanditTroops : CampaignBehaviorBase
    {
        public static bool femaleTreeEnabled = false;
        Dictionary<string, BProperties.BProperties> troopDic = new Dictionary<string, BProperties.BProperties>();
        Random rand = new Random();

        public BuyUnitsOption[] unitsToBuy = new BuyUnitsOption[]
        {
            new BuyUnitsOption(50, "pay_fee_5", new UnitDetail[]{
                new UnitDetail("looter", 1),
                new UnitDetail("Ruffian", 1),
                new UnitDetail("Cutthroat", 1)
            }),
            new BuyUnitsOption(150, "pay_fee_15",new UnitDetail[]{
                new UnitDetail("Bandit_Brigand", 1),
                new UnitDetail("Bandit_novice_scoundrel", 1),
                new UnitDetail("hidden_hand_tier_1", 1),
            }),
            new BuyUnitsOption(250,"pay_fee_25", new UnitDetail[]
            {
                new UnitDetail("Armored_Raider", 1),
                new UnitDetail("Armored_Brigand", 1),
                new UnitDetail("wolfskins_tier_1", 1),
            })
        };

        public static Dictionary<string, string> prettyNames = new Dictionary<string, string>()
        {
            { "Ruffian", "Ruffian" },
            {"looter", "Looter" },
            {"Cutthroat", "Cutthroat" },
            {"Bandit_Brigand", "Bandit Brigand" },
            {"Bandit_novice_scoundrel", "Bandit Novice Scoundrel" },
            {"hidden_hand_tier_1", "Hidden Pawn" },
            {"Armored_Raider", "Armored Raider" },
            {"Armored_Brigand", "Armored Brigand" },
            {"wolfskins_tier_1", "Young Wolf" },
        };

        public static Dictionary<string, string> genderPairs = new Dictionary<string, string>()
        {
            {"Ruffian", "Ruffian"},
            {"Cutthroat", "Cutthroat"},
            {"looter", "looter"},
            {"Bandit_Brigand","Bandit_Brigand" },
            {"wolfskins_tier_1","wolfskins_tier_1" },
            {"hidden_hand_tier_1","hidden_hand_tier_1" }
        };

        public static string[] militiaTroopIDs = new string[]
        {
            "Ruffian",
            "Cutthroat",
            "looter",
            "Bandit_Brigand",

        };

        public static string[] nobleTroopIDs = new string[]
        {
            "wolfskins_tier_1",
             "hidden_hand_tier_1",
              "Horsetief",
               "sea_raiders_bandit",
               "steppe_bandits_bandit",
            "Armored_Raider",
        };

        private Dictionary<string, CharacterObject> characterMap = new Dictionary<string, CharacterObject>();

        private CharacterObject getCharacter(string _characterID)
        {
            if (characterMap.ContainsKey(_characterID))
            {
                return characterMap[_characterID];
            }
            else
            {
                CharacterObject characterRef = CharacterObject.Find(_characterID);
                if (characterRef != null)
                {
                    characterMap[_characterID] = characterRef;
                    return characterRef;
                }
                else
                {
                    InformationManager.DisplayMessage(new InformationMessage("{Banditlord Error}: Unit Not Found " + _characterID, Color.FromUint(0xffe4e1)));
                }
            }

            return null;
        }

        private void AddUnits(UnitDetail _unitDetail)
        {
            CharacterObject unitToAdd = getCharacter(_unitDetail.unitID);
            if (unitToAdd != null)
            {
                MobileParty.MainParty.AddElementToMemberRoster(unitToAdd, _unitDetail.unitCount);
            }
        }

        private void AddUnits(UnitDetail[] _unitDetail)
        {
            foreach (UnitDetail _unit in _unitDetail)
            {
                CharacterObject unitToAdd = getCharacter(_unit.unitID);
                if (unitToAdd != null)
                {
                    MobileParty.MainParty.AddElementToMemberRoster(unitToAdd, _unit.unitCount);
                }
            }

        }
        private void AddHideoutBandits(int cost)
        {
            if (Hero.MainHero.Gold >= cost)
            {
                AddUnits(new UnitDetail[] { new UnitDetail("Banditlord", 1), new UnitDetail("BPGuard", 5) });
                Hero.MainHero.ChangeHeroGold(-cost);
            }
            MBSoundEvent.PlaySound(722, Hero.MainHero.GetPosition());
        }
        private void AddHideoutBandits1(int cost)
        {
            if (Hero.MainHero.Gold >= cost)
            {
                AddUnits(new UnitDetail[] { new UnitDetail("wolfskins_tier_1", 7), new UnitDetail("Bandit_novice_scoundrel", 3), new UnitDetail("hidden_hand_tier_1", 3), new UnitDetail("Horsethief", 2) });
                Hero.MainHero.ChangeHeroGold(-cost);
            }
            MBSoundEvent.PlaySound(722, Hero.MainHero.GetPosition());
        }
        private void AddHideoutBandits2(int cost)
        {
            if (Hero.MainHero.Gold >= cost)
            {
                AddUnits(new UnitDetail[] { new UnitDetail("Horsethief", 1) });
                Hero.MainHero.ChangeHeroGold(-cost);
            }
            MBSoundEvent.PlaySound(722, Hero.MainHero.GetPosition());
        }
        private void AddHideoutBandits3(int cost)
        {
            if (Hero.MainHero.Gold >= cost)
            {
                AddUnits(new UnitDetail[] { new UnitDetail("Ambusher", 1) });
                Hero.MainHero.ChangeHeroGold(-cost);
            }
            MBSoundEvent.PlaySound(722, Hero.MainHero.GetPosition());
        }
        private void AddHideoutBandits4(int cost)
        {
            if (Hero.MainHero.Gold >= cost)
            {
                AddUnits(new UnitDetail[] { new UnitDetail("Ruffian", 1), new UnitDetail("Cutthroat", 1), });
                Hero.MainHero.ChangeHeroGold(-cost);
            }
            MBSoundEvent.PlaySound(722, Hero.MainHero.GetPosition());
        }
        private void AddHideoutBandits5(int cost)
        {
            if (Hero.MainHero.Gold >= cost)
            {
                AddUnits(new UnitDetail[] { new UnitDetail("Bandit_Brigand", 1) });
                Hero.MainHero.ChangeHeroGold(-cost);
            }
            MBSoundEvent.PlaySound(722, Hero.MainHero.GetPosition());
        }
        private void AddHideoutBandits6(int cost)
        {
            if (Hero.MainHero.Gold >= cost)
            {
                AddUnits(new UnitDetail[] { new UnitDetail("Asyrina", 1), new UnitDetail("Frostmaiden", 5), });
                Hero.MainHero.ChangeHeroGold(-cost);
            }
            MBSoundEvent.PlaySound(722, Hero.MainHero.GetPosition());
        }

        private void AddHideoutBandits7(int cost)
        {
            if (Hero.MainHero.Gold >= cost)
            {
                AddUnits(new UnitDetail[] { new UnitDetail("Rangerlord", 1), new UnitDetail("Ranger", 5), });
                Hero.MainHero.ChangeHeroGold(-cost);
            }
            MBSoundEvent.PlaySound(722, Hero.MainHero.GetPosition());

        }

        private void AddHideoutBandits8(int cost)
        {
            if (Hero.MainHero.Gold >= cost)
            {
                AddUnits(new UnitDetail[] { new UnitDetail("Rangerlord", 1), new UnitDetail("Ranger", 5), });
                Hero.MainHero.ChangeHeroGold(-cost);
            }
            MBSoundEvent.PlaySound(722, Hero.MainHero.GetPosition());

        }



        private void OnSessionLaunched(CampaignGameStarter obj)
        {

            AddTroopMenu(obj);
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));

        }


        public void AddTroopMenu(CampaignGameStarter obj)
        {

            obj.AddGameMenuOption("town", "info_troop_type", "Recruit a Bandit Brigand from the local Gangleader. (30)", game_menu_just_add_recruit_conditional, (MenuCallbackArgs args) => { AddHideoutBandits5(30); }, false, 5);
            obj.AddGameMenuOption("town", "info_troop_type", "Recruit Asyrina and her Frostmaiden. (1000)", game_menu_just_add_recruit_conditional, (MenuCallbackArgs args) => { AddHideoutBandits6(1000); }, false, 5);
            obj.AddGameMenuOption("hideout_place", "info_troop_type", "Recruit Klyka the Banditlord and his Warrior. (1000)", game_menu_just_add_recruit_conditional, (MenuCallbackArgs args) => { AddHideoutBandits(1000); }, false, 5);
            obj.AddGameMenuOption("hideout_place", "info_troop_type", "Recruit Rengo and his Merry Men. (1000)", game_menu_just_add_recruit_conditional, (MenuCallbackArgs args) => { AddHideoutBandits7(1000); }, false, 5);
            obj.AddGameMenuOption("hideout_place", "info_troop_type", "Recruit Bandit Squad (500)", game_menu_just_add_recruit_conditional, (MenuCallbackArgs args) => { AddHideoutBandits1(500); }, false, 5);
            obj.AddGameMenuOption("hideout_place", "info_troop_type", "Recruit a Horsethief (30)", game_menu_just_add_recruit_conditional, (MenuCallbackArgs args) => { AddHideoutBandits2(30); }, false, 5);
            obj.AddGameMenuOption("hideout_place", "info_troop_type", "Recruit Recruit Bandit Ambusher (50)", game_menu_just_add_recruit_conditional, (MenuCallbackArgs args) => { AddHideoutBandits3(50); }, false, 5);
            obj.AddGameMenuOption("hideout_place", "info_troop_type", "Recruit Ruffian and Cutthroat (30)", game_menu_just_add_recruit_conditional, (MenuCallbackArgs args) => { AddHideoutBandits4(30); }, false, 5);
            obj.AddGameMenuOption("hideout_place", "info_troop_type", "Recruit a Bandit Brigand (30)", game_menu_just_add_recruit_conditional, (MenuCallbackArgs args) => { AddHideoutBandits5(30); }, false, 5);











        }

        private void game_menu_switch_to_town_menu(MenuCallbackArgs args)
        {
            GameMenu.SwitchToMenu("town");
        }

        private bool game_menu_just_add_recruit_conditional(MenuCallbackArgs args)
        {
            args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
            return true;
        }

        private bool game_menu_just_add_leave_conditional(MenuCallbackArgs args)
        {
            args.optionLeaveType = GameMenuOption.LeaveType.Leave;
            return true;
        }






        // Loads the mod data
        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("troopDic", ref troopDic);
        }

        // Saves the mod data
        public class BanditSaveDefiner : SaveableTypeDefiner
        {
            public BanditSaveDefiner() : base(91115119)
            {
            }

            protected override void DefineClassTypes()
            {
                AddClassDefinition(typeof(BProperties.BProperties), 1);
            }

            protected override void DefineContainerDefinitions()
            {
                ConstructContainerDefinition(typeof(Dictionary<string, BProperties.BProperties>));
            }
        }
    }
}
