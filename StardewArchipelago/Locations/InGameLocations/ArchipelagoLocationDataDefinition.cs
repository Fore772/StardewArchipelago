﻿using System;
using System.Collections.Generic;
using System.Linq;
using KaitoKid.ArchipelagoUtilities.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewArchipelago.Constants;
using StardewArchipelago.Constants.Vanilla;
using StardewArchipelago.Logging;
using StardewModdingAPI;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewArchipelago.Archipelago;

namespace StardewArchipelago.Locations.InGameLocations
{
    /// <summary>Manages the data for archipelago location items.</summary>
    public class ArchipelagoLocationDataDefinition : BaseItemDataDefinition
    {
        private static LogHandler _logger;
        private static IModHelper _modHelper;
        private static StardewArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(LogHandler logger, IModHelper modHelper, StardewArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _logger = logger;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        /// <inheritdoc />
        public override string Identifier => QualifiedItemIds.ARCHIPELAGO_QUALIFER;

        /// <inheritdoc />
        public override string StandardDescriptor => QualifiedItemIds.ARCHIPELAGO_QUALIFER.Substring(1, 2);

        /// <inheritdoc />
        public override IEnumerable<string> GetAllIds() => Enumerable.Empty<string>();

        /// <inheritdoc />
        public override bool Exists(string itemId) => itemId != null && (itemId.StartsWith(Identifier + IDProvider.AP_LOCATION) || itemId.StartsWith(IDProvider.AP_LOCATION));

        /// <inheritdoc />
        public override ParsedItemData GetData(string itemId)
        {
            if (!itemId.StartsWith(Identifier + IDProvider.AP_LOCATION) && !itemId.StartsWith(IDProvider.AP_LOCATION))
            {
                return null;
            }
            var itemData = new ParsedArchipelagoItemData(_logger, _modHelper, _archipelago, _locationChecker, this, itemId, 0, "", "", "", "", 0, "", null);
            return itemData;
        }

        /// <inheritdoc />
        public override Rectangle GetSourceRect(
            ParsedItemData data,
            Texture2D texture,
            int spriteIndex)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            return texture != null ? Game1.getSourceRectForStandardTileSheet(texture, spriteIndex, 16, 16) : throw new ArgumentNullException(nameof(texture));
        }

        /// <inheritdoc />
        public override Item CreateItem(ParsedItemData data)
        {
            var id = data.ItemId;
            var apLocationPrefix = IDProvider.AP_LOCATION;
            var indexOfPrefixStart = id.IndexOf(apLocationPrefix, StringComparison.InvariantCultureIgnoreCase);
            var locationName = id[(indexOfPrefixStart + apLocationPrefix.Length + 1)..];
            return new ObtainableArchipelagoLocation(locationName, _logger, _modHelper, _locationChecker, _archipelago, _archipelago.GetMyActiveHints());
        }
    }
}
