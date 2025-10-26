"""
TheNexusAvenger

Builds Avant Open Cloud for distribution.
"""

import os
import platform
import re
import shutil
import subprocess
import urllib.request

FILE_EXTENSIONS_TO_CLEAR = {
    ".pdb",
    ".dbg",
}
BUILD_PLATFORMS = {
    "Windows": [
        {
            "name": "Windows-x64", 
            "runtime": "win-x64",
        },
    ],
    "Darwin": [
        {
            "name": "macOS-x64", 
            "runtime": "osx-x64",
        },
        {
            "name": "macOS-ARM64", 
            "runtime": "osx-arm64",
        },
    ],
    "Linux": [
        {
            "name": "Linux-x64", 
            "runtime": "linux-x64",
        },
    ]
}


# Print if the platform is unsupported.
if platform.system() not in BUILD_PLATFORMS.keys():
    print("Unsupported platform: " + platform.system())
    exit(1)

# Determine the Avant Runtime version tag.
with open("Avant.Open.Cloud/Action/RojoBuild.cs") as file:
    avantRuntimeTag = re.findall(r"AvantRuntimeTag = \"([^\"]+)\"", file.read())[0]

# Download Avant Runtime.
avantRuntimeUrl = "https://github.com/Avant-Rbx/Avant-Runtime/releases/download/V.1.3.0/AvantRuntime.rbxmx"
print("Downloading Avant Runtime from " + avantRuntimeUrl)
urllib.request.urlretrieve(avantRuntimeUrl, "Avant.Open.Cloud/Resources/AvantRuntime.rbxmx")

# Create the output directory.
if os.path.exists("bin"):
    shutil.rmtree("bin")
os.mkdir("bin")

# Build the platforms.
for buildPlatform in BUILD_PLATFORMS[platform.system()]:
    platformName = buildPlatform["name"]
    platformRuntime = buildPlatform["runtime"]

    # Compile the project.
    print("Building for " + platformName)
    subprocess.call(["dotnet", "publish", "-r", platformRuntime, "-c", "Release", "Avant.Open.Cloud/Avant.Open.Cloud.csproj"])

    # Clear the unwanted files.
    buildPath = "Avant.Open.Cloud/bin/Release/"
    buildPath += os.listdir(buildPath)[0] + "/" + platformRuntime + "/publish/"
    for fileName in os.listdir(buildPath):
        for extension in FILE_EXTENSIONS_TO_CLEAR:
            if fileName.endswith(extension):
                os.remove(buildPath + fileName)
                break
    
    # Create the archive.
    shutil.make_archive("bin/Avant-Open-Cloud-" + platformName, "zip", buildPath)
