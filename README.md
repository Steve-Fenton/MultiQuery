# MultiQuery

This project was created to handle a legacy scenario where we had individual instances of a SQL database and we wanted 
to be able to run a report across all of them in a simple way.

Multi Query will take a configuration file (JSON) and merge the results from all databases into a single output file.

## Getting Started

### Query Configuration File

To start, you need a configuration file in the location `<drive>:\Temp\mq\input.json`

There is a sample `input.json` file in the solution, but the contents are shown below.

    {
      "Query": "SELECT Id, Title FROM ExampleTable",
      "Fields": [
        "Id",
        "Title"
      ],
      "ConnectionStrings": [
        "Server=.;Database=Example1;Trusted_Connection=True;MultipleActiveResultSets=true",
        "Server=.;Database=Example2;Trusted_Connection=True;MultipleActiveResultSets=true"
      ]
    }

The `Query` needs to be valid across all databases - they don't have to have the exact same schema, but the query must 
work across them all. For example, it might be copies of an identical database schema, or it might be lots of different 
databases that all have a consistent `User` table.

The `Fields` array is used to pull the data into a CSV file from the queries.

The `ConnectionStrings` array is a list of connection strings to execute the query against.

### Command Line

Once you have placed your input.json file at `<drive>:\Temp\mq\input.json` you can run `mq` from the command line

    > mq c

The only argument is the drive letter, if you run the above command, it will read from `c:\Temp\mq\input.json` and write 
to `c:\Temp\mq\output.csv`. You can use a drive letter that suits the machine configuration.

The output will confirm all the file locations used by the process. The below example shows the output for a run against 
the `s` drive on a server.

    MultiQuery>mq s
    Reading query from s:\Temp\mq\input.json
    Creating runner with supplied input
    Writing result to s:\Temp\mq\output.csv

## Tips

### Least Privalege

Use a SQL account with limited permissions (i.e. read only access to non-sensitive data) to ensure the tool is not used to 
export personal information, sensitive data, or change the data in the database.

### Aggregate / Calculated Fields

If you select calculated or aggregated data, give the field an alias, as shown with `NumberOfRows` below.

    SELECT COUNT(1) AS NumberOfRows FROM ExampleTable

Use the name `NumberOfRows` in the `Fields` array (not "COUNT(1)" or "COUNT(1) AS NumberOfRows").

## Troubleshooting

This is an administrative tool, so exception information will be output to assist with any issues.

Common problems would include:

1. No connectivity from the current machine to the database server
2. Query compatibilit issues, for example if a desired field does not exist on one of the target servers

## Under the Hood

The program has been written to enumerate records from the databases into the file, so the results are not all brought into memory.
This should allow reasonable volumes of data to be pulled into the file, but there will be limits.