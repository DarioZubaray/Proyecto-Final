use Trabajo_Final;

-- Agrega columna de preferencia de idioma a la tabla Users
-- Default 'es' (español) para usuarios existentes
ALTER TABLE [dbo].[Users] ADD [language] NVARCHAR(10) NOT NULL DEFAULT 'es';
