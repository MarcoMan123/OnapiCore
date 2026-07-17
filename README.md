# OnapiCore

Core del proyecto final para ONAPI. Contiene la base de datos y la API que expone las operaciones sobre Solicitudes, Solicitantes, Tipos de Registro y Estados.

## Cómo levantar el proyecto

1. Clona el repo.
2. Abre `OnapiCore.sln` en Visual Studio.
3. Crea tu propia base de datos local corriendo el script de abajo en el **SQL Server Object Explorer** (conéctate a `(localdb)\MSSQLLocalDB`, crea una base llamada `OnapiCore`, y ahí corre el script).
4. En `appsettings.json` (que no viene en el repo, cada quien tiene el suyo local), agrega:

```json
{
  "ConnectionStrings": {
    "OnapiCoreConnection": "Server=(localdb)\\MSSQLLocalDB;Database=OnapiCore;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

5. Dale F5. Si todo está bien, en consola debería aparecer "Now listening on: https://localhost:7199" (el puerto puede variar).

## Script de base de datos

```sql
CREATE TABLE Solicitantes (
    SolicitanteId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Cedula NVARCHAR(20) NOT NULL,
    Email NVARCHAR(150),
    Telefono NVARCHAR(20),
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE TiposDeRegistro (
    TipoId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL
);

CREATE TABLE Estados (
    EstadoId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL
);

CREATE TABLE Solicitudes (
    SolicitudId INT IDENTITY(1,1) PRIMARY KEY,
    SolicitanteId INT NOT NULL FOREIGN KEY REFERENCES Solicitantes(SolicitanteId),
    TipoId INT NOT NULL FOREIGN KEY REFERENCES TiposDeRegistro(TipoId),
    EstadoId INT NOT NULL FOREIGN KEY REFERENCES Estados(EstadoId),
    NombreProducto NVARCHAR(200) NOT NULL,
    Descripcion NVARCHAR(MAX),
    FechaSolicitud DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Roles (
    RolId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL -- Consulta, Mantenimiento, Administracion
);

CREATE TABLE Usuarios (
    UsuarioId INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario NVARCHAR(50) NOT NULL UNIQUE,
    ClaveHash NVARCHAR(200) NOT NULL,
    RolId INT NOT NULL FOREIGN KEY REFERENCES Roles(RolId),
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Auditoria (
    AuditoriaId INT IDENTITY(1,1) PRIMARY KEY,
    Metodo NVARCHAR(10) NOT NULL,
    Ruta NVARCHAR(300) NOT NULL,
    CodigoRespuesta INT NOT NULL,
    Fecha DATETIME NOT NULL DEFAULT GETDATE()
);

-- Solicitante anónimo, para trámites sin identificar al solicitante formal
INSERT INTO Solicitantes (Nombre, Cedula, Email, Telefono)
VALUES ('Anónimo', '000000000', NULL, NULL);
```

Después de correr el script, confirma con `SELECT * FROM Solicitantes;` qué `SolicitanteId` le quedó al registro "Anónimo" (normalmente el 2, si es la primera solicitante que insertas después del anónimo).

## Endpoints disponibles

Base local: `http://localhost:5244/api` (o el puerto que te asigne tu Visual Studio).

| Recurso | Método | Ruta | Qué hace |
|---|---|---|---|
| Solicitudes | GET | `/Solicitudes` | Lista todas |
| Solicitudes | GET | `/Solicitudes/{id}` | Trae una por id |
| Solicitudes | POST | `/Solicitudes` | Crea una nueva |
| Solicitudes | PUT | `/Solicitudes/{id}` | Reemplaza una existente completa |
| Solicitudes | DELETE | `/Solicitudes/{id}` | Borra una |
| Solicitantes | GET | `/Solicitantes` | Lista todos |
| Solicitantes | POST | `/Solicitantes` | Crea uno nuevo |
| TiposDeRegistro | GET | `/TiposDeRegistro` | Lista todos |
| TiposDeRegistro | POST | `/TiposDeRegistro` | Crea uno nuevo |
| Estados | GET | `/Estados` | Lista todos |
| Estados | POST | `/Estados` | Crea uno nuevo |
| Usuarios | GET | `/Usuarios` | Lista todos (sin exponer la clave) |
| Usuarios | POST | `/Usuarios` | Crea uno nuevo |
| Usuarios | PUT | `/Usuarios/{id}` | Edita nombre, rol y/o clave |
| Usuarios | DELETE | `/Usuarios/{id}` | Borra uno |
| Usuarios | POST | `/Usuarios/login` | Valida usuario y clave |
| Roles | GET | `/Roles` | Lista todos |
| Roles | GET | `/Roles/{id}` | Trae uno por id |
| Roles | POST | `/Roles` | Crea uno nuevo |
| Roles | PUT | `/Roles/{id}` | Reemplaza uno existente |
| Roles | DELETE | `/Roles/{id}` | Borra uno |

Ejemplo de body para crear un Usuario (POST) o editarlo (PUT):

```json
{
  "nombreUsuario": "jperez",
  "clave": "unaClaveSegura123",
  "rolId": 1
}
```

Ejemplo de body para login (POST a `/Usuarios/login`):

```json
{
  "nombreUsuario": "jperez",
  "clave": "unaClaveSegura123"
}
```

Nota: la clave nunca se guarda ni se devuelve en texto plano — se guarda como hash (con BCrypt), y el GET de usuarios jamás incluye ese hash en la respuesta.

El **solicitante anónimo** ya viene pre-cargado en la base con el script de arriba, para trámites donde no se identifica formalmente a quien solicita. Usa su `SolicitanteId` cuando necesites representar a alguien no identificado.

Ejemplo de body para crear una Solicitud (POST):

```json
{
  "solicitanteId": 1,
  "tipoId": 1,
  "estadoId": 1,
  "nombreProducto": "Marca de prueba",
  "descripcion": "Test",
  "fechaSolicitud": "2026-07-05"
}
```

Nota: `solicitanteId`, `tipoId` y `estadoId` tienen que existir ya en sus tablas correspondientes, o la base rechaza el insert.

CORS está abierto a cualquier origen, así que no importa en qué puerto corran su parte, van a poder llamar a esta API sin bloqueos del navegador.

Los errores no manejados devuelven un JSON con `mensaje` y `detalle` en vez del stack trace completo.

Toda petición que llega (sin importar el endpoint) queda registrada en la tabla `Auditoria`, con método, ruta, código de respuesta y fecha — para fines de auditoría.

---

## Para Integración

Ustedes son los que van a llamar directo a estos endpoints. El JSON de request/response es el que ven en la tabla de arriba — no hay autenticación todavía, así que no hace falta mandar tokens ni headers extra aparte de `Content-Type: application/json`.

Si necesitan un endpoint que no está aquí (por ejemplo, GetById para Estados o Solicitantes), avísenme y lo agrego.

## Para Caja

Ustedes van a ser quienes registren las Solicitudes nuevas. Lo que les interesa es el POST a `/Solicitudes`, y probablemente también necesiten leer `/TiposDeRegistro` y `/Estados` para llenar dropdowns o selects en su interfaz con las opciones válidas.

## Para Web

Probablemente consuman `/Solicitudes` (GET) para mostrar el estado de los trámites al público, y quizás `/Solicitantes` si necesitan mostrar información del solicitante junto a su solicitud.

---

Cualquier duda o si algo no corre, mándenme el error exacto (captura de la consola de Visual Studio) antes de asumir que está roto — casi siempre es algo de configuración local (connection string, base no creada, etc.).
