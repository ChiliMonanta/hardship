#!/bin/bash
set -e

#-------------------
export VALHEIM_INSTALL="/workspace/dependencies/valheim-steam"
export VALHEIM_MANAGED="/workspace/dependencies/valheim-steam/valheim_Data/Managed"
export VALHEIM_BEPINEX_PATH="$VALHEIM_INSTALL/BepInEx"
export DEPS_PATH="/workspace/dependencies"
export CECIL_VERSION=0.10.4 # Checked out in setup.sh
export CECIL_DEFINES="CECIL0_10"
export LOCAL_PACKAGES_PATH="/workspace/.locals-packages"

#-------------------
clean_build_files() {
  # Args: $1 = path to folder.
  find "$1" \( -type d \( -name "bin" -o -name "obj" \) -exec rm -rf {} + \) -o \
    \( -type f \( -name "*.dll" -o -name "*.exe" \) -delete \)
}

#-------------------
echo "# Package Unity libraries"
UNTIY_PATH="$DEPS_PATH/valheim-unity"

dotnet pack "$UNTIY_PATH/Unity.InputSystem.csproj" \
  -c Release \
  -p:PackageOutputPath="$LOCAL_PACKAGES_PATH"

dotnet pack "$UNTIY_PATH/UnityEngine.csproj" \
  -c Release \
  -p:PackageOutputPath="$LOCAL_PACKAGES_PATH"

dotnet pack "$UNTIY_PATH/UnityEngine.CoreModule.csproj" \
  -c Release \
  -p:PackageOutputPath="$LOCAL_PACKAGES_PATH"

dotnet pack "$UNTIY_PATH/UnityEngine.IMGUIModule.csproj" \
  -c Release \
  -p:PackageOutputPath="$LOCAL_PACKAGES_PATH"

dotnet pack "$UNTIY_PATH/UnityEngine.TextRenderingModule.csproj" \
  -c Release \
  -p:PackageOutputPath="$LOCAL_PACKAGES_PATH"

# ConfigurationManager uses direct HintPath references into the NuGet cache.
# Unpack the locally built Unity packages into that cache layout.
stage_local_package() {
  local package_id="$1"
  local package_version="$2"
  local package_root="$LOCAL_PACKAGES_PATH/$(printf '%s' "$package_id" | tr '[:upper:]' '[:lower:]')/$package_version"
  mkdir -p "$package_root"
  unzip -q -o "$LOCAL_PACKAGES_PATH/$package_id.$package_version.nupkg" -d "$package_root"
}

stage_local_package "Unity.InputSystem" "1.5.0"
stage_local_package "UnityEngine" "5.6.1"
stage_local_package "UnityEngine.CoreModule" "5.6.1"
stage_local_package "UnityEngine.IMGUIModule" "5.6.1"
stage_local_package "UnityEngine.TextRenderingModule" "5.6.1"

#-------------------
echo "# Build cecil"
clean_build_files "$DEPS_PATH/cecil"

dotnet build "$DEPS_PATH/cecil-wrapper/Mono.Cecil.csproj" \
  -c Release \
  -f net462

dotnet pack "$DEPS_PATH/cecil-wrapper/Mono.Cecil.csproj" \
  -c Release \
  -p:Version=$CECIL_VERSION \
  -o "$LOCAL_PACKAGES_PATH"

#-------------------
echo "# Build MonoMod"
clean_build_files "$DEPS_PATH/MonoMod"

dotnet build "$DEPS_PATH/MonoMod/MonoMod.Utils/MonoMod.Utils.csproj" \
  -c Release \
  -f net462 \
  -p:TargetFrameworks=net462 \
  -o "$LOCAL_PACKAGES_PATH" \
  -p:DefineConstants=$CECIL_DEFINES \
  -p:CecilVersion=$CECIL_VERSION

dotnet pack "$DEPS_PATH/MonoMod/MonoMod.Utils/MonoMod.Utils.csproj" \
  -c Release \
  -p:TargetFrameworks=net462 \
  -p:Version=22.1.29.1 \
  -o "$LOCAL_PACKAGES_PATH" \
  -p:DefineConstants=$CECIL_DEFINES \
  -p:CecilVersion=$CECIL_VERSION

dotnet build "$DEPS_PATH/MonoMod/MonoMod.RuntimeDetour/MonoMod.RuntimeDetour.csproj" \
  -c Release \
  -f net462 \
  -p:TargetFrameworks=net462 \
  -o "$LOCAL_PACKAGES_PATH" \
  -p:DefineConstants=$CECIL_DEFINES \
  -p:CecilVersion=$CECIL_VERSION

dotnet pack "$DEPS_PATH/MonoMod/MonoMod.RuntimeDetour/MonoMod.RuntimeDetour.csproj" \
  -c Release \
  -p:TargetFrameworks=net462 \
  -p:Version=22.1.29.1 \
  -o "$LOCAL_PACKAGES_PATH" \
  -p:DefineConstants=$CECIL_DEFINES \
  -p:CecilVersion=$CECIL_VERSION

#-------------------
echo "# HarmonyX"
clean_build_files "$DEPS_PATH/harmony-wrapper"

dotnet build "$DEPS_PATH/harmony-wrapper/HarmonyX.csproj" \
  -f net462 \
  -c Release

dotnet pack "$DEPS_PATH/harmony-wrapper/HarmonyX.csproj" \
  -c Release \
  -p:TargetFrameworks=net462 \
  -p:Version=2.9.0 \
  -p:PackageOutputPath="$LOCAL_PACKAGES_PATH" \
  -p:DefineConstants=$CECIL_DEFINES \
  -p:CecilVersion=$CECIL_VERSION

#-------------------
echo "# Build BepInEx"
BEPINEX_WRAPPER_PATH="$DEPS_PATH/bepinex-wrapper"
BEPINEX_FRAMEWORK_ROOT="$HOME/.nuget/packages/microsoft.netframework.referenceassemblies.net462/1.0.3/build"
BEPINEX_VERSION=5.4.23.5

clean_build_files "$BEPINEX_WRAPPER_PATH"
clean_build_files "$DEPS_PATH/BepInEx"

dotnet build "$BEPINEX_WRAPPER_PATH/BepInEx.csproj" \
  -c Release \
  -p:TargetFrameworkRootPath="$BEPINEX_FRAMEWORK_ROOT" \
  -p:BepInExVersionPrefix=$BEPINEX_VERSION \
  -p:DefineConstants=$CECIL_DEFINES

# Needed twice with different names
dotnet pack "$BEPINEX_WRAPPER_PATH/BepInEx.csproj" \
  -c Release \
  --no-build \
  -p:TargetFrameworks=net462 \
  -p:TargetFrameworkRootPath="$BEPINEX_FRAMEWORK_ROOT" \
  -p:BepInExVersionPrefix=$BEPINEX_VERSION \
  -p:PackageOutputPath="$LOCAL_PACKAGES_PATH" \
  -p:DefineConstants=$CECIL_DEFINES \
  -p:CecilVersion=$CECIL_VERSION
dotnet pack "$BEPINEX_WRAPPER_PATH/BepInEx.csproj" \
  -c Release \
  --no-build \
  -p:PackageId=BepInEx.BaseLib \
  -p:TargetFrameworks=net462 \
  -p:TargetFrameworkRootPath="$BEPINEX_FRAMEWORK_ROOT" \
  -p:BepInExVersionPrefix=$BEPINEX_VERSION \
  -p:PackageOutputPath="$LOCAL_PACKAGES_PATH" \
  -p:DefineConstants=$CECIL_DEFINES \
  -p:CecilVersion=$CECIL_VERSION

dotnet pack "$BEPINEX_WRAPPER_PATH/BepInEx.Bootstrap.csproj" \
  -c Release \
  -p:TargetFrameworks=net462 \
  -p:TargetFrameworkRootPath="$BEPINEX_FRAMEWORK_ROOT" \
  -p:Version=$BEPINEX_VERSION \
  -p:PackageOutputPath="$LOCAL_PACKAGES_PATH" \
  -p:DefineConstants=$CECIL_DEFINES \
  -p:CecilVersion=$CECIL_VERSION

dotnet pack "$BEPINEX_WRAPPER_PATH/BepInEx.Harmony.csproj" \
  -c Release \
  -p:TargetFrameworks=net462 \
  -p:TargetFrameworkRootPath="$BEPINEX_FRAMEWORK_ROOT" \
  -p:Version=$BEPINEX_VERSION \
  -p:PackageOutputPath="$LOCAL_PACKAGES_PATH" \
  -p:DefineConstants=$CECIL_DEFINES \
  -p:CecilVersion=$CECIL_VERSION

dotnet pack "$BEPINEX_WRAPPER_PATH/HarmonyX2Interop.csproj" \
  -c Release \
  -p:TargetFrameworks=net462 \
  -p:TargetFrameworkRootPath="$BEPINEX_FRAMEWORK_ROOT" \
  -p:BepInExVersionPrefix=$BEPINEX_VERSION \
  -p:PackageOutputPath="$LOCAL_PACKAGES_PATH" \
  -p:DefineConstants=$CECIL_DEFINES \
  -p:CecilVersion=$CECIL_VERSION

dotnet pack "$BEPINEX_WRAPPER_PATH/HarmonyXInterop.csproj" \
  -c Release \
  -p:TargetFrameworks=net462 \
  -p:TargetFrameworkRootPath="$BEPINEX_FRAMEWORK_ROOT" \
  -p:BepInExVersionPrefix=$BEPINEX_VERSION \
  -p:PackageOutputPath="$LOCAL_PACKAGES_PATH" \
  -p:DefineConstants=$CECIL_DEFINES \
  -p:CecilVersion=$CECIL_VERSION

dotnet pack "$BEPINEX_WRAPPER_PATH/BepInEx.Preloader.csproj" \
  -c Release \
  -p:TargetFrameworks=net462 \
  -p:TargetFrameworkRootPath="$BEPINEX_FRAMEWORK_ROOT" \
  -p:BepInExVersionPrefix=$BEPINEX_VERSION \
  -p:PackageOutputPath="$LOCAL_PACKAGES_PATH" \
  -p:DefineConstants=$CECIL_DEFINES \
  -p:CecilVersion=$CECIL_VERSION

#-------------------
# echo "# BepInEx.Analyzers"
# Optional tool
echo "# Build BepInEx.Analyzers"
BEPINEX_ANALYZERS_VERSION="1.0.8"
BEPINEX_ANALYZERS_PATH="$DEPS_PATH/BepInEx.Analyzers/BepInEx.Analyzers"
BEPINEX_ANALYZERS_BUILD_OVERRIDES="/workspace/dependencies/BepInEx.Analyzers.build.targets"

clean_build_files "$DEPS_PATH/BepInEx.Analyzers"

dotnet build "$BEPINEX_ANALYZERS_PATH/BepInEx.Analyzers/BepInEx.Analyzers.csproj" \
  -c Release \
  -p:VersionPrefix=$BEPINEX_ANALYZERS_VERSION \
  -p:CustomBeforeMicrosoftCommonTargets="$BEPINEX_ANALYZERS_BUILD_OVERRIDES"

dotnet build "$BEPINEX_ANALYZERS_PATH/BepInEx.Analyzers.CodeFixes/BepInEx.Analyzers.CodeFixes.csproj" \
  -c Release \
  -p:VersionPrefix=$BEPINEX_ANALYZERS_VERSION \
  -p:CustomBeforeMicrosoftCommonTargets="$BEPINEX_ANALYZERS_BUILD_OVERRIDES"

# The package project expects both analyzers in its own output directory.
ANALYZERS_PACKAGE_OUTPUT="$BEPINEX_ANALYZERS_PATH/BepInEx.Analyzers.Package/bin/Release/netstandard2.0"
mkdir -p "$ANALYZERS_PACKAGE_OUTPUT"
cp "$BEPINEX_ANALYZERS_PATH/BepInEx.Analyzers/bin/Release/netstandard2.0/BepInEx.Analyzers.dll" \
  "$ANALYZERS_PACKAGE_OUTPUT/"
cp "$BEPINEX_ANALYZERS_PATH/BepInEx.Analyzers.CodeFixes/bin/Release/netstandard2.0/BepInEx.Analyzers.CodeFixes.dll" \
  "$ANALYZERS_PACKAGE_OUTPUT/"

dotnet pack "$BEPINEX_ANALYZERS_PATH/BepInEx.Analyzers.Package/BepInEx.Analyzers.Package.csproj" \
  -c Release \
  -p:Version=$BEPINEX_ANALYZERS_VERSION \
  -p:PackageOutputPath="$LOCAL_PACKAGES_PATH" \
  -p:CustomBeforeMicrosoftCommonTargets="$BEPINEX_ANALYZERS_BUILD_OVERRIDES"

#-------------------
# echo "# Build BeepInEx.ConfigurationsManager"
# Optional plugin
echo "# Build BepInEx.ConfigurationManager"
CONFIGURATION_MANAGER_WRAPPER_PATH="$DEPS_PATH/BepInEx.ConfigurationManager-wrapper"

clean_build_files "$CONFIGURATION_MANAGER_WRAPPER_PATH"
clean_build_files "$DEPS_PATH/BepInEx.ConfigurationManager"

dotnet build "$CONFIGURATION_MANAGER_WRAPPER_PATH/ConfigurationManager.csproj" \
  -c Release \
  -f net462 \
  -p:NuGetPackageRoot="$LOCAL_PACKAGES_PATH/" \
  -p:TargetFrameworkRootPath="$BEPINEX_FRAMEWORK_ROOT"

dotnet pack "$CONFIGURATION_MANAGER_WRAPPER_PATH/ConfigurationManager.csproj" \
  -c Release \
  --no-build \
  -o "$LOCAL_PACKAGES_PATH" \
  -p:NuGetPackageRoot="$LOCAL_PACKAGES_PATH/" \
  -p:TargetFrameworkRootPath="$BEPINEX_FRAMEWORK_ROOT"

#-------------------
echo "# Build Jotunn"
JONTUNN_PATH="$DEPS_PATH/Jotunn"
JOTUNN_VERSION=2.30.2

clean_build_files "$JONTUNN_PATH"

# Jontunn has a prebuild step that requires the Mono.Cecil.dll, don't use the checked in one.
cp .locals-packages/Mono.Cecil.dll $DEPS_PATH/Jotunn/libraries

# The pinned v2.30.1 source still declares its BepInEx runtime version as 2.29.2.
# Synchronize it with the version used for the assembly and distribution package.
sed -i -E 's/(public const string Version = ")[^"]+/\1'"$JOTUNN_VERSION"'/' \
  "$JONTUNN_PATH/JotunnLib/Main.cs"

dotnet build "$DEPS_PATH/Jotunn/JotunnBuildTask/JotunnBuildTask.csproj" \
  -c Release \
  -f net462 \
  -p:SolutionDir="$DEPS_PATH/Jotunn/" \
  -p:DefineConstants=$CECIL_DEFINES \
  -p:CecilVersion=$CECIL_VERSION

dotnet build "$DEPS_PATH/Jotunn/JotunnLib.sln" \
  -c Release \
  -p:DefineConstants=$CECIL_DEFINES \
  -p:Version=$JOTUNN_VERSION \
  -p:ExecutePrebuild=true \
  -p:CecilVersion=$CECIL_VERSION \
  -p:PackageOutputPath="$LOCAL_PACKAGES_PATH"

echo "# Remove publicized_assemblies (build-time artefacts only)"
rm -rf "$VALHEIM_MANAGED/publicized_assemblies"

#-------------------
echo "# Build Doorstop"

DOORSTOP_PATH="$DEPS_PATH/UnityDoorstop"
DOORSTOP_XMAKE="$DOORSTOP_PATH/tools/xmake/bin/xmake"

# Windows build
DOORSTOP_WINDOWS_BUILD="$DOORSTOP_PATH/out/release/windows"
DOORSTOP_WINDOWS_SOURCE="$DOORSTOP_PATH/build/mingw/x64/release/libdoorstop.dll"
DOORSTOP_DLL="$DOORSTOP_WINDOWS_BUILD/libdoorstop.dll"

# Linux build
DOORSTOP_LINUX_BUILD="$DOORSTOP_PATH/out/release/linux"
DOORSTOP_LINUX_SOURCE="$DOORSTOP_PATH/build/linux/x64/release/libdoorstop.so"
DOORSTOP_SO="$DOORSTOP_LINUX_BUILD/libdoorstop.so"

# Remove generated platform outputs before building clean, reproducible artifacts.
rm -rf "$DOORSTOP_PATH/out"
clean_build_files "$DOORSTOP_PATH"

# The upstream script bootstraps xmake on Linux. Its first run also creates the
# local xmake binary used below with the MinGW platform selected explicitly.
if [ ! -x "$DOORSTOP_XMAKE" ]; then
	(cd "$DOORSTOP_PATH" && ./build.sh -arch=x64)
fi

# Windows
echo "# Build Doorstop Windows"
(cd "$DOORSTOP_PATH" && XMAKE_ROOT=y "$DOORSTOP_XMAKE" f -p mingw -a x64 -m release \
  --include_logging=n)
(cd "$DOORSTOP_PATH" && XMAKE_ROOT=y "$DOORSTOP_XMAKE" -y)

test -f "$DOORSTOP_WINDOWS_SOURCE"
mkdir -p "$DOORSTOP_WINDOWS_BUILD"
cp "$DOORSTOP_WINDOWS_SOURCE" "$DOORSTOP_DLL"
x86_64-w64-mingw32-objdump -f "$DOORSTOP_DLL" | grep -q 'pei-x86-64'

# Linux
echo "# Build Doorstop Linux"
(cd "$DOORSTOP_PATH" && XMAKE_ROOT=y "$DOORSTOP_XMAKE" f -p linux -a x64 -m release \
  --include_logging=n)
(cd "$DOORSTOP_PATH" && XMAKE_ROOT=y "$DOORSTOP_XMAKE" -y)

# Package
echo "# Package Linux"
test -f "$DOORSTOP_LINUX_SOURCE"
mkdir -p "$DOORSTOP_LINUX_BUILD"
cp "$DOORSTOP_LINUX_SOURCE" "$DOORSTOP_SO"
file "$DOORSTOP_SO" | grep -q 'ELF 64-bit'

echo "Doorstop Windows build: $DOORSTOP_DLL"
echo "Doorstop Linux build: $DOORSTOP_SO"

#-------------------
echo "# Build Hardship"
HARDSHIP_VERSION="0.0.0" # dev

dotnet build "hardship/Hardship.csproj" \
  -p:VersionPrefix=$HARDSHIP_VERSION

# -------------------
echo "# Create packages"

mkdir -p dist

echo "# Create Windows BepInEx package"
mkdir -p dist/tmp/BepInEx/core/
cp "$BEPINEX_WRAPPER_PATH/bin/Release/net462/"{BepInEx.dll,BepInEx.Preloader.dll,0Harmony.dll,HarmonyXInterop.dll,MonoMod.Utils.dll,MonoMod.RuntimeDetour.dll,Mono.Cecil.dll,System.ValueTuple.dll} dist/tmp/BepInEx/core/
cp "$DOORSTOP_DLL" "dist/tmp/winhttp.dll"
cp "$DOORSTOP_PATH/assets/windows/doorstop_config.ini" "dist/tmp/doorstop_config.ini"
sed -i 's|^target_assembly=.*|target_assembly=BepInEx\\core\\BepInEx.Preloader.dll|' dist/tmp/doorstop_config.ini
(cd dist/tmp && zip -r ../BepInEx-windows.zip *)
rm -rf dist/tmp

echo "# Create Linux BepInEx package"
mkdir -p dist/tmp/BepInEx/core/
cp "$BEPINEX_WRAPPER_PATH/bin/Release/net462/"{BepInEx.dll,BepInEx.Preloader.dll,0Harmony.dll,HarmonyXInterop.dll,MonoMod.Utils.dll,MonoMod.RuntimeDetour.dll,Mono.Cecil.dll,System.ValueTuple.dll} dist/tmp/BepInEx/core/
cp "$DOORSTOP_SO" dist/tmp/libdoorstop.so
cp "$DOORSTOP_PATH/assets/nix/run.sh" dist/tmp/run.sh
sed -i 's|^target_assembly=.*|target_assembly="BepInEx/core/BepInEx.Preloader.dll"|' dist/tmp/run.sh
(cd dist/tmp && zip -r ../BepInEx-linux.zip *)
rm -rf dist/tmp

echo "# Create Jotunn package"
mkdir -p dist/tmp/BepInEx/plugins/Jotunn
cp $DEPS_PATH/Jotunn/JotunnLib/bin/Release/net462/Jotunn.dll dist/tmp/BepInEx/plugins/Jotunn
cat << EOF > dist/tmp/BepInEx/plugins/Jotunn/manifest.json
{
    "name": "Jotunn",
  "version_number": "$JOTUNN_VERSION",
    "website_url": "",
    "description": "Jotunn is a modding library for Valheim.",
    "dependencies": [
        "denikson-BepInExPack_Valheim-5.4.2350"
    ]
}
EOF
(cd dist/tmp && zip -r ../Jotunn.zip *)
rm -rf dist/tmp

echo "# Create Configuration Manager package"
mkdir -p dist/tmp/BepInEx/plugins/ConfigurationManager
cp $DEPS_PATH/BepInEx.ConfigurationManager-wrapper/bin/BepInEx5/ConfigurationManager.dll dist/tmp/BepInEx/plugins/ConfigurationManager
cat << EOF > dist/tmp/BepInEx/plugins/ConfigurationManager/manifest.json
{
    "name": "ConfigurationManager",
    "version_number": "19.0",
    "website_url": "",
    "description": "Configuration Manager is a modding library for Valheim.",
    "dependencies": [
        "denikson-BepInExPack_Valheim-5.4.2350"
    ]
}
EOF
(cd dist/tmp && zip -r ../ConfigurationManager.zip *)
rm -rf dist/tmp

echo "# Create Mod package"
mkdir -p dist/tmp
cp hardship/bin/Debug/net462/Hardship.dll dist/tmp
cp hardship/thunderstore/{CHANGELOG.md,icon.png,manifest.json,README.md} dist/tmp
sed -i "s/\$HARDSHIP_VERSION/$HARDSHIP_VERSION/g" dist/tmp/manifest.json
(cd dist/tmp && zip -r ../Hardship.zip *)
rm -rf dist/tmp