﻿using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using StardewArchipelago.Constants.Modded;
using StardewArchipelago.Locations.CodeInjections.Vanilla;
using StardewArchipelago.Locations.CodeInjections.Vanilla.MonsterSlayer;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Locations;
using StardewArchipelago.Archipelago.SlotData.SlotEnums;
using StardewArchipelago.Constants.Vanilla;
using StardewArchipelago.Items;
using StardewArchipelago.Logging;
using StardewValley.Extensions;
using Object = StardewValley.Object;

namespace StardewArchipelago.Goals
{
    internal class GoalCodeInjection
    {
        public const string MASTER_ANGLER_LETTER = "CF_Fish";

        private static LogHandler _logger;
        private static IModHelper _modHelper;
        private static StardewArchipelagoClient _archipelago;
        private static StardewLocationChecker _locationChecker;
        private static BundleReader _bundleReader;
        private static MonsterKillList _killList;

        public static void Initialize(LogHandler logger, IModHelper modHelper, StardewArchipelagoClient archipelago, StardewLocationChecker locationChecker, BundleReader bundleReader, MonsterKillList killList)
        {
            _logger = logger;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _bundleReader = bundleReader;
            _killList = killList;
        }

        public static void CheckCommunityCenterGoalCompletion()
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.CommunityCenter)
            {
                return;
            }

            if (!_bundleReader.IsCommunityCenterComplete())
            {
                return;
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void CheckGrandpaEvaluationGoalCompletion()
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.GrandpaEvaluation)
            {
                return;
            }

            var farm = Game1.getFarm();

            if (farm.grandpaScore.Value < 4)
            {
                return;
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void CheckBottomOfTheMinesGoalCompletion()
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.BottomOfMines)
            {
                return;
            }

            var lowestMineLevel = Game1.netWorldState.Value.LowestMineLevel;

            if (lowestMineLevel < 120)
            {
                return;
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void CheckCrypticNoteGoalCompletion()
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.CrypticNote)
            {
                return;
            }

            if (!Game1.player.mailReceived.Contains("qiCave"))
            {
                return;
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void CheckMasterAnglerGoalCompletion(bool vanillaGoal = false)
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.MasterAngler)
            {
                return;
            }

            if (vanillaGoal || _archipelago.SlotData.Fishsanity == Fishsanity.None)
            {
                SendMasterAnglerLetterExcludingIsland();
                if (!Game1.player.hasOrWillReceiveMail(MASTER_ANGLER_LETTER))
                {
                    return;
                }
            }
            else
            {
                if (_locationChecker.IsAnyLocationNotCheckedStartingWith(FishingInjections.FISHSANITY_PREFIX))
                {
                    return;
                }
            }

            _archipelago.ReportGoalCompletion();
        }

        private static void SendMasterAnglerLetterExcludingIsland()
        {
            if (!_archipelago.SlotData.ExcludeGingerIsland || Game1.player.hasOrWillReceiveMail(MASTER_ANGLER_LETTER))
            {
                return;
            }

            var uniqueFishCaught = 0;
            var totalFishExist = 0;
            foreach (var (id, objectData) in Game1.objectData)
            {
                var isFish = objectData.Category == Category.FISH;
                var idIsNumeric = int.TryParse(id, out var numericId);
                var isTrash = idIsNumeric && (numericId >= 167 && numericId <= 172);
                var isLegendaryFamily = idIsNumeric && (numericId >= 898 && numericId <= 902);
                var isIslandFish = idIsNumeric && (numericId >= 836 && numericId <= 838);
                if (!isFish || isTrash || isLegendaryFamily || isIslandFish)
                {
                    continue;
                }

                ++totalFishExist;
                if (Game1.player.fishCaught.ContainsKey(id))
                {
                    ++uniqueFishCaught;
                }
            }

            if (uniqueFishCaught < totalFishExist)
            {
                return;
            }

            if (!Game1.player.hasOrWillReceiveMail(MASTER_ANGLER_LETTER))
            {
                Game1.addMailForTomorrow(MASTER_ANGLER_LETTER);
            }
        }

        public static void CheckCompleteCollectionGoalCompletion()
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.CompleteCollection)
            {
                return;
            }

            if (!Game1.player.hasOrWillReceiveMail("museumComplete"))
            {
                return;
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void CheckFullHouseGoalCompletion()
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.FullHouse)
            {
                return;
            }

            if (Game1.player.getChildrenCount() < 2 || !Game1.player.isMarriedOrRoommates() || Game1.player.HouseUpgradeLevel < 2)
            {
                return;
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void CheckWalnutHunterGoalCompletion()
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.GreatestWalnutHunter)
            {
                return;
            }

            if (Game1.netWorldState.Value.GoldenWalnutsFound < 130)
            {
                return;
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void CheckProtectorOfTheValleyGoalCompletion(bool vanillaGoal = false)
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.ProtectorOfTheValley)
            {
                return;
            }

            if (vanillaGoal || _archipelago.SlotData.Monstersanity == Monstersanity.None)
            {
                if (!_killList.AreAllGoalsComplete())
                {
                    return;
                }
            }
            else
            {
                if (_locationChecker.IsAnyLocationNotCheckedStartingWith(MonsterSlayerInjections.MONSTER_ERADICATION_AP_PREFIX))
                {
                    return;
                }
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void CheckFullShipmentGoalCompletion(bool vanillaGoal)
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.FullShipment)
            {
                return;
            }

            if (vanillaGoal || _archipelago.SlotData.Shipsanity == Shipsanity.None)
            {
                if (!HasShippedAllItems())
                {
                    return;
                }
            }
            else
            {
                if (_locationChecker.IsAnyLocationNotCheckedStartingWith(NightShippingBehaviors.SHIPSANITY_PREFIX))
                {
                    return;
                }
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void CheckGourmetChefGoalCompletion()
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.GourmetChef)
            {
                return;
            }

            if (_archipelago.SlotData.Cooksanity == Cooksanity.None)
            {
                if (!HasCookedAllRecipes())
                {
                    return;
                }
            }
            else
            {
                if (_locationChecker.IsAnyLocationNotCheckedStartingWith(CookingInjections.COOKING_LOCATION_PREFIX))
                {
                    return;
                }
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void CheckCraftMasterGoalCompletion()
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.CraftMaster)
            {
                return;
            }

            if (_archipelago.SlotData.Craftsanity == Craftsanity.None)
            {
                if (!HasCraftedAllRecipes())
                {
                    return;
                }
            }
            else
            {
                if (_locationChecker.IsAnyLocationNotCheckedStartingWith(CraftingInjections.CRAFTING_LOCATION_PREFIX))
                {
                    return;
                }
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void CheckLegendGoalCompletion()
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.Legend)
            {
                return;
            }

            if (Game1.player.totalMoneyEarned < 10000000)
            {
                return;
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void CheckMysteryOfTheStardropsGoalCompletion()
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.MysteryOfTheStardrops)
            {
                return;
            }

            if (Game1.player.maxStamina.Value < (508 + PlayerBuffInjections.CurrentStaminaBonus))
            {
                return;
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void CheckAllsanityGoalCompletion()
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.Allsanity)
            {
                return;
            }

            if (_locationChecker.GetAllMissingLocations().Any())
            {
                return;
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void CheckPerfectionGoalCompletion()
        {
            if (!_archipelago.IsConnected || _archipelago.SlotData.Goal != Goal.Perfection)
            {
                return;
            }

            if (Utility.percentGameComplete() < 1.0)
            {
                return;
            }

            _archipelago.ReportGoalCompletion();
        }

        public static void DoAreaCompleteReward_CommunityCenterGoal_PostFix(CommunityCenter __instance, int whichArea)
        {
            try
            {
                CheckCommunityCenterGoalCompletion();
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed in {nameof(DoAreaCompleteReward_CommunityCenterGoal_PostFix)}:\n{ex}");
                return;
            }
        }

        public static void EnterMine_Level120Goal_PostFix(int whatLevel)
        {
            try
            {
                if (whatLevel != 120)
                {
                    return;
                }

                _archipelago.ReportGoalCompletion();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed in {nameof(EnterMine_Level120Goal_PostFix)}:\n{ex}");
                return;
            }
        }

        // public void foundWalnut(int stack = 1)
        public static void FoundWalnut_WalnutHunterGoal_Postfix(Farmer __instance, int stack)
        {
            try
            {
                CheckWalnutHunterGoalCompletion();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed in {nameof(FoundWalnut_WalnutHunterGoal_Postfix)}:\n{ex}");
                return;
            }
        }

        private static bool HasShippedAllItems()
        {
            var numberOfUnavailableItems = _archipelago.SlotData.ExcludeGingerIsland ? 10 : 0;
            var numberOfMissedItems = 0;

            foreach (var parsedItemData in ItemRegistry.GetObjectTypeDefinition().GetAllData())
            {
                switch (parsedItemData.Category)
                {
                    case -7:
                    case -2:
                        continue;
                    default:
                        if (!Object.isPotentialBasicShipped(parsedItemData.ItemId, parsedItemData.Category, parsedItemData.ObjectType))
                        {
                            continue;
                        }

                        if (!Game1.player.basicShipped.ContainsKey(parsedItemData.ItemId))
                        {
                            numberOfMissedItems++;
                        }

                        if (numberOfMissedItems > numberOfUnavailableItems)
                        {
                            return false;
                        }
                        break;
                }
            }

            return true;
        }

        private static bool HasCookedAllRecipes()
        {
            var numberOfUnavailableRecipes = _archipelago.SlotData.ExcludeGingerIsland ? 5 : 0;
            var allRecipes = DataLoader.CookingRecipes(Game1.content);
            var numberOfMissedRecipes = 0;
            foreach (var (recipeName, recipe) in allRecipes)
            {
                if (!Game1.player.cookingRecipes.ContainsKey(recipeName))
                {
                    numberOfMissedRecipes++;
                    continue;
                }

                var recipeId = recipe.Split('/')[2].Split(' ')[0];
                if (!Game1.player.recipesCooked.ContainsKey(recipeId))
                {
                    numberOfMissedRecipes++;
                    continue;
                }
            }

            return numberOfMissedRecipes <= numberOfUnavailableRecipes;
        }

        private static bool HasCraftedAllRecipes()
        {
            var numberOfUnavailableRecipes = _archipelago.SlotData.ExcludeGingerIsland ? 8 : 0;
            numberOfUnavailableRecipes += _archipelago.SlotData.Mods.HasMod(ModNames.BOARDING_HOUSE) ? 5 : 0; // Restore crafts are ignored
            var allRecipes = DataLoader.CraftingRecipes(Game1.content);
            var numberOfMissedRecipes = 0;
            foreach (var recipe in allRecipes.Keys)
            {
                if (!Game1.player.craftingRecipes.ContainsKey(recipe) || Game1.player.craftingRecipes[recipe] <= 0)
                {
                    numberOfMissedRecipes++;
                }

                if (numberOfMissedRecipes > numberOfUnavailableRecipes)
                {
                    return false;
                }
            }

            return true;
        }

        // private void clickCraftingRecipe(ClickableTextureComponent c, bool playSound = true)
        public static void ClickCraftingRecipe_CraftMasterGoal_Postfix(CraftingPage __instance, ClickableTextureComponent c, bool playSound)
        {
            try
            {
                CheckCraftMasterGoalCompletion();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed in {nameof(ClickCraftingRecipe_CraftMasterGoal_Postfix)}:\n{ex}");
                return;
            }
        }

        // public uint totalMoneyEarned
        public static void TotalMoneyEarned_CheckLegendGoalCompletion_Postfix(Farmer __instance, uint value)
        {
            try
            {
                CheckLegendGoalCompletion();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed in {nameof(TotalMoneyEarned_CheckLegendGoalCompletion_Postfix)}:\n{ex}");
                return;
            }
        }

        // public static bool foundAllStardrops(Farmer who = null)
        public static void FoundAllStardrops_CheckStardropsGoalCompletion_Postfix(Farmer who, ref bool __result)
        {
            try
            {
                if (who.maxStamina.Value < 508)
                {
                    return;
                }

                who.ClearBuffs();
                if (who.maxStamina.Value < 508)
                {
                    return;
                }

                _archipelago.ReportGoalCompletion();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed in {nameof(FoundAllStardrops_CheckStardropsGoalCompletion_Postfix)}:\n{ex}");
                return;
            }
        }

        // public static float percentGameComplete()
        public static void PercentGameComplete_PerfectionGoal_Postfix(ref float __result)
        {
            try
            {
                if (__result < 1.0)
                {
                    return;
                }

                _archipelago.ReportGoalCompletion();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed in {nameof(PercentGameComplete_PerfectionGoal_Postfix)}:\n{ex}");
                return;
            }
        }

        public static string GetGoalString()
        {
            var goal = _archipelago.SlotData.Goal switch
            {
                Goal.GrandpaEvaluation => "Complete Grandpa's Evaluation with a score of at least 12 (4 candles)",
                Goal.BottomOfMines => "Reach Floor 120 in the Pelican Town Mineshaft",
                Goal.CommunityCenter => "Complete the Community Center",
                Goal.CrypticNote => "Find Secret Note #10 and complete the \"Cryptic Note\" Quest",
                Goal.MasterAngler => "Catch every single one of the 55 fish available in the game",
                Goal.CompleteCollection => "Complete the Museum Collection by donating all 95 items",
                Goal.FullHouse => "Get married and have two children",
                Goal.GreatestWalnutHunter => "Find all 130 Golden Walnuts",
                Goal.ProtectorOfTheValley => "Complete all the monster slaying goals",
                Goal.FullShipment => "Ship every item",
                Goal.GourmetChef => "Cook every recipe",
                Goal.CraftMaster => "Craft every item",
                Goal.Legend => "Earn 10 000 000g",
                Goal.MysteryOfTheStardrops => "Obtain all stardrops",
                Goal.Allsanity => "Complete every Archipelago check",
                Goal.Perfection => "Achieve Perfection",
                _ => throw new NotImplementedException(),
            };
            return goal;
        }

        public static string GetGoalStringGrandpa()
        {
            switch (_archipelago.SlotData.Goal)
            {
                case Goal.GrandpaEvaluation:
                    return "Make the most of this farm, and make me proud";
                case Goal.BottomOfMines:
                    return "Finish exploring the mineshaft in this town for me";
                case Goal.CommunityCenter:
                    return "Restore the old Community Center for the sake of all the villagers";
                case Goal.CrypticNote:
                    return "Meet an old friend of mine on floor 100 of the Skull Cavern";
                case Goal.MasterAngler:
                    return "Catch and document every specie of fish in the Ferngill Republic";
                case Goal.CompleteCollection:
                    return "Restore our beautiful museum with a full collection of various artifacts and minerals";
                case Goal.FullHouse:
                    return "I wish for my bloodline to thrive. Please find a partner and live happily ever after";
                case Goal.GreatestWalnutHunter:
                    return "Prove your worth to an old friend of mine, and become the greatest walnut hunter";
                case Goal.ProtectorOfTheValley:
                    var currentModFolder = _modHelper.DirectoryPath;
                    var soundsFolder = "Sounds";
                    var fileName = "doom-eternal.wav";
                    var relativePathToSound = Path.Combine(currentModFolder, soundsFolder, fileName);
                    var doomCueDefinition = new CueDefinition("doom", SoundEffect.FromFile(relativePathToSound), 0);
                    Game1.soundBank.AddCue(doomCueDefinition);
                    Game1.playSound("doom");
                    return "Make sure the valley is safe for generations to come. Rip and tear, until it is done";
                case Goal.FullShipment:
                    return "Contribute to the local economy and market, by shipping as many things as you can";
                case Goal.GourmetChef:
                    return "Become a world-class chef, learn and cook all the recipes you can find";
                case Goal.CraftMaster:
                    return "Get used to making things with your hands, and craft as many items as you can";
                case Goal.Legend:
                    return "Nothing beats cold hard cash. Become rich enough, and buy your happiness";
                case Goal.MysteryOfTheStardrops:
                    return "A healthy body is a healthy mind. Get in shape by increasing your energy to the maximum.";
                case Goal.Allsanity:
                    return "You cannot leave anyone stranded in a Burger King. Leave no loose ends";
                case Goal.Perfection:
                    return "For a fulfilling life, you need to do a lot of everything. Leave no loose ends";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
