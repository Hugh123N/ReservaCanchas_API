using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Reserva.Entity.Models;

public partial class ReservaCanchasContext : DbContext
{
    public ReservaCanchasContext()
    {
    }

    public ReservaCanchasContext(DbContextOptions<ReservaCanchasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cancha> Canchas { get; set; }

    public virtual DbSet<Comision> Comisions { get; set; }

    public virtual DbSet<Departamento> Departamentos { get; set; }

    public virtual DbSet<DetallePago> DetallePagos { get; set; }

    public virtual DbSet<DiaSemana> DiaSemanas { get; set; }

    public virtual DbSet<Disponibilidad> Disponibilidads { get; set; }

    public virtual DbSet<Distrito> Distritos { get; set; }

    public virtual DbSet<EstadoCancha> EstadoCanchas { get; set; }

    public virtual DbSet<EstadoPago> EstadoPagos { get; set; }

    public virtual DbSet<EstadoProveedor> EstadoProveedors { get; set; }

    public virtual DbSet<EstadoReserva> EstadoReservas { get; set; }

    public virtual DbSet<EstadoUsuario> EstadoUsuarios { get; set; }

    public virtual DbSet<GananciaProveedor> GananciaProveedors { get; set; }

    public virtual DbSet<ImagenCancha> ImagenCanchas { get; set; }

    public virtual DbSet<MetodoPago> MetodoPagos { get; set; }

    public virtual DbSet<Notificacion> Notificacions { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Proveedor> Proveedors { get; set; }

    public virtual DbSet<Provincia> Provincia { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<TipoCancha> TipoCanchas { get; set; }

    public virtual DbSet<TipoProveedor> TipoProveedors { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Zona> Zonas { get; set; }
    /*
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=10.147.18.177;Initial Catalog=ReservaCanchas;User ID=sa;Password=Basamea1;TrustServerCertificate=True;");
    
     scaffold-dbContext "Server=10.147.18.177;Initial Catalog=ReservaCanchas;User ID=sa;Password=Basamea1;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -OutPutDir Models -force
     */
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cancha>(entity =>
        {
            entity.HasKey(e => e.IdCancha).HasName("PK__Cancha__7ECD19EE24F60C6C");

            entity.ToTable("Cancha");

            entity.Property(e => e.IdCancha).HasColumnName("idCancha");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CreadoPor).HasColumnName("creadoPor");
            entity.Property(e => e.Descripcion)
                .HasColumnType("text")
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaActualizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaActualizacion");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.IdEstadoCancha).HasColumnName("idEstadoCancha");
            entity.Property(e => e.IdProveedor).HasColumnName("idProveedor");
            entity.Property(e => e.IdTipoCancha).HasColumnName("idTipoCancha");
            entity.Property(e => e.IdZona).HasColumnName("idZona");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.PrecioHora)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precioHora");
            entity.Property(e => e.Ubicacion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ubicacion");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.Canchas)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("FK__Cancha__creadoPo__5FB337D6");

            entity.HasOne(d => d.IdEstadoCanchaNavigation).WithMany(p => p.Canchas)
                .HasForeignKey(d => d.IdEstadoCancha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cancha__idEstado__628FA481");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Canchas)
                .HasForeignKey(d => d.IdProveedor)
                .HasConstraintName("FK__Cancha__idProvee__60A75C0F");

            entity.HasOne(d => d.IdTipoCanchaNavigation).WithMany(p => p.Canchas)
                .HasForeignKey(d => d.IdTipoCancha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cancha__idTipoCa__5EBF139D");

            entity.HasOne(d => d.IdZonaNavigation).WithMany(p => p.Canchas)
                .HasForeignKey(d => d.IdZona)
                .HasConstraintName("FK__Cancha__idZona__619B8048");
        });

        modelBuilder.Entity<Comision>(entity =>
        {
            entity.HasKey(e => e.IdComision).HasName("PK__Comision__12A3EDC2C47730D4");

            entity.ToTable("Comision");

            entity.Property(e => e.IdComision).HasColumnName("idComision");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.FechaActualizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaActualizacion");
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("fechaFin");
            entity.Property(e => e.FechaInicio)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaInicio");
            entity.Property(e => e.Porcentaje)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("porcentaje");
        });

        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.HasKey(e => e.IdDepartamento).HasName("PK__Departam__C225F98D26DBEC92");

            entity.ToTable("Departamento");

            entity.Property(e => e.IdDepartamento).HasColumnName("idDepartamento");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<DetallePago>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("DetallePago");

            entity.Property(e => e.IdDetallePago)
                .ValueGeneratedOnAdd()
                .HasColumnName("idDetallePago");
            entity.Property(e => e.IdPago).HasColumnName("idPago");
            entity.Property(e => e.IdReserva).HasColumnName("idReserva");

            entity.HasOne(d => d.IdPagoNavigation).WithMany()
                .HasForeignKey(d => d.IdPago)
                .HasConstraintName("FK__DetallePa__idPag__03F0984C");

            entity.HasOne(d => d.IdReservaNavigation).WithMany()
                .HasForeignKey(d => d.IdReserva)
                .HasConstraintName("FK__DetallePa__idRes__04E4BC85");
        });

        modelBuilder.Entity<DiaSemana>(entity =>
        {
            entity.HasKey(e => e.IdDiaSemana).HasName("PK__DiaSeman__10EB836B711C75E8");

            entity.ToTable("DiaSemana");

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
            entity.HasKey(e => e.IdDisponibilidad).HasName("PK__Disponib__3213E83F51973043");

            entity.ToTable("Disponibilidad");

            entity.Property(e => e.IdDisponibilidad).HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CreadoPor).HasColumnName("creadoPor");
            entity.Property(e => e.FechaActualizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaActualizacion");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.HoraFin).HasColumnName("horaFin");
            entity.Property(e => e.HoraInicio).HasColumnName("horaInicio");
            entity.Property(e => e.IdCancha).HasColumnName("idCancha");
            entity.Property(e => e.IdDiaSemana).HasColumnName("idDiaSemana");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.Disponibilidads)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("FK__Disponibi__cread__0C85DE4D");

            entity.HasOne(d => d.IdCanchaNavigation).WithMany(p => p.Disponibilidads)
                .HasForeignKey(d => d.IdCancha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Disponibi__idCan__0A9D95DB");

            entity.HasOne(d => d.IdDiaSemanaNavigation).WithMany(p => p.Disponibilidads)
                .HasForeignKey(d => d.IdDiaSemana)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Disponibi__idDia__0B91BA14");
        });

        modelBuilder.Entity<Distrito>(entity =>
        {
            entity.HasKey(e => e.IdDistrito).HasName("PK__Distrito__494092A84301EA00");

            entity.ToTable("Distrito");

            entity.Property(e => e.IdDistrito).HasColumnName("idDistrito");
            entity.Property(e => e.IdProvincia).HasColumnName("idProvincia");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdProvinciaNavigation).WithMany(p => p.Distritos)
                .HasForeignKey(d => d.IdProvincia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Distrito__idProv__3B75D760");
        });

        modelBuilder.Entity<EstadoCancha>(entity =>
        {
            entity.HasKey(e => e.IdEstadoCancha).HasName("PK__EstadoCa__3B089FAB04B42B96");

            entity.ToTable("EstadoCancha");

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
            entity.HasKey(e => e.IdEstadoPago).HasName("PK__EstadoPa__03C5BA224D375182");

            entity.ToTable("EstadoPago");

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
            entity.HasKey(e => e.IdEstadoProveedor).HasName("PK__EstadoPr__B0AF2C7315BDCA33");

            entity.ToTable("EstadoProveedor");

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
            entity.HasKey(e => e.IdEstadoReserva).HasName("PK__EstadoRe__AC7BB7062AF5E898");

            entity.ToTable("EstadoReserva");

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
            entity.HasKey(e => e.IdEstadoUsuario).HasName("PK__EstadoUs__57088573D3435047");

            entity.ToTable("EstadoUsuario");

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

        modelBuilder.Entity<GananciaProveedor>(entity =>
        {
            entity.HasKey(e => e.IdGananciaProveedor).HasName("PK__Ganancia__138496C122FFDD59");

            entity.ToTable("GananciaProveedor");

            entity.Property(e => e.IdGananciaProveedor).HasColumnName("idGananciaProveedor");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Comision)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("comision");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.GananciaNeta)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("gananciaNeta");
            entity.Property(e => e.IdProveedor).HasColumnName("idProveedor");
            entity.Property(e => e.IdReserva).HasColumnName("idReserva");
            entity.Property(e => e.MontoTotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("montoTotal");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.GananciaProveedors)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GananciaP__idPro__1AD3FDA4");

            entity.HasOne(d => d.IdReservaNavigation).WithMany(p => p.GananciaProveedors)
                .HasForeignKey(d => d.IdReserva)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GananciaP__idRes__19DFD96B");
        });

        modelBuilder.Entity<ImagenCancha>(entity =>
        {
            entity.HasKey(e => e.IdImagenCancha).HasName("PK__ImagenCa__A5EF7FB114E60FA9");

            entity.ToTable("ImagenCancha");

            entity.Property(e => e.IdImagenCancha).HasColumnName("idImagenCancha");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.EsPrincipal)
                .HasDefaultValue(false)
                .HasColumnName("esPrincipal");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.IdCancha).HasColumnName("idCancha");
            entity.Property(e => e.UrlImagen)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("urlImagen");

            entity.HasOne(d => d.IdCanchaNavigation).WithMany(p => p.ImagenCanchas)
                .HasForeignKey(d => d.IdCancha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ImagenCan__idCan__6754599E");
        });

        modelBuilder.Entity<MetodoPago>(entity =>
        {
            entity.HasKey(e => e.IdMetodoPago).HasName("PK__MetodoPa__817BFC32D1405153");

            entity.ToTable("MetodoPago");

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
            entity.HasKey(e => e.IdNotificacion).HasName("PK__Notifica__AFE1D7E4159729FF");

            entity.ToTable("Notificacion");

            entity.Property(e => e.IdNotificacion).HasColumnName("idNotificacion");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Leido)
                .HasDefaultValue(false)
                .HasColumnName("leido");
            entity.Property(e => e.Mensaje)
                .HasColumnType("text")
                .HasColumnName("mensaje");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Notificacions)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificac__idUsu__114A936A");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.IdPago).HasName("PK__Pago__BD2295AD9E5F4017");

            entity.ToTable("Pago");

            entity.Property(e => e.IdPago).HasColumnName("idPago");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CreadoPor).HasColumnName("creadoPor");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.IdEstadoPago).HasColumnName("idEstadoPago");
            entity.Property(e => e.IdMetodoPago).HasColumnName("idMetodoPago");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.PagoCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("FK__Pago__creadoPor__00200768");

            entity.HasOne(d => d.IdEstadoPagoNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdEstadoPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pago__idEstadoPa__7F2BE32F");

            entity.HasOne(d => d.IdMetodoPagoNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdMetodoPago)
                .HasConstraintName("FK__Pago__idMetodoPa__7E37BEF6");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.PagoIdUsuarioNavigations)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pago__idUsuario__7D439ABD");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.IdProveedor).HasName("PK__Proveedo__A3FA8E6B5F095396");

            entity.ToTable("Proveedor");

            entity.Property(e => e.IdProveedor)
                .ValueGeneratedNever()
                .HasColumnName("idProveedor");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Descripcion)
                .HasColumnType("text")
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.IdEstadoProveedor).HasColumnName("idEstadoProveedor");
            entity.Property(e => e.IdTipoProveedor).HasColumnName("idTipoProveedor");
            entity.Property(e => e.RazonSocial)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("razonSocial");
            entity.Property(e => e.Ruc)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ruc");

            entity.HasOne(d => d.IdEstadoProveedorNavigation).WithMany(p => p.Proveedors)
                .HasForeignKey(d => d.IdEstadoProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Proveedor__idEst__5441852A");

            entity.HasOne(d => d.IdProveedorNavigation).WithOne(p => p.Proveedor)
                .HasForeignKey<Proveedor>(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Proveedor__idPro__52593CB8");

            entity.HasOne(d => d.IdTipoProveedorNavigation).WithMany(p => p.Proveedors)
                .HasForeignKey(d => d.IdTipoProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Proveedor__idTip__534D60F1");
        });

        modelBuilder.Entity<Provincia>(entity =>
        {
            entity.HasKey(e => e.IdProvincia).HasName("PK__Provinci__5F9F113CE716594E");

            entity.Property(e => e.IdProvincia).HasColumnName("idProvincia");
            entity.Property(e => e.IdDepartamento).HasColumnName("idDepartamento");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdDepartamentoNavigation).WithMany(p => p.Provincia)
                .HasForeignKey(d => d.IdDepartamento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Provincia__idDep__38996AB5");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.IdReserva).HasName("PK__Reserva__94D104C82EBC7643");

            entity.ToTable("Reserva");

            entity.Property(e => e.IdReserva).HasColumnName("idReserva");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CreadoPor).HasColumnName("creadoPor");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.FechaActualizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaActualizacion");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.HoraFin).HasColumnName("horaFin");
            entity.Property(e => e.HoraInicio).HasColumnName("horaInicio");
            entity.Property(e => e.IdCancha).HasColumnName("idCancha");
            entity.Property(e => e.IdEstadoReserva).HasColumnName("idEstadoReserva");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");

            entity.HasOne(d => d.CreadoPorNavigation).WithMany(p => p.ReservaCreadoPorNavigations)
                .HasForeignKey(d => d.CreadoPor)
                .HasConstraintName("FK__Reserva__creadoP__72C60C4A");

            entity.HasOne(d => d.IdCanchaNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdCancha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reserva__idCanch__70DDC3D8");

            entity.HasOne(d => d.IdEstadoReservaNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdEstadoReserva)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reserva__idEstad__71D1E811");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.ReservaIdUsuarioNavigations)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reserva__idUsuar__6FE99F9F");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Rol__3C872F7618994769");

            entity.ToTable("Rol");

            entity.Property(e => e.IdRol).HasColumnName("idRol");
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

        modelBuilder.Entity<TipoCancha>(entity =>
        {
            entity.HasKey(e => e.IdTipoCancha).HasName("PK__TipoCanc__1E32E1ED8C9432D4");

            entity.ToTable("TipoCancha");

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
            entity.HasKey(e => e.IdTipoProveedor).HasName("PK__TipoProv__3CDA60060037FD8F");

            entity.ToTable("TipoProveedor");

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

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__645723A6BE84E39B");

            entity.ToTable("Usuario");

            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("apellidos");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FechaActualizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaActualizacion");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.IdEstadoUsuario).HasColumnName("idEstadoUsuario");
            entity.Property(e => e.IdRol).HasColumnName("idRol");
            entity.Property(e => e.Imagen)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("imagen");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");

            entity.HasOne(d => d.IdEstadoUsuarioNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdEstadoUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Usuario__idEstad__47DBAE45");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Usuario__idRol__46E78A0C");
        });

        modelBuilder.Entity<Zona>(entity =>
        {
            entity.HasKey(e => e.IdZona).HasName("PK__Zona__1EE4D75CA932BD51");

            entity.ToTable("Zona");

            entity.Property(e => e.IdZona).HasColumnName("idZona");
            entity.Property(e => e.IdDistrito).HasColumnName("idDistrito");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdDistritoNavigation).WithMany(p => p.Zonas)
                .HasForeignKey(d => d.IdDistrito)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Zona__idDistrito__3E52440B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
