"""
TheNexusAvenger

Creates the binaries for distribution using Docker.
For Linux, this ensures a lower glibc version can be supported.
"""

import getpass
import os
import platform
import shutil
import subprocess


# Determine the Dockerfile anme.
if platform.system() == "Linux":
    print("Using Linux Dockerfile.")
    dockerfileName = "Dockerfile.linux"
else:
    print("Unsupported platform: " + platform.system())
    exit(1)

# Create the directory.
if os.path.exists("bin"):
    shutil.rmtree("bin")
os.mkdir("bin")

# Build the Dockerfile and run it to copy the bin files.
workingDirectory = os.path.dirname(__file__)
subprocess.Popen(["docker", "build", "-f", dockerfileName, "-t", "avant-open-cloud-build", "."], cwd=workingDirectory).wait()
subprocess.Popen(["docker", "run", "--rm", "-v", "./bin:/publish", "avant-open-cloud-build", "cp", "-a", "/build/bin/.", "/publish/"], cwd=workingDirectory).wait()
