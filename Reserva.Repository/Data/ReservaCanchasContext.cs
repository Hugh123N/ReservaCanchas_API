using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Reserva.Entity;

namespace Reserva.Repository.Data;

public partial class ReservaCanchasContext : DbContext
{
    public ReservaCanchasContext(DbContextOptions<ReservaCanchasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }

    public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }

    public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }

    public virtual DbSet<BloqueoHorario> BloqueoHorario { get; set; }

    public virtual DbSet<Cancha> Cancha { get; set; }

    public virtual DbSet<CanchaFavorita> CanchaFavorita { get; set; }

    public virtual DbSet<ComprobantePago> ComprobantePago { get; set; }

    public virtual DbSet<ComprobantePagoPlan> ComprobantePagoPlan { get; set; }

    public virtual DbSet<ConfiguracionProveedor> ConfiguracionProveedor { get; set; }

    public virtual DbSet<DetalleReserva> DetalleReserva { get; set; }

    public virtual DbSet<DiaSemana> DiaSemana { get; set; }

    public virtual DbSet<EstadoCancha> EstadoCancha { get; set; }

    public virtual DbSet<EstadoPago> EstadoPago { get; set; }

    public virtual DbSet<EstadoProveedor> EstadoProveedor { get; set; }

    public virtual DbSet<EstadoReserva> EstadoReserva { get; set; }

    public virtual DbSet<EstadoUsuario> EstadoUsuario { get; set; }

    public virtual DbSet<Hora> Hora { get; set; }

    public virtual DbSet<HorarioCancha> HorarioCancha { get; set; }

    public virtual DbSet<ImagenCancha> ImagenCancha { get; set; }

    public virtual DbSet<MetodoPago> MetodoPago { get; set; }

    public virtual DbSet<Notificacion> Notificacion { get; set; }

    public virtual DbSet<Operador> Operador { get; set; }

    public virtual DbSet<OperadorCancha> OperadorCancha { get; set; }

    public virtual DbSet<Pago> Pago { get; set; }

    public virtual DbSet<PagoPlan> PagoPlan { get; set; }

    public virtual DbSet<PlanCaracteristica> PlanCaracteristica { get; set; }

    public virtual DbSet<PlanLimite> PlanLimite { get; set; }

    public virtual DbSet<PlanTarifa> PlanTarifa { get; set; }

    public virtual DbSet<Plane> Plane { get; set; }

    public virtual DbSet<Proveedor> Proveedor { get; set; }

    public virtual DbSet<ProveedorPlan> ProveedorPlan { get; set; }

    public virtual DbSet<Entity.Reserva> Reserva { get; set; }

    public virtual DbSet<Servicio> Servicio { get; set; }

    public virtual DbSet<ServicioCancha> ServicioCancha { get; set; }

    public virtual DbSet<TipoDeporte> TipoDeporte { get; set; }

    public virtual DbSet<TipoDeporteCancha> TipoDeporteCancha { get; set; }

    public virtual DbSet<TipoProveedor> TipoProveedor { get; set; }

    public virtual DbSet<TipoSuperficie> TipoSuperficie { get; set; }

    public virtual DbSet<Ubigeo> Ubigeo { get; set; }

    public virtual DbSet<UsoPlan> UsoPlan { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRoleClaims>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetRo__3214EC072F149AFE");
        });

        modelBuilder.Entity<AspNetRoles>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetRo__3214EC07AAE1F1A0");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("createDate");
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
            entity.Property(e => e.UpdateDate).HasColumnName("updateDate");
            entity.Property(e => e.UserNameCreate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameCreate");
            entity.Property(e => e.UserNameUpdate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameUpdate");
        });

        modelBuilder.Entity<AspNetUserClaims>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetUs__3214EC075309F19B");
        });

        modelBuilder.Entity<AspNetUserLogins>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AspNetUse__UserI__66603565");
        });

        modelBuilder.Entity<AspNetUserTokens>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
        });

        modelBuilder.Entity<AspNetUsers>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetUs__3214EC074F0C5F9E");

            entity.HasIndex(e => e.Email, "UQ__AspNetUs__A9D105341ECDEFE5").IsUnique();

            entity.HasIndex(e => e.Email, "idx_email");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("createDate");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IdEstadoUsuario).HasColumnName("idEstadoUsuario");
            entity.Property(e => e.Imagen)
                .IsUnicode(false)
                .HasColumnName("imagen");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UpdateDate).HasColumnName("updateDate");
            entity.Property(e => e.UserName).HasMaxLength(256);
            entity.Property(e => e.UserNameCreate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameCreate");
            entity.Property(e => e.UserNameUpdate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameUpdate");

            entity.HasOne(d => d.IdEstadoUsuarioNavigation).WithMany(p => p.AspNetUsers)
                .HasForeignKey(d => d.IdEstadoUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AspNetUse__idEst__5535A963");

            entity.HasMany(d => d.Role).WithMany(p => p.User)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRoles",
                    r => r.HasOne<AspNetRoles>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__AspNetUse__RoleI__5FB337D6"),
                    l => l.HasOne<AspNetUsers>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__AspNetUse__UserI__5EBF139D"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                    });
        });

        modelBuilder.Entity<BloqueoHorario>(entity =>
        {
            entity.HasKey(e => e.IdBloqueoHorario).HasName("PK__BloqueoH__59C495F451FEFC30");

            entity.HasIndex(e => e.IdCancha, "idx_cancha");

            entity.HasIndex(e => e.FechaBloqueo, "idx_fecha");

            entity.Property(e => e.IdBloqueoHorario).HasColumnName("idBloqueoHorario");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("createDate");
            entity.Property(e => e.FechaBloqueo).HasColumnName("fechaBloqueo");
            entity.Property(e => e.IdCancha).HasColumnName("idCancha");
            entity.Property(e => e.IdHoraFin).HasColumnName("idHoraFin");
            entity.Property(e => e.IdHoraInicio).HasColumnName("idHoraInicio");
            entity.Property(e => e.Motivo)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("motivo");
            entity.Property(e => e.UpdateDate).HasColumnName("updateDate");
            entity.Property(e => e.UserNameCreate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameCreate");
            entity.Property(e => e.UserNameUpdate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameUpdate");

            entity.HasOne(d => d.IdCanchaNavigation).WithMany(p => p.BloqueoHorario)
                .HasForeignKey(d => d.IdCancha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BloqueoHo__idCan__489AC854");

            entity.HasOne(d => d.IdHoraFinNavigation).WithMany(p => p.BloqueoHorarioIdHoraFinNavigation)
                .HasForeignKey(d => d.IdHoraFin)
                .HasConstraintName("FK__BloqueoHo__idHor__4A8310C6");

            entity.HasOne(d => d.IdHoraInicioNavigation).WithMany(p => p.BloqueoHorarioIdHoraInicioNavigation)
                .HasForeignKey(d => d.IdHoraInicio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BloqueoHo__idHor__498EEC8D");
        });

        modelBuilder.Entity<Cancha>(entity =>
        {
            entity.HasKey(e => e.IdCancha).HasName("PK__Cancha__7ECD19EE4012DD4E");

            entity.HasIndex(e => e.Codigo, "UQ__Cancha__40F9A20663CD8F85").IsUnique();

            entity.HasIndex(e => e.Codigo, "idx_codigo");

            entity.HasIndex(e => e.IdEstadoCancha, "idx_estado");

            entity.HasIndex(e => e.IdProveedor, "idx_proveedor");

            entity.Property(e => e.IdCancha).HasColumnName("idCancha");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CapacidadJugadores).HasColumnName("capacidadJugadores");
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("codigo");
            entity.Property(e => e.CodigoUbigeo)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("codigoUbigeo");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("createDate");
            entity.Property(e => e.Descripcion)
                .HasColumnType("text")
                .HasColumnName("descripcion");
            entity.Property(e => e.Direccion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("direccion");
            entity.Property(e => e.IdEstadoCancha).HasColumnName("idEstadoCancha");
            entity.Property(e => e.IdProveedor).HasColumnName("idProveedor");
            entity.Property(e => e.IdTipoSuperficie).HasColumnName("idTipoSuperficie");
            entity.Property(e => e.Latitud)
                .HasColumnType("decimal(10, 8)")
                .HasColumnName("latitud");
            entity.Property(e => e.Longitud)
                .HasColumnType("decimal(10, 8)")
                .HasColumnName("longitud");
            entity.Property(e => e.Nombre)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Pais)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("pais");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio");
            entity.Property(e => e.TelefonoCancha)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("telefonoCancha");
            entity.Property(e => e.TieneIluminacion)
                .HasDefaultValue(true)
                .HasColumnName("tieneIluminacion");
            entity.Property(e => e.TieneTecho).HasColumnName("tieneTecho");
            entity.Property(e => e.UpdateDate).HasColumnName("updateDate");
            entity.Property(e => e.UserNameCreate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameCreate");
            entity.Property(e => e.UserNameUpdate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameUpdate");
            entity.Property(e => e.ZonaHoraria)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("America/Lima")
                .HasColumnName("zonaHoraria");

            entity.HasOne(d => d.CodigoUbigeoNavigation).WithMany(p => p.Cancha)
                .HasForeignKey(d => d.CodigoUbigeo)
                .HasConstraintName("FK__Cancha__codigoUb__19DFD96B");

            entity.HasOne(d => d.IdEstadoCanchaNavigation).WithMany(p => p.Cancha)
                .HasForeignKey(d => d.IdEstadoCancha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cancha__idEstado__18EBB532");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Cancha)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cancha__idProvee__17036CC0");

            entity.HasOne(d => d.IdTipoSuperficieNavigation).WithMany(p => p.Cancha)
                .HasForeignKey(d => d.IdTipoSuperficie)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cancha__idTipoSu__17F790F9");
        });

        modelBuilder.Entity<CanchaFavorita>(entity =>
        {
            entity.HasKey(e => new { e.IdUsuario, e.IdCancha }).HasName("PK__CanchaFa__93BBF2381D73E04E");

            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.IdCancha).HasColumnName("idCancha");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.FechaAgregado)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("fechaAgregado");

            entity.HasOne(d => d.IdCanchaNavigation).WithMany(p => p.CanchaFavorita)
                .HasForeignKey(d => d.IdCancha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CanchaFav__idCan__2CF2ADDF");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.CanchaFavorita)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CanchaFav__idUsu__2BFE89A6");
        });

        modelBuilder.Entity<ComprobantePago>(entity =>
        {
            entity.HasKey(e => e.IdComprobantePago).HasName("PK__Comproba__31D0A1D9DFD34CE1");

            entity.HasIndex(e => e.NumeroComprobante, "UQ__Comproba__20F00E4D6F54FA0E").IsUnique();

            entity.HasIndex(e => e.NumeroComprobante, "idx_numero");

            entity.HasIndex(e => e.IdPago, "idx_pago");

            entity.Property(e => e.IdComprobantePago).HasColumnName("idComprobantePago");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("createDate");
            entity.Property(e => e.IdPago).HasColumnName("idPago");
            entity.Property(e => e.NumeroComprobante)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("numeroComprobante");
            entity.Property(e => e.TipoComprobante)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tipoComprobante");
            entity.Property(e => e.UrlPdf)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("urlPDF");
            entity.Property(e => e.UserNameCreate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameCreate");

            entity.HasOne(d => d.IdPagoNavigation).WithMany(p => p.ComprobantePago)
                .HasForeignKey(d => d.IdPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comproban__idPag__7E02B4CC");
        });

        modelBuilder.Entity<ComprobantePagoPlan>(entity =>
        {
            entity.HasKey(e => e.IdComprobantePagoPlan).HasName("PK__Comproba__C668D0CE686958B9");

            entity.Property(e => e.IdComprobantePagoPlan).HasColumnName("idComprobantePagoPlan");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Direccion)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("direccion");
            entity.Property(e => e.EstadoSunat)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("estadoSunat");
            entity.Property(e => e.FechaEmision).HasColumnName("fechaEmision");
            entity.Property(e => e.Hash)
                .IsUnicode(false)
                .HasColumnName("hash");
            entity.Property(e => e.IdPagoPlan).HasColumnName("idPagoPlan");
            entity.Property(e => e.Numero)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("numero");
            entity.Property(e => e.RazonSocial)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("razonSocial");
            entity.Property(e => e.Ruc)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ruc");
            entity.Property(e => e.Serie)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("serie");
            entity.Property(e => e.TipoComprobante)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tipoComprobante");
            entity.Property(e => e.UrlPdf)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("urlPdf");
            entity.Property(e => e.UrlXml)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("urlXml");

            entity.HasOne(d => d.IdPagoPlanNavigation).WithMany(p => p.ComprobantePagoPlan)
                .HasForeignKey(d => d.IdPagoPlan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comproban__idPag__2BC97F7C");
        });

        modelBuilder.Entity<ConfiguracionProveedor>(entity =>
        {
            entity.HasKey(e => e.IdConfiguracionProveedor).HasName("PK__Configur__23C7C60DAE7CF726");

            entity.HasIndex(e => e.IdProveedor, "UQ_ConfiguracionProveedor_Proveedor").IsUnique();

            entity.HasIndex(e => e.IdProveedor, "idx_proveedor");

            entity.Property(e => e.IdConfiguracionProveedor).HasColumnName("idConfiguracionProveedor");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("createDate");
            entity.Property(e => e.DuracionPreReserva).HasColumnName("duracionPreReserva");
            entity.Property(e => e.IdProveedor).HasColumnName("idProveedor");
            entity.Property(e => e.PorcentajeAdelantoMinimo)
                .HasDefaultValue(50.00m)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("porcentajeAdelantoMinimo");
            entity.Property(e => e.PorcentajeDevolucionCompleto)
                .HasDefaultValue(100.00m)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("porcentajeDevolucionCompleto");
            entity.Property(e => e.PorcentajeDevolucionParcial)
                .HasDefaultValue(50.00m)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("porcentajeDevolucionParcial");
            entity.Property(e => e.TiempoLimiteCancelacion)
                .HasDefaultValue(24)
                .HasColumnName("tiempoLimiteCancelacion");
            entity.Property(e => e.UpdateDate).HasColumnName("updateDate");
            entity.Property(e => e.UserNameCreate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameCreate");
            entity.Property(e => e.UserNameUpdate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameUpdate");

            entity.HasOne(d => d.IdProveedorNavigation).WithOne(p => p.ConfiguracionProveedor)
                .HasForeignKey<ConfiguracionProveedor>(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Configura__idPro__02084FDA");
        });

        modelBuilder.Entity<DetalleReserva>(entity =>
        {
            entity.HasKey(e => e.IdDetalleReserva).HasName("PK__DetalleR__74EEC7D1EEC7C9E7");

            entity.HasIndex(e => e.IdReserva, "idx_reserva");

            entity.Property(e => e.IdDetalleReserva).HasColumnName("idDetalleReserva");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.IdHorarioCancha).HasColumnName("idHorarioCancha");
            entity.Property(e => e.IdReserva).HasColumnName("idReserva");

            entity.HasOne(d => d.IdHorarioCanchaNavigation).WithMany(p => p.DetalleReserva)
                .HasForeignKey(d => d.IdHorarioCancha)
                .HasConstraintName("FK__DetalleRe__idHor__70A8B9AE");

            entity.HasOne(d => d.IdReservaNavigation).WithMany(p => p.DetalleReserva)
                .HasForeignKey(d => d.IdReserva)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetalleRe__idRes__6FB49575");
        });

        modelBuilder.Entity<DiaSemana>(entity =>
        {
            entity.HasKey(e => e.IdDiaSemana).HasName("PK__DiaSeman__10EB836BCE2E37FD");

            entity.Property(e => e.IdDiaSemana)
                .ValueGeneratedNever()
                .HasColumnName("idDiaSemana");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<EstadoCancha>(entity =>
        {
            entity.HasKey(e => e.IdEstadoCancha).HasName("PK__EstadoCa__3B089FAB12B2368B");

            entity.HasIndex(e => e.Codigo, "UQ__EstadoCa__40F9A206B46F1F24").IsUnique();

            entity.Property(e => e.IdEstadoCancha).HasColumnName("idEstadoCancha");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("codigo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<EstadoPago>(entity =>
        {
            entity.HasKey(e => e.IdEstadoPago).HasName("PK__EstadoPa__03C5BA222ABB18CF");

            entity.HasIndex(e => e.Codigo, "UQ__EstadoPa__40F9A206BDE1F2EF").IsUnique();

            entity.Property(e => e.IdEstadoPago).HasColumnName("idEstadoPago");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("codigo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<EstadoProveedor>(entity =>
        {
            entity.HasKey(e => e.IdEstadoProveedor).HasName("PK__EstadoPr__B0AF2C73265D54F7");

            entity.HasIndex(e => e.Codigo, "UQ__EstadoPr__40F9A206C66C9A08").IsUnique();

            entity.Property(e => e.IdEstadoProveedor).HasColumnName("idEstadoProveedor");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("codigo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<EstadoReserva>(entity =>
        {
            entity.HasKey(e => e.IdEstadoReserva).HasName("PK__EstadoRe__AC7BB706704764FF");

            entity.HasIndex(e => e.Codigo, "UQ__EstadoRe__40F9A206CEF02BED").IsUnique();

            entity.Property(e => e.IdEstadoReserva).HasColumnName("idEstadoReserva");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("codigo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<EstadoUsuario>(entity =>
        {
            entity.HasKey(e => e.IdEstadoUsuario).HasName("PK__EstadoUs__57088573EC940C96");

            entity.HasIndex(e => e.Codigo, "UQ__EstadoUs__40F9A206483882AA").IsUnique();

            entity.Property(e => e.IdEstadoUsuario).HasColumnName("idEstadoUsuario");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("codigo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Hora>(entity =>
        {
            entity.HasKey(e => e.IdHora).HasName("PK__Hora__770403DB579C38BE");

            entity.HasIndex(e => e.Hora1, "UQ__Hora__7F3086DBB70BAE3D").IsUnique();

            entity.HasIndex(e => e.HoraTexto, "UQ__Hora__BFA371CEB2F0261A").IsUnique();

            entity.Property(e => e.IdHora).HasColumnName("idHora");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Hora1).HasColumnName("hora");
            entity.Property(e => e.HoraTexto)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("horaTexto");
        });

        modelBuilder.Entity<HorarioCancha>(entity =>
        {
            entity.HasKey(e => e.IdHorarioCancha).HasName("PK__HorarioC__825B785D12363DA5");

            entity.HasIndex(e => new { e.IdCancha, e.IdDiaSemana, e.IdHoraInicio }, "UQ_HorarioCancha").IsUnique();

            entity.HasIndex(e => e.IdCancha, "idx_cancha");

            entity.HasIndex(e => e.IdDiaSemana, "idx_dia");

            entity.Property(e => e.IdHorarioCancha).HasColumnName("idHorarioCancha");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("createDate");
            entity.Property(e => e.IdCancha).HasColumnName("idCancha");
            entity.Property(e => e.IdDiaSemana).HasColumnName("idDiaSemana");
            entity.Property(e => e.IdHoraFin).HasColumnName("idHoraFin");
            entity.Property(e => e.IdHoraInicio).HasColumnName("idHoraInicio");
            entity.Property(e => e.PrecioHora)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precioHora");
            entity.Property(e => e.UpdateDate).HasColumnName("updateDate");
            entity.Property(e => e.UserNameCreate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameCreate");
            entity.Property(e => e.UserNameUpdate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameUpdate");

            entity.HasOne(d => d.IdCanchaNavigation).WithMany(p => p.HorarioCancha)
                .HasForeignKey(d => d.IdCancha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HorarioCa__idCan__40058253");

            entity.HasOne(d => d.IdDiaSemanaNavigation).WithMany(p => p.HorarioCancha)
                .HasForeignKey(d => d.IdDiaSemana)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HorarioCa__idDia__40F9A68C");

            entity.HasOne(d => d.IdHoraFinNavigation).WithMany(p => p.HorarioCanchaIdHoraFinNavigation)
                .HasForeignKey(d => d.IdHoraFin)
                .HasConstraintName("FK__HorarioCa__idHor__42E1EEFE");

            entity.HasOne(d => d.IdHoraInicioNavigation).WithMany(p => p.HorarioCanchaIdHoraInicioNavigation)
                .HasForeignKey(d => d.IdHoraInicio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HorarioCa__idHor__41EDCAC5");
        });

        modelBuilder.Entity<ImagenCancha>(entity =>
        {
            entity.HasKey(e => e.IdImagenCancha).HasName("PK__ImagenCa__A5EF7FB15A71E14D");

            entity.HasIndex(e => e.IdCancha, "idx_cancha");

            entity.HasIndex(e => e.EsPrincipal, "idx_principal");

            entity.Property(e => e.IdImagenCancha).HasColumnName("idImagenCancha");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("createDate");
            entity.Property(e => e.EsPrincipal).HasColumnName("esPrincipal");
            entity.Property(e => e.IdCancha).HasColumnName("idCancha");
            entity.Property(e => e.UpdateDate).HasColumnName("updateDate");
            entity.Property(e => e.UrlImagen)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("urlImagen");
            entity.Property(e => e.UserNameCreate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameCreate");
            entity.Property(e => e.UserNameUpdate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameUpdate");

            entity.HasOne(d => d.IdCanchaNavigation).WithMany(p => p.ImagenCancha)
                .HasForeignKey(d => d.IdCancha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ImagenCan__idCan__2645B050");
        });

        modelBuilder.Entity<MetodoPago>(entity =>
        {
            entity.HasKey(e => e.IdMetodoPago).HasName("PK__MetodoPa__817BFC327CC780EF");

            entity.HasIndex(e => e.Codigo, "UQ__MetodoPa__40F9A206EBFB1349").IsUnique();

            entity.Property(e => e.IdMetodoPago).HasColumnName("idMetodoPago");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("codigo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.HasKey(e => e.IdNotificacion).HasName("PK__Notifica__AFE1D7E492BE10F9");

            entity.Property(e => e.IdNotificacion).HasColumnName("idNotificacion");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Canal)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("canal");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("createDate");
            entity.Property(e => e.Destinatario)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("destinatario");
            entity.Property(e => e.FechaEnvio).HasColumnName("fechaEnvio");
            entity.Property(e => e.FechaProgramada).HasColumnName("fechaProgramada");
            entity.Property(e => e.Intentos).HasColumnName("intentos");
            entity.Property(e => e.EntidadTipo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("entidadTipo");
            entity.Property(e => e.EntidadId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("entidadId");
            entity.Property(e => e.Metadata)
                .IsUnicode(false)
                .HasColumnName("metadata");
            entity.Property(e => e.Modulo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("modulo");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tipo");
            entity.Property(e => e.UserNameCreate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameCreate");
        });

        modelBuilder.Entity<Operador>(entity =>
        {
            entity.HasKey(e => e.IdOperador).HasName("PK__Operador__D9DC4D4E29383BAD");

            entity.Property(e => e.IdOperador).HasColumnName("idOperador");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CreateDate).HasColumnName("createDate");
            entity.Property(e => e.IdProveedor).HasColumnName("idProveedor");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.UpdateDate).HasColumnName("updateDate");
            entity.Property(e => e.UserNameCreate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameCreate");
            entity.Property(e => e.UserNameUpdate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameUpdate");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Operador)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Operador__idProv__503BEA1C");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Operador)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Operador__idUsua__4F47C5E3");
        });

        modelBuilder.Entity<OperadorCancha>(entity =>
        {
            entity.HasKey(e => e.IdOperadorCancha).HasName("PK__Operador__D9620313B29E282C");

            entity.HasIndex(e => new { e.IdOperador, e.IdCancha }, "UQ_OperadorCancha").IsUnique();

            entity.HasIndex(e => e.IdCancha, "idx_cancha");

            entity.HasIndex(e => e.IdOperador, "idx_operador");

            entity.Property(e => e.IdOperadorCancha).HasColumnName("idOperadorCancha");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.IdCancha).HasColumnName("idCancha");
            entity.Property(e => e.IdOperador).HasColumnName("idOperador");

            entity.HasOne(d => d.IdCanchaNavigation).WithMany(p => p.OperadorCancha)
                .HasForeignKey(d => d.IdCancha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OperadorC__idCan__55F4C372");

            entity.HasOne(d => d.IdOperadorNavigation).WithMany(p => p.OperadorCancha)
                .HasForeignKey(d => d.IdOperador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OperadorC__idOpe__55009F39");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.IdPago).HasName("PK__Pago__BD2295AD3C7C0EF0");

            entity.HasIndex(e => e.CulqiChargeId, "idx_culqiChargeId");

            entity.HasIndex(e => e.IdEstadoPago, "idx_estado");

            entity.HasIndex(e => e.IdReserva, "idx_reserva");

            entity.Property(e => e.IdPago).HasColumnName("idPago");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CodigoOperacion)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("codigoOperacion");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("createDate");
            entity.Property(e => e.CulqiChargeId)
                .HasMaxLength(100)
                .HasColumnName("culqiChargeId");
            entity.Property(e => e.CulqiReferenceCode)
                .HasMaxLength(50)
                .HasColumnName("culqiReferenceCode");
            entity.Property(e => e.CulqiTokenId)
                .HasMaxLength(100)
                .HasColumnName("culqiTokenId");
            entity.Property(e => e.IdEstadoPago).HasColumnName("idEstadoPago");
            entity.Property(e => e.IdMetodoPago).HasColumnName("idMetodoPago");
            entity.Property(e => e.IdOperador).HasColumnName("idOperador");
            entity.Property(e => e.IdReserva).HasColumnName("idReserva");
            entity.Property(e => e.Moneda)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("PEN")
                .IsFixedLength()
                .HasColumnName("moneda");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto");
            entity.Property(e => e.MontoAdelanto)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("montoAdelanto");
            entity.Property(e => e.MontoPendiente)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("montoPendiente");
            entity.Property(e => e.MontoReembolso)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("montoReembolso");
            entity.Property(e => e.NumeroReferencia)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("numeroReferencia");
            entity.Property(e => e.UpdateDate).HasColumnName("updateDate");
            entity.Property(e => e.UserNameCreate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameCreate");
            entity.Property(e => e.UserNameUpdate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameUpdate");

            entity.HasOne(d => d.IdEstadoPagoNavigation).WithMany(p => p.Pago)
                .HasForeignKey(d => d.IdEstadoPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pago__idEstadoPa__7755B73D");

            entity.HasOne(d => d.IdMetodoPagoNavigation).WithMany(p => p.Pago)
                .HasForeignKey(d => d.IdMetodoPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pago__idMetodoPa__76619304");

            entity.HasOne(d => d.IdOperadorNavigation).WithMany(p => p.Pago)
                .HasForeignKey(d => d.IdOperador)
                .HasConstraintName("FK__Pago__idOperador__7849DB76");

            entity.HasOne(d => d.IdReservaNavigation).WithMany(p => p.Pago)
                .HasForeignKey(d => d.IdReserva)
                .HasConstraintName("FK__Pago__idReserva__74794A92");
        });

        modelBuilder.Entity<PagoPlan>(entity =>
        {
            entity.HasKey(e => e.IdPagoPlan).HasName("PK__PagoPlan__4C6BAB987F4D8A6E");

            entity.Property(e => e.IdPagoPlan).HasColumnName("idPagoPlan");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CodigoOperacion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("codigoOperacion");
            entity.Property(e => e.CulqiChargeId)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("culqiChargeId");
            entity.Property(e => e.FechaPago)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("fechaPago");
            entity.Property(e => e.IdEstadoPago).HasColumnName("idEstadoPago");
            entity.Property(e => e.IdMetodoPago).HasColumnName("idMetodoPago");
            entity.Property(e => e.IdProveedorPlan).HasColumnName("idProveedorPlan");
            entity.Property(e => e.Moneda)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("PEN")
                .IsFixedLength()
                .HasColumnName("moneda");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto");
            entity.Property(e => e.RespuestaGateway)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("respuestaGateway");

            entity.HasOne(d => d.IdEstadoPagoNavigation).WithMany(p => p.PagoPlan)
                .HasForeignKey(d => d.IdEstadoPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PagoPlan__idEsta__24285DB4");

            entity.HasOne(d => d.IdMetodoPagoNavigation).WithMany(p => p.PagoPlan)
                .HasForeignKey(d => d.IdMetodoPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PagoPlan__idMeto__2334397B");

            entity.HasOne(d => d.IdProveedorPlanNavigation).WithMany(p => p.PagoPlan)
                .HasForeignKey(d => d.IdProveedorPlan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PagoPlan__idProv__214BF109");
        });

        modelBuilder.Entity<PlanCaracteristica>(entity =>
        {
            entity.HasKey(e => e.IdPlanCaracteristica).HasName("PK__PlanCara__BBB5CA8798739DCF");

            entity.Property(e => e.IdPlanCaracteristica).HasColumnName("idPlanCaracteristica");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.IdPlane).HasColumnName("idPlane");
            entity.Property(e => e.Orden).HasColumnName("orden");

            entity.HasOne(d => d.IdPlaneNavigation).WithMany(p => p.PlanCaracteristica)
                .HasForeignKey(d => d.IdPlane)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlanCarac__idPla__1209AD79");
        });

        modelBuilder.Entity<PlanLimite>(entity =>
        {
            entity.HasKey(e => e.IdPlanLimite).HasName("PK__PlanLimi__A89B181A79B038ED");

            entity.Property(e => e.IdPlanLimite).HasColumnName("idPlanLimite");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("codigo");
            entity.Property(e => e.IdPlane).HasColumnName("idPlane");
            entity.Property(e => e.Valor).HasColumnName("valor");

            entity.HasOne(d => d.IdPlaneNavigation).WithMany(p => p.PlanLimite)
                .HasForeignKey(d => d.IdPlane)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlanLimit__idPla__15DA3E5D");
        });

        modelBuilder.Entity<PlanTarifa>(entity =>
        {
            entity.HasKey(e => e.IdPlanTarifa).HasName("PK__PlanTari__2D9DDEB143BB4B8D");

            entity.Property(e => e.IdPlanTarifa).HasColumnName("idPlanTarifa");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("codigo");
            entity.Property(e => e.DuracionDias).HasColumnName("duracionDias");
            entity.Property(e => e.IdPlane).HasColumnName("idPlane");
            entity.Property(e => e.Moneda)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("PEN")
                .IsFixedLength()
                .HasColumnName("moneda");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.PermiteAutoRenovacion).HasColumnName("permiteAutoRenovacion");
            entity.Property(e => e.PorcentajeDescuento)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("porcentajeDescuento");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio");
            entity.Property(e => e.TipoCobro)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tipoCobro");

            entity.HasOne(d => d.IdPlaneNavigation).WithMany(p => p.PlanTarifa)
                .HasForeignKey(d => d.IdPlane)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlanTarif__idPla__0D44F85C");
        });

        modelBuilder.Entity<Plane>(entity =>
        {
            entity.HasKey(e => e.IdPlane).HasName("PK__Plane__39B8603C0E335451");

            entity.HasIndex(e => e.Codigo, "UQ__Plane__40F9A2066AC4B25D").IsUnique();

            entity.Property(e => e.IdPlane).HasColumnName("idPlane");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("codigo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.OrdenVisual)
                .HasDefaultValue(1)
                .HasColumnName("ordenVisual");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.IdProveedor).HasName("PK__Proveedo__A3FA8E6BA6DA4ED7");

            entity.HasIndex(e => e.IdEstadoProveedor, "idx_estado");

            entity.HasIndex(e => e.IdTipoProveedor, "idx_tipo");

            entity.HasIndex(e => e.IdUsuario, "idx_usuario");

            entity.Property(e => e.IdProveedor).HasColumnName("idProveedor");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("createDate");
            entity.Property(e => e.Facebook)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("facebook");
            entity.Property(e => e.IdEstadoProveedor).HasColumnName("idEstadoProveedor");
            entity.Property(e => e.IdTipoProveedor).HasColumnName("idTipoProveedor");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Instagram)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("instagram");
            entity.Property(e => e.CulqiCustomerId)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("culqiCustomerId");
            entity.Property(e => e.RazonSocial)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("razonSocial");
            entity.Property(e => e.Ruc)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ruc");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");
            entity.Property(e => e.UpdateDate).HasColumnName("updateDate");
            entity.Property(e => e.UserNameCreate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameCreate");
            entity.Property(e => e.UserNameUpdate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameUpdate");

            entity.HasOne(d => d.IdEstadoProveedorNavigation).WithMany(p => p.Proveedor)
                .HasForeignKey(d => d.IdEstadoProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Proveedor__idEst__7C4F7684");

            entity.HasOne(d => d.IdTipoProveedorNavigation).WithMany(p => p.Proveedor)
                .HasForeignKey(d => d.IdTipoProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Proveedor__idTip__7B5B524B");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Proveedor)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Proveedor__idUsu__7A672E12");
        });

        modelBuilder.Entity<ProveedorPlan>(entity =>
        {
            entity.HasKey(e => e.IdProveedorPlan).HasName("PK__Proveedo__4A29306087D12D74");

            entity.Property(e => e.IdProveedorPlan).HasColumnName("idProveedorPlan");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.AutoRenovacion)
                .HasDefaultValue(true)
                .HasColumnName("autoRenovacion");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("createDate");
            entity.Property(e => e.CulqiCustomerId)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("culqiCustomerId");
            entity.Property(e => e.CulqiSubscriptionId)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("culqiSubscriptionId");
            entity.Property(e => e.EsActual)
                .HasDefaultValue(true)
                .HasColumnName("esActual");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCancelacion).HasColumnName("fechaCancelacion");
            entity.Property(e => e.FechaFin).HasColumnName("fechaFin");
            entity.Property(e => e.FechaInicio).HasColumnName("fechaInicio");
            entity.Property(e => e.FechaProximoCobro).HasColumnName("fechaProximoCobro");
            entity.Property(e => e.GracePeriodHasta).HasColumnName("gracePeriodHasta");
            entity.Property(e => e.IdPlanTarifa).HasColumnName("idPlanTarifa");
            entity.Property(e => e.IdPlane).HasColumnName("idPlane");
            entity.Property(e => e.IdProveedor).HasColumnName("idProveedor");
            entity.Property(e => e.MotivoCancelacion)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("motivoCancelacion");
            entity.Property(e => e.CancelAtPeriodEnd)
                .HasDefaultValue(false)
                .HasColumnName("cancelAtPeriodEnd");
            entity.Property(e => e.EsPruebaGratis)
                .HasDefaultValue(false)
                .HasColumnName("esPruebaGratis");
            entity.Property(e => e.SaldoFavor)
                .HasPrecision(18, 2)
                .HasDefaultValue(0m)
                .HasColumnName("saldoFavor");
            entity.Property(e => e.UserNameCreate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameCreate");

            entity.HasOne(d => d.IdPlanTarifaNavigation).WithMany(p => p.ProveedorPlan)
                .HasForeignKey(d => d.IdPlanTarifa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Proveedor__idPla__1A9EF37A");

            entity.HasOne(d => d.IdPlaneNavigation).WithMany(p => p.ProveedorPlan)
                .HasForeignKey(d => d.IdPlane)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Proveedor__idPla__19AACF41");
        });

        modelBuilder.Entity<Entity.Reserva>(entity =>
        {
            entity.HasKey(e => e.IdReserva).HasName("PK__Reserva__94D104C8B55A745C");

            entity.HasIndex(e => e.CodigoReserva, "UQ__Reserva__EFEC21CC5BF07731").IsUnique();

            entity.HasIndex(e => e.IdCancha, "idx_cancha");

            entity.HasIndex(e => e.IdCliente, "idx_cliente");

            entity.HasIndex(e => e.CodigoReserva, "idx_codigo");

            entity.HasIndex(e => e.IdEstadoReserva, "idx_estado");

            entity.HasIndex(e => e.FechaReserva, "idx_fecha");

            entity.HasIndex(e => e.IdOperadorConfirmo, "idx_operador");

            entity.Property(e => e.IdReserva).HasColumnName("idReserva");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CodigoReserva)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("codigoReserva");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("createDate");
            entity.Property(e => e.FechaConfirmacion).HasColumnName("fechaConfirmacion");
            entity.Property(e => e.FechaExpiracionPreReserva).HasColumnName("fechaExpiracionPreReserva");
            entity.Property(e => e.FechaReserva).HasColumnName("fechaReserva");
            entity.Property(e => e.IdCancha).HasColumnName("idCancha");
            entity.Property(e => e.IdCliente).HasColumnName("idCliente");
            entity.Property(e => e.IdEstadoReserva).HasColumnName("idEstadoReserva");
            entity.Property(e => e.IdOperadorConfirmo).HasColumnName("idOperadorConfirmo");
            entity.Property(e => e.IdTipoDeporte).HasColumnName("idTipoDeporte");
            entity.Property(e => e.MontoTotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("montoTotal");
            entity.Property(e => e.NotificacionAdvertenciaEnviada).HasColumnName("notificacionAdvertenciaEnviada");
            entity.Property(e => e.Observaciones)
                .HasColumnType("text")
                .HasColumnName("observaciones");
            entity.Property(e => e.RecordatorioEnviado)
                .HasDefaultValue(false)
                .HasColumnName("recordatorioEnviado");
            entity.Property(e => e.UpdateDate).HasColumnName("updateDate");
            entity.Property(e => e.UserNameCreate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameCreate");
            entity.Property(e => e.UserNameUpdate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameUpdate");

            entity.HasOne(d => d.IdCanchaNavigation).WithMany(p => p.Reserva)
                .HasForeignKey(d => d.IdCancha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reserva__idCanch__671F4F74");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Reserva)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reserva__idClien__662B2B3B");

            entity.HasOne(d => d.IdEstadoReservaNavigation).WithMany(p => p.Reserva)
                .HasForeignKey(d => d.IdEstadoReserva)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reserva__idEstad__69FBBC1F");

            entity.HasOne(d => d.IdOperadorConfirmoNavigation).WithMany(p => p.Reserva)
                .HasForeignKey(d => d.IdOperadorConfirmo)
                .HasConstraintName("FK__Reserva__idOpera__6AEFE058");

            entity.HasOne(d => d.IdTipoDeporteNavigation).WithMany(p => p.Reserva)
                .HasForeignKey(d => d.IdTipoDeporte)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reserva__idTipoD__681373AD");
        });

        modelBuilder.Entity<Servicio>(entity =>
        {
            entity.HasKey(e => e.IdServicio).HasName("PK__Servicio__CEB98119FCECA8D7");

            entity.HasIndex(e => e.Codigo, "UQ__Servicio__40F9A206CF413180").IsUnique();

            entity.HasIndex(e => e.Codigo, "idx_codigo");

            entity.Property(e => e.IdServicio).HasColumnName("idServicio");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("codigo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Icono)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("icono");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<ServicioCancha>(entity =>
        {
            entity.HasKey(e => e.IdServicioCancha).HasName("PK__Servicio__56B6658C3015199A");

            entity.HasIndex(e => new { e.IdCancha, e.IdServicio }, "UQ_ServicioCancha").IsUnique();

            entity.Property(e => e.IdServicioCancha).HasColumnName("idServicioCancha");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CostoAdicional)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("costoAdicional");
            entity.Property(e => e.EsIncluido)
                .HasDefaultValue(true)
                .HasColumnName("esIncluido");
            entity.Property(e => e.IdCancha).HasColumnName("idCancha");
            entity.Property(e => e.IdServicio).HasColumnName("idServicio");

            entity.HasOne(d => d.IdCanchaNavigation).WithMany(p => p.ServicioCancha)
                .HasForeignKey(d => d.IdCancha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ServicioC__idCan__367C1819");

            entity.HasOne(d => d.IdServicioNavigation).WithMany(p => p.ServicioCancha)
                .HasForeignKey(d => d.IdServicio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ServicioC__idSer__37703C52");
        });

        modelBuilder.Entity<TipoDeporte>(entity =>
        {
            entity.HasKey(e => e.IdTipoDeporte).HasName("PK__TipoDepo__913C11BF25309575");

            entity.HasIndex(e => e.Codigo, "UQ__TipoDepo__40F9A206E141EBE0").IsUnique();

            entity.HasIndex(e => e.Codigo, "idx_codigo");

            entity.Property(e => e.IdTipoDeporte).HasColumnName("idTipoDeporte");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("codigo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Icono)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("icono");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<TipoDeporteCancha>(entity =>
        {
            entity.HasKey(e => e.IdTipoDeporteCancha).HasName("PK__TipoDepo__781F42EA179C92CB");

            entity.HasIndex(e => e.IdCancha, "idx_cancha");

            entity.HasIndex(e => e.IdTipoDeporte, "idx_deporte");

            entity.Property(e => e.IdTipoDeporteCancha).HasColumnName("idTipoDeporteCancha");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.IdCancha).HasColumnName("idCancha");
            entity.Property(e => e.IdTipoDeporte).HasColumnName("idTipoDeporte");

            entity.HasOne(d => d.IdCanchaNavigation).WithMany(p => p.TipoDeporteCancha)
                .HasForeignKey(d => d.IdCancha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TipoDepor__idCan__2180FB33");

            entity.HasOne(d => d.IdTipoDeporteNavigation).WithMany(p => p.TipoDeporteCancha)
                .HasForeignKey(d => d.IdTipoDeporte)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TipoDepor__idTip__22751F6C");
        });

        modelBuilder.Entity<TipoProveedor>(entity =>
        {
            entity.HasKey(e => e.IdTipoProveedor).HasName("PK__TipoProv__3CDA600659430EFD");

            entity.HasIndex(e => e.Codigo, "UQ__TipoProv__40F9A206AFF8DFDC").IsUnique();

            entity.Property(e => e.IdTipoProveedor).HasColumnName("idTipoProveedor");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("codigo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<TipoSuperficie>(entity =>
        {
            entity.HasKey(e => e.IdTipoSuperficie).HasName("PK__TipoSupe__712490DAE5E2D3C9");

            entity.HasIndex(e => e.Codigo, "UQ__TipoSupe__40F9A20609C8E12D").IsUnique();

            entity.HasIndex(e => e.Codigo, "idx_codigo");

            entity.Property(e => e.IdTipoSuperficie).HasColumnName("idTipoSuperficie");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("codigo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Ubigeo>(entity =>
        {
            entity.HasKey(e => e.CodigoUbigeo).HasName("PK__Ubigeo__B096A3D75FB5BD50");

            entity.Property(e => e.CodigoUbigeo)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("codigoUbigeo");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Departamento)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("departamento");
            entity.Property(e => e.Distrito)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("distrito");
            entity.Property(e => e.Provincia)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("provincia");
        });

        modelBuilder.Entity<UsoPlan>(entity =>
        {
            entity.HasKey(e => e.IdUsoPlan).HasName("PK__UsoPlan__24EBD75A4B4BBF80");

            entity.Property(e => e.IdUsoPlan).HasColumnName("idUsoPlan");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("codigo");
            entity.Property(e => e.IdProveedor).HasColumnName("idProveedor");
            entity.Property(e => e.ValorActual).HasColumnName("valorActual");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
