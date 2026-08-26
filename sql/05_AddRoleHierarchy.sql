use Trabajo_Final;

-- Tabla de jerarquía de roles (padre -> hijo)
-- Permite que un rol contenga sub-roles (patrón Composite)
CREATE TABLE [dbo].[RoleHierarchy] (
    [parent_role_id] INT NOT NULL,
    [child_role_id]  INT NOT NULL,
    PRIMARY KEY ([parent_role_id], [child_role_id]),
    CONSTRAINT fk_rolehierarchy_parent FOREIGN KEY ([parent_role_id]) REFERENCES [dbo].[Roles]([id]),
    CONSTRAINT fk_rolehierarchy_child  FOREIGN KEY ([child_role_id])  REFERENCES [dbo].[Roles]([id])
);

-- Seed: Admin contiene Supervisor, Supervisor contiene Operador
INSERT INTO [dbo].[RoleHierarchy] (parent_role_id, child_role_id) VALUES
(1, 2),  -- Admin -> Supervisor
(2, 3);  -- Supervisor -> Operador
