-- ============================================
-- MIGRACIONES DE BASE DE DATOS
-- Sistema de Reserva de Canchas
-- ============================================

-- ============================================
-- 1. AGREGAR ESTADO "PARCIAL" PARA ADELANTOS
-- ============================================
-- Permite registrar pagos parciales en efectivo

-- Verificar si el estado ya existe
IF NOT EXISTS (SELECT 1 FROM EstadoPago WHERE Codigo = '04')
BEGIN
    INSERT INTO EstadoPago (Codigo, Nombre, Activo, UserNameCreate, CreateDate)
    VALUES (
        '04',
        'Parcial',
        1,
        'SYSTEM',
        GETDATE()
    );

    PRINT '✓ Estado "Parcial" agregado correctamente';
END
ELSE
BEGIN
    PRINT '⚠ Estado "Parcial" ya existe, omitiendo...';
END
GO

-- ============================================
-- 2. AGREGAR CAMPOS PARA ADELANTOS EN TABLA PAGO
-- ============================================
-- MontoAdelanto: Suma de todos los adelantos recibidos
-- MontoPendiente: Monto que falta por pagar

-- Agregar MontoAdelanto
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('Pago')
    AND name = 'MontoAdelanto'
)
BEGIN
    ALTER TABLE Pago
    ADD MontoAdelanto DECIMAL(18,2) NOT NULL DEFAULT 0;

    PRINT '✓ Campo "MontoAdelanto" agregado correctamente';
END
ELSE
BEGIN
    PRINT '⚠ Campo "MontoAdelanto" ya existe, omitiendo...';
END
GO

-- Agregar MontoPendiente
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('Pago')
    AND name = 'MontoPendiente'
)
BEGIN
    ALTER TABLE Pago
    ADD MontoPendiente DECIMAL(18,2) NOT NULL DEFAULT 0;

    PRINT '✓ Campo "MontoPendiente" agregado correctamente';
END
ELSE
BEGIN
    PRINT '⚠ Campo "MontoPendiente" ya existe, omitiendo...';
END
GO

-- ============================================
-- 3. ACTUALIZAR REGISTROS EXISTENTES
-- ============================================
-- Inicializar MontoPendiente con el Monto para pagos pendientes

UPDATE Pago
SET MontoPendiente = Monto,
    MontoAdelanto = 0
WHERE IdEstadoPago IN (
    SELECT IdEstadoPago
    FROM EstadoPago
    WHERE Codigo = '02' -- Pendiente
)
AND MontoPendiente = 0; -- Solo actualizar si no se ha configurado antes

PRINT '✓ Registros existentes actualizados';
GO

-- ============================================
-- 4. CREAR ÍNDICES PARA MEJORAR RENDIMIENTO
-- ============================================

-- Índice para búsquedas por estado de pago
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_Pago_IdEstadoPago'
    AND object_id = OBJECT_ID('Pago')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Pago_IdEstadoPago
    ON Pago(IdEstadoPago)
    INCLUDE (IdReserva, Monto, MontoAdelanto, MontoPendiente);

    PRINT '✓ Índice IX_Pago_IdEstadoPago creado';
END
GO

-- Índice para búsquedas de pagos parciales
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_Pago_Parciales'
    AND object_id = OBJECT_ID('Pago')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Pago_Parciales
    ON Pago(IdEstadoPago, IdReserva)
    WHERE IdEstadoPago IN (
        SELECT IdEstadoPago FROM EstadoPago WHERE Codigo = '04'
    );

    PRINT '✓ Índice IX_Pago_Parciales creado';
END
GO

-- ============================================
-- 5. VERIFICACIÓN DE INTEGRIDAD
-- ============================================

-- Verificar que todos los campos existen
SELECT
    'Estado Parcial' AS Item,
    CASE WHEN EXISTS(SELECT 1 FROM EstadoPago WHERE Codigo = '04')
        THEN 'OK ✓'
        ELSE 'FALTA ✗'
    END AS Estado
UNION ALL
SELECT
    'Campo MontoAdelanto',
    CASE WHEN EXISTS(
        SELECT 1 FROM sys.columns
        WHERE object_id = OBJECT_ID('Pago') AND name = 'MontoAdelanto'
    )
        THEN 'OK ✓'
        ELSE 'FALTA ✗'
    END
UNION ALL
SELECT
    'Campo MontoPendiente',
    CASE WHEN EXISTS(
        SELECT 1 FROM sys.columns
        WHERE object_id = OBJECT_ID('Pago') AND name = 'MontoPendiente'
    )
        THEN 'OK ✓'
        ELSE 'FALTA ✗'
    END;

-- ============================================
-- 6. NOTAS IMPORTANTES
-- ============================================

/*
DESPUÉS DE EJECUTAR ESTAS MIGRACIONES:

1. Actualizar Entity.Pago en el proyecto .NET:
   - Agregar: public decimal MontoAdelanto { get; set; }
   - Agregar: public decimal MontoPendiente { get; set; }

2. Regenerar migraciones de Entity Framework (si aplica):
   - dotnet ef migrations add AgregarCamposAdelanto
   - dotnet ef database update

3. Los nuevos endpoints para adelantos estarán disponibles:
   - POST /api/Pago/registrar-adelanto
   - POST /api/Pago/completar-pago

4. Reglas de negocio:
   - Adelanto mínimo: 50% del monto total
   - Si adelanto >= 50%: Reserva se CONFIRMA
   - Si adelanto < 50%: Reserva sigue PENDIENTE
   - Estado PARCIAL solo aplica para método EFECTIVO

5. Ver FLUJOS_PAGO.md para más detalles
*/

-- ============================================
-- FIN DE MIGRACIONES
-- ============================================
PRINT '';
PRINT '============================================';
PRINT '✓ TODAS LAS MIGRACIONES COMPLETADAS';
PRINT '============================================';
PRINT '';
PRINT 'Próximos pasos:';
PRINT '1. Actualizar Entity.Pago en el código .NET';
PRINT '2. Implementar endpoints de adelantos';
PRINT '3. Ver FLUJOS_PAGO.md para documentación';
GO
