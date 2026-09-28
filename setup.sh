#!/bin/bash
set -e
# Clone all dependencies for local development and checkout correct versions.

echo "⏳ Initializing development environment..."
export DEPS_PATH="$PWD/dependencies"

#-------------------
clean_build_files() {
  # Args: $1 = path to folder.
  find "$1" \( -type d \( -name "bin" -o -name "obj" \) -exec rm -rf {} + \) -o \
    \( -type f \( -name "*.dll" -o -name "*.exe" \) -delete \)
}

#-------------------
# Clone Cecil
# https://github.com/jbevain/cecil/tags
if [ ! -d "$DEPS_PATH/cecil" ]; then
    echo "📦 Cloning Cecil..."
    git -C "$DEPS_PATH" clone https://github.com/jbevain/cecil.git
    (cd $DEPS_PATH/cecil && \
    git checkout 98ec890d44643ad88d573e97be0e120435eda732) # v0.10.4
    clean_build_files "$DEPS_PATH/cecil"
else
    echo "✅ Cecil already installed."
fi

# Clone MonoMod
# https://github.com/MonoMod/MonoMod/tags
if [ ! -d "$DEPS_PATH/MonoMod" ]; then
    echo "📦 Cloning MonoMod..."
    git -C "$DEPS_PATH" clone https://github.com/MonoMod/MonoMod.git
    (cd $DEPS_PATH/MonoMod && \
    git checkout --detach 68cf23127bd2394004e8a812b160a0862c95a309 && \
    git submodule update --init --recursive) # v22.01.29.01
    clean_build_files "$DEPS_PATH/MonoMod"
else
    echo "✅ MonoMod already installed."
fi

# Clone HarmonyX
# https://github.com/BepInEx/HarmonyX/tags
if [ ! -d "$DEPS_PATH/HarmonyX" ]; then
    echo "📦 Cloning HarmonyX..."
    git -C "$DEPS_PATH" clone https://github.com/BepInEx/HarmonyX.git
    (cd $DEPS_PATH/HarmonyX && \
    git checkout 31d794f3affce55fa87c99efac7dae23a126cf52) # v2.9.0
    clean_build_files "$DEPS_PATH/HarmonyX"
else
    echo "✅ HarmonyX already installed."
fi

# Clone BepInEx v5 (LTS, the one Valheim use)
# https://github.com/AzumattDev/BepInEx
if [ ! -d "$DEPS_PATH/BepInEx" ]; then
    echo "📦 Cloning BepInEx..."
    # Vi klonar specifikt v5-lts branch och drar med HarmonyX, MonoMod osv automatiskt via --recursive
    git -C "$DEPS_PATH" clone https://github.com/AzumattDev/BepInEx.git
    (cd $DEPS_PATH/BepInEx && \
    git checkout --detach ef506e0a6bb98c49d85b7927b5ab625605826be0 && \
    git submodule update --init --recursive) # v5.4.2350
    clean_build_files "$DEPS_PATH/BepInEx"
else
    echo "✅ BepInEx already installed."
fi

# Clone BepInEx.Analyzers
# https://github.com/BepInEx/BepInEx.Analyzers/tags
if [ ! -d "$DEPS_PATH/BepInEx.Analyzers" ]; then
    git -C "$DEPS_PATH" clone https://github.com/BepInEx/BepInEx.Analyzers.git
    (cd $DEPS_PATH/BepInEx.Analyzers && \
    git checkout 141db9e2942f103c4c6a4ab21cb9de00fc6d5e9c) # v1.0.8
    clean_build_files "$DEPS_PATH/BepInEx.Analyzers"
else
    echo "✅ BepInEx.Analyzers already installed."
fi

# Clone BepInEx.ConfigurationManager
# https://github.com/BepInEx/BepInEx.ConfigurationManager/tags
if [ ! -d "$DEPS_PATH/BepInEx.ConfigurationManager" ]; then
    git -C "$DEPS_PATH" clone https://github.com/BepInEx/BepInEx.ConfigurationManager.git
    (cd $DEPS_PATH/BepInEx.ConfigurationManager && \
    git checkout d783c04b569284c5fbe94cf88ea49a6573185d36) # v19.0
    clean_build_files "$DEPS_PATH/BepInEx.ConfigurationManager"
else
    echo "✅ BepInEx.ConfigurationManager already installed."
fi

# Clone Jotunn
# https://github.com/Valheim-Modding/Jotunn/tags
if [ ! -d "$DEPS_PATH/Jotunn" ]; then
    echo "📦 Cloning Jotunn..."
    git -C "$DEPS_PATH" clone https://github.com/Valheim-Modding/Jotunn.git
    (cd $DEPS_PATH/Jotunn && \
    git checkout 5d5043db1e2bce00172aa79cba378b4524012081) # v2.30.2
    clean_build_files "$DEPS_PATH/Jotunn"
else
    echo "✅ Jotunn already installed."
fi

# Clone UnityDoorstop v4.5.0 for Windows/Valheim
# https://github.com/NeighTools/UnityDoorstop/tags
if [ ! -d "$DEPS_PATH/UnityDoorstop" ]; then
    echo "📦 Cloning UnityDoorstop..."
    git -C "$DEPS_PATH" clone https://github.com/NeighTools/UnityDoorstop.git
    (cd $DEPS_PATH/UnityDoorstop && \
    git checkout --detach 33dab9a6733862eb81869ff08431d9478b28784b) # v4.5.0
    clean_build_files "$DEPS_PATH/UnityDoorstop"
else
    echo "✅ UnityDoorstop already installed."
fi

echo "🎉 All repositories initialized."
