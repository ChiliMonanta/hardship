## v0.0.DEV

- **Ore Weights:** Added configurable weights for Tin Ore, Iron Scrap, Silver Ore, Copper Scrap, Iron Ore, Bronze Scrap, Black Metal Scrap, and Flametal Ore, matching the Copper Ore default.

## v0.0.11

- **Surtling Drops:** Surtlings no longer drop Surtling Cores.
- **Geyser Cores:** Each geyser attempts to spawn a Surtling Core after a random 120-720 minutes, only when no core is already nearby.
- **Geyser Gas:** Configure eruption intervals, cloud duration and radius, and stamina drain.

## v0.0.10

- Fix, Restrict Surtling Core processing to the current crypt's generated rooms.
- Fix, Prevent re-entering a Burial Chamber from rerolling its Surtling Core chance.
- Remove recipes requiring Curious or Mysterious Axe Heads to keep Birch, Oak, and Ancient Trees behind later progression.

## v0.0.9

- **Mod Rename:** The mod has been renamed from HardHeim to Hardship because the name HardHeim was already in use.

## v0.0.8

- **Torch is a Light Source, Not a Weapon:** The Torch's primary purpose is now to light your way, not to fight — combat use is a last resort, not a strategy.
- **Torch Recipe Cost:** Torch now costs 2 Wood and 5 Resin to craft (up from 1 Wood, 1 Resin), configurable under `Weapon Balance`.
- **Torch Combat Durability:** Torches now break after a configurable number of melee hits (default 2), in addition to their normal timed burn-out.

## v0.0.7

- **Tier 1 Weapon Balance:** Reduced the base damage of the Club to 8, Flint Knife to 8 total, Stone Axe to 9, Flint Spear to 12, and Crude Bow to 14.
- **Runtime Weapon Configuration:** Added configurable damage values for all rebalanced early-game weapons under the `Weapon Balance` section.

## v0.0.6

- Prevent raid-spawned creatures from dropping loot.
- Add the configurable `Raid Loot -> BlockRaidDrops` setting.
- Preserve normal loot drops for ordinary creatures.

## v0.0.5
- **Storm Ship Damage:** Ships now take damage from waves during severe storms with strong wind, so anchoring in a sheltered bay or shallow water near shore is safer than riding out a storm at sea.
- **Lightning Strikes:** During thunderstorms, standing out in the open (without a roof overhead) risks being struck by lightning, dropping your health to 10. It's riskier on a ship than on land, so seek shelter when thunder rolls in.

## v0.0.4
- **Surtling Core Scarcity:** Burial Chambers contain at most 1 Surtling Core, with a configurable spawn chance (defaults to 30%, resulting in 0 or 1 core per crypt).
- **Surtling Core Weight:** Configurable Surtling Core weight on the server (defaults to 150 kg).
- **Lighting Stability:** Improved interior lighting override handling across dungeon transitions and tuned handheld torch boost.

## v0.0.3
- **Crypt Darkness:** Crypts and caves now use black ambient lighting and fog, removing environmental light that can reveal the interior without a light source.
- **Dungeon Light Removal:** Dungeon torches, sconces, braziers, and related particle effects are disabled and prevented from reactivating.
- **Player Light Boost:** Handheld torches and Dvergr lanterns receive increased intensity and range so they remain useful in the forced darkness.

## v0.0.2
- **Custom Death Penalty:** Added level-based skill loss when a player dies.
- **Penalty Scaling:** Skills below level 50 use percentage-based loss, skills
	from level 50 lose fixed amounts, and skills from level 60 lose only one
	level.
- **Documentation:** Updated installation, configuration, and feature
	documentation for the current release.

## v0.0.1 (Beta)
- **Copper Ore Overhaul:** Increased Copper Ore weight to encourage cart usage and strategic base building near mines.
- **Singleplayer Support:** Fully functional in local, singleplayer worlds.
- **Server Enforcement:** Made the mod mandatory for all clients when running on a dedicated server.