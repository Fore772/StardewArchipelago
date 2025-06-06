﻿using System;
using System.Collections.Generic;
using System.Linq;
using KaitoKid.ArchipelagoUtilities.Net;
using StardewArchipelago.Bundles;
using StardewValley;
using StardewValley.Locations;

namespace StardewArchipelago.Stardew
{
    public class BundleReader
    {
        //private Dictionary<int, string> _bundleIdToName;
        //private Dictionary<string, int> _bundleNameToId;
        //private Dictionary<Area, List<int>> _areaToBundles;
        //private Dictionary<int, Area> _bundleToArea;

        public BundleReader()
        {
        }

        public bool IsCommunityCenterComplete()
        {
            var communityCenter = GetCommunityCenter();
            return communityCenter.areAllAreasComplete();
        }

        public void CheckAllBundleLocations(LocationChecker locationChecker)
        {
            var completedBundleNames = GetAllCompletedBundles();
            foreach (var completedBundleName in completedBundleNames)
            {
                if (locationChecker.IsLocationMissing(completedBundleName + " Bundle"))
                {
                    locationChecker.AddCheckedLocation(completedBundleName + " Bundle");
                }
                else if (locationChecker.IsLocationMissing(completedBundleName))
                {
                    locationChecker.AddCheckedLocation(completedBundleName);
                }
            }
        }

        public List<string> GetAllCompletedBundles()
        {
            var communityCenter = GetCommunityCenter();
            var completedBundles = new List<string>();
            foreach (var (key, bundleData) in Game1.netWorldState.Value.BundleData)
            {
                var splitKey = key.Split('/');
                var bundleId = Convert.ToInt32(splitKey[1]);
                var isCompleted = IsBundleComplete(communityCenter, bundleId, bundleData);
                if (isCompleted)
                {
                    var bundleName = bundleData.Split("/").First();
                    completedBundles.Add(bundleName);
                }
            }

            return completedBundles;
        }

        private Area GetAreaNumberFromId(int desiredBundleId)
        {
            foreach (var (key, bundleName) in Game1.netWorldState.Value.BundleData)
            {
                var splitKey = key.Split('/');
                var bundleId = Convert.ToInt32(splitKey[1]);
                if (bundleId == desiredBundleId)
                {
                    var areaName = splitKey[0];
                    return (Area)CommunityCenter.getAreaNumberFromName(areaName);
                }
            }

            throw new ArgumentException($"Failed in {nameof(GetAreaNumberFromId)}: Could not find a bundle with id {desiredBundleId}");
        }

        private CommunityCenter GetCommunityCenter()
        {
            return Game1.locations.OfType<CommunityCenter>().First();
        }

        private bool IsBundleComplete(CommunityCenter communityCenter, int bundleId, string bundleData)
        {
            var dataFields = bundleData.Split("/");
            var name = dataFields[0];
            var ingredients = dataFields[2];
            var firstIngredientId = ingredients.Split(" ")[0];
            if (CurrencyBundle.CurrencyIds.ContainsValue(firstIngredientId))
            {
                return communityCenter.bundles[bundleId][0];
            }

            return communityCenter.isBundleComplete(bundleId);
        }
    }

    public enum Area
    {
        None = -1,

        // Community Center
        Pantry = 0,
        CraftsRoom = 1,
        FishTank = 2,
        BoilerRoom = 3,
        Vault = 4,
        Bulletin = 5,

        // Other
        AbandonedJojaMart = 6,
        Bulletin2 = 7,
        JunimoHut = 8,
    }
}
