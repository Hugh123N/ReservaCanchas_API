-- Ejecutar una vez durante el despliegue en la base de ReservaCanchas.
INSERT INTO EstadoProveedor (codigo, nombre, descripcion, activo)
SELECT '04', 'Suspendido', 'Negocio restringido administrativamente; conserva acceso limitado a reservas existentes.', 1
WHERE NOT EXISTS (SELECT 1 FROM EstadoProveedor WHERE codigo = '04');
