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

        var copperOre = PrefabManager.Cache.GetPrefab<ItemDrop>("CopperOre");
        if (copperOre == null)
        {
            Logger.LogError("Could not find the CopperOre prefab.");
            return;
        }

        copperOre.m_itemData.m_shared.m_weight = copperOreWeight.Value;

        var surtlingCore = PrefabManager.Cache.GetPrefab<ItemDrop>("SurtlingCore");
        if (surtlingCore == null)
        {
            Logger.LogError("Could not find the SurtlingCore prefab.");
            return;
        }

        surtlingCore.m_itemData.m_shared.m_weight = surtlingCoreWeight.Value;
        SetWeaponBalance();
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