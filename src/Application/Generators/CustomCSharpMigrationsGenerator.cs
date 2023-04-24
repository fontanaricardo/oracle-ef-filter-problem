using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Oracle.EntityFrameworkCore.Infrastructure.Internal;
using Oracle.EntityFrameworkCore.Migrations;

namespace Application.Generators
{
    public class CustomCSharpMigrationsGenerator : OracleMigrationsSqlGenerator
    {
        public CustomCSharpMigrationsGenerator(MigrationsSqlGeneratorDependencies dependencies, IOracleOptions options) : base(dependencies, options)
        {
        }

        protected override void Generate(CreateIndexOperation operation, IModel? model, MigrationCommandListBuilder builder, bool terminate = true)
        {
            //base.Generate(operation, model, builder, terminate);

            builder.Append("CREATE ");

            if (operation.IsUnique)
            {
                builder.Append("UNIQUE ");
            }

            builder.Append("INDEX ");

            builder
                .Append(DelimitIdentifier(operation.Name))
                .Append(" ON ")
                .Append(DelimitIdentifier(operation.Table, operation.Schema));

            builder.Append(" (");

            var hasFilter = !string.IsNullOrWhiteSpace(operation.Filter);
            var columns = new string[operation.Columns.Length];
            for (int index = 0; index < operation.Columns.Length; index++)
            {
                var columnDelimited = DelimitIdentifier(operation.Columns[index]);

                if (hasFilter)
                {
                    var stringBuilder = new StringBuilder();
                    stringBuilder.Append("CASE WHEN ");
                    stringBuilder.Append(operation.Filter);
                    stringBuilder.Append($" THEN ");
                    stringBuilder.Append(columnDelimited);
                    stringBuilder.Append($" ELSE NULL END");
                    columns[index] = stringBuilder.ToString();

                    continue;
                }

                columns[index] = columnDelimited;
            }

            builder.Append(string.Join(", ", columns));

            builder.Append(")");

            IndexOptions(operation, model, builder);

            if (terminate)
            {
                builder.AppendLine(";");
                EndStatement(builder);
            }
        }

        private string DelimitIdentifier(string identifier) =>
            Dependencies.SqlGenerationHelper.DelimitIdentifier(identifier);

        private string DelimitIdentifier(string name, string? schema) =>
            Dependencies.SqlGenerationHelper.DelimitIdentifier(name, schema);
    }
}