# godot-dotnet-template
A general template for Godot 4 Mono. Includes:
- Project Settings and General Configuration for Export and Development
- CI/CD builds for Linux and Windows.
- Core System Managers (Video, Audio, Scenes, Input).
- Barebones Main and Pause Menu.
- Miscellaneous Utilities.

Using .NET 8 and the latest stable release of Godot Mono.

## TODO
- ~~Improve Input Management (ensure the configuration file is more readable)~~
- ~~Add Input Settings Functionality~~
- ~~Fix Video Manager's handling of WindowMode~~
- ~~Make Video, Audio, Scene and Input Managers Static~~
- ~~Adjust Configuration's saving functionality to minimize the performance cost of file writes while maintaining atomic writes~~
- Add comments explaining design decisions and weird code
- ~~Refactor code to avoid Exceptions~~
- Profile and Optimize
- Try cloning and creating a new project to identify any potential issues

### Potential TODO
- Create a `setup.sh` script
- Add Unit Testing with automatic testing via Github Actions
- Create plug-in to easily change the logging level
- Incorporate CLI arguments into configuration setup
- Improve build action to include build versions and use rcedit
- Create a documentation system
