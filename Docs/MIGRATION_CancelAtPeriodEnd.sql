-- =============================================
-- Migración: Agregar campo CancelAtPeriodEnd
-- Descripción: Campo para controlar cancelación al final del período
-- Fecha: 2026-08-09
-- =============================================

-- Verificar si la columna ya existe
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('ProveedorPlan') 
    AND name = 'cancelAtPeriodEnd'
)
BEGIN
    -- Agregar la columna
    ALTER TABLE ProveedorPlan
    ADD cancelAtPeriodEnd BIT NOT NULL DEFAULT 0;

    PRINT 'Columna cancelAtPeriodEnd agregada exitosamente';
END
ELSE
BEGIN
    PRINT 'La columna cancelAtPeriodEnd ya existe';
END
GO