using Microsoft.EntityFrameworkCore;
using Personal_Cabinet_Uni.ExternalInfoService.Services;
using Personal_Cabinet_Uni.Shared.Models.Enums;

namespace Personal_Cabinet_Uni.ExternalInfoService.Data;

public class ExternalInfoDatabaseInitializer : IExternalInfoDatabaseInitializer
{
    private readonly ExternalInfoDbContext _dbContext;

    public ExternalInfoDatabaseInitializer(ExternalInfoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.ExecuteSqlRawAsync($"""
            CREATE SCHEMA IF NOT EXISTS {ExternalInfoDbContext.SchemaName};

            CREATE TABLE IF NOT EXISTS {ExternalInfoDbContext.SchemaName}.education_levels (
                id integer PRIMARY KEY,
                name varchar(500) NOT NULL
            );

            CREATE TABLE IF NOT EXISTS {ExternalInfoDbContext.SchemaName}.faculties (
                id uuid PRIMARY KEY,
                create_time timestamp with time zone NOT NULL,
                name varchar(500) NOT NULL
            );

            CREATE TABLE IF NOT EXISTS {ExternalInfoDbContext.SchemaName}.education_document_types (
                id uuid PRIMARY KEY,
                create_time timestamp with time zone NOT NULL,
                name varchar(500) NOT NULL,
                education_level_id integer NOT NULL REFERENCES {ExternalInfoDbContext.SchemaName}.education_levels(id),
                next_education_level_ids text NOT NULL DEFAULT ''
            );

            CREATE TABLE IF NOT EXISTS {ExternalInfoDbContext.SchemaName}.education_programs (
                id uuid PRIMARY KEY,
                create_time timestamp with time zone NOT NULL,
                name varchar(500) NOT NULL,
                code varchar(100),
                language varchar(100) NOT NULL,
                education_form varchar(100) NOT NULL,
                faculty_id uuid NOT NULL REFERENCES {ExternalInfoDbContext.SchemaName}.faculties(id),
                education_level_id integer NOT NULL REFERENCES {ExternalInfoDbContext.SchemaName}.education_levels(id)
            );

            CREATE TABLE IF NOT EXISTS {ExternalInfoDbContext.SchemaName}.import_statuses (
                dictionary_name varchar(100) PRIMARY KEY,
                status varchar(50) NOT NULL,
                imported_count integer NOT NULL DEFAULT 0,
                error_message text,
                updated_at timestamp with time zone
            );
            """, cancellationToken);

        foreach (var dictionary in ExternalDictionaryNames.All)
        {
            if (!await _dbContext.ImportStatuses.AnyAsync(x => x.DictionaryName == dictionary, cancellationToken))
            {
                _dbContext.ImportStatuses.Add(new()
                {
                    DictionaryName = dictionary,
                    Status = DictionaryImportingStatus.Failed,
                    ErrorMessage = "Импорт еще не запускался",
                    UpdatedAt = null
                });
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
