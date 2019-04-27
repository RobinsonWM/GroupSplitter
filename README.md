# GroupSplitter

A tool for splitting a list of individuals into smaller groupings.

This tool is useful when individuals need to split into smaller groupings on regular basis.  It keeps track of history and uses a scoring system to group individuals with partners they have not grouped with recently.

This code is written as a .NET Standard assembly, and it also includes a small CLI wrapper.  The CLI wrapper can be considered a reference implementation for using the .NET assembly.  Individuals are identified by a string, which should typically just be their name.  The sample `.json` files in the `GroupSplitter.Cli` project use single letters to identify individuals, but this is only intended for debugging use.

# Input files for the CLI tool

The CLI tool requires three files to be in the working directory.  The `History.json` file is updated each time the tool is run.  The CLI tool reads all its inputs from hardcoded filenames; it does not accept any command line arguments.

`Individuals.json` should contain a JSON array of strings that identify the individuals to be grouped, for example:
```json
[ "PersonA", "PersonB", "PersonC" ]
```

`Roommates.json` should contain a JSON array or JSON arrays of individuals who should not be grouped together (for example, people who already see each other frequently), for example:
```json
[
    [ "PersonA", "PersonD" ],
    [ "PersonC", "PersonG", "PersonI" ]
]
```

`History.json` should contain a JSON array of occurrences where the individuals have met in the past.  The file is required, but if there is no history, it can be an empty array.  This example shows how the file should look:
```json
[
    {
        "Date": "2019-04-26",
        "Groupings": [
            [ "PersonA", "PersonB" ],
            [ "PersonC", "PersonD" ],
            [ "PersonE", "PersonF", "PersonG" ]
        ]
    }
]
```

# Building

1. Install the [.NET Core SDK](https://dotnet.microsoft.com/download) at least version 2.2.
1. Run `dotnet build` from the root of the repository
1. To run tests, run `dotnet test` from the root of the repository