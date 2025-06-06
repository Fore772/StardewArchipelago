﻿using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using StardewArchipelago.Bundles;
using StardewModdingAPI;
using StardewArchipelago.Logging;

namespace StardewArchipelago.Textures
{
    public static class BundleIcons
    {
        public const string BUNDLE_SUFFIX = "bundle";
        private static bool _useMemeBundles = false;

        public static Texture2D GetBundleIcon(LogHandler logger, IModHelper modHelper, string bundleName, LogLevel failureLogLevel = LogLevel.Error)
        {
            var folder = _useMemeBundles ? "Remixed" : "Meme";

            var icon = GetBundleIcon(logger, modHelper, bundleName, failureLogLevel, folder);

            if (icon != null)
            {
                return icon;
            }

            folder = _useMemeBundles ? "Meme" : "Remixed";
            icon = GetBundleIcon(logger, modHelper, bundleName, LogLevel.Debug, folder);
            if (icon != null)
            {
                logger.LogDebug($"Switching to different bundle icon set");
                _useMemeBundles = !_useMemeBundles;
                return icon;
            }

            return null;
        }

        private static Texture2D GetBundleIcon(LogHandler logger, IModHelper modHelper, string bundleName, LogLevel failureLogLevel, string folder)
        {
            var bundlesFolder = Path.Combine("Bundles", "Icons", folder);
            var cleanName = bundleName.Replace("'", "").Replace(" ", "_").ToLower();
            var fileNameBundleName = $"{cleanName}_{BUNDLE_SUFFIX}.png";
            if (bundleName == MemeBundleNames.BUN_DLE)
            {
                fileNameBundleName = $"{cleanName}.png";
            }
            var pathToTexture = Path.Combine(bundlesFolder, fileNameBundleName);
            logger.LogDebug($"Attempting to load bundle icon '{pathToTexture}'");
            return TexturesLoader.GetTexture(pathToTexture, failureLogLevel);
        }

        public static List<Texture2D> GetAllBundleIcons(IModHelper modHelper)
        {
            var currentModFolder = modHelper.DirectoryPath;
            const string texturesFolder = "Textures";
            var bundleIconsFolder = Path.Combine(currentModFolder, texturesFolder, "Bundles", "Icons");
            var bundleFilenamePattern = $"*.png";
            var allBundleIcons = Directory.EnumerateFiles(bundleIconsFolder, bundleFilenamePattern, SearchOption.AllDirectories);
            var textures = new List<Texture2D>();
            foreach (var bundleIcon in allBundleIcons)
            {
                var pathToTexture = Path.Combine(bundleIconsFolder, bundleIcon);
                var texture = TexturesLoader.GetTexture(pathToTexture);
                textures.Add(texture);
            }

            return textures;
        }
    }
}
