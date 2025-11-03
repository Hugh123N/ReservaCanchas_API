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

    public virtual DbSet<Cancha> Cancha { get; set; }

    public virtual DbSet<CanchaFavorita> CanchaFavorita { get; set; }

    public virtual DbSet<Comision> Comision { get; set; }

    public virtual DbSet<DiaSemana> DiaSemana { get; set; }

    public virtual DbSet<Disponibilidad> Disponibilidad { get; set; }

    public virtual DbSet<EstadoCancha> EstadoCancha { get; set; }

    public virtual DbSet<EstadoPago> EstadoPago { get; set; }

    public virtual DbSet<EstadoProveedor> EstadoProveedor { get; set; }

    public virtual DbSet<EstadoReserva> EstadoReserva { get; set; }

    public virtual DbSet<EstadoUsuario> EstadoUsuario { get; set; }

    public virtual DbSet<ImagenCancha> ImagenCancha { get; set; }

    public virtual DbSet<IntentoLogin> IntentoLogin { get; set; }

    public virtual DbSet<MetodoPago> MetodoPago { get; set; }

    public virtual DbSet<Notificacion> Notificacion { get; set; }

    public virtual DbSet<Operador> Operador { get; set; }

    public virtual DbSet<OperadorCancha> OperadorCancha { get; set; }

    public virtual DbSet<Pago> Pago { get; set; }

    public virtual DbSet<Proveedor> Proveedor { get; set; }

    public virtual DbSet<Entity.Reserva> Reserva { get; set; }

    public virtual DbSet<ReservaDetalle> ReservaDetalle { get; set; }

    public virtual DbSet<TipoCancha> TipoCancha { get; set; }

    public virtual DbSet<TipoProveedor> TipoProveedor { get; set; }

    public virtual DbSet<Ubigeo> Ubigeo { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRoles>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetRo__3214EC07C426D052");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CreateDate).HasColumnName("createDate");
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

        modelBuilder.Entity<AspNetUserLogins>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AspNetUse__UserI__3D5E1FD2");
        });

        modelBuilder.Entity<AspNetUserTokens>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
        });

        modelBuilder.Entity<AspNetUsers>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetUs__3214EC07B870A7AF");

            entity.HasIndex(e => e.Email, "UQ__AspNetUs__A9D10534A8EEEEDE").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CreateDate).HasColumnName("createDate");
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
                .HasConstraintName("FK__AspNetUse__idEst__2E1BDC42");

            entity.HasMany(d => d.Role).WithMany(p => p.User)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRoles",
                    r => r.HasOne<AspNetRoles>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__AspNetUse__RoleI__38996AB5"),
                    l => l.HasOne<AspNetUsers>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__AspNetUse__UserI__37A5467C"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                    });
        });

        modelBuilder.Entity<Cancha>(entity =>
        {
            entity.HasKey(e => e.IdCancha).HasName("PK__Cancha__7ECD19EE74C6BE40");

            entity.Property(e => e.IdCancha).HasColumnName("idCancha");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CodigoUbigeo)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("codigoUbigeo");
            entity.Property(e => e.CreateDate).HasColumnName("createDate");
            entity.Property(e => e.Descripcion)
                .HasColumnType("text")
                .HasColumnName("descripcion");
            entity.Property(e => e.Direccion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("direccion");
            entity.Property(e => e.DuracionPreReserva).HasColumnName("duracionPreReserva");
            entity.Property(e => e.IdEstadoCancha).HasColumnName("idEstadoCancha");
            entity.Property(e => e.IdProveedor).HasColumnName("idProveedor");
            entity.Property(e => e.IdTipoCancha).HasColumnName("idTipoCancha");
            entity.Property(e => e.Latitud)
                .HasColumnType("decimal(10, 8)")
                .HasColumnName("latitud");
            entity.Property(e => e.Longitud)
                .HasColumnType("decimal(10, 8)")
                .HasColumnName("longitud");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.PorcentajeAdelanto)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("porcentajeAdelanto");
            entity.Property(e => e.PrecioHora)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precioHora");
            entity.Property(e => e.TelefonoCancha)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("telefonoCancha");
            entity.Property(e => e.Ubicacion)
                .HasColumnType("ntext")
                .HasColumnName("ubicacion");
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
                .HasConstraintName("FK__Cancha__codigoUb__5EBF139D");

            entity.HasOne(d => d.IdEstadoCanchaNavigation).WithMany(p => p.Cancha)
                .HasForeignKey(d => d.IdEstadoCancha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cancha__idEstado__5FB337D6");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Cancha)
                .HasForeignKey(d => d.IdProveedor)
                .HasConstraintName("FK__Cancha__idProvee__5DCAEF64");

            entity.HasOne(d => d.IdTipoCanchaNavigation).WithMany(p => p.Cancha)
                .HasForeignKey(d => d.IdTipoCancha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cancha__idTipoCa__5CD6CB2B");
        });

        modelBuilder.Entity<CanchaFavorita>(entity =>
        {
            entity.HasKey(e => new { e.IdUsuario, e.IdCancha }).HasName("PK__CanchaFa__93BBF238B6D90D82");

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
                .HasConstraintName("FK__CanchaFav__idCan__693CA210");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.CanchaFavorita)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CanchaFav__idUsu__68487DD7");
        });

        modelBuilder.Entity<Comision>(entity =>
        {
            entity.HasKey(e => e.IdComision).HasName("PK__Comision__12A3EDC253AF59DC");

            entity.Property(e => e.IdComision).HasColumnName("idComision");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CreateDate).HasColumnName("createDate");
            entity.Property(e => e.FechaActualizacion).HasColumnName("fechaActualizacion");
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

        modelBuilder.Entity<DiaSemana>(entity =>
        {
            entity.HasKey(e => e.IdDiaSemana).HasName("PK__DiaSeman__10EB836B592D282D");

            entity.Property(e => e.IdDiaSemana).HasColumnName("idDiaSemana");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Disponibilidad>(entity =>
        {
            entity.HasKey(e => e.IdDisponibilidad).HasName("PK__Disponib__96A3EB6AE860B521");

            entity.Property(e => e.IdDisponibilidad).HasColumnName("idDisponibilidad");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CreateDate).HasColumnName("createDate");
            entity.Property(e => e.Disponible)
                .HasDefaultValue(true)
                .HasColumnName("disponible");
            entity.Property(e => e.HoraFin).HasColumnName("horaFin");
            entity.Property(e => e.HoraInicio).HasColumnName("horaInicio");
            entity.Property(e => e.IdCancha).HasColumnName("idCancha");
            entity.Property(e => e.IdDiaSemana).HasColumnName("idDiaSemana");
            entity.Property(e => e.UpdateDate).HasColumnName("updateDate");
            entity.Property(e => e.UserNameCreate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameCreate");
            entity.Property(e => e.UserNameUpdate)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("userNameUpdate");

            entity.HasOne(d => d.IdCanchaNavigation).WithMany(p => p.Disponibilidad)
                .HasForeignKey(d => d.IdCancha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Disponibi__idCan__160F4887");

            entity.HasOne(d => d.IdDiaSemanaNavigation).WithMany(p => p.Disponibilidad)
                .HasForeignKey(d => d.IdDiaSemana)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Disponibi__idDia__17036CC0");
        });

        modelBuilder.Entity<EstadoCancha>(entity =>
        {
            entity.HasKey(e => e.IdEstadoCancha).HasName("PK__EstadoCa__3B089FABBDA950E1");

            entity.HasIndex(e => e.Codigo, "UQ__EstadoCa__40F9A20698CDDB45").IsUnique();

            entity.Property(e => e.IdEstadoCancha).HasColumnName("idEstadoCancha");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("codigo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<EstadoPago>(entity =>
        {
            entity.HasKey(e => e.IdEstadoPago).HasName("PK__EstadoPa__03C5BA222A0733AE");

            entity.HasIndex(e => e.Codigo, "UQ__EstadoPa__40F9A206682EA575").IsUnique();

            entity.Property(e => e.IdEstadoPago).HasColumnName("idEstadoPago");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("codigo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<EstadoProveedor>(entity =>
        {
            entity.HasKey(e => e.IdEstadoProveedor).HasName("PK__EstadoPr__B0AF2C73D647EDEA");

            entity.HasIndex(e => e.Codigo, "UQ__EstadoPr__40F9A206CA4044C7").IsUnique();

            entity.Property(e => e.IdEstadoProveedor).HasColumnName("idEstadoProveedor");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("codigo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<EstadoReserva>(entity =>
        {
            entity.HasKey(e => e.IdEstadoReserva).HasName("PK__EstadoRe__AC7BB70656B097AC");

            entity.HasIndex(e => e.Codigo, "UQ__EstadoRe__40F9A206614E59BF").IsUnique();

            entity.Property(e => e.IdEstadoReserva).HasColumnName("idEstadoReserva");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("codigo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<EstadoUsuario>(entity =>
        {
            entity.HasKey(e => e.IdEstadoUsuario).HasName("PK__EstadoUs__57088573CB61C42D");

            entity.HasIndex(e => e.Codigo, "UQ__EstadoUs__40F9A2068522DA4E").IsUnique();

            entity.Property(e => e.IdEstadoUsuario).HasColumnName("idEstadoUsuario");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("codigo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<ImagenCancha>(entity =>
        {
            entity.HasKey(e => e.IdImagenCancha).HasName("PK__ImagenCa__A5EF7FB1DAC25E2C");

            entity.Property(e => e.IdImagenCancha).HasColumnName("idImagenCancha");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CreateDate).HasColumnName("createDate");
            entity.Property(e => e.EsPrincipal)
                .HasDefaultValue(false)
                .HasColumnName("esPrincipal");
            entity.Property(e => e.IdCancha).HasColumnName("idCancha");
            entity.Property(e => e.UpdateDate).HasColumnName("updateDate");
            entity.Property(e => e.UrlImagen)
                .HasMaxLength(255)
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
                .HasConstraintName("FK__ImagenCan__idCan__6383C8BA");
        });

        modelBuilder.Entity<IntentoLogin>(entity =>
        {
            entity.HasKey(e => e.IdIntentoLogin).HasName("PK__IntentoL__0EDA4F32DF12BDB3");

            entity.Property(e => e.IdIntentoLogin).HasColumnName("idIntentoLogin");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Exitoso).HasColumnName("exitoso");
            entity.Property(e => e.FechaIntento)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("fechaIntento");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.IntentoLogin)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__IntentoLo__idUsu__4222D4EF");
        });

        modelBuilder.Entity<MetodoPago>(entity =>
        {
            entity.HasKey(e => e.IdMetodoPago).HasName("PK__MetodoPa__817BFC3235C7B0F9");

            entity.HasIndex(e => e.Codigo, "UQ__MetodoPa__40F9A20654493321").IsUnique();

            entity.Property(e => e.IdMetodoPago).HasColumnName("idMetodoPago");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("codigo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.HasKey(e => e.IdNotificacion).HasName("PK__Notifica__AFE1D7E4A9CAE08F");

            entity.Property(e => e.IdNotificacion).HasColumnName("idNotificacion");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(sysdatetimeoffset())")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Leido)
                .HasDefaultValue(false)
                .HasColumnName("leido");
            entity.Property(e => e.Mensaje)
                .HasColumnType("text")
                .HasColumnName("mensaje");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Notificacion)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificac__idUsu__1BC821DD");
        });

        modelBuilder.Entity<Operador>(entity =>
        {
            entity.HasKey(e => e.IdOperador).HasName("PK__Operador__D9DC4D4E3346F2E4");

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
                .HasConstraintName("FK__Operador__idProv__6EF57B66");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Operador)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Operador__idUsua__6E01572D");
        });

        modelBuilder.Entity<OperadorCancha>(entity =>
        {
            entity.HasKey(e => new { e.IdOperador, e.IdCancha }).HasName("PK__Operador__2E309CD02B3F0B88");

            entity.Property(e => e.IdOperador).HasColumnName("idOperador");
            entity.Property(e => e.IdCancha).HasColumnName("idCancha");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");

            entity.HasOne(d => d.IdCanchaNavigation).WithMany(p => p.OperadorCancha)
                .HasForeignKey(d => d.IdCancha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OperadorC__idCan__73BA3083");

            entity.HasOne(d => d.IdOperadorNavigation).WithMany(p => p.OperadorCancha)
                .HasForeignKey(d => d.IdOperador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OperadorC__idOpe__72C60C4A");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.IdPago).HasName("PK__Pago__BD2295AD765700D9");

            entity.HasIndex(e => e.CulqiChargeId, "IX_Pago_CulqiChargeId").HasFilter("([CulqiChargeId] IS NOT NULL)");

            entity.Property(e => e.IdPago).HasColumnName("idPago");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CodigoOperacion)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("codigoOperacion");
            entity.Property(e => e.CreateDate).HasColumnName("createDate");
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
            entity.Property(e => e.IdPlan).HasColumnName("idPlan");
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
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("montoAdelanto");
            entity.Property(e => e.MontoPendiente)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("montoPendiente");
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
                .HasConstraintName("FK__Pago__idEstadoPa__0F624AF8");

            entity.HasOne(d => d.IdMetodoPagoNavigation).WithMany(p => p.Pago)
                .HasForeignKey(d => d.IdMetodoPago)
                .HasConstraintName("FK__Pago__idMetodoPa__0E6E26BF");

            entity.HasOne(d => d.IdReservaNavigation).WithMany(p => p.Pago)
                .HasForeignKey(d => d.IdReserva)
                .HasConstraintName("FK__Pago__idReserva__0C85DE4D");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.IdProveedor).HasName("PK__Proveedo__A3FA8E6BF4BD21D6");

            entity.HasIndex(e => e.RazonSocial, "UQ__Proveedo__17BADCA0B996ADD2").IsUnique();

            entity.HasIndex(e => e.Ruc, "UQ__Proveedo__C2B74E61B3AA9B71").IsUnique();

            entity.Property(e => e.IdProveedor).HasColumnName("idProveedor");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CreateDate).HasColumnName("createDate");
            entity.Property(e => e.IdEstadoProveedor).HasColumnName("idEstadoProveedor");
            entity.Property(e => e.IdTipoProveedor).HasColumnName("idTipoProveedor");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.RazonSocial)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("razonSocial");
            entity.Property(e => e.Ruc)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ruc");
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
            entity.HasKey(e => e.IdReserva).HasName("PK__Reserva__94D104C8486BC0CF");

            entity.HasIndex(e => e.CodigoReserva, "UQ__Reserva__EFEC21CC594381E0").IsUnique();

            entity.Property(e => e.IdReserva).HasColumnName("idReserva");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CodigoReserva)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("codigoReserva");
            entity.Property(e => e.CreateDate).HasColumnName("createDate");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.FechaExpiracionPreReserva).HasColumnName("fechaExpiracionPreReserva");
            entity.Property(e => e.NotificacionAdvertenciaEnviada)
                .HasDefaultValue(false)
                .HasColumnName("notificacionAdvertenciaEnviada");
            entity.Property(e => e.IdCancha).HasColumnName("idCancha");
            entity.Property(e => e.IdEstadoReserva).HasColumnName("idEstadoReserva");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto");
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
                .HasConstraintName("FK__Reserva__idCanch__7D439ABD");

            entity.HasOne(d => d.IdEstadoReservaNavigation).WithMany(p => p.Reserva)
                .HasForeignKey(d => d.IdEstadoReserva)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reserva__idEstad__7E37BEF6");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Reserva)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reserva__idUsuar__7C4F7684");
        });

        modelBuilder.Entity<ReservaDetalle>(entity =>
        {
            entity.HasKey(e => e.IdReservaDetalle).HasName("PK__ReservaD__E19B867F8B1BCC58");

            entity.Property(e => e.IdReservaDetalle).HasColumnName("idReservaDetalle");
            entity.Property(e => e.HoraFin).HasColumnName("horaFin");
            entity.Property(e => e.HoraInicio).HasColumnName("horaInicio");
            entity.Property(e => e.IdReserva).HasColumnName("idReserva");

            entity.HasOne(d => d.IdReservaNavigation).WithMany(p => p.ReservaDetalle)
                .HasForeignKey(d => d.IdReserva)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ReservaDe__idRes__02084FDA");
        });

        modelBuilder.Entity<TipoCancha>(entity =>
        {
            entity.HasKey(e => e.IdTipoCancha).HasName("PK__TipoCanc__1E32E1ED735E4516");

            entity.Property(e => e.IdTipoCancha).HasColumnName("idTipoCancha");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<TipoProveedor>(entity =>
        {
            entity.HasKey(e => e.IdTipoProveedor).HasName("PK__TipoProv__3CDA6006216C216E");

            entity.HasIndex(e => e.Codigo, "UQ__TipoProv__40F9A20640E27EDF").IsUnique();

            entity.Property(e => e.IdTipoProveedor).HasColumnName("idTipoProveedor");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("codigo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Ubigeo>(entity =>
        {
            entity.HasKey(e => e.CodigoUbigeo).HasName("PK__Ubigeo__B096A3D7EE322A1A");

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
