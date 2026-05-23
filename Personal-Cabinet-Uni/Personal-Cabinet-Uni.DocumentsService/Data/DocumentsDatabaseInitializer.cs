using Microsoft.EntityFrameworkCore;

namespace Personal_Cabinet_Uni.DocumentsService.Data;

public class DocumentsDatabaseInitializer : IDocumentsDatabaseInitializer
{
    private readonly DocumentsDbContext _context;

    public DocumentsDatabaseInitializer(DocumentsDbContext context)
    {
        _context = context;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            """
            CREATE SCHEMA IF NOT EXISTS document_service;

            CREATE TABLE IF NOT EXISTS document_service.documents (
                id uuid PRIMARY KEY,
                owner_email varchar(256) NOT NULL,
                document_type varchar(32) NOT NULL,
                name varchar(256) NOT NULL,
                original_file_name varchar(512) NOT NULL,
                content_type varchar(128) NOT NULL,
                relative_path varchar(1024) NOT NULL,
                size bigint NOT NULL,
                passport_series varchar(32),
                passport_number varchar(64),
                birth_place varchar(256),
                issued_at timestamp with time zone,
                issued_by varchar(512),
                education_document_name varchar(256),
                education_level_name varchar(256),
                created_at timestamp with time zone NOT NULL,
                updated_at timestamp with time zone
            );

            CREATE INDEX IF NOT EXISTS ix_documents_owner_email
                ON document_service.documents(owner_email);

            CREATE INDEX IF NOT EXISTS ix_documents_owner_email_document_type
                ON document_service.documents(owner_email, document_type);
            """,
            cancellationToken);
    }
}
