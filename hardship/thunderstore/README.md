# Hardship

**Odin's world, just brutal. Slower progression, harsher consequences, and deadly storms for those who are not prepared.**

Hardship makes Valheim's survival harsher from the very first day. Progression
is slower, death has greater consequences, and dangerous weather can turn the
world itself against you. Explore dark crypts with a handheld light, manage
your resources carefully, and prepare for storms before they find you exposed.

## Features
- **Copper Ore Weight:** Configure the weight of copper ore on the server.
- **Surtling Core Weight:** Configure the weight of Surtling Cores on the server (defaults to 150 kg).
- **Crypt Surtling Core Scarcity:** Burial Chambers contain at most 1 Surtling Core (configurable spawn chance, defaults to 50%, rolled once per crypt and limited to that crypt's rooms).
- **Custom Death Penalty:** Skills below level 50 use percentage-based loss;
	skills from level 50 use fixed reductions, with a smaller reduction at level
	60 and above.
- **Dark Crypts and Caves:** Ambient lighting, environmental light, fog, and
	dungeon light sources are disabled indoors, making handheld lighting
	necessary for exploration.
- **Enhanced Handheld Lights:** Player torches and Dvergr lanterns have adjusted
	light intensity and range while inside dark interiors.
- **Lighting Note:** Camera zoom can affect the perceived brightness and range
	of handheld lights because Valheim adjusts light detail based on distance.
- **Lightning Strikes:** During thunderstorms, players out in the open (not
	under a roof) risk being struck by lightning, with a higher chance while on
	a ship. A struck player's health drops to 10, with a screen message and a
	visible bolt effect seen by everyone nearby, followed by a per-player
	cooldown before it can happen again.
- **Server Synchronization:** Configuration is synchronized to clients, and
	every player must have the mod installed.
- **Storm Ship Damage:** Ships take configurable damage during severe storms, but are protected in shallow water so they do not take damage while close to shore.
- **Weapon Balance:** Early-game weapon damage can be adjusted at runtime through the configuration.
- **Torch Recipe & Durability:** Torch crafting cost and the number of melee hits before it breaks can be adjusted at runtime through the configuration.
- **Early Axe Progression:** Recipes requiring Curious or Mysterious Axe Heads are removed, keeping Birch, Oak, and Ancient Trees behind later progression.

## Installation
The easiest way to install Hardship is using a mod manager like **r2modman** or the **Thunderstore Mod Manager**. 

### Manual Installation
1. Ensure you have **BepInEx** and **Jötunn** installed.
2. Download the `Hardship.zip` archive.
3. Extract the contents and move `Hardship.dll` into your `Valheim/BepInEx/plugins/` folder.

## Configuration
After launching the game once, edit the generated configuration file:

```text
BepInEx/config/com.valheim.Hardship.cfg
```

- `Ore Weights -> CopperOre`: accepts `0.1` to `1000` (default `50`).
- `Ore Weights -> SurtlingCore`: accepts `0.1` to `1000` (default `150`).
- `Crypt Loot -> SurtlingCoreChance`: percent chance `0` to `100` (default `30`).
- `Storm Ship Damage -> ShallowWaterDepth`: storms do not damage ships when the seabed is close below them; this protects boats in shallow water and near shore.
- `Lightning Strikes -> Enabled`: toggles lightning strikes entirely (default `true`).
- `Lightning Strikes -> LandChancePercent`: percent chance `0` to `100` per check while on land during a thunderstorm (default `0.5`).
- `Lightning Strikes -> ShipChancePercent`: percent chance `0` to `100` per check while on a ship during a thunderstorm (default `1`).
- `Lightning Strikes -> CheckIntervalSeconds`: how often the strike chance is rolled per player (default `130`).
- `Lightning Strikes -> CooldownSeconds`: minimum time after being struck before a player can be struck again (default `120`).
- `Lightning Strikes -> ThunderstormEnvironments`: comma-separated environment names that count as a thunderstorm (default `ThunderStorm`).
- `Weapon Balance -> ClubDamage` (default `8`), `FlintKnifeDamage` (default `8`), `StoneAxeDamage` (default `9`), `FlintSpearDamage` (default `12`), and `CrudeBowDamage` (default `14`): control early-game weapon damage.
- `Weapon Balance -> TorchWoodCost` (default `2`) and `TorchResinCost` (default `5`): control the crafting cost of the Torch.
- `Weapon Balance -> TorchHitsToBreak` (default `2`): number of melee hits before a Torch breaks, in addition to its normal timed burn-out.

Death penalty behavior is built in and is not currently configurable.
