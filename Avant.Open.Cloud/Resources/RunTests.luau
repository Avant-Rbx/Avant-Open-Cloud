--Runs tests with Avant.
--!strict

--Find the Avant runtime.
local AvantRuntimeFolder: Folder? = nil
for _, AvantRuntimeFolderCandidate in game:GetDescendants() do
	if AvantRuntimeFolderCandidate.Name ~= "AvantRuntime{RuntimeSuffix}" then continue end
	if not AvantRuntimeFolderCandidate:FindFirstChild("RunTests") then continue end
	AvantRuntimeFolder = AvantRuntimeFolderCandidate
	AvantRuntimeFolder.Name = "AvantRuntime"
	break
end
if not AvantRuntimeFolder then
	error("Unable to find Avant runtime.")
end

--Run the tests.
local TestResults = require(AvantRuntimeFolder:FindFirstChild("RunTests"))() :: any
if TestResults.FAILED > 0 then
	error(`There were {TestResults.FAILED} test failure(s).`)
end