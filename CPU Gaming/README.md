# CPU Gaming

A simple tool to automatically set CPU affinity and priority for selected processes, ensuring better performance for specific applications and games.

## Features
- Automatically detects and lists running processes with visible windows
- Allows configuring CPU affinity and priority for specific applications
- Saves and loads configuration automatically
- Runs in the system tray for seamless background operation
- Designed for gaming optimization, especially for older titles

## Requirements
- Windows Only
- .NET 8.0 Runtime
- Administrator privileges (required for process modifications)

## Building
1. Install **Visual Studio 2022**
2. Clone the repository
3. Open the project in **Visual Studio**
4. Click **Build** (no extra configuration needed)

## Usage
1. Run **CPU Gaming.exe** with Administrator privileges
2. Select an application from the **Open Windows** list
3. Configure its **CPU affinity** and **priority**
4. The process will be automatically adjusted each time it runs
5. The configuration is saved and will be applied automatically in the future

## Setting Priority
1. Right-click an item in the **Free Windows** list.
2. Navigate to **Select Priority** and choose the desired priority level.
3. The context menu remains open after selection for easier adjustments.

## Setting Affinity
1. Right-click an item in the **Free Windows** list.
2. Navigate to **Select Affinity** and choose the CPU cores.
3. Selecting **All Processors** will select all cores if they are not already selected.
4. If all cores are selected, choosing **All Processors** will select only **Core 0**.
5. The context menu remains open after selection for easier adjustments.

## Saving and Removing Configuration
- Changing any option on the **Free Windows** list will automatically save the configuration for future use.
- To remove a saved configuration:
  1. Right-click the item in the **Configured Windows** list.
  2. Click **Reset**.
  3. The item will be removed from the saved list and returned to **Free Windows**.
  4. The program will no longer set affinity and priority for that process.

## Example Use Case
If a game like **LEGO Star Wars: The Complete Saga** runs poorly on multiple cores, you can set it to run only on **CPU 0** automatically using this tool.

## License
This project is licensed under the MIT License.