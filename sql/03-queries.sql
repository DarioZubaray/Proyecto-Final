use Trabajo_Final;

SELECT * FROM Users
SELECT * FROM Roles

INSERT INTO Users(user_name, password_hash, is_active, retries_count, last_update, created_at, role_id)
VALUES('pepe', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 1, 0, '20260819T21:54:00', '20260819T21:54:00', 1)

INSERT INTO Users(user_name, password_hash, is_active, retries_count, last_update, role_id)
VALUES('admin', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 1, 0, '2026-08-19 21:54:00', 1)

SELECT id, user_name, password_hash, is_active, retries_count, last_update,created_at, role_id
FROM Users 
WHERE user_name = 'pepe' and password_hash = 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3' and is_active = 1
