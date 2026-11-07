USE DarioZubaray_TF;
GO

-- =============================================
-- SEED DEMO v1.0.0: Datos de prueba
--
-- NO forma parte de la cadena de instalacion
-- (instalador: 01 -> 03). Se ejecuta a mano en
-- entornos de desarrollo / demostracion para
-- poblar reportes y ABMs sin cargar datos a mano.
--
-- Requiere haber corrido 01_v1.0.0_CreateTables.sql
-- (roles, permisos y usuario admin por defecto).
--
-- Idempotente por bloque: puede re-ejecutarse.
-- =============================================

-- Hash BCrypt para password "123": $2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy
-- Hash BCrypt para password "admin" (usuario admin, ver 01): $2a$11$h9OQXeWvXFC29T2cytB8HemgdlZQG35mnA6bmyZMTx3RNOMahmI0G

-- Usuario dev (rol 1)
IF NOT EXISTS (SELECT 1 FROM [dbo].[users] WHERE [user_name] = 'dario')
    INSERT INTO [dbo].[users] (user_name, password_hash, is_active, retries_count, last_update, created_at, role_id, language, theme) VALUES
    ('dario',  '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 1, 'es', 'System');

-- Coordinadores (rol 4)
IF NOT EXISTS (SELECT 1 FROM [dbo].[users] WHERE [user_name] = 'coord_maria')
    INSERT INTO [dbo].[users] (user_name, password_hash, is_active, retries_count, last_update, created_at, role_id, language, theme) VALUES
    ('coord_maria',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 4, 'es', 'System'),
    ('coord_carlos',  '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 4, 'es', 'System'),
    ('coord_laura',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 4, 'es', 'System');

-- Docentes / Profesores (rol 2)
IF NOT EXISTS (SELECT 1 FROM [dbo].[users] WHERE [user_name] = 'prof_garcia')
    INSERT INTO [dbo].[users] (user_name, password_hash, is_active, retries_count, last_update, created_at, role_id, language, theme) VALUES
    ('prof_garcia',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 2, 'es', 'System'),
    ('prof_lopez',    '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 2, 'es', 'System'),
    ('prof_martinez', '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 2, 'es', 'System'),
    ('prof_rodriguez','$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 2, 'es', 'System'),
    ('prof_fernandez','$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 2, 'es', 'System');

-- Alumnos (rol 3). Solo si el rol Alumno todavia existe en el
-- id 3 original: despues de correr 03 los alumnos son la entidad
-- students (15 filas) y el rol fue eliminado; re-ejecutar este
-- bloque violaria la FK de users.role_id. En bases renumeradas
-- (05) el id 3 es otro rol y el seed se omite.
IF EXISTS (SELECT 1 FROM [dbo].[roles] WHERE [id] = 3 AND [name] = 'Alumno')
IF NOT EXISTS (SELECT 1 FROM [dbo].[users] WHERE [user_name] = 'alumno_perez')
    INSERT INTO [dbo].[users] (user_name, password_hash, is_active, retries_count, last_update, created_at, role_id, language, theme) VALUES
    ('alumno_perez',     '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 0, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_gomez',     '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 0, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_diaz',      '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 0, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_morales',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 0, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_torres',    '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 0, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_ramos',     '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 0, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_silva',     '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 0, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_castro',    '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 0, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_ruiz',      '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 0, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_alvarez',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 0, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_romero',    '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 0, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_sanchez',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 0, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_herrera',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 0, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_patricio',  '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 0, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_soto',      '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 0, 0, GETDATE(), GETDATE(), 3, 'es', 'System');

-- Registrar migracion
IF NOT EXISTS (
    SELECT 1 FROM [dbo].[schema_versions]
    WHERE [script_name] = '02_v1.0.0_SeedDemoData.sql'
)
    INSERT INTO [dbo].[schema_versions] (version, script_name, applied_at)
    VALUES ('1.0.0', '02_v1.0.0_SeedDemoData.sql', GETDATE());
