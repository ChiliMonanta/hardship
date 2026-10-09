using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn;
using Jotunn.Managers;
using Jotunn.Utils;
using UnityEngine;
using UnityEngine.Rendering;

namespace Hardship;

[BepInPlugin(PluginGUID, PluginName, PluginInfo.PluginVersion)]
[BepInDependency(Main.ModGuid)]
[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Patch)]
public class Hardship : BaseUnityPlugin
{
    public const string PluginGUID = "com.valheim.hardship";
    public const string PluginName = "Hardship";

    private ConfigEntry<float> copperOreWeight;
    private ConfigEntry<float> tinOreWeight;
    private ConfigEntry<float> ironScrapWeight;
    private ConfigEntry<float> silverOreWeight;
    private ConfigEntry<float> copperScrapWeight;
    private ConfigEntry<float> ironOreWeight;
    private ConfigEntry<float> bronzeScrapWeight;
    private ConfigEntry<float> blackMetalScrapWeight;
    private ConfigEntry<float> flametalOreNewWeight;
    private ConfigEntry<float> surtlingCoreWeight;
    private ConfigEntry<float> stormWindThreshold;
    private ConfigEntry<float> stormShipDamagePerSecond;
    private ConfigEntry<float> stormMaxDamageMultiplier;
    private ConfigEntry<float> stormShakeStrength;
    private ConfigEntry<float> stormShakeRange;
    private ConfigEntry<float> stormShallowWaterDepth;
    private ConfigEntry<float> clubDamage;
    private ConfigEntry<float> flintKnifeDamage;
    private ConfigEntry<float> stoneAxeDamage;
    private ConfigEntry<float> flintSpearDamage;
    private ConfigEntry<float> crudeBowDamage;
    private ConfigEntry<int> torchWoodCost;
    private ConfigEntry<int> torchResinCost;
    private ConfigEntry<int> torchHitsToBreak;
    private ConfigEntry<bool> blockRaidDrops;
    internal static ConfigEntry<float> cryptSurtlingCoreChance;
    internal static ConfigEntry<bool> StormShipDamageEnabled;
    internal static ConfigEntry<float> StormWindThreshold;
    internal static ConfigEntry<float> StormShipDamagePerSecond;
    internal static ConfigEntry<float> StormMaxDamageMultiplier;
    internal static ConfigEntry<float> StormShakeStrength;
    internal static ConfigEntry<float> StormShakeRange;
    internal static ConfigEntry<float> StormShallowWaterDepth;
    internal static ConfigEntry<bool> BlockRaidDrops;
    internal static ConfigEntry<bool> LightningEnabled;
    internal static ConfigEntry<float> LightningLandChance;
    internal static ConfigEntry<float> LightningShipChance;
    internal static ConfigEntry<float> LightningCheckInterval;
    internal static ConfigEntry<float> LightningCooldownSeconds;
    internal static ConfigEntry<string> LightningWeatherNames;
    internal static ConfigEntry<float> GeyserGasMinimumIntervalSeconds;
    internal static ConfigEntry<float> GeyserGasMaximumIntervalSeconds;
    internal static ConfigEntry<float> GeyserGasCloudDuration;
    internal static ConfigEntry<float> GeyserGasCloudRadius;
    internal static ConfigEntry<float> GeyserGasStaminaDrainPerSecond;
    private static Harmony harmony;

    #region Plugin lifecycle and configuration

    private void Awake()
    {
        copperOreWeight = Config.Bind(
            "Ore Weights",
            "CopperOre",
            50f,
            new ConfigDescription(
                "Weight of one copper ore.",
                new AcceptableValueRange<float>(0.1f, 1000f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        tinOreWeight = Config.Bind(
            "Ore Weights",
            "TinOre",
            50f,
            new ConfigDescription(
                "Weight of one tin ore.",
                new AcceptableValueRange<float>(0.1f, 1000f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        ironScrapWeight = Config.Bind(
            "Ore Weights",
            "IronScrap",
            50f,
            new ConfigDescription(
                "Weight of one iron scrap.",
                new AcceptableValueRange<float>(0.1f, 1000f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        silverOreWeight = Config.Bind(
            "Ore Weights",
            "SilverOre",
            50f,
            new ConfigDescription(
                "Weight of one silver ore.",
                new AcceptableValueRange<float>(0.1f, 1000f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        copperScrapWeight = Config.Bind(
            "Ore Weights",
            "CopperScrap",
            50f,
            new ConfigDescription(
                "Weight of one copper scrap.",
                new AcceptableValueRange<float>(0.1f, 1000f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        ironOreWeight = Config.Bind(
            "Ore Weights",
            "IronOre",
            50f,
            new ConfigDescription(
                "Weight of one iron ore.",
                new AcceptableValueRange<float>(0.1f, 1000f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        bronzeScrapWeight = Config.Bind(
            "Ore Weights",
            "BronzeScrap",
            50f,
            new ConfigDescription(
                "Weight of one bronze scrap.",
                new AcceptableValueRange<float>(0.1f, 1000f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        blackMetalScrapWeight = Config.Bind(
            "Ore Weights",
            "BlackMetalScrap",
            50f,
            new ConfigDescription(
                "Weight of one black metal scrap.",
                new AcceptableValueRange<float>(0.1f, 1000f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        flametalOreNewWeight = Config.Bind(
            "Ore Weights",
            "FlametalOreNew",
            50f,
            new ConfigDescription(
                "Weight of one flametal ore.",
                new AcceptableValueRange<float>(0.1f, 1000f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        surtlingCoreWeight = Config.Bind(
            "Ore Weights",
            "SurtlingCore",
            150f,
            new ConfigDescription(
                "Weight of one surtling core.",
                new AcceptableValueRange<float>(0.1f, 1000f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        clubDamage = Config.Bind(
            "Weapon Balance",
            "ClubDamage",
            8f,
            new ConfigDescription(
                "Blunt damage dealt by the Club.",
                new AcceptableValueRange<float>(0f, 100f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        flintKnifeDamage = Config.Bind(
            "Weapon Balance",
            "FlintKnifeDamage",
            8f,
            new ConfigDescription(
                "Total damage dealt by the Flint Knife, split between slash and pierce.",
                new AcceptableValueRange<float>(0f, 100f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        stoneAxeDamage = Config.Bind(
            "Weapon Balance",
            "StoneAxeDamage",
            9f,
            new ConfigDescription(
                "Slash damage dealt by the Stone Axe.",
                new AcceptableValueRange<float>(0f, 100f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        flintSpearDamage = Config.Bind(
            "Weapon Balance",
            "FlintSpearDamage",
            12f,
            new ConfigDescription(
                "Pierce damage dealt by the Flint Spear.",
                new AcceptableValueRange<float>(0f, 100f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        crudeBowDamage = Config.Bind(
            "Weapon Balance",
            "CrudeBowDamage",
            14f,
            new ConfigDescription(
                "Pierce damage dealt by the Crude Bow before arrow damage.",
                new AcceptableValueRange<float>(0f, 100f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        torchWoodCost = Config.Bind(
            "Weapon Balance",
            "TorchWoodCost",
            2,
            new ConfigDescription(
                "Wood required to craft a Torch.",
                new AcceptableValueRange<int>(1, 20),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        torchResinCost = Config.Bind(
            "Weapon Balance",
            "TorchResinCost",
            5,
            new ConfigDescription(
                "Resin required to craft a Torch.",
                new AcceptableValueRange<int>(1, 20),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        torchHitsToBreak = Config.Bind(
            "Weapon Balance",
            "TorchHitsToBreak",
            2,
            new ConfigDescription(
                "Number of melee hits before a Torch breaks, in addition to its normal timed burn-out.",
                new AcceptableValueRange<int>(1, 20),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        cryptSurtlingCoreChance = Config.Bind(
            "Crypt Loot",
            "SurtlingCoreChance",
            50f,
            new ConfigDescription(
                "Percent chance (0-100) to find a Surtling Core in a Burial Chamber (0 or 1 total per crypt).",
                new AcceptableValueRange<float>(0f, 100f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        stormWindThreshold = Config.Bind(
            "Storm Ship Damage",
            "WindThreshold",
            0.8f,
            new ConfigDescription(
                "Minimum wind force required for storms to damage ships.",
                new AcceptableValueRange<float>(0f, 1f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        StormShipDamageEnabled = Config.Bind(
            "Storm Ship Damage",
            "Enabled",
            true,
            new ConfigDescription(
                "Enable storm damage to ships.",
                null,
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        stormShipDamagePerSecond = Config.Bind(
            "Storm Ship Damage",
            "DamagePerSecond",
            7f,
            new ConfigDescription(
                "Blunt damage dealt to ships per second during storms.",
                new AcceptableValueRange<float>(0f, 100f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        stormMaxDamageMultiplier = Config.Bind(
            "Storm Ship Damage",
            "MaxDamageMultiplier",
            2f,
            new ConfigDescription(
                "Maximum damage multiplier at the strongest wind.",
                new AcceptableValueRange<float>(1f, 10f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        stormShakeStrength = Config.Bind(
            "Storm Ship Damage",
            "ShakeStrength",
            0.2f,
            new ConfigDescription(
            "Camera shake strength when a wave damages the ship.",
            new AcceptableValueRange<float>(0f, 2f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

        stormShakeRange = Config.Bind(
            "Storm Ship Damage",
            "ShakeRange",
            20f,
            new ConfigDescription(
            "Maximum distance at which storm ship impacts shake the camera.",
            new AcceptableValueRange<float>(0f, 100f),
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

        stormShallowWaterDepth = Config.Bind(
            "Storm Ship Damage",
            "ShallowWaterDepth",
            15f,
            new ConfigDescription(
                "Ships do not take storm damage when the seabed is this close below them.",
                new AcceptableValueRange<float>(0f, 40f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        StormWindThreshold = stormWindThreshold;
        StormShipDamagePerSecond = stormShipDamagePerSecond;
        StormMaxDamageMultiplier = stormMaxDamageMultiplier;
        StormShakeStrength = stormShakeStrength;
        StormShakeRange = stormShakeRange;
        StormShallowWaterDepth = stormShallowWaterDepth;

        blockRaidDrops = Config.Bind(
            "Raid Loot",
            "BlockRaidDrops",
            true,
            new ConfigDescription(
                "Prevent creatures spawned by raids from dropping loot.",
                null,
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
        BlockRaidDrops = blockRaidDrops;

        LightningEnabled = Config.Bind(
            "Lightning Strikes",
            "Enabled",
            true,
            new ConfigDescription(
                "Enable lightning strikes on players during thunderstorms.",
                null,
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        // A thunderstorm is 666s
        // A check every 130s with percentage:
        // Land 0.5% will give a total 5% over the duration of a thunderstorm
        // Ship 1% will give a total 10% over the duration of a thunderstorm
        LightningLandChance = Config.Bind(
            "Lightning Strikes",
            "LandChancePercent",
            0.5f,
            new ConfigDescription(
                "Percent chance (0-100) per check to be struck while on land during a thunderstorm.",
                new AcceptableValueRange<float>(0f, 100f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        LightningShipChance = Config.Bind(
            "Lightning Strikes",
            "ShipChancePercent",
            1f,
            new ConfigDescription(
                "Percent chance (0-100) per check to be struck while on a ship during a thunderstorm.",
                new AcceptableValueRange<float>(0f, 100f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        LightningCheckInterval = Config.Bind(
            "Lightning Strikes",
            "CheckIntervalSeconds",
            130f,
            new ConfigDescription(
                "How often (in seconds) the strike chance is rolled per player.",
                new AcceptableValueRange<float>(0.1f, 240f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        LightningWeatherNames = Config.Bind(
            "Lightning Strikes",
            "ThunderstormEnvironments",
            "ThunderStorm",
            new ConfigDescription(
                "Comma-separated environment names that count as a thunderstorm.",
                null,
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        LightningCooldownSeconds = Config.Bind(
            "Lightning Strikes",
            "CooldownSeconds",
            120f,
            new ConfigDescription(
                "Minimum time after being struck before a player can be struck again.",
                new AcceptableValueRange<float>(0f, 3600f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        GeyserGasMinimumIntervalSeconds = Config.Bind(
            "Geyser Gas",
            "MinimumIntervalSeconds",
            4f,
            new ConfigDescription(
                "Minimum time between ambient gas eruptions at each geyser.",
                new AcceptableValueRange<float>(0.1f, 86400f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        GeyserGasMaximumIntervalSeconds = Config.Bind(
            "Geyser Gas",
            "MaximumIntervalSeconds",
            55f,
            new ConfigDescription(
                "Maximum time between ambient gas eruptions at each geyser.",
                new AcceptableValueRange<float>(0.1f, 86400f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        GeyserGasCloudDuration = Config.Bind(
            "Geyser Gas",
            "CloudDuration",
            15f,
            new ConfigDescription(
                "Duration of each geyser gas cloud in seconds.",
                new AcceptableValueRange<float>(0.1f, 3600f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        GeyserGasCloudRadius = Config.Bind(
            "Geyser Gas",
            "CloudRadius",
            18f,
            new ConfigDescription(
                "Radius of the area affected by each geyser gas cloud in meters.",
                new AcceptableValueRange<float>(0f, 1000f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        GeyserGasStaminaDrainPerSecond = Config.Bind(
            "Geyser Gas",
            "StaminaDrainPerSecond",
            25f,
            new ConfigDescription(
                "Stamina drained per second while exposed to geyser gas.",
                new AcceptableValueRange<float>(0f, 1000f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        harmony = new Harmony(PluginGUID);
        try
        {
            harmony.PatchAll();
        }
        catch (Exception exception)
        {
            Logger.LogError($"Failed to install Harmony patches: {exception}");
            throw;
        }

        Logger.LogInfo($"{PluginName} started.");
        Jotunn.Logger.LogInfo($"{PluginName} started through Jotunn.");
        LogConfiguration();
        ItemManager.OnItemsRegistered += SetItemWeights;
        SynchronizationManager.OnConfigurationSynchronized += OnConfigurationSynchronized;
    }

    // Logs every config value once at startup so the current setup is visible without repeating on every sync.
    private void LogConfiguration()
    {
        Logger.LogInfo($"Copper ore weight set to {copperOreWeight.Value}.");
        Logger.LogInfo($"Tin ore weight set to {tinOreWeight.Value}.");
        Logger.LogInfo($"Iron scrap weight set to {ironScrapWeight.Value}.");
        Logger.LogInfo($"Silver ore weight set to {silverOreWeight.Value}.");
        Logger.LogInfo($"Copper scrap weight set to {copperScrapWeight.Value}.");
        Logger.LogInfo($"Iron ore weight set to {ironOreWeight.Value}.");
        Logger.LogInfo($"Bronze scrap weight set to {bronzeScrapWeight.Value}.");
        Logger.LogInfo($"Black metal scrap weight set to {blackMetalScrapWeight.Value}.");
        Logger.LogInfo($"Flametal ore weight set to {flametalOreNewWeight.Value}.");
        Logger.LogInfo($"Surtling core weight set to {surtlingCoreWeight.Value}.");
        Logger.LogInfo($"Crypt Surtling Core chance set to {cryptSurtlingCoreChance.Value}%.");
        Logger.LogInfo($"Storm ship damage enabled set to {StormShipDamageEnabled.Value}.");
        Logger.LogInfo($"Storm wind threshold set to {StormWindThreshold.Value}.");
        Logger.LogInfo($"Storm ship damage per second set to {StormShipDamagePerSecond.Value}.");
        Logger.LogInfo($"Storm max damage multiplier set to {StormMaxDamageMultiplier.Value}.");
        Logger.LogInfo($"Storm shake strength set to {StormShakeStrength.Value}.");
        Logger.LogInfo($"Storm shake range set to {StormShakeRange.Value}.");
        Logger.LogInfo($"Storm shallow water depth set to {StormShallowWaterDepth.Value}.");
        Logger.LogInfo($"Club damage set to {clubDamage.Value}.");
        Logger.LogInfo($"Flint Knife damage set to {flintKnifeDamage.Value}.");
        Logger.LogInfo($"Stone Axe damage set to {stoneAxeDamage.Value}.");
        Logger.LogInfo($"Flint Spear damage set to {flintSpearDamage.Value}.");
        Logger.LogInfo($"Crude Bow damage set to {crudeBowDamage.Value}.");
        Logger.LogInfo($"Torch recipe cost set to {torchWoodCost.Value} Wood, {torchResinCost.Value} Resin.");
        Logger.LogInfo($"Torch hits to break set to {torchHitsToBreak.Value}.");
        Logger.LogInfo($"Raid drops blocked set to {BlockRaidDrops.Value}.");
        Logger.LogInfo($"Lightning strikes enabled set to {LightningEnabled.Value}.");
        Logger.LogInfo($"Lightning land chance set to {LightningLandChance.Value}%.");
        Logger.LogInfo($"Lightning ship chance set to {LightningShipChance.Value}%.");
        Logger.LogInfo($"Lightning check interval set to {LightningCheckInterval.Value} seconds.");
        Logger.LogInfo($"Lightning cooldown set to {LightningCooldownSeconds.Value} seconds.");
        Logger.LogInfo($"Lightning thunderstorm environments set to {LightningWeatherNames.Value}.");
    }

    private void SetItemWeights()
    {
        RemoveEarlyAxeRecipes();

        SetItemWeight("CopperOre", copperOreWeight);
        SetItemWeight("TinOre", tinOreWeight);
        SetItemWeight("IronScrap", ironScrapWeight);
        SetItemWeight("SilverOre", silverOreWeight);
        SetItemWeight("CopperScrap", copperScrapWeight);
        SetItemWeight("IronOre", ironOreWeight);
        SetItemWeight("BronzeScrap", bronzeScrapWeight);
        SetItemWeight("BlackMetalScrap", blackMetalScrapWeight);
        SetItemWeight("FlametalOreNew", flametalOreNewWeight);

        var surtlingCore = PrefabManager.Cache.GetPrefab<ItemDrop>("SurtlingCore");
        if (surtlingCore == null)
        {
            Logger.LogError("Could not find the SurtlingCore prefab.");
            return;
        }

        surtlingCore.m_itemData.m_shared.m_weight = surtlingCoreWeight.Value;
        SetWeaponBalance();
    }

    private void SetItemWeight(string prefabName, ConfigEntry<float> weight)
    {
        var item = PrefabManager.Cache.GetPrefab<ItemDrop>(prefabName);
        if (item == null)
        {
            Logger.LogError($"Could not find the {prefabName} prefab.");
            return;
        }

        item.m_itemData.m_shared.m_weight = weight.Value;
    }

    private void RemoveEarlyAxeRecipes()
    {
        var recipes = ObjectDB.instance?.m_recipes;
        if (recipes == null)
        {
            Logger.LogError("Could not find the recipe list while removing Early Axes.");
            return;
        }

        int removedCount = recipes.RemoveAll(recipe =>
            recipe != null
            && recipe.m_resources != null
            && recipe.m_resources.Any(requirement =>
            {
                var itemName = requirement?.m_resItem?.m_itemData?.m_shared?.m_name;
                return itemName == "$item_axehead1" || itemName == "$item_axehead2";
            }));

        Logger.LogInfo($"Removed {removedCount} recipes requiring Curious or Mysterious Axe Heads.");
    }

    private void SetWeaponBalance()
    {
        SetWeaponDamage("Club", clubDamage.Value);
        SetWeaponDamage("KnifeFlint", flintKnifeDamage.Value);
        SetWeaponDamage("AxeStone", stoneAxeDamage.Value);
        SetWeaponDamage("SpearFlint", flintSpearDamage.Value);
        SetWeaponDamage("BowCrude", crudeBowDamage.Value);
        SetTorchRecipeCost();
        SetTorchHitsToBreak();
    }

    private void SetTorchHitsToBreak()
    {
        var torch = PrefabManager.Cache.GetPrefab<ItemDrop>("Torch");
        if (torch == null || torch.m_itemData?.m_shared == null)
        {
            Logger.LogError("Could not find the Torch weapon prefab.");
            return;
        }

        var shared = torch.m_itemData.m_shared;
        shared.m_useDurability = true;
        shared.m_useDurabilityDrain = shared.m_maxDurability / torchHitsToBreak.Value;
        Logger.LogInfo($"Torch hits to break set to {torchHitsToBreak.Value}.");
    }

    private void SetTorchRecipeCost()
    {
        var recipe = ObjectDB.instance?.m_recipes.Find(r => r.m_item != null && r.m_item.name == "Torch");
        if (recipe == null)
        {
            Logger.LogError("Could not find the Torch recipe.");
            return;
        }

        foreach (var requirement in recipe.m_resources)
        {
            if (requirement.m_resItem == null)
            {
                continue;
            }

            if (requirement.m_resItem.name == "Wood")
            {
                requirement.m_amount = torchWoodCost.Value;
            }
            else if (requirement.m_resItem.name == "Resin")
            {
                requirement.m_amount = torchResinCost.Value;
            }
        }

        Logger.LogInfo($"Torch recipe cost set to {torchWoodCost.Value} Wood, {torchResinCost.Value} Resin.");
    }

    private void SetWeaponDamage(string prefabName, float damage)
    {
        var weapon = PrefabManager.Cache.GetPrefab<ItemDrop>(prefabName);
        if (weapon == null || weapon.m_itemData?.m_shared == null)
        {
            Logger.LogError($"Could not find the {prefabName} weapon prefab.");
            return;
        }

        var damages = weapon.m_itemData.m_shared.m_damages;
        switch (prefabName)
        {
            case "Club":
                damages.m_blunt = damage;
                break;
            case "KnifeFlint":
                damages.m_slash = damage / 2f;
                damages.m_pierce = damage / 2f;
                break;
            case "AxeStone":
                damages.m_slash = damage;
                break;
            case "SpearFlint":
            case "BowCrude":
                damages.m_pierce = damage;
                break;
        }

        weapon.m_itemData.m_shared.m_damages = damages;
        Logger.LogInfo($"{prefabName} damage set to {damage}.");
    }

    private void OnConfigurationSynchronized(object sender, ConfigurationSynchronizationEventArgs args)
    {
        SetItemWeights();
    }

    private void OnDestroy()
    {
        harmony?.UnpatchSelf();
    }

    // Ensures dungeon lighting overrides stay active across interior transitions.
    [HarmonyPatch(typeof(Player), "FixedUpdate")]
    public static class CryptEntryPatch
    {
        private static void Postfix()
        {
            if (EnvMan.instance != null)
            {
                CryptEnvironmentPatch.Refresh(EnvMan.instance);
            }
        }
    }

    #endregion

    #region Geyser terrain raising

    [HarmonyPatch(typeof(Location), "Awake")]
    public static class GeyserTerrainPatch
    {
        private const string ModifierObjectName = "Hardship_GeyserTerrainModifier";
        private const string PlateObjectName = "Hardship_GeyserBasePlate";
        private const string TarRingObjectName = "Hardship_GeyserTarRing";
        private const string SurtlingCorePrefabName = "SurtlingCore";
        private const float PlateOuterRadius = 12.5f;
        private const float PlateScaleFactor = 21.5f;
        private const float PlateInnerRimRadius = 0.49f;
        private const float PlateMeshOuterRadius = PlateScaleFactor * 0.66f;
        private const float TarPoolRadius = 7.6f;
        private const float TarFlowRadius = PlateScaleFactor * PlateInnerRimRadius;
        private const float TerrainLoweringOffset = -0.5f;
        private const float TarPoolDepth = 0.35f;
        private const int TarSegments = 96;
        private const float LevelRadius = PlateOuterRadius;
        private const float SmoothRadius = 16f;
        private const float WaterClearance = 0.4f;
        private const float TerrainLowering = 0.2f;
        private const float WaterSampleStep = 2.5f;
        private const float RaycastHeight = 500f;
        private const float RaycastDistance = 1000f;
        private const float PlateEmbedDepth = 3f;
        private const float PlateTopHeight = 0.04f;
        private const float PlateBowlDepth = 0.65f;
        private const float PlateTopRimHeight = 0.03f;
        private const int PlateSegments = 48;
        private static readonly int WaterLayerMask = LayerMask.GetMask("Water");
        private static readonly Vector3 PlateScale = new Vector3(PlateScaleFactor, 1f, PlateScaleFactor);
        private static GameObject platePrefab;
        private static bool platePrefabInitialized;
        private static GameObject tarRingPrefab;
        private static bool tarRingPrefabInitialized;

        [HarmonyPostfix]
        private static void OnLocationAwake(Location __instance)
        {
            if (__instance == null
                || !__instance.gameObject.name.StartsWith("FireHole", StringComparison.Ordinal))
            {
                return;
            }

            var geyserPosition = __instance.transform.position;
            var modifierTransform = __instance.transform.Find(ModifierObjectName);
            bool shouldPlaceTarRing = modifierTransform != null;
            Jotunn.Logger.LogInfo(
                $"Hardship: found geyser '{__instance.gameObject.name}' at {geyserPosition}; "
                + $"terrain modifier exists={modifierTransform != null}, water layer mask={WaterLayerMask}.");
            bool hasSampledGround = TryGetGroundHeight(geyserPosition, out float sampledGroundHeight);
            float plateGroundHeight = hasSampledGround ? sampledGroundHeight : geyserPosition.y;

            if (modifierTransform != null)
            {
                plateGroundHeight = modifierTransform.position.y;
                var existingModifier = modifierTransform.GetComponent<TerrainModifier>();
                if (existingModifier != null)
                {
                    ConfigureTerrainModifier(existingModifier);
                    PokeTerrainHeightmaps(existingModifier);
                    shouldPlaceTarRing = true;
                    Jotunn.Logger.LogInfo(
                        $"Hardship: updated existing geyser terrain modifier; "
                        + $"level radius={LevelRadius:F2}m, target offset={TerrainLoweringOffset:F2}m.");
                }
            }

            if (modifierTransform == null)
            {
                bool hasWater = TryGetHighestWaterSurface(geyserPosition, out float waterSurface);
                if (hasWater && waterSurface > geyserPosition.y)
                {
                    float highestTerrain = GetHighestTerrainHeight(geyserPosition, SmoothRadius);
                    float unloweredTargetHeight = Mathf.Max(waterSurface + WaterClearance, highestTerrain);
                    float targetHeight = unloweredTargetHeight - TerrainLowering;

                    var modifierObject = new GameObject(ModifierObjectName);
                    modifierObject.SetActive(false);
                    modifierObject.transform.SetParent(__instance.transform, worldPositionStays: true);
                    modifierObject.transform.position = new Vector3(geyserPosition.x, targetHeight, geyserPosition.z);

                    var modifier = modifierObject.AddComponent<TerrainModifier>();
                    ConfigureTerrainModifier(modifier);

                    modifierObject.SetActive(true);
                    plateGroundHeight = targetHeight;
                    shouldPlaceTarRing = true;
                    Jotunn.Logger.LogInfo(
                        $"Hardship: raised terrain at {__instance.gameObject.name}; "
                        + $"radius={LevelRadius}m, target y={targetHeight:F2}, "
                        + $"lowered from y={unloweredTargetHeight:F2} by {TerrainLowering:F2}m, "
                        + $"additional level offset={TerrainLoweringOffset:F2}m, "
                        + $"water y={waterSurface:F2}, highest ground={highestTerrain:F2}.");
                }
                else
                {
                    Jotunn.Logger.LogInfo(
                        $"Hardship: did not raise terrain at '{__instance.gameObject.name}'; "
                        + $"water found={hasWater}, water y={(hasWater ? waterSurface.ToString("F2") : "<none>")}, "
                        + $"geyser y={geyserPosition.y:F2}.");
                }
            }

            PlaceGeyserEffectsAtBowlFloor(__instance, plateGroundHeight);
            var gasSpawner = __instance.GetComponent<GeyserGasSpawner>();
            if (gasSpawner == null)
            {
                gasSpawner = __instance.gameObject.AddComponent<GeyserGasSpawner>();
            }
            gasSpawner.Configure(plateGroundHeight);

            var coreSpawner = __instance.GetComponent<GeyserCoreSpawner>();
            if (coreSpawner == null)
            {
                coreSpawner = __instance.gameObject.AddComponent<GeyserCoreSpawner>();
            }
            coreSpawner.Configure(plateGroundHeight);

            if (shouldPlaceTarRing)
            {
                PlaceTarRing(__instance, plateGroundHeight);
            }

            if (__instance.transform.Find(PlateObjectName) is Transform existingPlate)
            {
                existingPlate.localScale = PlateScale;
                var existingRenderer = existingPlate.GetComponent<MeshRenderer>();
                var existingCollider = existingPlate.GetComponent<MeshCollider>();
                Jotunn.Logger.LogInfo(
                    $"Hardship: geyser plate already exists at {existingPlate.position}; "
                    + $"sampled ground y={(TryGetGroundHeight(geyserPosition, out float existingGround) ? existingGround.ToString("F2") : "<no heightmap>")}, "
                    + $"renderer bounds={FormatBounds(existingRenderer)}, "
                    + $"collider enabled={existingCollider != null && existingCollider.enabled}.");
                return;
            }

            if (GetPlatePrefab(__instance) is not GameObject prefab)
            {
                Jotunn.Logger.LogError(
                    $"Hardship: could not create a base plate for geyser '{__instance.gameObject.name}'.");
                return;
            }

            if (hasSampledGround && sampledGroundHeight + 0.01f < plateGroundHeight)
            {
                Jotunn.Logger.LogWarning(
                    $"Hardship: using terrain target y={plateGroundHeight:F2} instead of sampled ground "
                    + $"y={sampledGroundHeight:F2} "
                    + $"for geyser '{__instance.gameObject.name}'.");
            }
            else if (!hasSampledGround)
            {
                Jotunn.Logger.LogWarning(
                    $"Hardship: no heightmap found below geyser '{__instance.gameObject.name}' "
                    + $"at {geyserPosition}; placing plate using terrain target y={plateGroundHeight:F2}.");
            }

            var platePosition = new Vector3(geyserPosition.x, plateGroundHeight, geyserPosition.z);
            var plate = UnityEngine.Object.Instantiate(
                prefab,
                platePosition,
                Quaternion.identity,
                __instance.transform);
            plate.name = PlateObjectName;
            plate.transform.localScale = PlateScale;
            plate.SetActive(true);

            var plateRenderer = plate.GetComponent<MeshRenderer>();
            var plateCollider = plate.GetComponent<MeshCollider>();
            Jotunn.Logger.LogInfo(
                $"Hardship: spawned geyser plate '{__instance.gameObject.name}'; "
                + $"plate ground y={plateGroundHeight:F2}, sampled ground y={sampledGroundHeight:F2}, "
                + $"outer radius={PlateMeshOuterRadius:F2}m, "
                + $"rim height={PlateTopHeight:F2}m above ground, "
                + $"bowl depth={PlateBowlDepth:F2}m, "
                + $"tar pool diameter={TarPoolRadius * 2f:F2}m, "
                + $"embed depth={PlateEmbedDepth:F2}m, "
                + $"plate center={plate.transform.position}, "
                + $"world scale={plate.transform.lossyScale}, "
                + $"renderer bounds={FormatBounds(plateRenderer)}, "
                + $"collider enabled={plateCollider != null && plateCollider.enabled}, "
                + $"collider bounds={FormatBounds(plateCollider)}.");
        }

        private static void ConfigureTerrainModifier(TerrainModifier modifier)
        {
            modifier.m_level = true;
            modifier.m_levelRadius = LevelRadius;
            modifier.m_square = false;
            modifier.m_levelOffset = TerrainLoweringOffset;
            modifier.m_smooth = true;
            modifier.m_smoothRadius = SmoothRadius;
            modifier.m_smoothPower = 3f;
            modifier.m_playerModifiction = false;
            modifier.m_paintCleared = false;
        }

        private static void PokeTerrainHeightmaps(TerrainModifier modifier)
        {
            foreach (var heightmap in Heightmap.GetAllHeightmaps())
            {
                if (heightmap != null && heightmap.TerrainVSModifier(modifier))
                {
                    heightmap.Poke(2);
                }
            }
        }

        private static bool TryGetGroundHeight(Vector3 position, out float groundHeight)
        {
            foreach (var heightmap in Heightmap.GetAllHeightmaps())
            {
                if (heightmap != null && heightmap.GetWorldHeight(position, out groundHeight))
                {
                    return true;
                }
            }

            groundHeight = 0f;
            return false;
        }

        private static string FormatBounds(Renderer renderer)
        {
            return renderer != null ? renderer.bounds.ToString() : "<missing>";
        }

        private static string FormatBounds(Collider collider)
        {
            return collider != null ? collider.bounds.ToString() : "<missing>";
        }

        private static void PlaceTarRing(Location geyser, float groundHeight)
        {
            var tarRingTransform = geyser.transform.Find(TarRingObjectName);
            GameObject tarRing;
            if (tarRingTransform != null)
            {
                tarRing = tarRingTransform.gameObject;
            }
            else
            {
                if (GetTarRingPrefab(geyser) is not GameObject prefab)
                {
                    Jotunn.Logger.LogError(
                        $"Hardship: could not create a tar ring for geyser '{geyser.gameObject.name}'.");
                    return;
                }

                tarRing = UnityEngine.Object.Instantiate(
                    prefab,
                    new Vector3(geyser.transform.position.x, groundHeight, geyser.transform.position.z),
                    Quaternion.identity,
                    geyser.transform);
                tarRing.name = TarRingObjectName;
                tarRing.transform.localScale = Vector3.one;
                tarRing.SetActive(true);
            }

            tarRing.transform.position = new Vector3(
                geyser.transform.position.x,
                groundHeight,
                geyser.transform.position.z);
            var tarEffect = tarRing.GetComponent<GeyserTarRingEffect>();
            if (tarEffect == null)
            {
                tarEffect = tarRing.AddComponent<GeyserTarRingEffect>();
            }

            EnsureMudBubbles(tarRing);
            var tarMesh = BuildTarPoolMesh();
            float tarSurfaceHeight = groundHeight + PlateTopHeight;
            tarEffect.Configure(groundHeight, tarSurfaceHeight, tarMesh);
            Jotunn.Logger.LogInfo(
                $"Hardship: placed tar pool in geyser plate '{geyser.gameObject.name}'; "
                + $"pool diameter={TarPoolRadius * 2f:F2}m, flow reaches radius={TarFlowRadius:F2}m, "
                + $"bowl diameter={PlateScale.x * 0.8f:F2}m, bowl depth={PlateBowlDepth:F2}m, "
                + $"tar bottom offset={-PlateBowlDepth + TarPoolDepth:F2}m, "
                + $"surface offset={PlateTopHeight:F2}m, no collider.");
        }

        private static Texture2D mudBubbleTexture;

        private static Texture2D GetMudBubbleTexture()
        {
            if (mudBubbleTexture != null)
            {
                return mudBubbleTexture;
            }

            const int size = 32;
            mudBubbleTexture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = "Hardship_MudBubble",
                hideFlags = HideFlags.HideAndDontSave,
                wrapMode = TextureWrapMode.Clamp
            };
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(size / 2f, size / 2f))
                        / (size / 2f);
                    // Soft ring with a faint filled interior and a highlight, like a mud bubble.
                    float ring = Mathf.Clamp01(1f - Mathf.Abs(d - 0.8f) / 0.2f);
                    float fill = d < 0.8f ? 0.25f : 0f;
                    float a = Mathf.Max(ring, fill);
                    mudBubbleTexture.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                }
            }
            mudBubbleTexture.Apply();
            return mudBubbleTexture;
        }

        private static void EnsureMudBubbles(GameObject tarRing)
        {
            if (tarRing.transform.Find("Hardship_MudBubbles") != null)
            {
                return;
            }

            var bubbleObject = new GameObject("Hardship_MudBubbles");
            bubbleObject.transform.SetParent(tarRing.transform, worldPositionStays: false);
            bubbleObject.transform.localPosition = Vector3.up * (PlateTopHeight + 0.03f);
            bubbleObject.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

            var ps = bubbleObject.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = ps.main;
            main.loop = true;
            main.playOnAwake = true;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.8f, 1.8f);
            main.startSpeed = 0f;
            main.startSize = new ParticleSystem.MinMaxCurve(0.35f, 1.1f);
            main.startColor = new ParticleSystem.MinMaxGradient(
                new Color(0.55f, 0.43f, 0.28f, 0.9f), new Color(0.42f, 0.32f, 0.2f, 0.9f));
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            main.maxParticles = 150;

            var emission = ps.emission;
            emission.rateOverTime = new ParticleSystem.MinMaxCurve(2f, 6f);

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = TarPoolRadius * 0.95f;
            shape.radiusThickness = 1f;

            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(
                new Keyframe(0f, 0.2f), new Keyframe(0.7f, 1f), new Keyframe(0.85f, 1.15f), new Keyframe(1f, 0.3f)));

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            var gradient = new Gradient();
            gradient.SetKeys(
                new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
                new[]
                {
                    new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 0.15f),
                    new GradientAlphaKey(1f, 0.85f), new GradientAlphaKey(0f, 1f)
                });
            colorOverLifetime.color = gradient;

            var renderer = bubbleObject.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.HorizontalBillboard;
            var shader = Shader.Find("Particles/Standard Unlit")
                ?? Shader.Find("Legacy Shaders/Particles/Alpha Blended")
                ?? Shader.Find("Sprites/Default");
            if (shader != null)
            {
                var material = new Material(shader) { name = "Hardship_MudBubbleMaterial" };
                material.mainTexture = GetMudBubbleTexture();
                if (material.HasProperty("_Mode"))
                {
                    material.SetFloat("_Mode", 2f);
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.renderQueue = 3000;
                }
                renderer.sharedMaterial = material;
            }

            ps.Play();
        }

        private static GameObject GetTarRingPrefab(Location geyser)
        {
            if (tarRingPrefabInitialized)
            {
                return tarRingPrefab;
            }

            if (!TryGetPlateMaterial(geyser, out Material sourceMaterial))
            {
                Jotunn.Logger.LogError(
                    $"Hardship: could not find a reusable material for tar around geyser "
                    + $"'{geyser.gameObject.name}'.");
                return null;
            }

            tarRingPrefab = new GameObject(TarRingObjectName);
            tarRingPrefab.SetActive(false);
            tarRingPrefab.hideFlags = HideFlags.HideAndDontSave;
            tarRingPrefab.AddComponent<MeshFilter>();
            var material = new Material(sourceMaterial)
            {
                name = "Hardship_GeyserTarMaterial"
            };
            var tarColor = new Color(0.286f, 0.267f, 0.231f);
            if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", tarColor);
            }
            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", tarColor);
            }
            if (material.HasProperty("_Glossiness"))
            {
                material.SetFloat("_Glossiness", 0.25f);
            }
            if (material.HasProperty("_Metallic"))
            {
                material.SetFloat("_Metallic", 0f);
            }

            tarRingPrefab.AddComponent<MeshRenderer>().sharedMaterial = material;
            tarRingPrefabInitialized = true;
            Jotunn.Logger.LogInfo($"Hardship: created geyser tar ring material from '{sourceMaterial.name}'.");
            return tarRingPrefab;
        }

        private static Mesh BuildTarPoolMesh()
        {
            var mesh = new Mesh { name = "Hardship_GeyserTarPoolMesh" };
            var topRadii = new[] { 0.5f, 2f, 4.5f, 6.3f, 7f, TarPoolRadius - 0.3f, TarPoolRadius, 8.2f, 8.8f, 9.4f, TarFlowRadius };
            int verticesPerRing = TarSegments;
            var vertices = new List<Vector3>(1 + topRadii.Length * verticesPerRing)
            {
                new Vector3(0f, PlateTopHeight, 0f)
            };
            var triangles = new List<int>(TarSegments * (topRadii.Length + 2) * 6);

            for (int ring = 0; ring < topRadii.Length; ring++)
            {
                for (int segment = 0; segment < TarSegments; segment++)
                {
                    float angle = segment / (float)TarSegments * Mathf.PI * 2f;
                    float wave = Mathf.Sin(angle * 3f + ring * 0.7f) * 0.015f
                        + Mathf.Sin(angle * 7f - ring) * 0.008f;
                    float radius = topRadii[ring];
                    float flow = Mathf.SmoothStep(TarPoolRadius - 0.4f, TarFlowRadius, radius);
                    float waveStrength = 1f - flow;
                    vertices.Add(new Vector3(
                        Mathf.Cos(angle) * radius,
                        PlateTopHeight + wave * waveStrength,
                        Mathf.Sin(angle) * radius));
                }
            }

            for (int segment = 0; segment < TarSegments; segment++)
            {
                int nextSegment = (segment + 1) % TarSegments;
                triangles.Add(0);
                triangles.Add(1 + nextSegment);
                triangles.Add(1 + segment);

                for (int ring = 0; ring < topRadii.Length - 1; ring++)
                {
                    int upper = 1 + ring * verticesPerRing;
                    int lower = upper + verticesPerRing;
                    int current = upper + segment;
                    int next = upper + nextSegment;
                    int lowerCurrent = lower + segment;
                    int lowerNext = lower + nextSegment;
                    triangles.Add(current);
                    triangles.Add(next);
                    triangles.Add(lowerNext);
                    triangles.Add(current);
                    triangles.Add(lowerNext);
                    triangles.Add(lowerCurrent);
                }
            }

            float bottomHeight = -PlateBowlDepth + TarPoolDepth;
            int bottomCenter = vertices.Count;
            vertices.Add(new Vector3(0f, bottomHeight, 0f));
            int bottomRing = vertices.Count;
            for (int segment = 0; segment < TarSegments; segment++)
            {
                float angle = segment / (float)TarSegments * Mathf.PI * 2f;
                vertices.Add(new Vector3(
                    Mathf.Cos(angle) * TarFlowRadius,
                    bottomHeight,
                    Mathf.Sin(angle) * TarFlowRadius));
            }

            int topOuterRing = 1 + (topRadii.Length - 1) * verticesPerRing;
            for (int segment = 0; segment < TarSegments; segment++)
            {
                int nextSegment = (segment + 1) % TarSegments;
                int topCurrent = topOuterRing + segment;
                int topNext = topOuterRing + nextSegment;
                int bottomCurrent = bottomRing + segment;
                int bottomNext = bottomRing + nextSegment;

                triangles.Add(topCurrent);
                triangles.Add(topNext);
                triangles.Add(bottomNext);
                triangles.Add(topCurrent);
                triangles.Add(bottomNext);
                triangles.Add(bottomCurrent);

                triangles.Add(bottomCenter);
                triangles.Add(bottomCurrent);
                triangles.Add(bottomNext);
            }

            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private sealed class GeyserTarRingEffect : MonoBehaviour
        {
            private static readonly List<GeyserTarRingEffect> ActiveRings = new List<GeyserTarRingEffect>();
            private static Character affectedCharacter;
            private static GeyserTarRingEffect activeRing;
            private float groundHeight;
            private float movementSurfaceHeight;
            private Mesh ownedMesh;

            private void OnEnable()
            {
                if (!ActiveRings.Contains(this))
                {
                    ActiveRings.Add(this);
                }
            }

            private void OnDisable()
            {
                ActiveRings.Remove(this);
                if (activeRing == this)
                {
                    ClearLiquidLevel();
                }
            }

            public static void UpdatePlayer(Player player)
            {
                GeyserTarRingEffect matchingRing = null;
                var playerPosition = player.transform.position;
                foreach (var ring in ActiveRings)
                {
                    if (ring == null)
                    {
                        continue;
                    }

                    float offsetX = playerPosition.x - ring.transform.position.x;
                    float offsetZ = playerPosition.z - ring.transform.position.z;
                    float radiusSquared = offsetX * offsetX + offsetZ * offsetZ;
                    float poolRadiusSquared = TarPoolRadius * TarPoolRadius;
                    float verticalOffset = playerPosition.y - ring.groundHeight;
                    if (radiusSquared <= poolRadiusSquared
                        && verticalOffset >= -1.5f
                        && verticalOffset <= ring.movementSurfaceHeight - ring.groundHeight + 1.5f)
                    {
                        matchingRing = ring;
                        break;
                    }
                }

                if (affectedCharacter != null && affectedCharacter != player)
                {
                    ClearLiquidLevel();
                }

                if (matchingRing == null)
                {
                    ClearLiquidLevel();
                    return;
                }

                player.SetLiquidLevel(
                    matchingRing.movementSurfaceHeight,
                    LiquidType.Tar,
                    matchingRing);
                player.GetSEMan().AddStatusEffect(SEMan.s_statusEffectTared, true, 0, 0f, -1);
                affectedCharacter = player;
                activeRing = matchingRing;
            }

            private static void ClearLiquidLevel()
            {
                if (affectedCharacter != null && activeRing != null)
                {
                    affectedCharacter.SetLiquidLevel(-10000f, LiquidType.Tar, activeRing);
                }

                affectedCharacter = null;
                activeRing = null;
            }

            public void Configure(float terrainHeight, float liquidSurfaceHeight, Mesh mesh)
            {
                groundHeight = terrainHeight;
                movementSurfaceHeight = liquidSurfaceHeight;
                var meshFilter = GetComponent<MeshFilter>();
                if (ownedMesh != null)
                {
                    UnityEngine.Object.Destroy(ownedMesh);
                }

                ownedMesh = mesh;
                meshFilter.sharedMesh = ownedMesh;
            }

            private void OnDestroy()
            {
                if (ownedMesh != null)
                {
                    UnityEngine.Object.Destroy(ownedMesh);
                    ownedMesh = null;
                }
            }
        }

        [HarmonyPatch(typeof(Player), "FixedUpdate")]
        private static class GeyserTarRingPlayerPatch
        {
            [HarmonyPostfix]
            private static void ApplyGeyserTarToLocalPlayer(Player __instance)
            {
                if (__instance == Player.m_localPlayer)
                {
                    GeyserTarRingEffect.UpdatePlayer(__instance);
                }
            }
        }

        private static void PlaceGeyserEffectsAtBowlFloor(Location geyser, float plateGroundHeight)
        {
            var placedTransforms = new HashSet<Transform>();
            int particleSystemCount = 0;
            foreach (var particleSystem in geyser.GetComponentsInChildren<ParticleSystem>(true))
            {
                particleSystemCount++;
                var effectTransform = particleSystem.transform;
                if (effectTransform == geyser.transform)
                {
                    continue;
                }

                bool hasParticleSystemAncestor = false;
                for (var ancestor = effectTransform.parent;
                    ancestor != null && ancestor != geyser.transform;
                    ancestor = ancestor.parent)
                {
                    if (ancestor.GetComponent<ParticleSystem>() != null)
                    {
                        hasParticleSystemAncestor = true;
                        break;
                    }
                }

                if (hasParticleSystemAncestor || !placedTransforms.Add(effectTransform))
                {
                    continue;
                }

                var previousPosition = effectTransform.position;
                effectTransform.position = new Vector3(
                    previousPosition.x,
                    plateGroundHeight - PlateBowlDepth,
                    previousPosition.z);
                Jotunn.Logger.LogInfo(
                    $"Hardship: positioned geyser particle effect '{effectTransform.name}' "
                    + $"at bowl floor y={effectTransform.position.y:F2} "
                    + $"(plate ground y={plateGroundHeight:F2}, depth={PlateBowlDepth:F2}m).");
            }

            if (placedTransforms.Count == 0)
            {
                Jotunn.Logger.LogWarning(
                    $"Hardship: found {particleSystemCount} particle system(s) on geyser "
                    + $"'{geyser.gameObject.name}' but no child effect transform to place at the bowl floor.");
            }
        }

        private sealed class GeyserGasSpawner : MonoBehaviour
        {
            private const string GasCloudRpcName = "Hardship_GeyserGasCloud";
            private static readonly Dictionary<long, Dictionary<Vector3, double>> NextCloudTimesByWorld =
                new Dictionary<long, Dictionary<Vector3, double>>();
            private static bool rpcRegistered;
            private float groundHeight;
            private bool configured;

            public void Configure(float terrainHeight)
            {
                groundHeight = terrainHeight;
                configured = true;
            }

            private void Update()
            {
                EnsureRpcRegistered();
                if (!configured || ZNet.instance == null || !ZNet.instance.IsServer())
                {
                    return;
                }

                long worldUid = ZNet.instance.GetWorldUID();
                double now = ZNet.instance.GetTimeSeconds();
                if (!NextCloudTimesByWorld.TryGetValue(worldUid, out var worldCloudTimes))
                {
                    worldCloudTimes = new Dictionary<Vector3, double>();
                    NextCloudTimesByWorld.Add(worldUid, worldCloudTimes);
                }

                Vector3 geyserPosition = transform.position;
                if (!worldCloudTimes.TryGetValue(geyserPosition, out double nextCloudTime))
                {
                    nextCloudTime = now + GetRandomInterval();
                    worldCloudTimes.Add(geyserPosition, nextCloudTime);
                }

                if (now < nextCloudTime || !rpcRegistered)
                {
                    return;
                }

                var cloudOrigin = new Vector3(
                    geyserPosition.x,
                    groundHeight - PlateBowlDepth + 0.5f,
                    geyserPosition.z);
                ZRoutedRpc.instance.InvokeRoutedRPC(
                    ZRoutedRpc.Everybody,
                    GasCloudRpcName,
                    cloudOrigin,
                    GeyserGasCloudRadius.Value,
                    GeyserGasCloudDuration.Value);

                nextCloudTime = now + GetRandomInterval();
                worldCloudTimes[geyserPosition] = nextCloudTime;
                Jotunn.Logger.LogInfo(
                    $"Hardship: geyser at {geyserPosition} released a suffocating gas cloud; "
                    + $"next eruption in {nextCloudTime - now:F1} seconds.");
            }

            private static double GetRandomInterval()
            {
                float minimum = Mathf.Min(
                    GeyserGasMinimumIntervalSeconds.Value,
                    GeyserGasMaximumIntervalSeconds.Value);
                float maximum = Mathf.Max(
                    GeyserGasMinimumIntervalSeconds.Value,
                    GeyserGasMaximumIntervalSeconds.Value);
                return UnityEngine.Random.Range(minimum, maximum);
            }

            private static void EnsureRpcRegistered()
            {
                if (rpcRegistered || ZRoutedRpc.instance == null)
                {
                    return;
                }

                ZRoutedRpc.instance.Register<Vector3, float, float>(
                    GasCloudRpcName,
                    OnGasCloudRpc);
                rpcRegistered = true;
                Jotunn.Logger.LogInfo("Hardship: geyser gas RPC registered.");
            }

            private static void OnGasCloudRpc(long sender, Vector3 position, float radius, float duration)
            {
                GeyserGasCloudEffect.Spawn(position, radius, duration);
            }

            private sealed class GeyserGasCloudEffect : MonoBehaviour
            {
                private static readonly List<GeyserGasCloudEffect> ActiveClouds =
                    new List<GeyserGasCloudEffect>();
                private static float staminaExhaustedTime;
                private static float nextDrowningDamageTime;
                private static readonly string[] GasParticleShaderNames =
                {
                    "Particles/Standard Unlit",
                    "Legacy Shaders/Particles/Alpha Blended",
                    "Sprites/Default"
                };
                private Vector3 cloudPosition;
                private float radius;
                private float duration;
                private float elapsed;
                private Material particleMaterial;

                public static void Spawn(Vector3 position, float cloudRadius, float cloudDuration)
                {
                    var cloud = new GameObject("Hardship_GeyserSuffocatingGas");
                    cloud.transform.position = position;
                    var effect = cloud.AddComponent<GeyserGasCloudEffect>();
                    effect.Configure(position, cloudRadius, cloudDuration);
                }

                private void OnEnable()
                {
                    if (!ActiveClouds.Contains(this))
                    {
                        ActiveClouds.Add(this);
                    }
                }

                private void OnDisable()
                {
                    ActiveClouds.Remove(this);
                }

                private void Configure(Vector3 position, float cloudRadius, float cloudDuration)
                {
                    cloudPosition = position;
                    radius = cloudRadius;
                    duration = cloudDuration;
                    CreateVisibleCloud();
                }

                private void Update()
                {
                    elapsed += Time.deltaTime;
                    if (elapsed >= duration)
                    {
                        Destroy(gameObject);
                    }
                }

                private void OnDestroy()
                {
                    if (particleMaterial != null)
                    {
                        Destroy(particleMaterial);
                    }
                }

                private void CreateVisibleCloud()
                {
                    var particleObject = new GameObject("GasParticles");
                    particleObject.transform.SetParent(transform, worldPositionStays: false);
                    var particleSystem = particleObject.AddComponent<ParticleSystem>();
                    var main = particleSystem.main;
                    main.duration = duration;
                    main.loop = false;
                    main.startLifetime = new ParticleSystem.MinMaxCurve(5f, 9f);
                    main.startSpeed = new ParticleSystem.MinMaxCurve(0.7f, 1.7f);
                    main.startSize = new ParticleSystem.MinMaxCurve(1.5f, 3f);
                    main.startColor = new Color(0.55f, 0.7f, 0.28f, 0.65f);
                    main.maxParticles = 160;
                    main.simulationSpace = ParticleSystemSimulationSpace.World;

                    var emission = particleSystem.emission;
                    emission.rateOverTime = 10f;
                    emission.SetBursts(new[]
                    {
                        new ParticleSystem.Burst(0f, 28),
                        new ParticleSystem.Burst(2f, 18),
                        new ParticleSystem.Burst(4f, 18)
                    });

                    var shape = particleSystem.shape;
                    shape.shapeType = ParticleSystemShapeType.Sphere;
                    shape.radius = 0.55f;
                    shape.position = Vector3.up * 0.5f;
                    shape.randomDirectionAmount = 0.8f;

                    var colorOverLifetime = particleSystem.colorOverLifetime;
                    colorOverLifetime.enabled = true;
                    var alpha = new Gradient();
                    alpha.SetKeys(
                        new[]
                        {
                            new GradientColorKey(new Color(0.62f, 0.76f, 0.3f), 0f),
                            new GradientColorKey(new Color(0.35f, 0.54f, 0.18f), 1f)
                        },
                        new[]
                        {
                            new GradientAlphaKey(0f, 0f),
                            new GradientAlphaKey(0.55f, 0.15f),
                            new GradientAlphaKey(0.4f, 0.75f),
                            new GradientAlphaKey(0f, 1f)
                        });
                    colorOverLifetime.color = alpha;

                    var particleRenderer = particleObject.GetComponent<ParticleSystemRenderer>();
                    particleRenderer.renderMode = ParticleSystemRenderMode.Billboard;
                    var shader = FindGasParticleShader();
                    if (shader != null)
                    {
                        particleMaterial = new Material(shader)
                        {
                            name = "Hardship_GeyserGasMaterial",
                            color = new Color(0.55f, 0.7f, 0.28f, 0.65f)
                        };
                        particleRenderer.sharedMaterial = particleMaterial;
                    }

                    particleSystem.Play();
                }

                private static Shader FindGasParticleShader()
                {
                    foreach (var shaderName in GasParticleShaderNames)
                    {
                        var shader = Shader.Find(shaderName);
                        if (shader != null)
                        {
                            return shader;
                        }
                    }

                    Jotunn.Logger.LogError(
                        "Hardship: could not find a particle shader for the geyser gas cloud.");
                    return null;
                }

                private static void ApplyToLocalPlayer(Player player)
                {
                    if (player == null || player.IsDead())
                    {
                        staminaExhaustedTime = 0f;
                        nextDrowningDamageTime = 0f;
                        return;
                    }

                    bool exposed = false;
                    foreach (var cloud in ActiveClouds)
                    {
                        if (cloud == null)
                        {
                            continue;
                        }

                        var offset = player.transform.position - cloud.cloudPosition;
                        if (offset.sqrMagnitude <= cloud.radius * cloud.radius)
                        {
                            exposed = true;
                            break;
                        }
                    }

                    if (!exposed)
                    {
                        staminaExhaustedTime = 0f;
                        nextDrowningDamageTime = 0f;
                        return;
                    }

                    player.UseStamina(GeyserGasStaminaDrainPerSecond.Value * Time.deltaTime);
                    if (player.HaveStamina())
                    {
                        staminaExhaustedTime = 0f;
                        nextDrowningDamageTime = 0f;
                        return;
                    }

                    staminaExhaustedTime += Time.deltaTime;
                    if (staminaExhaustedTime < 1f || staminaExhaustedTime < nextDrowningDamageTime)
                    {
                        return;
                    }

                    nextDrowningDamageTime = staminaExhaustedTime + 1f;
                    var hit = new HitData
                    {
                        m_damage = new HitData.DamageTypes
                        {
                            m_damage = Mathf.Ceil(player.GetMaxHealth() / 20f)
                        },
                        m_point = player.GetCenterPoint(),
                        m_dir = Vector3.down,
                        m_pushForce = 10f,
                        m_hitType = HitData.HitType.Drowning
                    };
                    player.Damage(hit);
                }

                [HarmonyPatch(typeof(Player), "FixedUpdate")]
                private static class GeyserGasPlayerPatch
                {
                    [HarmonyPostfix]
                    private static void ApplyGeyserGasToLocalPlayer(Player __instance)
                    {
                        if (__instance == Player.m_localPlayer)
                        {
                            ApplyToLocalPlayer(__instance);
                        }
                    }
                }
            }
        }

        private sealed class GeyserCoreSpawner : MonoBehaviour
        {
            private const double MinimumSpawnIntervalSeconds = 120d * 60d;
            private const double MaximumSpawnIntervalSeconds = 720d * 60d;
            private const double ExistingCoreCheckIntervalSeconds = 30d;
            private const double SpawnRetryIntervalSeconds = 60d;
            private const float CoreSearchRadius = PlateMeshOuterRadius;
            private static readonly Dictionary<long, Dictionary<Vector3, double>> NextSpawnTimesByWorld =
                new Dictionary<long, Dictionary<Vector3, double>>();
            private float groundHeight;
            private double nextCheckTime;
            private bool configured;
            private bool waitingForExistingCore;

            public void Configure(float terrainHeight)
            {
                groundHeight = terrainHeight;
                configured = true;
            }

            private void Update()
            {
                if (!configured || ZNet.instance == null || !ZNet.instance.IsServer())
                {
                    return;
                }

                long worldUid = ZNet.instance.GetWorldUID();
                double now = ZNet.instance.GetTimeSeconds();
                if (!NextSpawnTimesByWorld.TryGetValue(worldUid, out var worldSpawnTimes))
                {
                    worldSpawnTimes = new Dictionary<Vector3, double>();
                    NextSpawnTimesByWorld.Add(worldUid, worldSpawnTimes);
                }

                Vector3 geyserPosition = transform.position;
                if (!worldSpawnTimes.TryGetValue(geyserPosition, out double nextSpawnTime))
                {
                    nextSpawnTime = now + GetRandomSpawnInterval();
                    worldSpawnTimes.Add(geyserPosition, nextSpawnTime);
                    Jotunn.Logger.LogInfo(
                        $"Hardship: geyser at {geyserPosition} will first spawn a Surtling Core "
                        + $"in {(nextSpawnTime - now) / 60d:F1} minutes.");
                    return;
                }

                if (now < nextSpawnTime || now < nextCheckTime)
                {
                    return;
                }

                nextCheckTime = now + ExistingCoreCheckIntervalSeconds;
                if (HasNearbySurtlingCore(geyserPosition))
                {
                    if (!waitingForExistingCore)
                    {
                        Jotunn.Logger.LogInfo(
                            $"Hardship: geyser at {geyserPosition} already has a Surtling Core; "
                            + "waiting for it to be collected before spawning another.");
                        waitingForExistingCore = true;
                    }
                    return;
                }

                waitingForExistingCore = false;
                var corePrefab = ZNetScene.instance != null
                    ? ZNetScene.instance.GetPrefab(SurtlingCorePrefabName)
                    : null;
                if (corePrefab == null || corePrefab.GetComponent<ItemDrop>() == null)
                {
                    Jotunn.Logger.LogError(
                        $"Hardship: could not find the '{SurtlingCorePrefabName}' item prefab "
                        + $"for geyser at {geyserPosition}; will retry in {SpawnRetryIntervalSeconds:F0} seconds.");
                    nextCheckTime = now + SpawnRetryIntervalSeconds;
                    return;
                }

                if (corePrefab.GetComponent<ZNetView>() == null)
                {
                    Jotunn.Logger.LogError(
                        $"Hardship: prefab '{SurtlingCorePrefabName}' has no ZNetView; "
                        + $"cannot network-spawn a geyser core at {geyserPosition}.");
                    nextCheckTime = now + SpawnRetryIntervalSeconds;
                    return;
                }

                var spawnPosition = new Vector3(
                    geyserPosition.x,
                    groundHeight - PlateBowlDepth + 0.2f,
                    geyserPosition.z);
                UnityEngine.Object.Instantiate(corePrefab, spawnPosition, Quaternion.identity);
                nextSpawnTime = now + GetRandomSpawnInterval();
                worldSpawnTimes[geyserPosition] = nextSpawnTime;
                Jotunn.Logger.LogInfo(
                    $"Hardship: spawned a Surtling Core at geyser {geyserPosition}; "
                    + $"next spawn attempt in {(nextSpawnTime - now) / 60d:F1} minutes.");
            }

            private static double GetRandomSpawnInterval()
            {
                return UnityEngine.Random.Range(
                    (float)MinimumSpawnIntervalSeconds,
                    (float)MaximumSpawnIntervalSeconds);
            }

            private static bool HasNearbySurtlingCore(Vector3 geyserPosition)
            {
                float searchRadiusSquared = CoreSearchRadius * CoreSearchRadius;
                foreach (var itemDrop in UnityEngine.Object.FindObjectsByType<ItemDrop>(FindObjectsSortMode.None))
                {
                    if (itemDrop == null
                        || !itemDrop.gameObject.scene.IsValid()
                        || !itemDrop.gameObject.name.StartsWith(SurtlingCorePrefabName, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    var offset = itemDrop.transform.position - geyserPosition;
                    if (offset.x * offset.x + offset.z * offset.z <= searchRadiusSquared
                        && Mathf.Abs(offset.y) <= CoreSearchRadius)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private static GameObject GetPlatePrefab(Location geyser)
        {
            if (platePrefabInitialized)
            {
                return platePrefab;
            }

            if (!TryGetPlateMaterial(geyser, out Material sourceMaterial))
            {
                Jotunn.Logger.LogError(
                    $"Hardship: could not find a reusable material for geyser '{geyser.gameObject.name}'. "
                    + "Checked geyser renderers, terrain renderers, and loaded mesh renderers.");
                return null;
            }

            var mesh = BuildStonePlateMesh();

            platePrefab = new GameObject(PlateObjectName);
            platePrefab.SetActive(false);
            platePrefab.hideFlags = HideFlags.HideAndDontSave;
            platePrefab.transform.localScale = PlateScale;
            platePrefab.AddComponent<MeshFilter>().sharedMesh = mesh;
            var material = new Material(sourceMaterial)
            {
                name = "Hardship_GeyserBasePlateMaterial"
            };
            var stoneColor = new Color(0.34f, 0.31f, 0.27f);
            if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", stoneColor);
            }
            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", stoneColor);
            }
            platePrefab.AddComponent<MeshRenderer>().sharedMaterial = material;
            platePrefab.AddComponent<MeshCollider>().sharedMesh = mesh;
            platePrefabInitialized = true;
            Jotunn.Logger.LogInfo(
                $"Hardship: created geyser plate mesh; vertices={mesh.vertexCount}, "
                + $"mesh bounds={mesh.bounds}, material='{sourceMaterial.name}', "
                + $"shader='{sourceMaterial.shader.name}', scale={PlateScale}, "
                + $"rim height={PlateTopHeight:F2}m, bowl depth={PlateBowlDepth:F2}m, "
                + $"tar pool diameter={TarPoolRadius * 2f:F2}m, "
                + $"embed depth={PlateEmbedDepth:F2}m.");

            return platePrefab;
        }

        private static bool TryGetPlateMaterial(Location geyser, out Material material)
        {
            foreach (var renderer in geyser.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (TryGetFirstMaterial(renderer, out material))
                {
                    Jotunn.Logger.LogInfo(
                        $"Hardship: using geyser material '{material.name}' "
                        + $"(shader '{material.shader.name}') for its base plate.");
                    return true;
                }
            }

            foreach (var renderer in UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))
            {
                if (renderer == null || renderer.GetComponentInParent<Heightmap>() != null)
                {
                    continue;
                }

                if (TryGetFirstMaterial(renderer, out material))
                {
                    Jotunn.Logger.LogInfo(
                        $"Hardship: using scene material '{material.name}' "
                        + $"(shader '{material.shader.name}') for geyser plate.");
                    return true;
                }
            }

            material = null;
            return false;
        }

        private static bool TryGetFirstMaterial(Renderer renderer, out Material material)
        {
            if (renderer != null)
            {
                foreach (var candidate in renderer.sharedMaterials)
                {
                    if (candidate != null && candidate.shader != null)
                    {
                        material = candidate;
                        return true;
                    }
                }
            }

            material = null;
            return false;
        }

        private static Mesh BuildStonePlateMesh()
        {
            var mesh = new Mesh { name = "Hardship_GeyserBasePlateMesh" };
            var ringRadii = new[] { 0.02f, 0.08f, 0.16f, 0.24f, 0.32f, 0.40f, 0.43f, 0.46f, 0.49f, 0.515f, 0.54f, 0.56f, 0.58f, 0.60f, 0.62f, 0.64f, 0.66f, 0.62f, 0.55f, 0.50f, 0.47f, 0.02f };
            var ringHeights = new[]
            {
                -PlateBowlDepth,
                -PlateBowlDepth,
                -PlateBowlDepth,
                -PlateBowlDepth,
                -PlateBowlDepth,
                -PlateBowlDepth,
                -PlateBowlDepth * 0.675f,
                -PlateBowlDepth * 0.325f,
                PlateTopHeight,
                -0.08f,
                -0.22f,
                -0.36f,
                TerrainLoweringOffset,
                -0.7f,
                -1.2f,
                -2f,
                -PlateEmbedDepth,
                -PlateEmbedDepth,
                -PlateEmbedDepth,
                -PlateEmbedDepth,
                -PlateEmbedDepth,
                -PlateEmbedDepth
            };
            var triangles = new List<int>(PlateSegments * 6 * ringRadii.Length + PlateSegments * 3);
            var ringStarts = new int[ringRadii.Length];

            int centerIndex = 0;
            var vertices = new List<Vector3>(PlateSegments * ringRadii.Length + 1)
            {
                new Vector3(0f, -PlateBowlDepth, 0f)
            };

            for (int ring = 0; ring < ringRadii.Length; ring++)
            {
                ringStarts[ring] = vertices.Count;
                for (int segment = 0; segment < PlateSegments; segment++)
                {
                    float angle = segment / (float)PlateSegments * Mathf.PI * 2f;
                    float noise = Mathf.PerlinNoise(
                        Mathf.Cos(angle) * 4f + ring * 19f + 13f,
                        Mathf.Sin(angle) * 4f + ring * 23f + 29f) - 0.5f;
                    float radiusNoise = ring < 2
                        || ring >= ringRadii.Length - 4
                        || ringRadii[ring] == PlateInnerRimRadius
                        ? 0f
                        : 0.012f;
                    float radius = ringRadii[ring] + noise * radiusNoise;

                    vertices.Add(new Vector3(
                        Mathf.Cos(angle) * radius,
                        ringHeights[ring],
                        Mathf.Sin(angle) * radius));
                }
            }

            for (int segment = 0; segment < PlateSegments; segment++)
            {
                int current = segment;
                int next = (segment + 1) % PlateSegments;

                triangles.Add(centerIndex);
                triangles.Add(ringStarts[0] + next);
                triangles.Add(ringStarts[0] + current);

                for (int ring = 0; ring < ringStarts.Length; ring++)
                {
                    int nextRing = (ring + 1) % ringStarts.Length;
                    int upperCurrent = ringStarts[ring] + current;
                    int upperNext = ringStarts[ring] + next;
                    int lowerCurrent = ringStarts[nextRing] + current;
                    int lowerNext = ringStarts[nextRing] + next;

                    triangles.Add(lowerCurrent);
                    triangles.Add(upperCurrent);
                    triangles.Add(upperNext);
                    triangles.Add(lowerCurrent);
                    triangles.Add(upperNext);
                    triangles.Add(lowerNext);
                }
            }

            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private static bool TryGetHighestWaterSurface(Vector3 center, out float highestWaterSurface)
        {
            highestWaterSurface = float.NegativeInfinity;
            if (WaterLayerMask == 0)
            {
                return false;
            }

            for (float z = -LevelRadius; z <= LevelRadius; z += WaterSampleStep)
            {
                for (float x = -LevelRadius; x <= LevelRadius; x += WaterSampleStep)
                {
                    if (x * x + z * z > LevelRadius * LevelRadius)
                    {
                        continue;
                    }

                    var samplePosition = center + new Vector3(x, 0f, z);
                    var rayOrigin = samplePosition + Vector3.up * RaycastHeight;
                    if (Physics.Raycast(
                        rayOrigin,
                        Vector3.down,
                        out RaycastHit hit,
                        RaycastDistance,
                        WaterLayerMask,
                        QueryTriggerInteraction.Collide))
                    {
                        highestWaterSurface = Mathf.Max(highestWaterSurface, hit.point.y);
                    }
                }
            }

            return !float.IsNegativeInfinity(highestWaterSurface);
        }

        private static float GetHighestTerrainHeight(Vector3 center, float radius)
        {
            float highestTerrain = center.y;
            float radiusSquared = radius * radius;

            foreach (var heightmap in Heightmap.GetAllHeightmaps())
            {
                if (heightmap == null)
                {
                    continue;
                }

                float halfSize = heightmap.m_width * heightmap.m_scale * 0.5f;
                for (int z = 0; z <= heightmap.m_width; z++)
                {
                    for (int x = 0; x <= heightmap.m_width; x++)
                    {
                        var samplePosition = heightmap.transform.position + new Vector3(
                            -halfSize + x * heightmap.m_scale,
                            0f,
                            -halfSize + z * heightmap.m_scale);
                        float offsetX = samplePosition.x - center.x;
                        float offsetZ = samplePosition.z - center.z;
                        if (offsetX * offsetX + offsetZ * offsetZ > radiusSquared
                            || !heightmap.GetWorldHeight(samplePosition, out float terrainHeight))
                        {
                            continue;
                        }

                        highestTerrain = Mathf.Max(highestTerrain, terrainHeight);
                    }
                }
            }

            return highestTerrain;
        }
    }

    #endregion

    #region Raid loot

    private const string RaidSpawnZdoKey = "Hardship_RaidSpawn";

    [HarmonyPatch(typeof(MonsterAI), nameof(MonsterAI.SetEventCreature))]
    public static class RaidCreaturePatch
    {
        private static void Postfix(MonsterAI __instance, bool despawn)
        {
            if (!despawn)
            {
                return;
            }

            var networkView = __instance?.GetComponent<ZNetView>();
            if (networkView == null || !networkView.IsValid())
            {
                return;
            }

            networkView.GetZDO().Set(RaidSpawnZdoKey, true);
        }
    }

    [HarmonyPatch(typeof(CharacterDrop), nameof(CharacterDrop.GenerateDropList))]
    public static class RaidLootPatch
    {
        private static bool Prefix(
            CharacterDrop __instance,
            ref List<KeyValuePair<GameObject, int>> __result)
        {
            if (BlockRaidDrops == null || !BlockRaidDrops.Value || __instance == null)
            {
                return true;
            }

            var networkView = __instance.GetComponent<ZNetView>();
            bool taggedAsRaidSpawn = networkView != null
                && networkView.IsValid()
                && networkView.GetZDO().GetBool(RaidSpawnZdoKey, false);

            var monsterAI = __instance.GetComponent<MonsterAI>();
            bool markedAsEventCreature = monsterAI != null && monsterAI.IsEventCreature();
            if (!taggedAsRaidSpawn && !markedAsEventCreature)
            {
                return true;
            }

            __result = new List<KeyValuePair<GameObject, int>>();
            return false;
        }

        private static void Postfix(
            CharacterDrop __instance,
            ref List<KeyValuePair<GameObject, int>> __result)
        {
            if (__instance == null
                || !__instance.gameObject.name.StartsWith("Surtling", StringComparison.Ordinal)
                || __result == null)
            {
                return;
            }

            __result.RemoveAll(drop => drop.Key != null && drop.Key.name == "SurtlingCore");
        }
    }

    #endregion

    #region Storm ship damage

    [HarmonyPatch(typeof(Ship), "CustomFixedUpdate")]
    public static class StormShipDamagePatch
    {
        private static readonly Dictionary<int, float> nextDamageTime = new();

        private static void Postfix(Ship __instance)
        {
            if (__instance == null
                || StormShipDamageEnabled == null
                || !StormShipDamageEnabled.Value
                || StormShipDamagePerSecond == null
                || StormShipDamagePerSecond.Value <= 0f
                || EnvMan.instance == null)
            {
                return;
            }

            var windStrength = EnvMan.instance.GetWindForce().magnitude;

            var networkView = __instance.GetComponent<ZNetView>();
            var wearNTear = __instance.GetComponent<WearNTear>();
            if (networkView == null
                || wearNTear == null
                || !networkView.IsValid()
                || !networkView.IsOwner()
                || windStrength < StormWindThreshold.Value)
            {
                return;
            }

            int shipId = __instance.GetInstanceID();
            if (nextDamageTime.TryGetValue(shipId, out float nextTime)
                && Time.time < nextTime)
            {
                return;
            }

            if (StormShallowWaterDepth.Value > 0f && IsInShallowWater(__instance))
            {
                return;
            }

            nextDamageTime[shipId] = Time.time + 1f;

            float stormProgress = StormWindThreshold.Value >= 1f
                ? 1f
                : Mathf.Clamp01((windStrength - StormWindThreshold.Value)
                    / (1f - StormWindThreshold.Value));
            float damageMultiplier = Mathf.Lerp(
                1f,
                StormMaxDamageMultiplier.Value,
                stormProgress);

            var hit = new HitData
            {
                m_damage = new HitData.DamageTypes
                {
                    m_blunt = StormShipDamagePerSecond.Value * damageMultiplier
                },
                m_point = __instance.transform.position + __instance.transform.right * 1.5f,
                m_dir = Vector3.up,
                m_dodgeable = false
            };

            wearNTear.Damage(hit);

            if (GameCamera.instance != null
                && StormShakeStrength.Value > 0f
                && StormShakeRange.Value > 0f)
            {
                GameCamera.instance.AddShake(
                    __instance.transform.position,
                    StormShakeRange.Value,
                    StormShakeStrength.Value,
                    false);
            }
        }

        private static bool IsInShallowWater(Ship ship)
        {
            if (ship == null || StormShallowWaterDepth.Value <= 0f)
            {
                return false;
            }

            var heightmap = Heightmap.FindHeightmap(ship.transform.position);
            if (heightmap == null)
            {
                return false;
            }

            float oceanDepth = heightmap.GetOceanDepth(ship.transform.position);
            float shallowThreshold = StormShallowWaterDepth.Value + 1.5f;
            return oceanDepth >= 0f && oceanDepth <= shallowThreshold;
        }
    }

    #endregion

    #region Lightning strikes

    // Rolls a chance to strike the local player with lightning during thunderstorms, unless sheltered.
    [HarmonyPatch]
    public static class LightningStrikePatch
    {
        private const string RpcName = "Hardship_LightningStrike";

        // Anything solid overhead counts as a roof; character-related layers are excluded so players don't shield each other.
        private static readonly int RoofRaycastMask = ~LayerMask.GetMask("Character", "character_trigger", "character_noenv", "Default_small");

        private static float nextCheckTime;
        private static float nextStrikeAllowedTime;
        private static bool rpcRegistered;

        // ZRoutedRpc.instance does not exist during plugin Awake, so register lazily once it appears.
        private static void EnsureRpcRegistered()
        {
            if (rpcRegistered || ZRoutedRpc.instance == null)
            {
                return;
            }

            ZRoutedRpc.instance.Register<Vector3, string>(RpcName, OnLightningStrikeRpc);
            rpcRegistered = true;
            Jotunn.Logger.LogInfo("Hardship: lightning RPC registered.");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Player), "FixedUpdate")]
        private static void OnPlayerFixedUpdate()
        {
            EnsureRpcRegistered();

            var player = Player.m_localPlayer;
            if (player == null || LightningEnabled == null || !LightningEnabled.Value)
            {
                return;
            }

            if (Time.time < nextCheckTime)
            {
                return;
            }

            nextCheckTime = Time.time + Mathf.Max(0.1f, LightningCheckInterval.Value);

            if (Time.time < nextStrikeAllowedTime)
            {
                return;
            }

            if (!IsThunderstorm() || HasRoofOverhead(player))
            {
                return;
            }

            bool onShip = player.GetComponentInParent<Ship>() != null;
            float chance = onShip ? LightningShipChance.Value : LightningLandChance.Value;
            if (UnityEngine.Random.Range(0f, 100f) >= chance)
            {
                return;
            }

            StrikePlayer(player);
        }

        private static bool IsThunderstorm()
        {
            var environment = EnvMan.instance?.GetCurrentEnvironment();
            if (environment == null || string.IsNullOrEmpty(LightningWeatherNames.Value))
            {
                return false;
            }

            return LightningWeatherNames.Value
                .Split(',')
                .Select(name => name.Trim())
                .Any(name => string.Equals(name, environment.m_name, StringComparison.OrdinalIgnoreCase));
        }

        private static bool HasRoofOverhead(Player player)
        {
            var origin = player.transform.position + Vector3.up * 0.5f;
            return Physics.Raycast(origin, Vector3.up, 300f, RoofRaycastMask);
        }

        private static void StrikePlayer(Player player)
        {
            nextStrikeAllowedTime = Time.time + Mathf.Max(0f, LightningCooldownSeconds.Value);

            player.SetHealth(10f);
            player.Message(MessageHud.MessageType.Center, "Thor's wrath has struck you down!");
            player.StartCoroutine(ClearCenterMessageAfter(3f));

            Jotunn.Logger.LogInfo($"Hardship: lightning struck {player.GetPlayerName()} at {player.transform.position}, sending RPC.");
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, RpcName, player.transform.position, player.GetPlayerName());
        }

        // Center messages otherwise stay up for the game's default duration; cut it short instead.
        private static IEnumerator ClearCenterMessageAfter(float delay)
        {
            yield return new WaitForSeconds(delay);
            MessageHud.instance?.ShowMessage(MessageHud.MessageType.Center, string.Empty);
        }

        private static void OnLightningStrikeRpc(long sender, Vector3 position, string playerName)
        {
            Jotunn.Logger.LogInfo($"Hardship: lightning RPC received for {playerName} at {position}.");
            SpawnLightningVisual(position);

            // The struck player already gets a center message locally; only notify everyone else.
            if (Player.m_localPlayer != null && !string.Equals(Player.m_localPlayer.GetPlayerName(), playerName, StringComparison.Ordinal))
            {
                MessageHud.instance?.ShowMessage(MessageHud.MessageType.TopLeft, $"Thor struck {playerName} down!");
            }
        }

        // Built purely from engine primitives so the effect never depends on guessing a game asset name.
        private static void SpawnLightningVisual(Vector3 position)
        {
            var boltObject = new GameObject("Hardship_LightningBolt");
            boltObject.transform.position = position;

            var flash = boltObject.AddComponent<Light>();
            flash.type = LightType.Point;
            flash.color = new Color(0.75f, 0.85f, 1f);
            flash.intensity = 6f;
            flash.range = 20f;
            flash.shadows = LightShadows.None;

            var shader = ResolveBoltShader();
            var points = GenerateBoltPoints(position + Vector3.up * 40f, position);

            // A wide, faint glow plus a thin bright core reads as a lightning bolt rather than a straight laser line.
            CreateBoltLine(boltObject, "Glow", points, shader, 0.6f, new Color(0.6f, 0.8f, 1f, 0.35f));
            CreateBoltLine(boltObject, "Core", points, shader, 0.12f, Color.white);

            boltObject.AddComponent<LightningFlashEffect>();

            if (GameCamera.instance != null)
            {
                GameCamera.instance.AddShake(position, 15f, 0.3f, false);
            }
        }

        // Zigzags the bolt between the sky and the impact point, tapering the jitter near both ends.
        private static Vector3[] GenerateBoltPoints(Vector3 start, Vector3 end)
        {
            const int segments = 10;
            var points = new Vector3[segments + 1];
            points[0] = start;
            points[segments] = end;

            for (int i = 1; i < segments; i++)
            {
                float t = (float)i / segments;
                var basePoint = Vector3.Lerp(start, end, t);
                float jitterScale = Mathf.Sin(t * Mathf.PI);
                var jitter = new Vector3(
                    UnityEngine.Random.Range(-1f, 1f),
                    0f,
                    UnityEngine.Random.Range(-1f, 1f)) * jitterScale * 1.5f;
                points[i] = basePoint + jitter;
            }

            return points;
        }

        private static void CreateBoltLine(GameObject parent, string childName, Vector3[] points, Shader shader, float width, Color color)
        {
            var child = new GameObject(childName);
            child.transform.SetParent(parent.transform, worldPositionStays: true);

            var line = child.AddComponent<LineRenderer>();
            line.positionCount = points.Length;
            line.SetPositions(points);
            line.startWidth = width;
            line.endWidth = width * 0.3f;
            line.useWorldSpace = true;
            line.startColor = color;
            line.endColor = new Color(color.r, color.g, color.b, color.a * 0.5f);

            if (shader != null)
            {
                line.material = new Material(shader) { color = color };
            }
        }

        private static readonly string[] BoltShaderNames =
        {
            "Unlit/Color",
            "Sprites/Default",
            "Standard"
        };

        private static Shader ResolveBoltShader()
        {
            foreach (var shaderName in BoltShaderNames)
            {
                var shader = Shader.Find(shaderName);
                if (shader != null)
                {
                    return shader;
                }
            }

            return null;
        }

        // Holds the flash at full brightness before fading, so it lingers long enough to notice.
        private sealed class LightningFlashEffect : MonoBehaviour
        {
            private const float HoldDuration = 2f;
            private const float FadeDuration = 1f;
            private const float StartIntensity = 6f;
            private float elapsed;
            private Light flashLight;

            private void Awake()
            {
                flashLight = GetComponent<Light>();
            }

            private void Update()
            {
                elapsed += Time.deltaTime;
                if (flashLight != null)
                {
                    float fadeProgress = Mathf.Clamp01((elapsed - HoldDuration) / FadeDuration);
                    flashLight.intensity = Mathf.Lerp(StartIntensity, 0f, fadeProgress);
                }

                if (elapsed >= HoldDuration + FadeDuration)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    #endregion

    #region Crypt loot

    // Reduces Surtling Core stands in Burial Chambers to 0 or 1 total per crypt based on configurable chance.
    [HarmonyPatch]
    public static class CryptLootPatch
    {
        private const string ProcessedCoreStandZdoKey = "Hardship_CryptCoreProcessed";
        private static bool wasInsideCrypt;
        private static readonly HashSet<int> processedStandInstanceIds = new();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Player), "FixedUpdate")]
        private static void OnPlayerFixedUpdate()
        {
            if (Player.m_localPlayer == null)
            {
                return;
            }

            bool insideCrypt = Player.m_localPlayer.InInterior();
            if (!insideCrypt)
            {
                wasInsideCrypt = false;
                return;
            }

            if (!wasInsideCrypt)
            {
                wasInsideCrypt = true;
                ProcessCurrentCrypt();
            }
        }

        private static void ProcessCurrentCrypt()
        {
            var surtlingCore = PrefabManager.Cache.GetPrefab<ItemDrop>("SurtlingCore");
            if (surtlingCore == null)
            {
                return;
            }

            var playerPos = Player.m_localPlayer.transform.position;
            var playerRoom = UnityEngine.Object.FindObjectsByType<Room>(FindObjectsSortMode.None)
                .Where(room => room.gameObject.scene.IsValid() && IsPositionInsideRoom(room, playerPos))
                .OrderBy(room => (room.transform.position - playerPos).sqrMagnitude)
                .FirstOrDefault();

            var dungeon = playerRoom != null ? playerRoom.GetComponentInParent<DungeonGenerator>() : null;
            if (playerRoom == null || dungeon == null)
            {
                Jotunn.Logger.LogWarning($"Hardship: Could not find the current crypt room for player at {playerPos}; skipping Surtling Core processing.");
                return;
            }

            var rooms = dungeon.GetComponentsInChildren<Room>(true);

            var stands = UnityEngine.Object.FindObjectsByType<Pickable>(FindObjectsSortMode.None)
                .Where(p => p.gameObject.scene.IsValid()
                         && p.m_itemPrefab == surtlingCore.gameObject
                         && rooms.Any(room => IsPositionInsideRoom(room, p.transform.position)))
                .OrderBy(p => (p.transform.position - playerPos).sqrMagnitude)
                .ToList();

            Jotunn.Logger.LogDebug($"Hardship: Entered crypt room '{playerRoom.gameObject.name}' ({playerRoom.m_theme}); checking its {rooms.Length} rooms.");
            Jotunn.Logger.LogDebug($"Hardship: Found {stands.Count} Surtling Core stand(s) in this crypt.");
            foreach (var stand in stands)
            {
                float distance = Vector3.Distance(stand.transform.position, playerPos);
                Jotunn.Logger.LogDebug($"Hardship: Core stand at {stand.transform.position}, {distance:F1}m away, processed={IsCoreStandProcessed(stand)}.");
            }

            if (stands.Count == 0)
            {
                return;
            }

            if (stands.Any(IsCoreStandProcessed))
            {
                Jotunn.Logger.LogDebug("Hardship: A core stand was already processed; keeping the existing result.");
                return;
            }

            float roll = UnityEngine.Random.Range(0f, 100f);
            float chance = cryptSurtlingCoreChance != null ? cryptSurtlingCoreChance.Value : 50f;
            bool keepOne = roll < chance;

            Jotunn.Logger.LogDebug($"Hardship: roll: {roll}");
            if (keepOne)
            {
                Jotunn.Logger.LogDebug("Hardship: Keeping one Surtling Core stand in the crypt.");
                MarkCoreStandProcessed(stands[0]);
            }
            else
            {
                Jotunn.Logger.LogDebug("Hardship: Not keeping any Surtling Core stands in the crypt.");
            }

            // Keep only the first stand if roll succeeded; otherwise destroy all
            var toRemove = keepOne ? stands.Skip(1) : stands;
            foreach (var stand in toRemove)
            {
                var target = stand.gameObject;
                if (ZNetScene.instance != null)
                {
                    ZNetScene.instance.Destroy(target);
                }
                else
                {
                    UnityEngine.Object.Destroy(target);
                }
            }
        }

        private static bool IsPositionInsideRoom(Room room, Vector3 position)
        {
            var localPosition = room.transform.InverseTransformPoint(position);
            const float boundaryTolerance = 0.5f;
            return Mathf.Abs(localPosition.x) <= room.m_size.x * 0.5f + boundaryTolerance
                && Mathf.Abs(localPosition.y) <= room.m_size.y * 0.5f + boundaryTolerance
                && Mathf.Abs(localPosition.z) <= room.m_size.z * 0.5f + boundaryTolerance;
        }

        private static bool IsCoreStandProcessed(Pickable stand)
        {
            if (stand == null)
            {
                return false;
            }

            var networkView = stand.GetComponent<ZNetView>();
            return (networkView != null
                    && networkView.IsValid()
                    && networkView.GetZDO().GetBool(ProcessedCoreStandZdoKey, false))
                || processedStandInstanceIds.Contains(stand.GetInstanceID());
        }

        private static void MarkCoreStandProcessed(Pickable stand)
        {
            processedStandInstanceIds.Add(stand.GetInstanceID());

            var networkView = stand.GetComponent<ZNetView>();
            if (networkView != null && networkView.IsValid())
            {
                networkView.GetZDO().Set(ProcessedCoreStandZdoKey, true);
            }
        }
    }

    #endregion

    #region Death penalty

    // Replaces Valheim's default skill loss with level-based penalties.
    [HarmonyPatch(typeof(Skills), nameof(Skills.LowerAllSkills))]
    public static class DeathPenaltyPatch
    {
        public static bool Prefix(Skills __instance, float factor)
        {
            if (factor <= 0f) return false;

            foreach (var skill in __instance.GetSkillList())
            {
                skill.m_level = CalculatePenaltyLevel(skill.m_level, factor);
                skill.m_accumulator = 0f;
            }

            return false;
        }

        private static float CalculatePenaltyLevel(float currentLevel, float factor)
        {
            if (currentLevel >= 60f)
            {
                return UnityEngine.Mathf.Max(0f, currentLevel - 1f);
            }

            if (currentLevel >= 50f)
            {
                return UnityEngine.Mathf.Max(0f, currentLevel - 2f);
            }

            // Use Valheim's default percentage-based penalty below level 50.
            return currentLevel * (1f - factor);
        }
    }

    #endregion

    #region Dungeon darkness

    // Applies black ambient lighting and overrides Valheim's environment lighting indoors.
    [HarmonyPatch(typeof(EnvMan), "UpdateEnvironment")]
    public static class CryptEnvironmentPatch
    {
        public static bool IsDarkDungeon { get; private set; }
        private static bool darknessApplied;
        private static AmbientMode originalAmbientMode;
        private static SphericalHarmonicsL2 originalAmbientProbe;
        private static Color originalAmbientLight;
        private static float originalAmbientIntensity;
        private static float originalReflectionIntensity;
        private static Color originalFogColor;
        private static float originalFogDensity;
        private static EnvSetup darkEnvironment;
        private static Color originalEnvironmentAmbientColorDay;
        private static Color originalEnvironmentAmbientColorNight;
        private static float originalEnvironmentLightIntensityDay;
        private static float originalEnvironmentLightIntensityNight;
        private static Color originalEnvironmentFogColorDay;
        private static Color originalEnvironmentFogColorNight;
        private static float originalEnvironmentFogDensityDay;
        private static float originalEnvironmentFogDensityNight;

        static void Postfix(EnvMan __instance)
        {
            Refresh(__instance);
        }

        public static void Refresh(EnvMan environmentManager)
        {
            if (environmentManager == null || Player.m_localPlayer == null)
            {
                RestoreLighting();
                return;
            }

            if (!Player.m_localPlayer.InInterior())
            {
                RestoreLighting();
                return;
            }

            IsDarkDungeon = true;
            ApplyEnvironmentDarkness(environmentManager.GetCurrentEnvironment());
            ApplyDarkness();
            DungeonLightPatch.DisableDungeonLights();
            PlayerLightPatch.BoostPlayerLights();
        }

        private static void ApplyEnvironmentDarkness(EnvSetup environment)
        {
            if (environment == null || darkEnvironment == environment)
            {
                return;
            }

            RestoreEnvironmentLighting();
            darkEnvironment = environment;
            originalEnvironmentAmbientColorDay = environment.m_ambColorDay;
            originalEnvironmentAmbientColorNight = environment.m_ambColorNight;
            originalEnvironmentLightIntensityDay = environment.m_lightIntensityDay;
            originalEnvironmentLightIntensityNight = environment.m_lightIntensityNight;
            originalEnvironmentFogColorDay = environment.m_fogColorDay;
            originalEnvironmentFogColorNight = environment.m_fogColorNight;
            originalEnvironmentFogDensityDay = environment.m_fogDensityDay;
            originalEnvironmentFogDensityNight = environment.m_fogDensityNight;

            environment.m_ambColorDay = Color.black;
            environment.m_ambColorNight = Color.black;
            environment.m_lightIntensityDay = 0f;
            environment.m_lightIntensityNight = 0f;
            environment.m_fogColorDay = Color.black;
            environment.m_fogColorNight = Color.black;
            environment.m_fogDensityDay = 1f;
            environment.m_fogDensityNight = 1f;
        }

        private static void ApplyDarkness()
        {
            if (!darknessApplied)
            {
                originalAmbientMode = RenderSettings.ambientMode;
                originalAmbientProbe = RenderSettings.ambientProbe;
                originalAmbientLight = RenderSettings.ambientLight;
                originalAmbientIntensity = RenderSettings.ambientIntensity;
                originalReflectionIntensity = RenderSettings.reflectionIntensity;
                originalFogColor = RenderSettings.fogColor;
                originalFogDensity = RenderSettings.fogDensity;
                darknessApplied = true;
            }

            // Remove ambient and reflection light so dungeon walls do not glow.
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = Color.black;
            RenderSettings.ambientIntensity = 0f;
            RenderSettings.ambientProbe = new SphericalHarmonicsL2();
            RenderSettings.reflectionIntensity = 0f;

            // Keep fog from adding a gray glow to the scene.
            RenderSettings.fogColor = Color.black;
            RenderSettings.fogDensity = 0.1f;
        }

        private static void RestoreLighting()
        {
            if (!darknessApplied)
            {
                return;
            }

            RenderSettings.ambientMode = originalAmbientMode;
            RenderSettings.ambientProbe = originalAmbientProbe;
            RenderSettings.ambientLight = originalAmbientLight;
            RenderSettings.ambientIntensity = originalAmbientIntensity;
            RenderSettings.reflectionIntensity = originalReflectionIntensity;
            RenderSettings.fogColor = originalFogColor;
            RenderSettings.fogDensity = originalFogDensity;
            RestoreEnvironmentLighting();
            PlayerLightPatch.RestorePlayerLights();
            darknessApplied = false;
            IsDarkDungeon = false;
        }

        private static void RestoreEnvironmentLighting()
        {
            if (darkEnvironment == null)
            {
                return;
            }

            darkEnvironment.m_ambColorDay = originalEnvironmentAmbientColorDay;
            darkEnvironment.m_ambColorNight = originalEnvironmentAmbientColorNight;
            darkEnvironment.m_lightIntensityDay = originalEnvironmentLightIntensityDay;
            darkEnvironment.m_lightIntensityNight = originalEnvironmentLightIntensityNight;
            darkEnvironment.m_fogColorDay = originalEnvironmentFogColorDay;
            darkEnvironment.m_fogColorNight = originalEnvironmentFogColorNight;
            darkEnvironment.m_fogDensityDay = originalEnvironmentFogDensityDay;
            darkEnvironment.m_fogDensityNight = originalEnvironmentFogDensityNight;
            darkEnvironment = null;
        }
    }

    #endregion

    #region Dungeon light cleanup

    // Disables dungeon light sources and their fire or smoke particles.
    public static class DungeonLightPatch
    {
        public static void DisableDungeonLights()
        {
            foreach (var light in UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
            {
                DisableDungeonLight(light);
            }
        }

        private static void DisableDungeonLight(Light light)
        {
            if (light == null || !IsDungeonLight(light))
            {
                return;
            }

            light.enabled = false;
            light.intensity = 0f;

            foreach (var particleSystem in light.transform.root.GetComponentsInChildren<ParticleSystem>())
            {
                particleSystem.Stop();
                particleSystem.Clear();
            }
        }

        public static bool IsDungeonLight(Light light)
        {
            string rootName = light.transform.root.name.ToLowerInvariant();
            if (rootName.Contains("player") || rootName.Contains("character"))
            {
                return false;
            }

            string objectName = light.gameObject.name.ToLowerInvariant();
            return rootName.Contains("torch")
                || rootName.Contains("sconce")
                || rootName.Contains("brazier")
                || rootName.Contains("fire")
                || rootName.Contains("flame")
                || objectName.Contains("torch")
                || objectName.Contains("sconce")
                || objectName.Contains("brazier")
                || objectName.Contains("fire")
                || objectName.Contains("flame");
        }
    }

    #endregion

    #region Player torch boost

    // Makes the local player's torch more useful in the forced darkness.
    public static class PlayerLightPatch
    {
        private const float IntensityMultiplier = 0.25f;
        private const float RangeMultiplier = 0.8f;
        private static readonly Dictionary<Light, LightSettings> originalLights = new();

        public static void BoostPlayerLights()
        {
            if (Player.m_localPlayer == null)
            {
                return;
            }

            foreach (var light in UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
            {
                if (DungeonLightPatch.IsDungeonLight(light) || !IsPlayerLight(light))
                {
                    continue;
                }

                if (!originalLights.ContainsKey(light))
                {
                    originalLights[light] = new LightSettings(light.intensity, light.range);
                }

                var original = originalLights[light];
                light.intensity = original.Intensity * IntensityMultiplier;
                light.range = original.Range * RangeMultiplier;
            }
        }

        public static void BoostAttachedLights(Component component)
        {
            if (!CryptEnvironmentPatch.IsDarkDungeon
                || component == null
                || Player.m_localPlayer == null
                || !component.transform.IsChildOf(Player.m_localPlayer.transform))
            {
                return;
            }

            foreach (var light in component.GetComponentsInChildren<Light>(true))
            {
                if (DungeonLightPatch.IsDungeonLight(light))
                {
                    continue;
                }

                BoostLight(light);
            }
        }

        private static void BoostLight(Light light)
        {
            if (!originalLights.ContainsKey(light))
            {
                originalLights[light] = new LightSettings(light.intensity, light.range);
            }

            var original = originalLights[light];
            light.intensity = original.Intensity * IntensityMultiplier;
            light.range = original.Range * RangeMultiplier;
        }

        private static bool IsPlayerLight(Light light)
        {
            return Player.m_localPlayer != null
                && light.transform.IsChildOf(Player.m_localPlayer.transform);
        }

        public static void RestorePlayerLights()
        {
            foreach (var entry in originalLights)
            {
                if (entry.Key == null)
                {
                    continue;
                }

                entry.Key.intensity = entry.Value.Intensity;
                entry.Key.range = entry.Value.Range;
            }

            originalLights.Clear();
        }

        private readonly struct LightSettings
        {
            public LightSettings(float intensity, float range)
            {
                Intensity = intensity;
                Range = range;
            }

            public float Intensity { get; }
            public float Range { get; }
        }
    }

    #endregion

    #region Light reactivation guard

    // Dvergr lanterns expose their light through these components instead of the player hierarchy.
    [HarmonyPatch(typeof(LightFlicker), "Awake")]
    public static class LightFlickerPatch
    {
        static void Postfix(LightFlicker __instance)
        {
            PlayerLightPatch.BoostAttachedLights(__instance);
        }
    }

    [HarmonyPatch(typeof(ParticleIntensityScaler), "Start")]
    public static class ParticleLightPatch
    {
        static void Postfix(ParticleIntensityScaler __instance)
        {
            PlayerLightPatch.BoostAttachedLights(__instance);
        }
    }

    // Prevents Valheim from turning dungeon lights back on after they are disabled.
    [HarmonyPatch(typeof(Behaviour), "set_enabled")]
    public static class DungeonLightEnabledPatch
    {
        static void Prefix(Behaviour __instance, ref bool value)
        {
            if (value && CryptEnvironmentPatch.IsDarkDungeon
                && __instance is Light light
                && DungeonLightPatch.IsDungeonLight(light))
            {
                value = false;
            }
        }
    }

    #endregion

}