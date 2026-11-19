-- Ejecutar en la base de ReservaCanchas antes de habilitar la gestión Admin.
-- No modifica proveedores, canchas, reservas ni pagos existentes.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF NOT EXISTS (SELECT 1 FROM dbo.EstadoProveedor WITH (UPDLOCK, HOLDLOCK) WHERE codigo = '04')
    INSERT INTO dbo.EstadoProveedor (codigo, nombre, descripcion, activo)
    VALUES ('04', 'Suspendido', 'Negocio restringido administrativamente; conserva acceso limitado a reservas existentes.', 1);
COMMIT;
