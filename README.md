# prueba
# Tienda API

Este proyecto es una API para la gestión de clientes, productos y pedidos en una tienda. La API permite crear y gestionar productos, clientes y pedidos, así como autenticar a los usuarios mediante JWT.

## Descripción

La API está construida con .NET y utiliza Entity Framework Core para la gestión de la base de datos. Los usuarios pueden realizar operaciones CRUD (Crear, Leer, Actualizar, Eliminar) para productos, clientes y pedidos. Además, se incluye un sistema de autenticación mediante JWT.

## Características

- **Gestión de productos**: Añadir, actualizar y listar productos.
- **Gestión de clientes**: Crear clientes.
- **Gestión de pedidos**: Crear pedidos con productos asociados.
- **Autenticación JWT**: Acceso a la API mediante token JWT.

## Tecnologías utilizadas

- **.NET 6**
- **Entity Framework Core**
- **JWT (JSON Web Tokens)**
- **C#**

## Requisitos

- **.NET SDK 6.0** o superior.
- **SQL Server** (o cualquier otra base de datos compatible con EF Core).
- **Postman** o **cURL** para probar las API.

## Instalación

1. Clona el repositorio:

    ```bash
    git clone https://github.com/DRTECHLOGI/prueba.git
    cd prueba
    ```

2. Restaura las dependencias de NuGet:

    ```bash
    dotnet restore
    ```

3. Configura la cadena de conexión de la base de datos en el archivo `appsettings.json`:

    ```json
    "ConnectionStrings": {
        "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TiendaDb;Trusted_Connection=True;"
    }
    ```

4. Ejecuta las migraciones para crear la base de datos:

    ```bash
    dotnet ef database update
    ```

5. Inicia la aplicación:

    ```bash
    dotnet run
    ```

6. La API debería estar disponible en [http://localhost:5000](http://localhost:5000).

## Uso

### Endpoints

#### 1. **Clientes**

- **Crear un cliente**: `POST /api/clientes`

  Cuerpo de la solicitud (JSON):
  ```json
  {
    "Daniel Gonzalez', 'danielgonzalez@example.com"
    "fechaRegistro": "2024-12-18T10:00:00"
  }

2. Productos
Obtener productos: GET /api/productos

Parámetros opcionales:

precioMin: Precio mínimo.
precioMax: Precio máximo.
stockMin: Stock mínimo.
Actualizar un producto: PUT /api/productos/{id}

Cuerpo de la solicitud (JSON):

json
Copiar código
{
  "precio": 20.99,
  "stock": 50
}
3. Pedidos
Crear un pedido: POST /api/pedidos

Cuerpo de la solicitud (JSON):

json
Copiar código
{
  "clienteId": 1,
  "pedidoProductos": [
    {
      "productoId": 1,
      "cantidad": 2
    }
  ]
}
Obtener un pedido: GET /api/pedidos/{id}

4. Autenticación
Iniciar sesión: POST /api/auth/login

Cuerpo de la solicitud (JSON):

json
Copiar código
{
  "email": "admin@example.com",
  "password": "password"
}
Respuesta (JSON):

json
Copiar código
{
  "token": "jwt_token_aqui"
}

## Estructura del Proyecto
Tienda.Application: Contiene la lógica de negocio, servicios e interfaces.
Tienda.Infrastructure: Proporciona la implementación de los servicios y acceso a datos (DbContext).
Tienda.WebApi: Contiene los controladores de la API.
Tienda.Core: Contiene las entidades del dominio y modelos de datos.
Licencia
Este proyecto está bajo la licencia MIT. Puedes ver más detalles en el archivo LICENSE.
##

### 
**Descripción**: Explica el propósito general del proyecto.
 **Tecnologías utilizadas**: Describe las tecnologías y herramientas usadas en el proyecto.
 **Requisitos**: Incluye las herramientas necesarias para ejecutar el proyecto.
 **Instalación**: Detalla los pasos para clonar, configurar e iniciar la aplicación.
 **Uso**: Explica cómo interactuar con la API mediante sus endpoints.
 **Estructura del Proyecto**: Proporciona una visión general de cómo está organizado el código.
 **Licencia**: Información sobre la licencia del proyecto.

