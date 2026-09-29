# EatConscious

A simple Avalonia (.NET 8) desktop app for managing ingredients, recipes and a meal plan calendar.

## Running

```bash
dotnet run --project EatConscious
```

Or open `EatConscious.sln` in Visual Studio / Rider and run the `EatConscious` project.

## Sample data

The app keeps its data in three files (`ingredients.json`, `recipes.json` and `mealplan.json`)
in its **working directory**. This is typically `bin/Debug/net8.0` or the directory you run `dotnet run` from.
It loads them on start (missing files mean an empty app) and writes them back when the app closes.

The [`SampleData`](SampleData) folder contains ready-made data to get you started. There are two ways to use it:

**Copy the JSON files** from `SampleData/` into the working directory, e.g.:

```bash
cp SampleData/*.json EatConscious/bin/Debug/net8.0/
```

Existing data files in the target directory are overwritten, so back them up first
if you want to keep them. The build output folder only exists after the first build.
