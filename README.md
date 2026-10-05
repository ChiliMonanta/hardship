# Hardship

Hardship is a Valheim mod built with BepInEx and Jötunn. Its purpose is to
make survival more demanding by changing gameplay rules that are normally
fixed in vanilla Valheim.

## Built for Hardcore Players
Valheim's official world modifiers mainly increase enemy damage and health. They
do not change the balance of items or the pace of progression. Hardship extends
those settings by changing the rules around progression, survival, and the
environment, making each stage of the game more demanding without simply
turning every enemy into a damage sponge.

Download and follow releases on [Thunderstore](https://thunderstore.io/c/valheim/p/Dudes/Hardship/).

## Current Features

- **Copper Ore Weight:** Configurable copper ore weight on the server (default `50`).
- **Surtling Core Weight:** Configurable Surtling Core weight on the server (default `150`).
- **Surtling Drops:** Surtlings no longer drop Surtling Cores.
- **Geyser Cores:** Each geyser attempts to spawn a Surtling Core after a random 120-720 minutes, only when no core is already nearby.
- **Geyser Gas:** Geysers release a visible toxic cloud at random intervals of 4-55 seconds. Clouds last 15 seconds and affect players within 18 m; they drain 25 stamina per second, then deal drowning-type damage of 5% max health (rounded up) per second once stamina is exhausted. Gas intervals, cloud duration and radius, and stamina drain are configurable under `Geyser Gas`.
- **Crypt Surtling Core Scarcity:** Burial Chambers contain 0 or 1 Surtling Core total based on a configurable chance (default `50%`), rolled once per crypt and limited to that crypt's rooms.
- **Custom Death Penalty:** Skill loss scaled by level.
- **Dark Crypts & Caves:** Forced darkness indoors with tuned handheld lighting.
- **Storm Ship Damage:** Ships take blunt damage while wind force reaches the configured storm threshold, but are protected in shallow water so they do not take damage while close to shore.
- **Lightning Strikes:** Players out in the open during a thunderstorm risk being struck by lightning (higher chance on a ship than on land); a struck player's health drops to 10, with a visible bolt and messages, followed by a per-player cooldown.
- **Raid Loot Suppression:** Creatures spawned by raids do not drop loot by default, while ordinary creatures continue to drop loot normally.
- **Weapon Balance:** Club, Flint Knife, Stone Axe, Flint Spear, and Crude Bow damage can be adjusted at runtime through the `Weapon Balance` configuration section.
- **Torch Recipe & Durability:** Torch crafting cost and the number of melee hits before it breaks can be adjusted at runtime through the `Weapon Balance` configuration section.
- **Early Axe Progression:** Recipes requiring Curious or Mysterious Axe Heads are removed, keeping Birch, Oak, and Ancient Trees behind later progression.
- **Server Synchronization:** Jötunn-managed server-to-client config synchronization and version enforcement.

The generated BepInEx configuration can be adjusted in:

```text
BepInEx/config/com.valheim.hardship.cfg
```

- `Storm Ship Damage -> ShallowWaterDepth`: storms do not damage ships when the seabed is close below them; this protects boats in shallow water and near shore.
- `Geyser Gas -> MinimumIntervalSeconds` (default `4`), `MaximumIntervalSeconds` (`55`), `CloudDuration` (`15`), `CloudRadius` (`18`), and `StaminaDrainPerSecond` (`25`): configure ambient geyser gas eruptions and their effects.
- `Lightning Strikes -> Enabled`, `LandChancePercent` (default `0.5`), `ShipChancePercent` (default `1`), `CheckIntervalSeconds` (default `130`), `CooldownSeconds` (default `120`), `ThunderstormEnvironments`: control whether, how often, and how likely lightning strikes are, and the cooldown before a player can be struck again.
- `Raid Loot -> BlockRaidDrops` (default `true`): controls whether raid-spawned creatures are prevented from dropping loot.
- `Weapon Balance -> ClubDamage` (default `8`), `FlintKnifeDamage` (default `8`), `StoneAxeDamage` (default `9`), `FlintSpearDamage` (default `12`), and `CrudeBowDamage` (default `14`): control the base damage of the early-game weapons.
- `Weapon Balance -> TorchWoodCost` (default `2`) and `TorchResinCost` (default `5`): control the crafting cost of the Torch.
- `Weapon Balance -> TorchHitsToBreak` (default `2`): number of melee hits before a Torch breaks, in addition to its normal timed burn-out.

## Architecture

Hardship is a managed .NET Framework `net462` assembly. It runs inside the
Mono runtime used by the Windows version of Valheim and is loaded by BepInEx.
The mod uses Jötunn for Valheim integration and configuration
synchronization.

The runtime dependency chain is:

```text
Hardship
├── BepInEx 5.4.23.5
├── Jötunn 2.30.2
│   └── YamlDotNet and JotunnBuildTask dependencies
├── UnityEngine assemblies from the local Valheim installation
└── Valheim assemblies from valheim_Data/Managed
```

The build also creates the libraries needed to build Jötunn and BepInEx from
source. The important development-time chain is:

```text
Jötunn
└── JotunnBuildTask
		└── Mono.Cecil 0.10.4

BepInEx and Jötunn
├── HarmonyX 2.9.0
└── MonoMod.Utils / MonoMod.RuntimeDetour 22.1.29.1
		├── Mono.Cecil 0.10.4
		└── MonoMod.Common source compiled into the MonoMod assemblies
```

MonoMod.Common is not distributed as a separate assembly. Its source is
compiled into the relevant MonoMod outputs.

### Repository layout

```text
/
├── build.sh                 # Builds dependencies, mod, and deployment zips
├── setup.sh                 # Clones dependencies at pinned revisions
├── install-local.sh         # Installs selected zips into the local Valheim tree
├── hardship/
│   ├── Hardship.csproj      # Mod project
│   ├── Hardship.cs          # Mod implementation
│   └── thunderstore/        # Thunderstore package metadata and documentation
├── dependencies/            # Local source checkouts and wrapper projects
├── .locals-packages/        # Locally produced NuGet packages (not committed)
└── dist/                    # Generated deployment packages (not committed)
```

The Valheim installation is expected at:

```text
dependencies/valheim-steam/
├── valheim_Data/Managed/    # Original Unity and Valheim assemblies
└── BepInEx/                 # Local test installation
```

Valheim assemblies are build inputs only. They are not replaced or modified
by the mod build. Jötunn's `publicized_assemblies` directory is a temporary
build artifact and is removed by `build.sh` after Jötunn has been built.

## Security and Supply Chain

The project is designed to keep third-party build inputs local and auditable:

- Third-party dependencies are cloned at pinned Git commit revisions by
	`setup.sh`.
- BepInEx, HarmonyX, MonoMod, Mono.Cecil, Jötunn, and Unity Doorstop are built
	locally from source.
- Prebuilt third-party DLLs are not required or committed for the build.
- The local NuGet source is `.locals-packages`, which is populated by the
	build. NuGet access is restricted by `nuget.config` to approved package
	families and explicitly allowed build/test packages.
- Valheim and Unity assemblies come from the locally mounted game installation;
	they are not downloaded by this repository.
- Generated binaries should be treated as build artifacts. Before a release,
	record their SHA-256 values and the Valheim Steam build/depot information
	used for the build.

Only install release packages from a source you trust. The mod executes inside
the Valheim/BepInEx process and therefore has the same privileges as the game
process. Review dependency source revisions and generated package contents
before distributing them.

## Local Development

### Prerequisites

The only local prerequisite is VS Code with the Dev Containers support. Open
the repository in the provided devcontainer; it includes the Linux
environment, .NET 8 SDK, Git, MinGW-w64, and the other build tools required by
the project. The devcontainer also mounts the local Valheim installation at
`dependencies/valheim-steam`.

Initialize the pinned source dependencies:

```bash
./setup.sh
```

The setup script checks out the revisions documented in
[`hardship.spec`](hardship.spec), including the MonoMod.Common submodule.

Build all local dependencies, the mod, Unity Doorstop, and deployment
packages:

```bash
./build.sh
```

The build produces packages in `dist/`, including:

```text
dist/
├── BepInEx-windows.zip
├── BepInEx-linux.zip
├── Jotunn.zip
├── ConfigurationManager.zip
└── Hardship.zip
```

`Hardship.zip` contains the mod DLL and the package metadata from
`hardship/thunderstore/`. It does not contain the development dependency
source tree or the local Valheim assemblies.

## Local Deployment

`install-local.sh` extracts generated packages into the mounted Valheim
directory. For a complete Windows test installation:

```bash
./install-local.sh --all-windows
```

To install only Hardship after rebuilding it:

```bash
./install-local.sh --hardship
```

For a Linux test installation, use:

```bash
./install-local.sh --all-linux
```

The intended plugin layout is:

```text
dependencies/valheim-steam/BepInEx/plugins/
└── Hardship.dll
```

The `Hardship.zip` build artifact contains `Hardship.dll` at the zip root,
which is the expected Thunderstore package layout. `install-local.sh` extracts
that package into `BepInEx/plugins/Hardship` for local testing.

BepInEx and Jötunn are installed as separate packages. The game installation
and its original `valheim_Data/Managed` assemblies remain outside the release
package and are not modified by the Hardship deployment step.

## Release Package

The Thunderstore package metadata is maintained in
`hardship/thunderstore/manifest.json`. Its declared dependencies are:

- `denikson-BepInExPack_Valheim-5.4.2350`
- `ValheimModding-Jotunn-2.30.2`

For end users, BepInEx must already be installed in the Valheim directory.
Install the release package with a mod manager, or extract `Hardship.zip` and
place `Hardship.dll` in `BepInEx/plugins/`.
