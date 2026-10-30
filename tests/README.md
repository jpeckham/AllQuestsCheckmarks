# Local SPT 5.0 checks

Requires .NET 10 SDK, Python 3, the matching installed BE runtime and `git submodule update --init --recursive` for Shared. From this repo root:

```powershell
dotnet build AllQuestsCheckmarksClient/AllQuestsCheckmarksClient.csproj -c Release
dotnet build AllQuestsCheckmarksServer/AllQuestsCheckmarksServer.csproj -c Release
dotnet build tests/RuntimeChecks.csproj -c Release
python tests/server_probe.py --runtime '<game>/SPT_Runtime' --profile '<24-hex-account-id>'
```

Builds accept `-p:SPTPath='<game>'`; default is the local 48484 install. The Python probe requires the server running at https://127.0.0.1:6969 with the server mod installed. It uses that server's self-signed certificate and the local game's compressed/shuffled protocol. It checks real quest JSON and active-item-quest responses without completing or changing quests. On this installation it observed 790 quest definitions and 16 active item quests; counts can vary with profiles/mods.

For native client checks, install the staged production files, then temporarily copy **only** `tests/bin/Release/net10.0/AllQuestsCheckmarks.RuntimeChecks.dll` into `BepInEx/plugins/AllQuestsCheckmarks`. Start server/client using a test account; see parent SPT-BE scripts/README.md for local orchestration. The harness explicitly refreshes quest data after 15 seconds, then checks a populated item cache, independent native MongoID value equality, loaded checkmark sprite, RGBA config serialization and injected Unity Update. It reads data; it does not buy items or complete quests.

Inspect BepInEx/LogOutput.log for PASS/FAIL and plugin errors. A PASS here does not establish the normal character-selection refresh hook, tooltip visuals, stash counts, or active quest progression. Select a character and verify those interactively. Fika is not ported/tested on this branch.

Afterward stop the client and remove AllQuestsCheckmarks.RuntimeChecks.dll, then stop the test server. Keep log captures and profiles outside source control. Never package the harness or game reference assemblies with the production mod.
