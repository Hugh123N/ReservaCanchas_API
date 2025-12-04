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

    public virtual DbSet<Comision> Comision { get; set; }

    public virtual DbSet<ComprobantePago> ComprobantePago { get; set; }

    public virtual DbSet<ConfiguracionProveedor> ConfiguracionProveedor { get; set; }

    public virtual DbSet<DetalleReserva> DetalleReserva { get; set; }

    public virtual DbSet<DiaSemana> DiaSemana { get; set; }

    public virtual DbSet<EstadoCancha> EstadoCancha { get; set; }

    public virtual DbSet<EstadoPago> EstadoPago { get; set; }

    public virtual DbSet<EstadoProveedor> EstadoProveedor { get; set; }

    public virtual DbSet<EstadoReserva> EstadoReserva { get; set; }

    public virtual DbSet<EstadoUsuario> EstadoUsuario { get; set; }

    public virtual DbSet<HorarioCancha> HorarioCancha { get; set; }

    public virtual DbSet<ImagenCancha> ImagenCancha { get; set; }

    public virtual DbSet<MetodoPago> MetodoPago { get; set; }

    public virtual DbSet<Notificacion> Notificacion { get; set; }

    public virtual DbSet<Operador> Operador { get; set; }

    public virtual DbSet<OperadorCancha> OperadorCancha { get; set; }

    public virtual DbSet<Pago> Pago { get; set; }

    public virtual DbSet<Proveedor> Proveedor { get; set; }

    public virtual DbSet<Entity.Reserva> Reserva { get; set; }

    public virtual DbSet<Servicio> Servicio { get; set; }

    public virtual DbSet<ServicioCancha> ServicioCancha { get; set; }

    public virtual DbSet<TipoDeporte> TipoDeporte { get; set; }

    public virtual DbSet<TipoDeporteCancha> TipoDeporteCancha { get; set; }

    public virtual DbSet<TipoProveedor> TipoProveedor { get; set; }

    public virtual DbSet<TipoSuperficie> TipoSuperficie { get; set; }

    public virtual DbSet<Ubigeo> Ubigeo { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRoleClaims>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetRo__3214EC07189DB2AF");
        });

        modelBuilder.Entity<AspNetRoles>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetRo__3214EC0717C57E1E");

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
            entity.HasKey(e => e.Id).HasName("PK__AspNetUs__3214EC071B5F4F85");
        });

        modelBuilder.Entity<AspNetUserLogins>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AspNetUse__UserI__412EB0B6");
        });

        modelBuilder.Entity<AspNetUserTokens>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
        });

        modelBuilder.Entity<AspNetUsers>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetUs__3214EC0733B0EB8C");

            entity.HasIndex(e => e.Email, "UQ__AspNetUs__A9D1053458A197E1").IsUnique();

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
                .HasConstraintName("FK__AspNetUse__idEst__300424B4");

            entity.HasMany(d => d.Role).WithMany(p => p.User)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRoles",
                    r => r.HasOne<AspNetRoles>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__AspNetUse__RoleI__3A81B327"),
                    l => l.HasOne<AspNetUsers>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__AspNetUse__UserI__398D8EEE"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                    });
        });

        modelBuilder.Entity<BloqueoHorario>(entity =>
        {
            entity.HasKey(e => e.IdBloqueoHorario).HasName("PK__BloqueoH__59C495F4FF37ADE1");

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
            entity.Property(e => e.HoraFin).HasColumnName("horaFin");
            entity.Property(e => e.HoraInicio).HasColumnName("horaInicio");
            entity.Property(e => e.IdCancha).HasColumnName("idCancha");
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
                .HasConstraintName("FK__BloqueoHo__idCan__1DB06A4F");
        });

        modelBuilder.Entity<Cancha>(entity =>
        {
            entity.HasKey(e => e.IdCancha).HasName("PK__Cancha__7ECD19EE28F6091B");

            entity.HasIndex(e => e.Codigo, "UQ__Cancha__40F9A206F9F0429A").IsUnique();

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

            entity.HasOne(d => d.CodigoUbigeoNavigation).WithMany(p => p.Cancha)
                .HasForeignKey(d => d.CodigoUbigeo)
                .HasConstraintName("FK__Cancha__codigoUb__6FE99F9F");

            entity.HasOne(d => d.IdEstadoCanchaNavigation).WithMany(p => p.Cancha)
                .HasForeignKey(d => d.IdEstadoCancha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cancha__idEstado__6EF57B66");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Cancha)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cancha__idProvee__6D0D32F4");

            entity.HasOne(d => d.IdTipoSuperficieNavigation).WithMany(p => p.Cancha)
                .HasForeignKey(d => d.IdTipoSuperficie)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cancha__idTipoSu__6E01572D");
        });

        modelBuilder.Entity<CanchaFavorita>(entity =>
        {
            entity.HasKey(e => new { e.IdUsuario, e.IdCancha }).HasName("PK__CanchaFa__93BBF23855B1922F");

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
                .HasConstraintName("FK__CanchaFav__idCan__02FC7413");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.CanchaFavorita)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CanchaFav__idUsu__02084FDA");
        });

        modelBuilder.Entity<Comision>(entity =>
        {
            entity.HasKey(e => e.IdComision).HasName("PK__Comision__12A3EDC2EC6AD17F");

            entity.Property(e => e.IdComision).HasColumnName("idComision");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("createDate");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaFin).HasColumnName("fechaFin");
            entity.Property(e => e.FechaInicio)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("fechaInicio");
            entity.Property(e => e.Porcentaje)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("porcentaje");
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

        modelBuilder.Entity<ComprobantePago>(entity =>
        {
            entity.HasKey(e => e.IdComprobantePago).HasName("PK__Comproba__31D0A1D9A25E54E1");

            entity.HasIndex(e => e.NumeroComprobante, "UQ__Comproba__20F00E4DCC0646B7").IsUnique();

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
                .HasConstraintName("FK__Comproban__idPag__4F47C5E3");
        });

        modelBuilder.Entity<ConfiguracionProveedor>(entity =>
        {
            entity.HasKey(e => e.IdConfiguracionProveedor).HasName("PK__Configur__23C7C60D091820FC");

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
                .HasConstraintName("FK__Configura__idPro__5812160E");
        });

        modelBuilder.Entity<DetalleReserva>(entity =>
        {
            entity.HasKey(e => e.IdDetalleReserva).HasName("PK__DetalleR__74EEC7D16C942F0E");

            entity.HasIndex(e => e.IdReserva, "idx_reserva");

            entity.Property(e => e.IdDetalleReserva).HasColumnName("idDetalleReserva");
            entity.Property(e => e.DuracionHoras)
                .HasColumnType("decimal(4, 2)")
                .HasColumnName("duracionHoras");
            entity.Property(e => e.HoraFin).HasColumnName("horaFin");
            entity.Property(e => e.HoraInicio).HasColumnName("horaInicio");
            entity.Property(e => e.IdReserva).HasColumnName("idReserva");
            entity.Property(e => e.PrecioHora)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precioHora");
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("subtotal");

            entity.HasOne(d => d.IdReservaNavigation).WithMany(p => p.DetalleReserva)
                .HasForeignKey(d => d.IdReserva)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetalleRe__idRes__42E1EEFE");
        });

        modelBuilder.Entity<DiaSemana>(entity =>
        {
            entity.HasKey(e => e.IdDiaSemana).HasName("PK__DiaSeman__10EB836BADC58142");

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
            entity.HasKey(e => e.IdEstadoCancha).HasName("PK__EstadoCa__3B089FABF0C331B1");

            entity.HasIndex(e => e.Codigo, "UQ__EstadoCa__40F9A2062C06BD20").IsUnique();

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
            entity.HasKey(e => e.IdEstadoPago).HasName("PK__EstadoPa__03C5BA22D086036A");

            entity.HasIndex(e => e.Codigo, "UQ__EstadoPa__40F9A206893080F7").IsUnique();

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
            entity.HasKey(e => e.IdEstadoProveedor).HasName("PK__EstadoPr__B0AF2C73D761D2E7");

            entity.HasIndex(e => e.Codigo, "UQ__EstadoPr__40F9A206AFFB404E").IsUnique();

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
            entity.HasKey(e => e.IdEstadoReserva).HasName("PK__EstadoRe__AC7BB706F3447F74");

            entity.HasIndex(e => e.Codigo, "UQ__EstadoRe__40F9A206DE644D9F").IsUnique();

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
            entity.HasKey(e => e.IdEstadoUsuario).HasName("PK__EstadoUs__570885731AC696A6");

            entity.HasIndex(e => e.Codigo, "UQ__EstadoUs__40F9A206956CBDAA").IsUnique();

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

        modelBuilder.Entity<HorarioCancha>(entity =>
        {
            entity.HasKey(e => e.IdHorarioCancha).HasName("PK__HorarioC__825B785D870D8294");

            entity.HasIndex(e => new { e.IdCancha, e.IdDiaSemana, e.HoraInicio }, "UQ_HorarioCancha").IsUnique();

            entity.HasIndex(e => e.IdCancha, "idx_cancha");

            entity.HasIndex(e => e.IdDiaSemana, "idx_dia");

            entity.Property(e => e.IdHorarioCancha).HasColumnName("idHorarioCancha");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("createDate");
            entity.Property(e => e.HoraFin).HasColumnName("horaFin");
            entity.Property(e => e.HoraInicio).HasColumnName("horaInicio");
            entity.Property(e => e.IdCancha).HasColumnName("idCancha");
            entity.Property(e => e.IdDiaSemana).HasColumnName("idDiaSemana");
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
                .HasConstraintName("FK__HorarioCa__idCan__160F4887");

            entity.HasOne(d => d.IdDiaSemanaNavigation).WithMany(p => p.HorarioCancha)
                .HasForeignKey(d => d.IdDiaSemana)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HorarioCa__idDia__17036CC0");
        });

        modelBuilder.Entity<ImagenCancha>(entity =>
        {
            entity.HasKey(e => e.IdImagenCancha).HasName("PK__ImagenCa__A5EF7FB13186D7C1");

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
                .HasConstraintName("FK__ImagenCan__idCan__7C4F7684");
        });

        modelBuilder.Entity<MetodoPago>(entity =>
        {
            entity.HasKey(e => e.IdMetodoPago).HasName("PK__MetodoPa__817BFC32C43D8580");

            entity.HasIndex(e => e.Codigo, "UQ__MetodoPa__40F9A206FC924256").IsUnique();

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
            entity.HasKey(e => e.IdNotificacion).HasName("PK__Notifica__AFE1D7E452FD6DA7");

            entity.HasIndex(e => e.FechaCreacion, "idx_fecha");

            entity.HasIndex(e => e.Leido, "idx_leido");

            entity.HasIndex(e => e.IdUsuario, "idx_usuario");

            entity.Property(e => e.IdNotificacion).HasColumnName("idNotificacion");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.FechaLeido).HasColumnName("fechaLeido");
            entity.Property(e => e.IdReserva).HasColumnName("idReserva");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Leido)
                .HasDefaultValue(false)
                .HasColumnName("leido");
            entity.Property(e => e.Mensaje)
                .HasColumnType("text")
                .HasColumnName("mensaje");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tipo");
            entity.Property(e => e.Titulo)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("titulo");

            entity.HasOne(d => d.IdReservaNavigation).WithMany(p => p.Notificacion)
                .HasForeignKey(d => d.IdReserva)
                .HasConstraintName("FK__Notificac__idRes__55009F39");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Notificacion)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificac__idUsu__531856C7");
        });

        modelBuilder.Entity<Operador>(entity =>
        {
            entity.HasKey(e => e.IdOperador).HasName("PK__Operador__D9DC4D4E56F8FA76");

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
                .HasConstraintName("FK__Operador__idProv__236943A5");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Operador)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Operador__idUsua__22751F6C");
        });

        modelBuilder.Entity<OperadorCancha>(entity =>
        {
            entity.HasKey(e => e.IdOperadorCancha).HasName("PK__Operador__D9620313F5E9338F");

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
                .HasConstraintName("FK__OperadorC__idCan__29221CFB");

            entity.HasOne(d => d.IdOperadorNavigation).WithMany(p => p.OperadorCancha)
                .HasForeignKey(d => d.IdOperador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OperadorC__idOpe__282DF8C2");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.IdPago).HasName("PK__Pago__BD2295AD0486DDB1");

            entity.HasIndex(e => e.CulqiChargeId, "idx_culqiChargeId");

            entity.HasIndex(e => e.IdEstadoPago, "idx_estado");

            entity.HasIndex(e => e.IdReserva, "idx_reserva");

            entity.Property(e => e.IdPago).HasColumnName("idPago");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
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
                .HasConstraintName("FK__Pago__idEstadoPa__489AC854");

            entity.HasOne(d => d.IdMetodoPagoNavigation).WithMany(p => p.Pago)
                .HasForeignKey(d => d.IdMetodoPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pago__idMetodoPa__47A6A41B");

            entity.HasOne(d => d.IdOperadorNavigation).WithMany(p => p.Pago)
                .HasForeignKey(d => d.IdOperador)
                .HasConstraintName("FK__Pago__idOperador__498EEC8D");

            entity.HasOne(d => d.IdReservaNavigation).WithMany(p => p.Pago)
                .HasForeignKey(d => d.IdReserva)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pago__idReserva__45BE5BA9");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.IdProveedor).HasName("PK__Proveedo__A3FA8E6B96F3D8D2");

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
                .HasConstraintName("FK__Proveedor__idEst__52593CB8");

            entity.HasOne(d => d.IdTipoProveedorNavigation).WithMany(p => p.Proveedor)
                .HasForeignKey(d => d.IdTipoProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Proveedor__idTip__5165187F");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Proveedor)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Proveedor__idUsu__5070F446");
        });

        modelBuilder.Entity<Entity.Reserva>(entity =>
        {
            entity.HasKey(e => e.IdReserva).HasName("PK__Reserva__94D104C8A948DF7C");

            entity.HasIndex(e => e.CodigoReserva, "UQ__Reserva__EFEC21CCC68193DA").IsUnique();

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
                .HasConstraintName("FK__Reserva__idCanch__3A4CA8FD");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Reserva)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reserva__idClien__395884C4");

            entity.HasOne(d => d.IdEstadoReservaNavigation).WithMany(p => p.Reserva)
                .HasForeignKey(d => d.IdEstadoReserva)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reserva__idEstad__3D2915A8");

            entity.HasOne(d => d.IdOperadorConfirmoNavigation).WithMany(p => p.Reserva)
                .HasForeignKey(d => d.IdOperadorConfirmo)
                .HasConstraintName("FK__Reserva__idOpera__3E1D39E1");

            entity.HasOne(d => d.IdTipoDeporteNavigation).WithMany(p => p.Reserva)
                .HasForeignKey(d => d.IdTipoDeporte)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reserva__idTipoD__3B40CD36");
        });

        modelBuilder.Entity<Servicio>(entity =>
        {
            entity.HasKey(e => e.IdServicio).HasName("PK__Servicio__CEB981195F2CABC6");

            entity.HasIndex(e => e.Codigo, "UQ__Servicio__40F9A20633A5DC1E").IsUnique();

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
            entity.HasKey(e => e.IdServicioCancha).HasName("PK__Servicio__56B6658C80EE9081");

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
                .HasConstraintName("FK__ServicioC__idCan__0C85DE4D");

            entity.HasOne(d => d.IdServicioNavigation).WithMany(p => p.ServicioCancha)
                .HasForeignKey(d => d.IdServicio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ServicioC__idSer__0D7A0286");
        });

        modelBuilder.Entity<TipoDeporte>(entity =>
        {
            entity.HasKey(e => e.IdTipoDeporte).HasName("PK__TipoDepo__913C11BF7A0A7474");

            entity.HasIndex(e => e.Codigo, "UQ__TipoDepo__40F9A206B2B138A8").IsUnique();

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
            entity.HasKey(e => e.IdTipoDeporteCancha).HasName("PK__TipoDepo__781F42EA65E50E6B");

            entity.HasIndex(e => e.IdCancha, "idx_cancha");

            entity.HasIndex(e => e.IdTipoDeporte, "idx_deporte");

            entity.Property(e => e.IdTipoDeporteCancha).HasColumnName("idTipoDeporteCancha");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("createDate");
            entity.Property(e => e.IdCancha).HasColumnName("idCancha");
            entity.Property(e => e.IdTipoDeporte).HasColumnName("idTipoDeporte");
            entity.Property(e => e.UserNameCreate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameCreate");

            entity.HasOne(d => d.IdCanchaNavigation).WithMany(p => p.TipoDeporteCancha)
                .HasForeignKey(d => d.IdCancha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TipoDepor__idCan__76969D2E");

            entity.HasOne(d => d.IdTipoDeporteNavigation).WithMany(p => p.TipoDeporteCancha)
                .HasForeignKey(d => d.IdTipoDeporte)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TipoDepor__idTip__778AC167");
        });

        modelBuilder.Entity<TipoProveedor>(entity =>
        {
            entity.HasKey(e => e.IdTipoProveedor).HasName("PK__TipoProv__3CDA60062C62677A");

            entity.HasIndex(e => e.Codigo, "UQ__TipoProv__40F9A206B55B7C2C").IsUnique();

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
            entity.HasKey(e => e.IdTipoSuperficie).HasName("PK__TipoSupe__712490DA45C3EC44");

            entity.HasIndex(e => e.Codigo, "UQ__TipoSupe__40F9A2061391E7A0").IsUnique();

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
            entity.HasKey(e => e.CodigoUbigeo).HasName("PK__Ubigeo__B096A3D7017E02EE");

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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
