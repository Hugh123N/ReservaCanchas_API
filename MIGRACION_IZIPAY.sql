-- ========================================
-- MIGRACIÓN: INTEGRACIÓN CON IZIPAY
-- ========================================
-- Fecha: 2025-10-31
-- Descripción: Agrega campos necesarios para la integración con Izipay
--              para pagos con Yape y Plin mediante pasarela de pago oficial
-- ========================================

USE [TuBaseDeDatos]; -- ⚠️ CAMBIA ESTO POR EL NOMBRE DE TU BASE DE DATOS
GO

-- ========================================
-- 1. AGREGAR CAMPOS A LA TABLA PAGO
-- ========================================

-- Verificar si la columna ya existe antes de agregarla
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[Pago]')
    AND name = 'IzipayTransactionId'
)
BEGIN
    ALTER TABLE [dbo].[Pago]
    ADD [IzipayTransactionId] NVARCHAR(100) NULL;

    PRINT '✅ Campo IzipayTransactionId agregado correctamente';
END
ELSE
BEGIN
    PRINT '⏩ Campo IzipayTransactionId ya existe, omitiendo...';
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[Pago]')
    AND name = 'IzipayFormToken'
)
BEGIN
    ALTER TABLE [dbo].[Pago]
    ADD [IzipayFormToken] NVARCHAR(500) NULL;

    PRINT '✅ Campo IzipayFormToken agregado correctamente';
END
ELSE
BEGIN
    PRINT '⏩ Campo IzipayFormToken ya existe, omitiendo...';
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[Pago]')
    AND name = 'IzipayPaymentUrl'
)
BEGIN
    ALTER TABLE [dbo].[Pago]
    ADD [IzipayPaymentUrl] NVARCHAR(500) NULL;

    PRINT '✅ Campo IzipayPaymentUrl agregado correctamente';
END
ELSE
BEGIN
    PRINT '⏩ Campo IzipayPaymentUrl ya existe, omitiendo...';
END
GO

-- ========================================
-- 2. CREAR ÍNDICE PARA OPTIMIZAR BÚSQUEDAS
-- ========================================

-- Índice para buscar pagos por TransactionId de Izipay
-- Esto acelera la búsqueda cuando llega un webhook de Izipay
IF NOT EXISTS (
    SELECT * FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'[dbo].[Pago]')
    AND name = 'IX_Pago_IzipayTransactionId'
)
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Pago_IzipayTransactionId]
    ON [dbo].[Pago] ([IzipayTransactionId])
    WHERE [IzipayTransactionId] IS NOT NULL;

    PRINT '✅ Índice IX_Pago_IzipayTransactionId creado correctamente';
END
ELSE
BEGIN
    PRINT '⏩ Índice IX_Pago_IzipayTransactionId ya existe, omitiendo...';
END
GO

-- ========================================
-- 3. VERIFICACIÓN
-- ========================================

-- Mostrar estructura actualizada de la tabla Pago
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Pago'
AND COLUMN_NAME LIKE 'Izipay%'
ORDER BY ORDINAL_POSITION;
GO

PRINT '';
PRINT '========================================';
PRINT '✅ MIGRACIÓN COMPLETADA EXITOSAMENTE';
PRINT '========================================';
PRINT '';
PRINT 'Campos agregados a la tabla Pago:';
PRINT '  - IzipayTransactionId (NVARCHAR(100))';
PRINT '  - IzipayFormToken (NVARCHAR(500))';
PRINT '  - IzipayPaymentUrl (NVARCHAR(500))';
PRINT '';
PRINT 'Índice creado:';
PRINT '  - IX_Pago_IzipayTransactionId';
PRINT '';
PRINT 'PRÓXIMOS PASOS:';
PRINT '1. Actualizar appsettings.json con credenciales de Izipay';
PRINT '2. Configurar Webhook URL en el BackOffice de Izipay';
PRINT '3. Probar con credenciales de prueba antes de producción';
PRINT '';
GO
