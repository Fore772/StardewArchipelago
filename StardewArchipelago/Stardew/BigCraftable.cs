﻿using System;
using Microsoft.Xna.Framework;
using KaitoKid.ArchipelagoUtilities.Net.Client;
using StardewArchipelago.Constants.Vanilla;
using StardewArchipelago.Items.Mail;
using StardewValley;
using Object = StardewValley.Object;

namespace StardewArchipelago.Stardew
{
    public class BigCraftable : StardewItem
    {
        public const string BIG_CRAFTABLE_SEPARATOR = ":";

        public bool Outdoors { get; private set; }
        public bool Indoors { get; private set; }
        public int Fragility { get; private set; }

        public BigCraftable(string id, string name, int sellPrice, string description, bool outdoors, bool indoors, int fragility, string displayName)
            : base(id, name, sellPrice, displayName, description)
        {
            Outdoors = outdoors;
            Indoors = indoors;
            Fragility = fragility;

            if (Name == "Rarecrow")
            {
                var rarecrowNumber = GetRarecrowNumber(int.Parse(id));
                Name += $" #{rarecrowNumber}";
            }
        }

        public static string ConvertToApName(Object salableItem)
        {
            if (salableItem.Name != "Rarecrow")
            {
                return salableItem.Name;
            }

            var rarecrowNumber = GetRarecrowNumber(salableItem);
            return $"{salableItem.Name} #{rarecrowNumber}";
        }

        private static int GetRarecrowNumber(Object salableItem)
        {
            try
            {
                return GetRarecrowNumber(salableItem.ParentSheetIndex);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"{salableItem.Name} is not a recognized rarecrow! {ex}");
            }
        }

        private static int GetRarecrowNumber(int id)
        {
            return id switch
            {
                110 => 1,
                113 => 2,
                126 => 3,
                136 => 4,
                137 => 5,
                138 => 6,
                139 => 7,
                140 => 8,
                _ => throw new ArgumentException($"{id} is not a recognized rarecrow!"),
            };
        }

        public override Item PrepareForGivingToFarmer(int amount = 1)
        {
            var bigCraftable = new Object(Vector2.Zero, Id);
            bigCraftable.Stack = amount;
            return bigCraftable;
        }

        public override void GiveToFarmer(Farmer farmer, int amount = 1)
        {
            var bigCraftable = PrepareForGivingToFarmer();
            farmer.addItemByMenuIfNecessaryElseHoldUp(bigCraftable);
        }

        public override LetterAttachment GetAsLetter(ReceivedItem receivedItem, int amount = 1)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.GiveBigCraftable, $"{Id}{BIG_CRAFTABLE_SEPARATOR}{amount}");
        }

        public override string GetQualifiedId()
        {
            return $"{QualifiedItemIds.BIG_CRAFTABLE_QUALIFIER}{Id}";
        }
    }
}
