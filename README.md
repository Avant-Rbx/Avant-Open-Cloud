# Avant Open Cloud
Avant Open Cloud is a CLI application used for running unit tests using
Roblox's [Open Cloud Luau execution](https://devforum.roblox.com/t/open-cloud-engine-api-for-executing-luau/3172185).
Tests are run using [Avant Runtime](https://github.com/Avant-Rbx/Avant-Runtime).
Framework compatibility can be found in Avant Runtime's README.

For the plugin, see [Avant Plugin](https://github.com/Avant-Rbx/Avant-Plugin).

⚠️ At the moment, only [TestEZ](https://github.com/Roblox/testez) is supported.
See [Avant Runtime](https://github.com/Avant-Rbx/Avant-Runtime)'s README for more.

## Running
### Open Cloud API Key
In order to use Open Cloud, an [https://create.roblox.com/dashboard/credentials?activeTab=ApiKeysTab](API key)
needs to be created with the following permissions for the place to upload test
code to:
- `universe-places:write`
- `universe.place.luau-execution-session:read`
- `universe.place.luau-execution-session:write`

One place can be used between multiple projects and run concurrently, but uploaded
places will appear in the version history.

### Configuration
Within the project, a JSON file needs to be created. `avant.json` is
recommended. It must have the following structure:
- `RojoBuildStrategy`:
    - `ProjectFile`: Relative path to the Rojo project file. It must
      be set up as a place (not a model).
    - `AvantInjectionDirectory` *(Optional)*: Optional parent directory
      for Avant Runtime to be added and removed during builds. If not
      provided, Avant Runtime must be somewhere in the structure.
- `OpenCloud`:
    - `ApiKeyEnvironmentVariable`: Name of the *environment variable*
      containing the Open Cloud API key. For security, the actual API
      key *must not* be stored in this file.
    - `UniverseId`: Id of the universe (game) containing the place to upload
      for running.
    - `PlaceId`: Id of the place to upload for running.

For Avant Runtime, this is an example configuration:
```json
{
    "RojoBuildStrategy": {
        "ProjectFile": "place.project.json",
        "AvantInjectionDirectory": "test/"
    },
    "OpenCloud": {
        "ApiKeyEnvironmentVariable": "MY_API_KEY_ENVIRONMENT_VARIABLE",
        "UniverseId": 123456,
        "PlaceId": 234567
    }
}
```

### Running (Command Line)
With the configuration file named `avant.json`, Avant Open Cloud can
be run with no arguments.
```
AvantOpenCloud
```

For custom locations or different file names, the path can be passed in.
```
AvantOpenCloud ./SomeFile.json
```

Debug logging can be enabled with the `--debug` flag.
```
AvantOpenCloud --debug
AvantOpenCloud ./SomeFile.json --debug
```

### Running (GitHub Action)
Before using the action [a GitHub Action Secret must be created](https://docs.github.com/en/actions/security-for-github-actions/security-guides/using-secrets-in-github-actions)
with the Open Cloud API key created before. The name does not need to
match the variable name in the JSON configuration.

When using `avant.json`, the following can be added to the workflow:
```yaml
- name: Run Avant Open Cloud # Name can be anything.
  uses: Avant-Rbx/Avant-Open-Cloud@V.1.1.0
  env:
    # Left side MUST match the JSON file. Right side MUST match the name of the GitHub Action secret.
    MY_API_KEY_ENVIRONMENT_VARIABLE: ${{ secrets.MY_API_KEY }}
```

For custom file names, add `with` and provide the file path.
```yaml
- name: Run Avant Open Cloud # Name can be anything.
  uses: Avant-Rbx/Avant-Open-Cloud@V.1.1.0
  env:
    # Left side MUST match the JSON file. Right side MUST match the name of the GitHub Action secret.
    MY_API_KEY_ENVIRONMENT_VARIABLE: ${{ secrets.MY_API_KEY }}
  with:
    configuration-file: SomeFile.json
```

## License
Avant Open Cloud is available under the terms of the MIT License. See
[LICENSE](LICENSE) for details.