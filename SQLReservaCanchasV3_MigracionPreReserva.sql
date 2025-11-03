-- =============================================
-- Script de Migración V3: Sistema de Pre-Reserva
-- Descripción: Agrega funcionalidad de pre-reserva con expiración automática
-- Fecha: 2025-01-11
-- =============================================

USE ReservaCanchas;
GO

PRINT 'Iniciando migración V3: Sistema de Pre-Reserva...';
GO

-- =============================================
-- 1. AGREGAR CAMPOS A TABLA CANCHA
-- =============================================
PRINT 'Agregando campos a tabla Cancha...';

-- Duración en horas de la pre-reserva antes de expirar
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cancha]') AND name = 'duracionPreReserva')
BEGIN
    ALTER TABLE [dbo].[Cancha]
    ADD [duracionPreReserva] INT NULL;

    PRINT '  ✓ Campo duracionPreReserva agregado';
END
ELSE
BEGIN
    PRINT '  - Campo duracionPreReserva ya existe';
END
GO

-- Porcentaje mínimo de adelanto requerido (0-100)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cancha]') AND name = 'porcentajeAdelanto')
BEGIN
    ALTER TABLE [dbo].[Cancha]
    ADD [porcentajeAdelanto] DECIMAL(5, 2) NULL;

    PRINT '  ✓ Campo porcentajeAdelanto agregado';
END
ELSE
BEGIN
    PRINT '  - Campo porcentajeAdelanto ya existe';
END
GO

-- Establecer valores por defecto para canchas existentes
UPDATE [dbo].[Cancha]
SET [duracionPreReserva] = 48  -- 48 horas por defecto
WHERE [duracionPreReserva] IS NULL;

UPDATE [dbo].[Cancha]
SET [porcentajeAdelanto] = 50.00  -- 50% por defecto
WHERE [porcentajeAdelanto] IS NULL;

PRINT '  ✓ Valores por defecto establecidos para canchas existentes';
GO

-- =============================================
-- 2. AGREGAR CAMPOS A TABLA RESERVA
-- =============================================
PRINT 'Agregando campos a tabla Reserva...';

-- Código único legible de la reserva
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Reserva]') AND name = 'codigoReserva')
BEGIN
    ALTER TABLE [dbo].[Reserva]
    ADD [codigoReserva] VARCHAR(50) NULL;

    PRINT '  ✓ Campo codigoReserva agregado';
END
ELSE
BEGIN
    PRINT '  - Campo codigoReserva ya existe';
END
GO

-- Fecha y hora límite de la pre-reserva
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Reserva]') AND name = 'fechaExpiracionPreReserva')
BEGIN
    ALTER TABLE [dbo].[Reserva]
    ADD [fechaExpiracionPreReserva] DATETIMEOFFSET NULL;

    PRINT '  ✓ Campo fechaExpiracionPreReserva agregado';
END
ELSE
BEGIN
    PRINT '  - Campo fechaExpiracionPreReserva ya existe';
END
GO

-- Crear índice único para codigoReserva
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Reserva]') AND name = 'UQ_Reserva_CodigoReserva')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [UQ_Reserva_CodigoReserva]
    ON [dbo].[Reserva] ([codigoReserva])
    WHERE [codigoReserva] IS NOT NULL;

    PRINT '  ✓ Índice único creado para codigoReserva';
END
ELSE
BEGIN
    PRINT '  - Índice único para codigoReserva ya existe';
END
GO

-- Crear índice para búsquedas por fecha de expiración
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Reserva]') AND name = 'IX_Reserva_FechaExpiracion')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Reserva_FechaExpiracion]
    ON [dbo].[Reserva] ([fechaExpiracionPreReserva], [idEstadoReserva])
    WHERE [fechaExpiracionPreReserva] IS NOT NULL;

    PRINT '  ✓ Índice creado para fechaExpiracionPreReserva';
END
ELSE
BEGIN
    PRINT '  - Índice para fechaExpiracionPreReserva ya existe';
END
GO

-- =============================================
-- 3. AGREGAR NUEVO ESTADO DE RESERVA: EXPIRADO
-- =============================================
PRINT 'Agregando estado EXPIRADO a EstadoReserva...';

IF NOT EXISTS (SELECT * FROM [dbo].[EstadoReserva] WHERE [codigo] = '04')
BEGIN
    INSERT INTO [dbo].[EstadoReserva] ([codigo], [nombre], [activo])
    VALUES ('04', 'Expirado', 1);

    PRINT '  ✓ Estado Expirado agregado';
END
ELSE
BEGIN
    PRINT '  - Estado Expirado ya existe';
END
GO

-- =============================================
-- 4. GENERAR CÓDIGOS DE RESERVA PARA RESERVAS EXISTENTES
-- =============================================
PRINT 'Generando códigos para reservas existentes...';

DECLARE @contador INT = 1;
DECLARE @año INT = YEAR(GETDATE());

DECLARE reservas_cursor CURSOR FOR
SELECT [idReserva]
FROM [dbo].[Reserva]
WHERE [codigoReserva] IS NULL
ORDER BY [createDate];

DECLARE @idReserva INT;

OPEN reservas_cursor;
FETCH NEXT FROM reservas_cursor INTO @idReserva;

WHILE @@FETCH_STATUS = 0
BEGIN
    DECLARE @codigo VARCHAR(50);
    SET @codigo = 'RES-' + CAST(@año AS VARCHAR(4)) + '-' + RIGHT('0000' + CAST(@contador AS VARCHAR), 4);

    UPDATE [dbo].[Reserva]
    SET [codigoReserva] = @codigo
    WHERE [idReserva] = @idReserva;

    SET @contador = @contador + 1;
    FETCH NEXT FROM reservas_cursor INTO @idReserva;
END;

CLOSE reservas_cursor;
DEALLOCATE reservas_cursor;

PRINT '  ✓ Códigos generados para ' + CAST(@contador - 1 AS VARCHAR) + ' reservas existentes';
GO

-- =============================================
-- 5. CREAR STORED PROCEDURE PARA GENERAR CÓDIGO DE RESERVA
-- =============================================
PRINT 'Creando stored procedure para generar códigos de reserva...';

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GenerarCodigoReserva]') AND type in (N'P', N'PC'))
BEGIN
    DROP PROCEDURE [dbo].[sp_GenerarCodigoReserva];
    PRINT '  - Stored procedure anterior eliminado';
END
GO

CREATE PROCEDURE [dbo].[sp_GenerarCodigoReserva]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @año INT = YEAR(GETDATE());
    DECLARE @ultimoNumero INT;

    -- Obtener el último número de reserva del año actual
    SELECT @ultimoNumero = ISNULL(MAX(CAST(RIGHT([codigoReserva], 4) AS INT)), 0)
    FROM [dbo].[Reserva]
    WHERE [codigoReserva] LIKE 'RES-' + CAST(@año AS VARCHAR(4)) + '-%';

    -- Incrementar el número
    SET @ultimoNumero = @ultimoNumero + 1;

    -- Generar el código
    DECLARE @codigo VARCHAR(50);
    SET @codigo = 'RES-' + CAST(@año AS VARCHAR(4)) + '-' + RIGHT('0000' + CAST(@ultimoNumero AS VARCHAR), 4);

    SELECT @codigo AS CodigoReserva;
END
GO

PRINT '  ✓ Stored procedure sp_GenerarCodigoReserva creado';
GO

-- =============================================
-- 6. VERIFICACIÓN DE LA MIGRACIÓN
-- =============================================
PRINT '';
PRINT '==========================================';
PRINT 'VERIFICACIÓN DE LA MIGRACIÓN';
PRINT '==========================================';

-- Verificar campos en Cancha
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cancha]') AND name = 'duracionPreReserva')
AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cancha]') AND name = 'porcentajeAdelanto')
BEGIN
    PRINT '✓ Campos de Cancha: OK';
END
ELSE
BEGIN
    PRINT '✗ ERROR: Campos de Cancha no fueron creados correctamente';
END

-- Verificar campos en Reserva
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Reserva]') AND name = 'codigoReserva')
AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Reserva]') AND name = 'fechaExpiracionPreReserva')
BEGIN
    PRINT '✓ Campos de Reserva: OK';
END
ELSE
BEGIN
    PRINT '✗ ERROR: Campos de Reserva no fueron creados correctamente';
END

-- Verificar estado Expirado
IF EXISTS (SELECT * FROM [dbo].[EstadoReserva] WHERE [codigo] = '04')
BEGIN
    PRINT '✓ Estado Expirado: OK';
END
ELSE
BEGIN
    PRINT '✗ ERROR: Estado Expirado no fue creado';
END

-- Verificar stored procedure
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GenerarCodigoReserva]') AND type in (N'P', N'PC'))
BEGIN
    PRINT '✓ Stored Procedure: OK';
END
ELSE
BEGIN
    PRINT '✗ ERROR: Stored Procedure no fue creado';
END

-- Mostrar estadísticas
DECLARE @totalCanchas INT;
DECLARE @totalReservas INT;
DECLARE @reservasConCodigo INT;

SELECT @totalCanchas = COUNT(*) FROM [dbo].[Cancha];
SELECT @totalReservas = COUNT(*) FROM [dbo].[Reserva];
SELECT @reservasConCodigo = COUNT(*) FROM [dbo].[Reserva] WHERE [codigoReserva] IS NOT NULL;

PRINT '';
PRINT 'ESTADÍSTICAS:';
PRINT '  - Total de canchas: ' + CAST(@totalCanchas AS VARCHAR);
PRINT '  - Total de reservas: ' + CAST(@totalReservas AS VARCHAR);
PRINT '  - Reservas con código: ' + CAST(@reservasConCodigo AS VARCHAR);
PRINT '';
PRINT '==========================================';
PRINT 'MIGRACIÓN V3 COMPLETADA EXITOSAMENTE';
PRINT '==========================================';
GO
