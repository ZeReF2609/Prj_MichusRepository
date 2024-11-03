using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Michus.Models;

public partial class MichusContext : DbContext
{

    public MichusContext(DbContextOptions<MichusContext> options)
            : base(options)
    {
    }

    public virtual DbSet<AlmacenIngrediente> AlmacenIngredientes { get; set; }

    public virtual DbSet<AlmacenProducto> AlmacenProductos { get; set; }

    public virtual DbSet<CalificacionesOpinione> CalificacionesOpiniones { get; set; }

    public virtual DbSet<CategoriaGasto> CategoriaGastos { get; set; }

    public virtual DbSet<Categorium> Categoria { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Contacto> Contactos { get; set; }

    public virtual DbSet<Cuenta> Cuentas { get; set; }

    public virtual DbSet<DescuentoCab> DescuentoCabs { get; set; }

    public virtual DbSet<DescuentoConfig> DescuentoConfigs { get; set; }

    public virtual DbSet<DescuentoDetalle> DescuentoDetalles { get; set; }

    public virtual DbSet<DetallePromocion> DetallePromocions { get; set; }

    public virtual DbSet<DetalleVentum> DetalleVenta { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<Evento> Eventos { get; set; }

    public virtual DbSet<Ingrediente> Ingredientes { get; set; }

    public virtual DbSet<InventarioLog> InventarioLogs { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Mesa> Mesas { get; set; }

    public virtual DbSet<MetodoPago> MetodoPagos { get; set; }

    public virtual DbSet<Opinione> Opiniones { get; set; }

    public virtual DbSet<PermisosRol> PermisosRols { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Promocion> Promocions { get; set; }

    public virtual DbSet<PuntosFidelidad> PuntosFidelidads { get; set; }

    public virtual DbSet<RegistroGasto> RegistroGastos { get; set; }

    public virtual DbSet<RegistroIngreso> RegistroIngresos { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Submenu> Submenus { get; set; }

    public virtual DbSet<TipoDocumento> TipoDocumentos { get; set; }

    public virtual DbSet<UsuariosSistema> UsuariosSistemas { get; set; }

    public virtual DbSet<Venta> Ventas { get; set; }

    public virtual DbSet<VentasNoRegistrada> VentasNoRegistradas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AlmacenIngrediente>(entity =>
        {
            entity.HasKey(e => e.IdAlmacen).HasName("PK__ALMACEN___B7DA4D92D3AF1A4F");

            entity.ToTable("ALMACEN_INGREDIENTES");

            entity.Property(e => e.IdAlmacen).HasColumnName("ID_ALMACEN");
            entity.Property(e => e.Cantidad).HasColumnName("CANTIDAD");
            entity.Property(e => e.FechaIngreso)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_INGRESO");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_MODIFICACION");
            entity.Property(e => e.FechaVenc)
                .HasColumnType("datetime")
                .HasColumnName("FECHA_VENC");
            entity.Property(e => e.IdIngrediente).HasColumnName("ID_INGREDIENTE");
            entity.Property(e => e.Ubicacion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("UBICACION");

            entity.HasOne(d => d.IdIngredienteNavigation).WithMany(p => p.AlmacenIngredientes)
                .HasForeignKey(d => d.IdIngrediente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ALMACEN_I__ID_IN__7D439ABD");
        });

        modelBuilder.Entity<AlmacenProducto>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ALMACEN_PRODUCTOS");

            entity.Property(e => e.Cantidad).HasColumnName("CANTIDAD");
            entity.Property(e => e.FechaIngreso)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_INGRESO");
            entity.Property(e => e.FechaVenc)
                .HasColumnType("datetime")
                .HasColumnName("FECHA_VENC");
            entity.Property(e => e.IdAlmacen)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID_ALMACEN");
            entity.Property(e => e.IdProducto)
                .HasMaxLength(8)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_PRODUCTO");
            entity.Property(e => e.Presentacion)
                .HasMaxLength(10)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("PRESENTACION");
            entity.Property(e => e.Ubicacion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("UBICACION");

            entity.HasOne(d => d.IdProductoNavigation).WithMany()
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ALMACEN_P__ID_PR__74AE54BC");
        });

        modelBuilder.Entity<CalificacionesOpinione>(entity =>
        {
            entity.HasKey(e => e.IdCalificacion).HasName("PK__CALIFICA__3300F332536AC7E5");

            entity.ToTable("CALIFICACIONES_OPINIONES");

            entity.Property(e => e.IdCalificacion)
                .HasMaxLength(8)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_CALIFICACION");
            entity.Property(e => e.Calificacion).HasColumnName("CALIFICACION");
            entity.Property(e => e.Estado)
                .HasDefaultValue((byte)1)
                .HasColumnName("ESTADO");
            entity.Property(e => e.FechaCalificacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_CALIFICACION");
            entity.Property(e => e.IdOpinion)
                .HasMaxLength(8)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_OPINION");
            entity.Property(e => e.IdUsuario)
                .HasMaxLength(5)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_USUARIO");

            entity.HasOne(d => d.IdOpinionNavigation).WithMany(p => p.CalificacionesOpiniones)
                .HasForeignKey(d => d.IdOpinion)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__CALIFICAC__ID_OP__42E1EEFE");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.CalificacionesOpiniones)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__CALIFICAC__ID_US__41EDCAC5");
        });

        modelBuilder.Entity<CategoriaGasto>(entity =>
        {
            entity.HasKey(e => e.IdCategoriaGasto).HasName("PK__CATEGORI__1E7616513F5BD791");

            entity.ToTable("CATEGORIA_GASTOS");

            entity.Property(e => e.IdCategoriaGasto)
                .HasMaxLength(8)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_CATEGORIA_GASTO");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("DESCRIPCION");
            entity.Property(e => e.Estado)
                .HasDefaultValue((byte)1)
                .HasColumnName("ESTADO");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_CREACION");
        });

        modelBuilder.Entity<Categorium>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK__CATEGORI__4BD51FA52AEBC21C");

            entity.ToTable("CATEGORIA");

            entity.Property(e => e.IdCategoria)
                .HasMaxLength(8)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_CATEGORIA");
            entity.Property(e => e.Categoria)
                .HasMaxLength(30)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("CATEGORIA");
            entity.Property(e => e.Descripcion)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnType("text")
                .HasColumnName("DESCRIPCION");
            entity.Property(e => e.FechaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_ACTUALIZACION");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PK__CLIENTES__23A341308D7253BC");

            entity.ToTable("CLIENTES");

            entity.Property(e => e.IdCliente)
                .HasMaxLength(5)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_CLIENTE");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("APELLIDOS");
            entity.Property(e => e.DocIdent)
                .HasMaxLength(15)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("DOC_IDENT");
            entity.Property(e => e.FechaNacimiento).HasColumnName("FECHA_NACIMIENTO");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_REGISTRO");
            entity.Property(e => e.FechaUltimaCompra)
                .HasColumnType("datetime")
                .HasColumnName("FECHA_ULTIMA_COMPRA");
            entity.Property(e => e.IdDoc).HasColumnName("ID_DOC");
            entity.Property(e => e.NivelFidelidad)
                .HasDefaultValue((byte)1)
                .HasColumnName("NIVEL_FIDELIDAD");
            entity.Property(e => e.Nombres)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("NOMBRES");
            entity.Property(e => e.PuntosFidelidad).HasColumnName("PUNTOS_FIDELIDAD");

            entity.HasOne(d => d.IdDocNavigation).WithMany(p => p.Clientes)
                .HasForeignKey(d => d.IdDoc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CLIENTES__PUNTOS__4E88ABD4");
        });

        modelBuilder.Entity<Contacto>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CONTACTOS");

            entity.Property(e => e.Direccion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("DIRECCION");
            entity.Property(e => e.IdUsuario)
                .HasMaxLength(5)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_USUARIO");
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("TELEFONO");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany()
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CONTACTOS__ID_US__693CA210");
        });

        modelBuilder.Entity<Cuenta>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CUENTAS");

            entity.Property(e => e.IdEmpleado)
                .HasMaxLength(5)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_EMPLEADO");
            entity.Property(e => e.TipoCuenta).HasColumnName("TIPO_CUENTA");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany()
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CUENTAS__ID_EMPL__59FA5E80");
        });

        modelBuilder.Entity<DescuentoCab>(entity =>
        {
            entity.HasKey(e => e.IdDescuento).HasName("PK__DESCUENT__52FD18798E55D8FA");

            entity.ToTable("DESCUENTO_CAB");

            entity.Property(e => e.IdDescuento)
                .HasMaxLength(8)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_DESCUENTO");
            entity.Property(e => e.Estado).HasColumnName("ESTADO");
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("FECHA_FIN");
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("FECHA_INICIO");
            entity.Property(e => e.IdEvento)
                .HasMaxLength(8)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_EVENTO");
            entity.Property(e => e.IdPromocion).HasColumnName("ID_PROMOCION");
            entity.Property(e => e.PrecioDescuento)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("PRECIO_DESCUENTO");
            entity.Property(e => e.TiSitu)
                .HasMaxLength(3)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("TI_SITU");
            entity.Property(e => e.TipoDescuento).HasColumnName("TIPO_DESCUENTO");
            entity.Property(e => e.TipoFil).HasColumnName("TIPO_FIL");

            entity.HasOne(d => d.IdEventoNavigation).WithMany(p => p.DescuentoCabs)
                .HasForeignKey(d => d.IdEvento)
                .HasConstraintName("FK__DESCUENTO__ID_EV__245D67DE");

            entity.HasOne(d => d.IdPromocionNavigation).WithMany(p => p.DescuentoCabs)
                .HasForeignKey(d => d.IdPromocion)
                .HasConstraintName("FK__DESCUENTO__ID_PR__236943A5");
        });

        modelBuilder.Entity<DescuentoConfig>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("DESCUENTO_CONFIG");

            entity.Property(e => e.IdCategoria)
                .HasMaxLength(8)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_CATEGORIA");
            entity.Property(e => e.IdDescuento)
                .HasMaxLength(8)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_DESCUENTO");
            entity.Property(e => e.Valor)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("VALOR");

            entity.HasOne(d => d.IdDescuentoNavigation).WithMany()
                .HasForeignKey(d => d.IdDescuento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DESCUENTO__ID_DE__2645B050");
        });

        modelBuilder.Entity<DescuentoDetalle>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("DESCUENTO_DETALLE");

            entity.Property(e => e.CantidadAplicable).HasColumnName("CANTIDAD_APLICABLE");
            entity.Property(e => e.Estado).HasColumnName("ESTADO");
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("FECHA_FIN");
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("FECHA_INICIO");
            entity.Property(e => e.IdArticulos)
                .HasMaxLength(8)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_ARTICULOS");
            entity.Property(e => e.IdDescuento)
                .HasMaxLength(8)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_DESCUENTO");
            entity.Property(e => e.PrecioFinal)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("PRECIO_FINAL");
            entity.Property(e => e.Tipo).HasColumnName("TIPO");
        });

        modelBuilder.Entity<DetallePromocion>(entity =>
        {
            entity.HasKey(e => e.IdDetallePromocion).HasName("PK__DETALLE___D666F23809297FC9");

            entity.ToTable("DETALLE_PROMOCION");

            entity.Property(e => e.IdDetallePromocion).HasColumnName("ID_DETALLE_PROMOCION");
            entity.Property(e => e.CantidadAplicable)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CANTIDAD_APLICABLE");
            entity.Property(e => e.IdProducto)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_PRODUCTO");
            entity.Property(e => e.IdPromocion).HasColumnName("ID_PROMOCION");
            entity.Property(e => e.TipoAplicacion).HasColumnName("TIPO_APLICACION");

            entity.HasOne(d => d.IdPromocionNavigation).WithMany(p => p.DetallePromocions)
                .HasForeignKey(d => d.IdPromocion)
                .HasConstraintName("FK__DETALLE_P__ID_PR__1AD3FDA4");
        });

        modelBuilder.Entity<DetalleVentum>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("DETALLE_VENTA");

            entity.Property(e => e.Cantidad).HasColumnName("CANTIDAD");
            entity.Property(e => e.IdProducto)
                .HasMaxLength(8)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_PRODUCTO");
            entity.Property(e => e.IdVenta)
                .HasMaxLength(8)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_VENTA");
            entity.Property(e => e.PrecioTotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("PRECIO_TOTAL");
            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("PRECIO_UNITARIO");

            entity.HasOne(d => d.IdProductoNavigation).WithMany()
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DETALLE_V__ID_PR__3587F3E0");

            entity.HasOne(d => d.IdVentaNavigation).WithMany()
                .HasForeignKey(d => d.IdVenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DETALLE_V__ID_VE__3493CFA7");
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.IdEmpleado).HasName("PK__EMPLEADO__922CA26902F024C1");

            entity.ToTable("EMPLEADO");

            entity.Property(e => e.IdEmpleado)
                .HasMaxLength(5)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_EMPLEADO");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(50)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("APELLIDOS");
            entity.Property(e => e.DocIdent)
                .HasMaxLength(15)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("DOC_IDENT");
            entity.Property(e => e.Estado)
                .HasDefaultValue((byte)1)
                .HasColumnName("ESTADO");
            entity.Property(e => e.FechaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_ACTUALIZACION");
            entity.Property(e => e.FechaIngreso)
                .HasColumnType("datetime")
                .HasColumnName("FECHA_INGRESO");
            entity.Property(e => e.FechaNacimiento).HasColumnName("FECHA_NACIMIENTO");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_REGISTRO");
            entity.Property(e => e.IdDoc).HasColumnName("ID_DOC");
            entity.Property(e => e.Nombres)
                .HasMaxLength(50)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("NOMBRES");
            entity.Property(e => e.Salario)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("SALARIO");

            entity.HasOne(d => d.IdDocNavigation).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.IdDoc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EMPLEADO__ID_DOC__5812160E");
        });

        modelBuilder.Entity<Evento>(entity =>
        {
            entity.HasKey(e => e.IdEventos).HasName("PK__EVENTOS__6E4199955D0E65C0");

            entity.ToTable("EVENTOS");

            entity.Property(e => e.IdEventos)
                .HasMaxLength(8)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_EVENTOS");
            entity.Property(e => e.Descripcion)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnType("text")
                .HasColumnName("DESCRIPCION");
            entity.Property(e => e.Estado)
                .HasDefaultValue(1)
                .HasColumnName("ESTADO");
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("FECHA_FIN");
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("FECHA_INICIO");
            entity.Property(e => e.NombreEvento)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("NOMBRE_EVENTO");
        });

        modelBuilder.Entity<Ingrediente>(entity =>
        {
            entity.HasKey(e => e.IdIngredientes).HasName("PK__INGREDIE__BD469414B1E92DE5");

            entity.ToTable("INGREDIENTES");

            entity.Property(e => e.IdIngredientes).HasColumnName("ID_INGREDIENTES");
            entity.Property(e => e.Estado)
                .HasDefaultValue((byte)1)
                .HasColumnName("ESTADO");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_CREACION");
            entity.Property(e => e.NombreIngrediente)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("NOMBRE_INGREDIENTE");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("PRECIO");
        });

        modelBuilder.Entity<InventarioLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("INVENTARIO_LOG");

            entity.Property(e => e.CantidadMovimiento).HasColumnName("CANTIDAD_MOVIMIENTO");
            entity.Property(e => e.DescripcionMovimiento)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("DESCRIPCION_MOVIMIENTO");
            entity.Property(e => e.FechaMovimiento)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_MOVIMIENTO");
            entity.Property(e => e.IdAlmacen).HasColumnName("ID_ALMACEN");
            entity.Property(e => e.IdLog)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID_LOG");
            entity.Property(e => e.TipoMovimiento).HasColumnName("TIPO_MOVIMIENTO");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.IdMenu).HasName("PK__MENUS__4728FC60119A74B8");

            entity.ToTable("MENUS");

            entity.Property(e => e.IdMenu).HasColumnName("ID_MENU");
            entity.Property(e => e.EstadoMenu)
                .HasDefaultValue(true)
                .HasColumnName("ESTADO_MENU");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_CREACION");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_MODIFICACION");
            entity.Property(e => e.NombreMenu)
                .HasMaxLength(50)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("NOMBRE_MENU");
            entity.Property(e => e.OrdenMenu).HasColumnName("ORDEN_MENU");
        });

        modelBuilder.Entity<Mesa>(entity =>
        {
            entity.HasKey(e => e.IdMesa).HasName("PK__MESA__472B15EADB792FC6");

            entity.ToTable("MESA");

            entity.Property(e => e.IdMesa)
                .HasMaxLength(5)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_MESA");
            entity.Property(e => e.Asientos).HasColumnName("ASIENTOS");
            entity.Property(e => e.Disponibilidad)
                .HasDefaultValue(1)
                .HasColumnName("DISPONIBILIDAD");
            entity.Property(e => e.Estado)
                .HasDefaultValue(1)
                .HasColumnName("ESTADO");
            entity.Property(e => e.NumeroMesa).HasColumnName("NUMERO_MESA");
        });

        modelBuilder.Entity<MetodoPago>(entity =>
        {
            entity.HasKey(e => e.IdMetodoPago).HasName("PK__METODO_P__A5FDB0D0622EFEFB");

            entity.ToTable("METODO_PAGO");

            entity.Property(e => e.IdMetodoPago)
                .ValueGeneratedNever()
                .HasColumnName("ID_METODO_PAGO");
            entity.Property(e => e.Metodo)
                .HasMaxLength(150)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("METODO");
        });

        modelBuilder.Entity<Opinione>(entity =>
        {
            entity.HasKey(e => e.IdOpinion).HasName("PK__OPINIONE__CEE511F3CA456A46");

            entity.ToTable("OPINIONES");

            entity.Property(e => e.IdOpinion)
                .HasMaxLength(8)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_OPINION");
            entity.Property(e => e.Comentarios)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnType("text")
                .HasColumnName("COMENTARIOS");
            entity.Property(e => e.Estado)
                .HasDefaultValue((byte)1)
                .HasColumnName("ESTADO");
            entity.Property(e => e.FechaOpinion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_OPINION");
            entity.Property(e => e.IdClientes)
                .HasMaxLength(5)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_CLIENTES");
            entity.Property(e => e.PromedioCalificaciones)
                .HasDefaultValue(0.0m)
                .HasColumnType("decimal(2, 1)")
                .HasColumnName("PROMEDIO_CALIFICACIONES");
            entity.Property(e => e.TotalCalificaciones)
                .HasDefaultValue(0)
                .HasColumnName("TOTAL_CALIFICACIONES");

            entity.HasOne(d => d.IdClientesNavigation).WithMany(p => p.Opiniones)
                .HasForeignKey(d => d.IdClientes)
                .HasConstraintName("FK__OPINIONES__ID_CL__3C34F16F");
        });

        modelBuilder.Entity<PermisosRol>(entity =>
        {
            entity.HasKey(e => new { e.IdRol, e.IdSubmenu }).HasName("PK__PERMISOS__8DA31CD352C13EE0");

            entity.ToTable("PERMISOS_ROL");

            entity.Property(e => e.IdRol)
                .HasMaxLength(5)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_ROL");
            entity.Property(e => e.IdSubmenu).HasColumnName("ID_SUBMENU");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_CREACION");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_MODIFICACION");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.PermisosRols)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PERMISOS___ID_RO__45F365D3");

            entity.HasOne(d => d.IdSubmenuNavigation).WithMany(p => p.PermisosRols)
                .HasForeignKey(d => d.IdSubmenu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PERMISOS___ID_SU__46E78A0C");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__PRODUCTO__88BD0357D61A207B");

            entity.ToTable("PRODUCTO");

            entity.Property(e => e.IdProducto)
                .HasMaxLength(8)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_PRODUCTO");
            entity.Property(e => e.Descripcion)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnType("text")
                .HasColumnName("DESCRIPCION");
            entity.Property(e => e.Estado)
                .HasDefaultValue(1)
                .HasColumnName("ESTADO");
            entity.Property(e => e.IdCategoria)
                .HasMaxLength(8)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_CATEGORIA");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("PRECIO");
            entity.Property(e => e.ProdFchcmrl)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("PROD_FCHCMRL");
            entity.Property(e => e.ProdNom)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("PROD_NOM");
            entity.Property(e => e.ProdNomweb)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("PROD_NOMWEB");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PRODUCTO__ESTADO__70DDC3D8");
        });

        modelBuilder.Entity<Promocion>(entity =>
        {
            entity.HasKey(e => e.IdPromocion).HasName("PK__PROMOCIO__04C2BDB296E9D431");

            entity.ToTable("PROMOCION");

            entity.Property(e => e.IdPromocion).HasColumnName("ID_PROMOCION");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("DESCRIPCION");
            entity.Property(e => e.Descuento)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("DESCUENTO");
            entity.Property(e => e.Estado)
                .HasDefaultValue((byte)1)
                .HasColumnName("ESTADO");
            entity.Property(e => e.FechaFin).HasColumnName("FECHA_FIN");
            entity.Property(e => e.FechaInicio).HasColumnName("FECHA_INICIO");
            entity.Property(e => e.NombrePromocion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("NOMBRE_PROMOCION");
            entity.Property(e => e.TipoPromocion).HasColumnName("TIPO_PROMOCION");
        });

        modelBuilder.Entity<PuntosFidelidad>(entity =>
        {
            entity.HasKey(e => e.IdPunto).HasName("PK__PUNTOS_F__E88439C94A9BC837");

            entity.ToTable("PUNTOS_FIDELIDAD");

            entity.Property(e => e.IdPunto).HasColumnName("ID_PUNTO");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("DESCRIPCION");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("FECHA");
            entity.Property(e => e.IdCliente)
                .HasMaxLength(5)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_CLIENTE");
            entity.Property(e => e.Puntos).HasColumnName("PUNTOS");
            entity.Property(e => e.TipoAccion).HasColumnName("TIPO_ACCION");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.PuntosFidelidads)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK__PUNTOS_FI__ID_CL__52593CB8");
        });

        modelBuilder.Entity<RegistroGasto>(entity =>
        {
            entity.HasKey(e => e.IdGasto).HasName("PK__REGISTRO__45B01B7E897075C6");

            entity.ToTable("REGISTRO_GASTOS");

            entity.Property(e => e.IdGasto).HasColumnName("ID_GASTO");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("DESCRIPCION");
            entity.Property(e => e.FechaGasto)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_GASTO");
            entity.Property(e => e.IdCategoriaGasto)
                .HasMaxLength(8)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_CATEGORIA_GASTO");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("MONTO");

            entity.HasOne(d => d.IdCategoriaGastoNavigation).WithMany(p => p.RegistroGastos)
                .HasForeignKey(d => d.IdCategoriaGasto)
                .HasConstraintName("FK__REGISTRO___ID_CA__0B91BA14");
        });

        modelBuilder.Entity<RegistroIngreso>(entity =>
        {
            entity.HasKey(e => e.IdIngreso).HasName("PK__REGISTRO__627D3FC45FB65353");

            entity.ToTable("REGISTRO_INGRESOS");

            entity.Property(e => e.IdIngreso).HasColumnName("ID_INGRESO");
            entity.Property(e => e.FechaIngreso)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_INGRESO");
            entity.Property(e => e.IdMetodoPago).HasColumnName("ID_METODO_PAGO");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("MONTO");

            entity.HasOne(d => d.IdMetodoPagoNavigation).WithMany(p => p.RegistroIngresos)
                .HasForeignKey(d => d.IdMetodoPago)
                .HasConstraintName("FK__REGISTRO___ID_ME__0F624AF8");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.IdReserva).HasName("PK__RESERVA__1ED54AE3FC9519C6");

            entity.ToTable("RESERVA");

            entity.Property(e => e.IdReserva)
                .HasMaxLength(8)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_RESERVA");
            entity.Property(e => e.FechaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_ACTUALIZACION");
            entity.Property(e => e.FechaReserva).HasColumnName("FECHA_RESERVA");
            entity.Property(e => e.HoraReserva).HasColumnName("HORA_RESERVA");
            entity.Property(e => e.IdMesa)
                .HasMaxLength(5)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_MESA");
            entity.Property(e => e.IdUsuario)
                .HasMaxLength(5)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_USUARIO");
            entity.Property(e => e.TiSitu)
                .HasDefaultValue(1)
                .HasColumnName("TI_SITU");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RESERVA__ID_USUA__14270015");

            entity.HasOne(d => d.IdUsuario1).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RESERVA__ID_USUA__151B244E");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__ROLES__203B0F68048F49E6");

            entity.ToTable("ROLES");

            entity.Property(e => e.IdRol)
                .HasMaxLength(5)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_ROL");
            entity.Property(e => e.Descripcion)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnType("text")
                .HasColumnName("DESCRIPCION");
            entity.Property(e => e.Rol)
                .HasMaxLength(50)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ROL");
            entity.Property(e => e.TipoUsuario).HasColumnName("TIPO_USUARIO");
        });

        modelBuilder.Entity<Submenu>(entity =>
        {
            entity.HasKey(e => e.IdSubmenu).HasName("PK__SUBMENUS__D9813BBB0C72DAB8");

            entity.ToTable("SUBMENUS");

            entity.Property(e => e.IdSubmenu).HasColumnName("ID_SUBMENU");
            entity.Property(e => e.EnlaceSubmenu)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ENLACE_SUBMENU");
            entity.Property(e => e.EstadoSubmenu)
                .HasDefaultValue(true)
                .HasColumnName("ESTADO_SUBMENU");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_CREACION");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_MODIFICACION");
            entity.Property(e => e.IdMenu).HasColumnName("ID_MENU");
            entity.Property(e => e.NombreSubmenu)
                .HasMaxLength(50)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("NOMBRE_SUBMENU");
            entity.Property(e => e.OrdenSubmenu).HasColumnName("ORDEN_SUBMENU");

            entity.HasOne(d => d.IdMenuNavigation).WithMany(p => p.Submenus)
                .HasForeignKey(d => d.IdMenu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SUBMENUS__ID_MEN__412EB0B6");
        });

        modelBuilder.Entity<TipoDocumento>(entity =>
        {
            entity.HasKey(e => e.IdDoc).HasName("PK__TIPO_DOC__2BBF728894809129");

            entity.ToTable("TIPO_DOCUMENTO");

            entity.Property(e => e.IdDoc)
                .ValueGeneratedNever()
                .HasColumnName("ID_DOC");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(30)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("DESCRIPCION");
        });

        modelBuilder.Entity<UsuariosSistema>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__USUARIOS__91136B907633E7EB");

            entity.ToTable("USUARIOS_SISTEMAS");

            entity.HasIndex(e => e.Email, "UQ__USUARIOS__161CF724E1E4447F").IsUnique();

            entity.Property(e => e.IdUsuario)
                .HasMaxLength(5)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_USUARIO");
            entity.Property(e => e.Contrasenia)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("CONTRASENIA");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("EMAIL");
            entity.Property(e => e.Estado)
                .HasDefaultValue((byte)1)
                .HasColumnName("ESTADO");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_CREACION");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_MODIFICACION");
            entity.Property(e => e.IdRol)
                .HasMaxLength(5)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_ROL");
            entity.Property(e => e.Usuario)
                .HasMaxLength(50)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("USUARIO");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.UsuariosSistemas)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__USUARIOS___ID_RO__6477ECF3");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.IdVenta).HasName("PK__VENTAS__F3B6C1B49A82869F");

            entity.ToTable("VENTAS");

            entity.Property(e => e.IdVenta)
                .HasMaxLength(8)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_VENTA");
            entity.Property(e => e.Estado)
                .HasDefaultValue(1)
                .HasColumnName("ESTADO");
            entity.Property(e => e.FechaVenta)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_VENTA");
            entity.Property(e => e.IdMetodoPago).HasColumnName("ID_METODO_PAGO");
            entity.Property(e => e.IdUsuario)
                .HasMaxLength(5)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_USUARIO");
            entity.Property(e => e.MontoTotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("MONTO_TOTAL");

            entity.HasOne(d => d.IdMetodoPagoNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.IdMetodoPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__VENTAS__ID_METOD__2DE6D218");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__VENTAS__ID_USUAR__2CF2ADDF");
        });

        modelBuilder.Entity<VentasNoRegistrada>(entity =>
        {
            entity.HasKey(e => e.IdVenta).HasName("PK__VENTAS_N__F3B6C1B44B69F932");

            entity.ToTable("VENTAS_NO_REGISTRADAS");

            entity.Property(e => e.IdVenta)
                .HasMaxLength(8)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AI")
                .HasColumnName("ID_VENTA");
            entity.Property(e => e.Estado)
                .HasDefaultValue(1)
                .HasColumnName("ESTADO");
            entity.Property(e => e.FechaVenta)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA_VENTA");
            entity.Property(e => e.IdMetodoPago).HasColumnName("ID_METODO_PAGO");
            entity.Property(e => e.MontoTotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("MONTO_TOTAL");

            entity.HasOne(d => d.IdMetodoPagoNavigation).WithMany(p => p.VentasNoRegistrada)
                .HasForeignKey(d => d.IdMetodoPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__VENTAS_NO__ID_ME__32AB8735");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
