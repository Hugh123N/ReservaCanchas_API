-- Referencia para instalaciones nuevas en SQL Server. No ejecutar en MySQL.
-- NumeroHoja es una columna normal: la API asigna RF-ID-ANIO después de obtener el ID.
-- No modifica ni elimina una tabla existente. No incluye Version.
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.Reclamacion', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Reclamacion
    (
        -- Identificación de la hoja
        IdReclamacion int IDENTITY(1,1) NOT NULL, -- Clave y correlativo asignado por SQL Server.
        NumeroHoja varchar(20) NULL, -- Código RF-ID-ANIO asignado por la API dentro de la transacción de registro.
        FechaRegistro datetimeoffset(7) NOT NULL, -- Instante de recepción; el año de la hoja se calcula en horario peruano.

        -- Comercio: datos vigentes cuando se recibe la hoja
        ComercioTitular nvarchar(200) NOT NULL, -- Persona natural o razón social responsable.
        ComercioNombre nvarchar(100) NOT NULL, -- Nombre comercial.
        ComercioRuc varchar(11) NOT NULL, -- RUC del titular del libro.
        ComercioDireccion nvarchar(400) NOT NULL, -- Dirección del establecimiento.
        ComercioTelefono varchar(20) NOT NULL, -- Teléfono de atención.
        ComercioEmail nvarchar(254) NOT NULL, -- Correo de atención.

        -- Identificación del consumidor
        Nombres nvarchar(200) NOT NULL, -- Nombres y apellidos completos.
        TipDoc varchar(1) NOT NULL, -- 1: DNI; 4: carné de extranjería; 7: pasaporte.
        NroDoc varchar(20) NOT NULL, -- Documento como texto para conservar ceros iniciales.
        Domicilio nvarchar(400) NOT NULL, -- Dirección del consumidor.
        Email nvarchar(254) NOT NULL, -- Correo para enviar la copia y responder.
        Telefono varchar(20) NULL, -- Teléfono adicional opcional.
        EsMenor bit NOT NULL, -- Indica si corresponde identificar al representante.
        Representante nvarchar(200) NULL, -- Padre, madre o representante cuando el consumidor es menor.

        -- Servicio involucrado
        DescripcionServicio nvarchar(500) NOT NULL, -- Descripción libre del servicio.
        MontoReclamado decimal(12,2) NULL, -- Importe en soles cuando corresponde.
        ReferenciaOperacion nvarchar(100) NULL, -- Referencia de pago opcional.

        -- Manifestación original del consumidor
        Tipo varchar(7) NOT NULL, -- RECLAMO o QUEJA.
        Detalle nvarchar(4000) NOT NULL, -- Relato del consumidor.
        Pedido nvarchar(2000) NOT NULL, -- Solución solicitada.

        -- Atención administrativa
        Estado varchar(12) NOT NULL CONSTRAINT DF_Reclamacion_Estado DEFAULT 'PENDIENTE', -- PENDIENTE, EN_ATENCION o RESPONDIDA.
        Observaciones nvarchar(2000) NULL, -- Acciones adoptadas por el proveedor.
        Respuesta nvarchar(4000) NULL, -- Respuesta comunicada al consumidor.
        FechaComunicacionRespuesta datetimeoffset(7) NULL, -- Instante real de comunicación de la respuesta.
        ReferenciaRespuesta nvarchar(250) NULL, -- Referencia del correo, carta o constancia.
        ResponsableRespuesta nvarchar(250) NULL, -- Administrador que registra la atención.

        -- Auditoría
        UserNameCreate nvarchar(250) NULL, -- Usuario técnico de creación.
        CreateDate datetimeoffset(7) NOT NULL, -- Fecha de creación auditada.
        UserNameUpdate nvarchar(250) NULL, -- Último usuario que actualizó la atención.
        UpdateDate datetimeoffset(7) NULL, -- Fecha de actualización auditada.
        Activo bit NOT NULL CONSTRAINT DF_Reclamacion_Activo DEFAULT 1, -- Marca técnica del registro.

        CONSTRAINT PK_Reclamacion PRIMARY KEY (IdReclamacion),
        CONSTRAINT CK_Reclamacion_TipDoc CHECK (TipDoc IN ('1','4','7')),
        CONSTRAINT CK_Reclamacion_Tipo CHECK (Tipo IN ('RECLAMO','QUEJA')),
        CONSTRAINT CK_Reclamacion_Estado CHECK (Estado IN ('PENDIENTE','EN_ATENCION','RESPONDIDA')),
        CONSTRAINT CK_Reclamacion_Monto CHECK (MontoReclamado IS NULL OR MontoReclamado >= 0),
        CONSTRAINT CK_Reclamacion_Representante CHECK (EsMenor = 0 OR (Representante IS NOT NULL AND LEN(LTRIM(RTRIM(Representante))) > 0)),
        CONSTRAINT CK_Reclamacion_Respuesta CHECK (Estado <> 'RESPONDIDA' OR
            (Respuesta IS NOT NULL AND LEN(LTRIM(RTRIM(Respuesta))) > 0 AND FechaComunicacionRespuesta IS NOT NULL
             AND ReferenciaRespuesta IS NOT NULL AND LEN(LTRIM(RTRIM(ReferenciaRespuesta))) > 0))
    );

    -- A diferencia de UNIQUE en SQL Server, este índice permite varias inserciones todavía sin número.
    CREATE UNIQUE INDEX UQ_Reclamacion_NumeroHoja ON dbo.Reclamacion (NumeroHoja) WHERE NumeroHoja IS NOT NULL;
    CREATE INDEX IX_Reclamacion_Estado_FechaRegistro ON dbo.Reclamacion (Estado, FechaRegistro);
    CREATE INDEX IX_Reclamacion_FechaRegistro ON dbo.Reclamacion (FechaRegistro);
    CREATE INDEX IX_Reclamacion_TipDoc_NroDoc ON dbo.Reclamacion (TipDoc, NroDoc);
END;

COMMIT TRANSACTION;
