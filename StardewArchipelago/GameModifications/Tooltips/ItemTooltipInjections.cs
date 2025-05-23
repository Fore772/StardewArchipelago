﻿using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KaitoKid.ArchipelagoUtilities.Net.Client;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
using StardewArchipelago.Locations;
using StardewArchipelago.Locations.CodeInjections.Vanilla;
using StardewArchipelago.Logging;
using StardewArchipelago.Stardew.NameMapping;
using StardewArchipelago.Textures;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace StardewArchipelago.GameModifications.Tooltips
{
    public class ItemTooltipInjections
    {
        private static ILogger _logger;
        private static IModHelper _modHelper;
        private static ModConfig _config;
        private static ArchipelagoClient _archipelago;
        private static StardewLocationChecker _locationChecker;
        private static NameSimplifier _nameSimplifier;
        private static Texture2D _miniArchipelagoIcon;

        public static void Initialize(LogHandler logger, IModHelper modHelper, ModConfig config, ArchipelagoClient archipelago, StardewLocationChecker locationChecker, NameSimplifier nameSimplifier)
        {
            _logger = logger;
            _modHelper = modHelper;
            _config = config;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _nameSimplifier = nameSimplifier;

            var desiredTextureName = ArchipelagoTextures.COLOR;
            _miniArchipelagoIcon = ArchipelagoTextures.GetArchipelagoLogo(12, desiredTextureName);
        }

        // public abstract void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        public static void DrawInMenu_AddArchipelagoLogoIfNeeded_Postfix(Object __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            try
            {
                ObjectDrawInMenuPostfix(__instance, spriteBatch, location, scaleSize, transparency, layerDepth, color);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed in {nameof(DrawInMenu_AddArchipelagoLogoIfNeeded_Postfix)}:\n{ex}");
                return;
            }
        }

        // public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency,
        // float layerDepth, StackDrawType drawStackNumber, Color colorOverride, bool drawShadow)
        public static void DrawInMenuColored_AddArchipelagoLogoIfNeeded_Postfix(ColoredObject __instance, SpriteBatch spriteBatch,
            Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber,
            Color colorOverride, bool drawShadow)
        {
            try
            {
                ObjectDrawInMenuPostfix(__instance, spriteBatch, location, scaleSize, transparency, layerDepth, colorOverride);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed in {nameof(DrawInMenuColored_AddArchipelagoLogoIfNeeded_Postfix)}:\n{ex}");
                return;
            }
        }

        private static bool ObjectDrawInMenuPostfix(Object item, SpriteBatch spriteBatch, Vector2 location,
            float scaleSize, float transparency, float layerDepth, Color color)
        {
            if (item == null || _config.ShowItemIndicators == ItemIndicatorPreference.False)
            {
                return true;
            }

            var simplifiedName = _nameSimplifier.GetSimplifiedName(item);
            var allUncheckedLocations = _locationChecker.GetAllLocationsNotCheckedContainingWord(simplifiedName);

            allUncheckedLocations = FilterLocationsBasedOnConfig(allUncheckedLocations);

            if (!allUncheckedLocations.Any())
            {
                return true;
            }


            var position = location + new Vector2(14f, 14f);
            var sourceRectangle = new Rectangle(0, 0, 12, 12);
            var transparentColor = color * transparency;
            var origin = new Vector2(8f, 8f);

            spriteBatch.Draw(_miniArchipelagoIcon, position, sourceRectangle, transparentColor, 0.0f, origin, scaleSize,
                SpriteEffects.None, layerDepth);
            return false;
        }

        // public override string getDescription()
        public static void GetDescription_AddMissingChecks_Postfix(Object __instance, ref string __result)
        {
            try
            {
                if (__instance == null || _config.ShowItemIndicators == ItemIndicatorPreference.False)
                {
                    return;
                }

                var simplifiedName = _nameSimplifier.GetSimplifiedName(__instance);
                var allUncheckedLocations = _locationChecker.GetAllLocationsNotCheckedContainingWord(simplifiedName);

                allUncheckedLocations = FilterLocationsBasedOnConfig(allUncheckedLocations);

                foreach (var uncheckedLocation in allUncheckedLocations)
                {
                    __result += $"{Environment.NewLine}{uncheckedLocation}";
                }

                return;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed in {nameof(GetDescription_AddMissingChecks_Postfix)}:\n{ex}");
                return;
            }
        }

        private static string[] FilterLocationsBasedOnConfig(string[] allUncheckedLocations)
        {
            if (_config.ShowItemIndicators == ItemIndicatorPreference.OnlyShipsanity)
            {
                return allUncheckedLocations.Where(x =>
                    x.Contains(NightShippingBehaviors.SHIPSANITY_PREFIX,
                        StringComparison.InvariantCultureIgnoreCase)).ToArray();
            }

            return allUncheckedLocations;
        }
    }
}
