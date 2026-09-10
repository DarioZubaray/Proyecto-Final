use Trabajo_Final;

-- =============================================
-- MIGRACION v1.1.0: Aulas, Cursos e Inscripciones
-- Create tables + Alter tables + Seed data + Fixes
-- =============================================

-- 1. Verificar si esta migracion ya fue aplicada
IF NOT EXISTS (SELECT 1 FROM [dbo].[SchemaVersions] WHERE [Version] = '1.1.0')
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY

        -- =============================================
        -- 2. Tabla Aulas (solo si no existe)
        -- =============================================
        IF OBJECT_ID('dbo.Aulas', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[Aulas] (
                [Id]          INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
                [Nombre]      NVARCHAR(100)  NOT NULL,
                [Capacidad]   INT            NOT NULL,
                [Is_Active]   BIT            NOT NULL DEFAULT 1,
                [Created_At]  DATETIME       NOT NULL DEFAULT GETDATE(),
                [Last_Update] DATETIME       NOT NULL DEFAULT GETDATE()
            );
        END;

        -- =============================================
        -- 3. Tabla Cursos (solo si no existe)
        -- =============================================
        IF OBJECT_ID('dbo.Cursos', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[Cursos] (
                [Id]            INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
                [Nombre]        NVARCHAR(200)  NOT NULL,
                [Descripcion]   NVARCHAR(500)  NULL,
                [Fecha_Inicio]  DATETIME       NOT NULL,
                [Fecha_Fin]     DATETIME       NOT NULL,
                [Aula_Id]       INT            NOT NULL,
                [Is_Active]     BIT            NOT NULL DEFAULT 1,
                [Created_At]    DATETIME       NOT NULL DEFAULT GETDATE(),
                [Last_Update]   DATETIME       NOT NULL DEFAULT GETDATE(),
                CONSTRAINT fk_cursos_aulas FOREIGN KEY ([Aula_Id]) REFERENCES [dbo].[Aulas]([Id])
            );
        END;
        ELSE
        BEGIN
            -- Si la tabla existe pero Aula_Id es NULL, actualizar datos y cambiar a NOT NULL
            IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cursos') AND name = 'Aula_Id' AND is_nullable = 1)
            BEGIN
                UPDATE [dbo].[Cursos]
                SET [Aula_Id] = 1
                WHERE [Aula_Id] IS NULL;

                ALTER TABLE [dbo].[Cursos]
                    ALTER COLUMN [Aula_Id] INT NOT NULL;
            END;
        END;

        -- =============================================
        -- 4. Agregar campos de horario a Cursos (si no existen)
        -- IMPORTANTE: debe ir antes de cualquier INSERT/UPDATE que use estas columnas
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cursos') AND name = 'dia_semana')
        BEGIN
            ALTER TABLE [dbo].[Cursos]
                ADD [dia_semana]  INT  NULL,
                    [hora_inicio] TIME NULL,
                    [hora_fin]    TIME NULL;
        END;

        -- =============================================
        -- 5. Tabla CursoDocentes (N:N, solo si no existe)
        -- =============================================
        IF OBJECT_ID('dbo.CursoDocentes', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[CursoDocentes] (
                [Id]          INT      NOT NULL PRIMARY KEY IDENTITY(1,1),
                [Curso_Id]    INT      NOT NULL,
                [Docente_Id]  INT      NOT NULL,
                [Is_Active]   BIT      NOT NULL DEFAULT 1,
                [Created_At]  DATETIME NOT NULL DEFAULT GETDATE(),
                CONSTRAINT fk_cursodocentes_cursos   FOREIGN KEY ([Curso_Id])   REFERENCES [dbo].[Cursos]([Id]),
                CONSTRAINT fk_cursodocentes_users    FOREIGN KEY ([Docente_Id]) REFERENCES [dbo].[Users]([Id])
            );
        END;

        -- =============================================
        -- 6. Tabla CursoAlumnos (inscripciones, solo si no existe)
        -- =============================================
        IF OBJECT_ID('dbo.CursoAlumnos', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[CursoAlumnos] (
                [Id]          INT      NOT NULL PRIMARY KEY IDENTITY(1,1),
                [Curso_Id]    INT      NOT NULL,
                [Alumno_Id]   INT      NOT NULL,
                [Is_Active]   BIT      NOT NULL DEFAULT 1,
                [Created_At]  DATETIME NOT NULL DEFAULT GETDATE(),
                CONSTRAINT fk_cursoalumnos_cursos FOREIGN KEY ([Curso_Id])  REFERENCES [dbo].[Cursos]([Id]),
                CONSTRAINT fk_cursoalumnos_users  FOREIGN KEY ([Alumno_Id]) REFERENCES [dbo].[Users]([Id])
            );
        END;

        -- =============================================
        -- 7. Permiso FORM_CURSO_MGMT
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [name] = 'FORM_CURSO_MGMT')
        BEGIN
            INSERT INTO [dbo].[Permissions] (name, label, description, is_system) VALUES
            ('FORM_CURSO_MGMT', 'ABM Cursos', 'Formulario de gestion de cursos', 0);
        END;

        -- =============================================
        -- 8. Permiso FORM_INSCRIPCION_MGMT
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [name] = 'FORM_INSCRIPCION_MGMT')
        BEGIN
            INSERT INTO [dbo].[Permissions] (name, label, description, is_system) VALUES
            ('FORM_INSCRIPCION_MGMT', 'Inscripciones', 'Gestion de inscripciones de alumnos a cursos', 0);
        END;

        -- =============================================
        -- 9. Rol Coordinador
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [name] = 'Coordinador')
        BEGIN
            INSERT INTO [dbo].[Roles] (name) VALUES ('Coordinador');
        END;

        -- =============================================
        -- 10. Asignacion de permisos
        -- =============================================

        -- Admin (1): agregar permisos de cursos e inscripciones
        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermissions] WHERE [role_id] = 1 AND [permission_id] = (SELECT id FROM [dbo].[Permissions] WHERE name = 'FORM_CURSO_MGMT'))
        BEGIN
            INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
            SELECT 1, id FROM [dbo].[Permissions] WHERE name = 'FORM_CURSO_MGMT';
        END;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermissions] WHERE [role_id] = 1 AND [permission_id] = (SELECT id FROM [dbo].[Permissions] WHERE name = 'FORM_INSCRIPCION_MGMT'))
        BEGIN
            INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
            SELECT 1, id FROM [dbo].[Permissions] WHERE name = 'FORM_INSCRIPCION_MGMT';
        END;

        -- Coordinador (4): todos los ABMs
        DECLARE @coordinadorId INT = (SELECT id FROM [dbo].[Roles] WHERE name = 'Coordinador');
        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermissions] WHERE [role_id] = @coordinadorId AND [permission_id] = (SELECT id FROM [dbo].[Permissions] WHERE name = 'FORM_CURSO_MGMT'))
        BEGIN
            INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
            SELECT @coordinadorId, id FROM [dbo].[Permissions]
            WHERE name IN ('FORM_USER_MGMT', 'FORM_ROLE_MGMT', 'FORM_CURSO_MGMT', 'FORM_COMPLAINTS', 'FORM_REPORTS');
        END;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermissions] WHERE [role_id] = @coordinadorId AND [permission_id] = (SELECT id FROM [dbo].[Permissions] WHERE name = 'FORM_INSCRIPCION_MGMT'))
        BEGIN
            INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
            SELECT @coordinadorId, id FROM [dbo].[Permissions] WHERE name = 'FORM_INSCRIPCION_MGMT';
        END;

        -- Profesor (2): gestionar cursos e inscripciones
        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermissions] WHERE [role_id] = 2 AND [permission_id] = (SELECT id FROM [dbo].[Permissions] WHERE name = 'FORM_CURSO_MGMT'))
        BEGIN
            INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
            SELECT 2, id FROM [dbo].[Permissions] WHERE name = 'FORM_CURSO_MGMT';
        END;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermissions] WHERE [role_id] = 2 AND [permission_id] = (SELECT id FROM [dbo].[Permissions] WHERE name = 'FORM_INSCRIPCION_MGMT'))
        BEGIN
            INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
            SELECT 2, id FROM [dbo].[Permissions] WHERE name = 'FORM_INSCRIPCION_MGMT';
        END;

        -- Alumno (3): inscripciones
        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermissions] WHERE [role_id] = 3 AND [permission_id] = (SELECT id FROM [dbo].[Permissions] WHERE name = 'FORM_INSCRIPCION_MGMT'))
        BEGIN
            INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
            SELECT 3, id FROM [dbo].[Permissions] WHERE name = 'FORM_INSCRIPCION_MGMT';
        END;

        -- =============================================
        -- 11. Seed data: Aulas
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Aulas])
        BEGIN
            INSERT INTO [dbo].[Aulas] (Nombre, Capacidad, Is_Active, Created_At, Last_Update) VALUES
            ('Aula 101 - Goiescalza',   30, 1, GETDATE(), GETDATE()),
            ('Aula 102 - Goiescalza',   30, 1, GETDATE(), GETDATE()),
            ('Aula 201 - Goiescalza',   25, 1, GETDATE(), GETDATE()),
            ('Laboratorio 1 - Goiescalza', 20, 1, GETDATE(), GETDATE()),
            ('Aula Magna - Goiescalza',  100, 1, GETDATE(), GETDATE());
        END;

        -- =============================================
        -- 12. Seed data: Cursos (sin columnas de horario)
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Cursos])
        BEGIN
            INSERT INTO [dbo].[Cursos] (Nombre, Descripcion, Fecha_Inicio, Fecha_Fin, Aula_Id, Is_Active, Created_At, Last_Update) VALUES
            ('Ingles Basico A1',    'Curso de ingles para principiantes, nivel A1 del MCER',       '2026-08-01', '2026-12-15', 1, 1, GETDATE(), GETDATE()),
            ('Ingles Intermedio B1', 'Curso de ingles nivel intermedio, preparacion para certificacion', '2026-08-01', '2026-12-15', 2, 1, GETDATE(), GETDATE()),
            ('Frances Inicial A1',   'Introduccion al frances, alfabetizacion y frases basicas',  '2026-08-01', '2026-12-15', 3, 1, GETDATE(), GETDATE()),
            ('Portugues Basico',     'Curso de portugues para hispanohablantes, nivel inicial',   '2026-08-01', '2026-12-15', 4, 1, GETDATE(), GETDATE()),
            ('Italiano A1',          'Curso de italiano para principiantes, conversacion y gramatica', '2026-08-01', '2026-12-15', 1, 1, GETDATE(), GETDATE());
        END;

        -- =============================================
        -- 13. Seed data: Asignacion de docentes a cursos
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[CursoDocentes])
        BEGIN
            INSERT INTO [dbo].[CursoDocentes] (Curso_Id, Docente_Id, Is_Active, Created_At) VALUES
            (1, 7, 1, GETDATE()),
            (1, 8, 1, GETDATE()),
            (2, 9, 1, GETDATE()),
            (3, 10, 1, GETDATE()),
            (4, 11, 1, GETDATE()),
            (4, 7, 1, GETDATE()),
            (5, 8, 1, GETDATE());
        END;

        -- =============================================
        -- 14. Actualizar cursos con horarios (dynamic SQL
        -- para evitar error de parse-time en batch)
        -- =============================================
        IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cursos') AND name = 'dia_semana')
        BEGIN
            EXEC sp_executesql N'
                UPDATE [dbo].[Cursos] SET [dia_semana] = 1, [hora_inicio] = ''08:00'', [hora_fin] = ''10:00'' WHERE [Nombre] = ''Ingles Basico A1'' AND [dia_semana] IS NULL;
                UPDATE [dbo].[Cursos] SET [dia_semana] = 3, [hora_inicio] = ''10:00'', [hora_fin] = ''12:00'' WHERE [Nombre] = ''Ingles Intermedio B1'' AND [dia_semana] IS NULL;
                UPDATE [dbo].[Cursos] SET [dia_semana] = 2, [hora_inicio] = ''14:00'', [hora_fin] = ''16:00'' WHERE [Nombre] = ''Frances Inicial A1'' AND [dia_semana] IS NULL;
                UPDATE [dbo].[Cursos] SET [dia_semana] = 4, [hora_inicio] = ''18:00'', [hora_fin] = ''20:00'' WHERE [Nombre] = ''Portugues Basico'' AND [dia_semana] IS NULL;
                UPDATE [dbo].[Cursos] SET [dia_semana] = 5, [hora_inicio] = ''09:00'', [hora_fin] = ''11:00'' WHERE [Nombre] = ''Italiano A1'' AND [dia_semana] IS NULL;
            ';
        END;

        -- =============================================
        -- 15. Registrar migracion aplicada
        -- =============================================
        INSERT INTO [dbo].[SchemaVersions] (Version, ScriptName, AppliedAt)
        VALUES ('1.1.0', '03_v1.1.0_AulasCursosInscripciones.sql', GETDATE());

        COMMIT TRANSACTION;

        PRINT 'Migracion v1.1.0 aplicada exitosamente.';

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        PRINT 'Error al aplicar migracion v1.1.0: ' + ERROR_MESSAGE();
        THROW;
    END CATCH;
END
ELSE
BEGIN
    PRINT 'La migracion v1.1.0 ya fue aplicada anteriormente.';
END;
