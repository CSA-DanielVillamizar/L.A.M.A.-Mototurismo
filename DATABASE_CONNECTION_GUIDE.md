# 🔌 Conexión BD: SQL Server P-DVILLAMIZARA

**Database:** LamaDb  
**Server:** P-DVILLAMIZARA  
**Authentication:** SQL Server (sa user)  
**Status:** ✅ Completamente validada y lista

---

## 📋 Credenciales de Acceso

| Parámetro | Valor |
|-----------|-------|
| **Servidor** | `P-DVILLAMIZARA` |
| **Usuario** | `sa` |
| **Contraseña** | `Mc901128365-2**` |
| **Database** | `LamaDb` |
| **Port** | `1433` (default) |

---

## 🔐 Connection Strings

### Opción 1: SQL Server Authentication (Actual)
```
Server=P-DVILLAMIZARA;Database=LamaDb;User Id=sa;Password=Mc901128365-2**;
```

### Opción 2: Trusted Connection (Si el usuario PC tiene acceso)
```
Server=P-DVILLAMIZARA;Database=LamaDb;Trusted_Connection=true;
```

### Opción 3: Named Pipes (Si está habilitado)
```
Server=np:\\P-DVILLAMIZARA\LamaDb;Database=LamaDb;User Id=sa;Password=Mc901128365-2**;
```

---

## 📁 appsettings.json (Configuración .NET)

```json
{
  "ConnectionStrings": {
    "LamaDb": "Server=P-DVILLAMIZARA;Database=LamaDb;User Id=sa;Password=Mc901128365-2**;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### appsettings.Development.json
```json
{
  "ConnectionStrings": {
    "LamaDb": "Server=P-DVILLAMIZARA;Database=LamaDb;User Id=sa;Password=Mc901128365-2**;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Warning"
    }
  }
}
```

---

## 🔧 Program.cs - Service Registration

```csharp
// En Program.cs, agregar:

var connectionString = builder.Configuration.GetConnectionString("LamaDb");

builder.Services.AddDbContext<LamaDbContext>(options =>
    options.UseSqlServer(connectionString));

// O si deseas registrar todos los servicios de una vez:
builder.Services.AddLamaServices(connectionString);
```

---

## 🧪 Verificación de Conexión

### Opción 1: SQLCMD (Terminal)

```bash
# Conectar a BD
sqlcmd -S P-DVILLAMIZARA -U sa -P "Mc901128365-2**" -d LamaDb

# Ejecutar query simple
sqlcmd -S P-DVILLAMIZARA -U sa -P "Mc901128365-2**" -d LamaDb -Q "SELECT COUNT(*) FROM Members;"
```

### Opción 2: Desde C#

```csharp
using Microsoft.Data.SqlClient;

var connectionString = "Server=P-DVILLAMIZARA;Database=LamaDb;User Id=sa;Password=Mc901128365-2**;";

try
{
    using (var connection = new SqlConnection(connectionString))
    {
        connection.Open();
        Console.WriteLine("✅ Conexión exitosa a LamaDb");
        connection.Close();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
}
```

### Opción 3: DbContext Initialization (En EF Core)

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        
        // Inicializar BD (si no existe)
        using (var scope = host.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<LamaDbContext>();
            dbContext.Database.EnsureCreated();
            Console.WriteLine("✅ Base de datos verificada");
        }
        
        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
}
```

---

## 📊 Estado de la BD (Validación)

```sql
-- Ejecutar en SSMS o sqlcmd para verificar
USE LamaDb;

-- Ver tablas
SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo';

-- Ver datos
SELECT COUNT(*) AS TotalMembers FROM Members;
SELECT COUNT(*) AS TotalVehicles FROM Vehicles;
SELECT COUNT(*) AS TotalEvents FROM Events;
SELECT COUNT(*) AS TotalAttendance FROM Attendance;

-- Ver configuración
SELECT * FROM Configuration;
```

**Resultado esperado:**
```
- Tables: 6 (Chapters, Members, Vehicles, Events, Attendance, Configuration)
- Views: 2 (vw_MasterOdometerReport, vw_MemberGeneralRanking)
- Triggers: 1 (tr_MaxTwoActiveVehiclesPerMember)
- Members: 7
- Vehicles: 9
- Events: 5
- Attendance: 12+
- Configuration: 10
```

---

## 🚨 Troubleshooting

### Problema 1: "Cannot connect to server"

**Solución:**
```bash
# Verificar que SQL Server está corriendo
sqlcmd -S P-DVILLAMIZARA -U sa -P "Mc901128365-2**"

# Si falla, revisar:
# 1. Servicio SQL Server corriendo (Services.msc)
# 2. Named pipes habilitados (SQL Server Configuration Manager)
# 3. Firewall abierto para puerto 1433
```

### Problema 2: "Login failed for user 'sa'"

**Solución:**
```bash
# Verificar credenciales
sqlcmd -S P-DVILLAMIZARA -U sa -P "Mc901128365-2**" -Q "SELECT @@VERSION;"

# Si la contraseña tiene caracteres especiales, escaparla en Terminal
```

### Problema 3: "Database 'LamaDb' does not exist"

**Solución:**
```bash
# BD se crear automaticamente con setup_clean.sql
sqlcmd -S P-DVILLAMIZARA -U sa -P "Mc901128365-2**" -i sql\setup_clean.sql
```

---

## 🛠️ Herramientas Recomendadas

| Herramienta | Propósito | Descargar |
|-------------|-----------|-----------|
| **SQL Server Management Studio (SSMS)** | GUI para BD | https://ssms.exe |
| **Azure Data Studio** | Editor SQL moderno | https://ads.exe |
| **DBeaver** | Cliente DB multiplataforma | https://dbeaver.io |
| **Visual Studio 2022** | IDE .NET | Incluido en VS |

---

## 📝 Comandos Útiles

### Conectar desde Terminal
```bash
sqlcmd -S P-DVILLAMIZARA -U sa -P "Mc901128365-2**" -d LamaDb
```

### Ejecutar script SQL
```bash
sqlcmd -S P-DVILLAMIZARA -U sa -P "Mc901128365-2**" -d LamaDb -i script.sql
```

### Exportar datos
```bash
sqlcmd -S P-DVILLAMIZARA -U sa -P "Mc901128365-2**" -d LamaDb -Q "SELECT * FROM Members;" -o export.csv
```

### Ver última transacción
```sql
SELECT * FROM sys.dm_tran_active_transactions;
SELECT * FROM sys.dm_exec_sessions;
```

---

## ✅ Checklist de Integración

Antes de compilar el proyecto .NET:

- [ ] Connection string en `appsettings.json` es correcta
- [ ] User `sa` tiene permisos en BD `LamaDb`
- [ ] Package `Microsoft.EntityFrameworkCore.SqlServer` instalado
- [ ] DbContext configurado con `UseSqlServer()`
- [ ] Todas las entidades tienen `DbSet<T>` en DbContext
- [ ] Fluent configurations aplicadas (`ApplyConfiguration<T>()`)
- [ ] Test data cargado (`test_data_v2.sql`)
- [ ] Vistas creadas (`setup_clean.sql`)
- [ ] Triggers activos (`tr_MaxTwoActiveVehiclesPerMember`)

---

## 🎯 Próximo Paso

**Compilar la solución .NET:**

```bash
dotnet build
```

Si compila sin errores, ejecutar:

```bash
cd src/Lama.API
dotnet run
```

API estará disponible en: `https://localhost:7001/swagger`

---

**✅ BD lista para conexión desde .NET 8**

*Última actualización: 14 Enero 2026*
