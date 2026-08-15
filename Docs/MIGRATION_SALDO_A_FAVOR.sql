-- ==========================================
-- Migración: Agregar campo SaldoAFavor
-- Fecha: 2026-08-01
-- Descripción: Agrega columna saldoAFavor para
-- manejar créditos en downgrade de planes
-- ==========================================

-- Verificar si la columna ya existe
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'ProveedorPlan' 
    AND COLUMN_NAME = 'saldoAFavor'
)
BEGIN
    -- Agregar columna con valor por defecto 0
    ALTER TABLE ProveedorPlan 
    ADD saldoAFavor DECIMAL(10,2) NOT NULL DEFAULT 0;

    PRINT 'Columna saldoAFavor agregada exitosamente';
END
ELSE
BEGIN
    PRINT 'La columna saldoAFavor ya existe';
END
GO

-- Verificar estructura
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    CHARACTER_MAXIMUM_LENGTH,
    NUMERIC_PRECISION,
    NUMERIC_SCALE,
    COLUMN_DEFAULT,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'ProveedorPlan'
ORDER BY ORDINAL_POSITION;
GO
