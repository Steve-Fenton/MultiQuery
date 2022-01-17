using CsvHelper;
using CsvHelper.Configuration;
using System.Data.SqlClient;
using System.Dynamic;
using System.Globalization;

namespace Fenton.MultiQuery
{
    public class QueryRunner
    {
        private readonly IEnumerable<string> _connectionStrings;
        private readonly string _query;
        private readonly IList<string> _fields;

        public QueryRunner(IEnumerable<string> connectionStrings, string query, IList<string> fields)
        {
            _connectionStrings = connectionStrings;
            _query = query;
            _fields = fields;
        }

        public void ToCsv(string path)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
            };

            using var writer = new StreamWriter(path);
            using var csv = new CsvWriter(writer, config);

            csv.WriteRecords(Run());

            writer.Flush();
        }

        public IEnumerable<dynamic> Run()
        {
            int source = 0;

            foreach (string connectionString in _connectionStrings)
            {
                source++;
                string sourceName = $"source_{source}";

                using SqlConnection connection = new SqlConnection(connectionString);
                using SqlCommand command = new SqlCommand(_query, connection);

                try
                {
                    connection.Open();

                    using SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        dynamic kvp = MapToExpandoObject(reader, sourceName);

                        yield return kvp;
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private dynamic MapToExpandoObject(SqlDataReader reader, string sourceName)
        {
            dynamic kvp = new ExpandoObject();

            Add(kvp, "__source", sourceName);

            foreach (string field in _fields)
            {
                Add(kvp, field, reader[field].ToString() ?? string.Empty);
            }

            return kvp;
        }

        public static void Add(ExpandoObject obj, string key, object value)
        {
            (obj as IDictionary<string, object>).Add(key, value);
        }
    }
}