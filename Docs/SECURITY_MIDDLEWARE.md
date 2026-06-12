# Seguridad API — Rate Limiting & Detección de Inyección

---

## 1. Visión General

El proyecto implementa dos middlewares de seguridad en la capa de presentación (`Reserva.Api`) que protegen la API antes de que las peticiones lleguen a los controladores:

| Middleware | Propósito | Respuesta |
|-----------|-----------|-----------|
| `RateLimitMiddleware` | Limitar cantidad de peticiones por IP | HTTP 429 Too Many Requests |
| `InjectionDetectionMiddleware` | Detectar patrones de SQL Injection y XSS | HTTP 400 Bad Request |

---

## 2. Pipeline de Seguridad

```
┌─────────────────────────────────────────────────────────────────┐
│                     PIPELINE DE SEGURIDAD                       │
│                                                                 │
│  Request HTTP del Cliente                                       │
│       │                                                         │
│       ▼                                                         │
│  ┌──────────────────────────────────┐                           │
│  │  1. UseRouting()                 │  Enruta la petición       │
│  ├──────────────────────────────────┤                           │
│  │  2. UseCors("CorsPolicy")        │  Valida origen            │
│  ├──────────────────────────────────┤                           │
│  │  3. RateLimitMiddleware          │  ← NUEVO                 │
│  │     ¿Supera el límite?          │                           │
│  │     SÍ → 429 + Retry-After      │                           │
│  │     NO → Continúa               │                           │
│  ├──────────────────────────────────┤                           │
│  │  4. InjectionDetectionMiddleware │  ← NUEVO                 │
│  │     ¿Contiene amenazas?         │                           │
│  │     SÍ → 400 Bad Request        │                           │
│  │     NO → Continúa               │                           │
│  ├──────────────────────────────────┤                           │
│  │  5. UseAuthentication()          │  Valida JWT              │
│  ├──────────────────────────────────┤                           │
│  │  6. UseAuthorization()           │  Valida permisos         │
│  ├──────────────────────────────────┤                           │
│  │  7. Controllers                  │  Lógica de negocio       │
│  └──────────────────────────────────┘                           │
│       │                                                         │
│       ▼                                                         │
│  Response HTTP al Cliente                                       │
└─────────────────────────────────────────────────────────────────┘
```

---

## 3. Rate Limiting — Límite de Peticiones

### 3.1. ¿Qué hace?

Controla cuántas peticiones HTTP puede hacer una misma dirección IP dentro de una ventana de tiempo. Si se excede el límite, la IP queda bloqueada temporalmente.

### 3.2. Algoritmo: Sliding Window (Ventana Deslizante)

```
Ventana = 10 segundos | Máximo = 30 requests | Bloqueo = 60 segundos

Línea de tiempo:
─────────────────────────────────────────────────────────────►

  IP: 192.168.1.1
  ┌────────────────────────────┐
  │ t=0s   request #1          │
  │ t=1s   request #2          │
  │ t=2s   request #3          │
  │ ...                        │
  │ t=9s   request #30         │  ← Límite alcanzado
  └────────────────────────────┘
  Resultado: IP bloqueada por 60 segundos
  Headers:   X-RateLimit-Limit: 30
             X-RateLimit-Remaining: 0
             X-RateLimit-Reset: 60s
  Status:    429 Too Many Requests
             Retry-After: 60

  ┌────────────────────────────┐
  │ t=60s  bloqueo expira      │
  │        IP puede volver     │
  └────────────────────────────┘
```

### 3.3. Flujo detallado de una petición

```
Request: POST /api/Cancha/create
IP: 192.168.1.50
```

**Paso 1 — Verificar si está habilitado:**
```
¿ApiProtection:RateLimitEnabled = true?
  SÍ → Continúa
  NO → Skip, pasa al siguiente middleware
```

**Paso 2 — Verificar si es recurso estático:**
```
¿La extensión es .js, .css, .png, .jpg, etc.?
  SÍ → Skip (los estáticos no se limitan)
  NO → Continúa
```

**Paso 3 — Obtener IP del cliente:**
```
¿Tiene header X-Forwarded-For?
  SÍ → Usar la primera IP (proxy/load balancer)
  NO → Usar RemoteIpAddress directamente
```

**Paso 4 — Verificar si la IP está bloqueada:**
```
¿Existe registro de bloqueo para esta IP?
  SÍ → ¿Expiró?
        SÍ → Remover bloqueo, continuar
        NO → Retornar 429 + Retry-After
  NO → Continúa
```

**Paso 5 — Contar requests en la ventana:**
```
¿Requests en los últimos 10 segundos >= 30?
  SÍ → Bloquear IP por 60 segundos
       Retornar 429
  NO → Agregar timestamp actual
       Continúa
```

**Paso 6 — Agregar headers informativos:**
```
Response Headers:
  X-RateLimit-Limit: 30          ← Máximo permitido
  X-RateLimit-Remaining: 25      ← Cuántos requests faltan
  X-RateLimit-Window: 10s        ← Duración de la ventana
```

### 3.4. Ejemplo con curl

```bash
# Petición normal
curl -i -X POST https://api.reservafast.com/api/Cancha \
  -H "Authorization: Bearer xxx" \
  -H "Content-Type: application/json"

# Response 200 OK
# Headers incluyen:
#   X-RateLimit-Limit: 30
#   X-RateLimit-Remaining: 29

# Después de 30 peticiones en 10 segundos...
curl -i -X POST https://api.reservafast.com/api/Cancha \
  -H "Authorization: Bearer xxx"

# Response 429 Too Many Requests
# Headers incluyen:
#   Retry-After: 60
#   X-RateLimit-Reset: 60s
# Body:
#   "Rate limit exceeded. Try again in 60 seconds."
```

### 3.5. Configuración

```json
"ApiProtection": {
  "RateLimitEnabled": true,
  "RateLimitWindowSeconds": 10,
  "RateLimitMaxRequests": 30,
  "RateLimitBlockSeconds": 60
}
```

| Parámetro | Default | Descripción |
|-----------|---------|-------------|
| `RateLimitEnabled` | `true` | Habilita/deshabilita el rate limit |
| `RateLimitWindowSeconds` | `10` | Ventana de tiempo en segundos |
| `RateLimitMaxRequests` | `30` | Máximo de requests por ventana |
| `RateLimitBlockSeconds` | `60` | Segundos de bloqueo al exceder |

### 3.6. Recursos estáticos excluidos

Los siguientes archivos **no** se limitan:

```
.js, .css, .png, .jpg, .jpeg, .gif, .ico, .svg,
.woff, .woff2, .ttf, .eot, .map, .json,
.html, .htm
```

---

## 4. Detección de Inyección — SQL Injection & XSS

### 4.1. ¿Qué hace?

Analiza el contenido de las peticiones (body, query params, headers) en busca de patrones maliciosos de SQL Injection y Cross-Site Scripting (XSS). Si detecta una amenaza, rechaza la petición con HTTP 400.

### 4.2. Tipos de amenazas detectadas

#### SQL Injection (13 patrones)

| # | Patrón | Ejemplo malicioso |
|---|--------|-------------------|
| 1 | `EXEC/EXECUTE/XP_CMDSHELL` | `EXEC sp_helptext` |
| 2 | `UNION SELECT` | `UNION SELECT * FROM usuarios` |
| 3 | `SELECT...FROM` | `SELECT password FROM usuarios` |
| 4 | `DROP/ALTER/TRUNCATE TABLE` | `DROP TABLE usuarios` |
| 5 | `INSERT/UPDATE/DELETE INTO` | `DELETE FROM usuarios WHERE 1=1` |
| 6 | `WAITFOR/DELAY` | `WAITFOR DELAY '0:0:5'` |
| 7 | `SHUTDOWN/KILL/RESTORE` | `SHUTDOWN` |
| 8 | `OR/AND 1=1` | `' OR 1=1 --` |
| 9 | `OR/AND 'x'='x'` | `' OR 'a'='a'` |
| 10 | `CHAR/NVARCHAR/CONVERT/CAST` | `CHAR(65)` |
| 11 | `0x` hex literals | `0x41424344` |
| 12 | Comentarios SQL (`/*`, `--`, `;`) | `/* bypass */` |
| 13 | System schemas | `INFORMATION_SCHEMA` |

#### XSS (14 patrones)

| # | Patrón | Ejemplo malicioso |
|---|--------|-------------------|
| 1 | `<script>` | `<script>alert('XSS')</script>` |
| 2 | `<iframe>` | `<iframe src="evil.com">` |
| 3 | `<embed>` | `<embed src="evil.swf">` |
| 4 | `<object>` | `<object data="evil.swf">` |
| 5 | `<svg>` | `<svg onload=alert(1)>` |
| 6 | `<style>` | `<style>body{background:red}</style>` |
| 7 | `<link>` | `<link rel="stylesheet" href="evil.css">` |
| 8 | `<form>` | `<form action="evil.com">` |
| 9 | `javascript:` | `javascript:alert(1)` |
| 10 | Event handlers | `onload=`, `onclick=`, `onerror=` |
| 11 | `expression()` | `expression(alert(1))` |
| 12 | `data:text/html` | `data:text/html,<script>alert(1)</script>` |
| 13 | `<base>` | `<base href="evil.com">` |
| 14 | `<meta http-equiv>` | `<meta http-equiv="refresh" content="0;url=evil.com">` |

### 4.3. Flujo detallado de una petición

```
Request: POST /api/Reserva/search
Body: { "nombre": "Cancha Norte" }
```

**Paso 1 — Verificar si está habilitado:**
```
¿ApiProtection:InjectionDetectionEnabled = true?
  SÍ → Continúa
  NO → Skip
```

**Paso 2 — Verificar método HTTP:**
```
¿Método es GET, HEAD u OPTIONS?
  SÍ → Skip (estos métodos no llevan body, menos riesgo)
  NO → Continúa (POST, PUT, DELETE, PATCH)
```

**Paso 3 — Analizar Query Parameters:**
```
URL: /api/Cancha/search?nombre=test&orden=nombre

¿Alguna key o value contiene patrones maliciosos?
  Ejemplo: ?nombre= DROP TABLE usuarios--
  → InjectionDetector.HasThreats("DROP TABLE usuarios--")
  → Retorna "Access denied"
  → Retornar 400 Bad Request
```

**Paso 4 — Analizar Headers:**
```
Headers:
  Content-Type: application/json
  Authorization: Bearer xxx
  X-Custom: <script>alert(1)</script>

¿Alguno contiene patrones XSS?
  → InjectionDetector.HasThreats("<script>alert(1)</script>")
  → Retorna "Access denied"
  → Retornar 400 Bad Request

Nota: El header Cookie se excluye del análisis
```

**Paso 5 — Analizar Body:**
```
Body: { "nombre": "Cancha Norte" }

¿El body es mayor a 100,000 caracteres?
  SÍ → Skip (protección contra payloads gigantes)
  NO → Continúa

¿Contiene patrones de SQL injection o XSS?
  Ejemplo: { "nombre": "' OR 1=1 --" }
  → InjectionDetector.HasThreats("' OR 1=1 --")
  → Retorna "Access denied"
  → Retornar 400 Bad Request
```

**Paso 6 — Respuesta exitosa:**
```
Si no se detectó ninguna amenaza:
  → Continúa al siguiente middleware (Authentication)
```

### 4.4. Ejemplo con curl

```bash
# Petición legítima — pasa
curl -X POST https://api.reservafast.com/api/Reserva/search \
  -H "Content-Type: application/json" \
  -d '{"nombre": "Cancha Norte"}'

# Response 200 OK

# SQL Injection — bloqueado
curl -X POST https://api.reservafast.com/api/Reserva/search \
  -H "Content-Type: application/json" \
  -d '{"nombre": "' OR 1=1 --"}'

# Response 400 Bad Request
# {
#   "data": null,
#   "messages": [{
#     "message": "Solicitud rechazada: se detectó un patrón no permitido.",
#     "messageType": "Error"
#   }],
#   "isValid": false
# }

# XSS — bloqueado
curl -X POST https://api.reservafast.com/api/Cancha/create \
  -H "Content-Type: application/json" \
  -d '{"nombre": "<script>alert(1)</script>"}'

# Response 400 Bad Request
```

### 4.5. Configuración

```json
"ApiProtection": {
  "InjectionDetectionEnabled": true,
  "MaxPayloadLength": 100000
}
```

| Parámetro | Default | Descripción |
|-----------|---------|-------------|
| `InjectionDetectionEnabled` | `true` | Habilita/deshabilita la detección |
| `MaxPayloadLength` | `100000` | Tamaño máximo del body a analizar (bytes) |

---

## 5. Respuestas HTTP

### 5.1. Rate Limit Excedido — 429

```http
HTTP/1.1 429 Too Many Requests
Content-Type: application/json
Retry-After: 60
X-RateLimit-Limit: 30
X-RateLimit-Remaining: 0
X-RateLimit-Window: 10s
X-RateLimit-Reset: 60s

{
  "title": "Too Many Requests",
  "status": 429
}
```

### 5.2. Inyección Detectada — 400

```http
HTTP/1.1 400 Bad Request
Content-Type: application/json

{
  "data": null,
  "messages": [
    {
      "message": "Solicitud rechazada: se detectó un patrón no permitido.",
      "messageType": "Error"
    }
  ],
  "isValid": false
}
```

### 5.3. Respuesta Exitosa — 200

```http
HTTP/1.1 200 OK
X-RateLimit-Limit: 30
X-RateLimit-Remaining: 27
X-RateLimit-Window: 10s

{
  "data": { ... },
  "messages": [{ "message": "OK", "messageType": "Ok" }],
  "isValid": true
}
```

---

## 6. Headers de Rate Limiting

Todos los responses (exitosos o no) incluyen estos headers:

| Header | Descripción | Ejemplo |
|--------|-------------|---------|
| `X-RateLimit-Limit` | Máximo de requests permitidos | `30` |
| `X-RateLimit-Remaining` | Requests restantes en la ventana | `27` |
| `X-RateLimit-Window` | Duración de la ventana | `10s` |
| `X-RateLimit-Reset` | Tiempo hasta reset (solo en 429) | `60s` |
| `Retry-After` | Segundos para reintentar (solo en 429) | `60` |

---

## 7. Manejo de Concurrencia

### Rate Limiting

El `RateLimitMiddleware` usa `ConcurrentDictionary<string, SlidingWindow>` para manejar múltiples IPs simultáneamente:

```
Thread 1 (IP: 192.168.1.1)  ──→  SlidingWindow para 192.168.1.1
Thread 2 (IP: 192.168.1.2)  ──→  SlidingWindow para 192.168.1.2
Thread 3 (IP: 192.168.1.1)  ──→  Mismo SlidingWindow (con lock)
```

- Cada IP tiene su propia ventana independiente
- El `lock` por IP evita condiciones de carrera
- Un `Timer` limpia ventanas vacías cada 5 minutos (evita memory leaks)

### Injection Detection

El `InjectionDetectionMiddleware` es **stateless** — no acumula estado entre peticiones. Cada request se analiza de forma independiente.

---

## 8. Arquitectura de Clases

```
Reserva.Api/
├── Security/
│   └── ApiProtectionOptions.cs        ← Configuración tipada
├── Middleware/
│   ├── RateLimitMiddleware.cs         ← Límite de peticiones
│   └── InjectionDetectionMiddleware.cs ← Detección de amenazas
├── Program.cs                          ← Registro en pipeline
└── appsettings.json                   ← Configuración

Reserva.Common/
└── Helpers/
    ├── InjectionDetector.cs            ← Motor de detección (regex)
    └── Sanitizer.cs                    ← Limpieza de inputs
```

### Dependencias

```
RateLimitMiddleware
  └── ApiProtectionOptions (configuración)

InjectionDetectionMiddleware
  ├── ApiProtectionOptions (configuración)
  └── InjectionDetector (detección de patrones)
        └── Regex compilados (SQL + XSS patterns)
```

---

## 9. Deshabilitar en Desarrollo

Para deshabilitar los middlewares en el entorno de desarrollo, agregar en `appsettings.Development.json`:

```json
{
  "ApiProtection": {
    "RateLimitEnabled": false,
    "InjectionDetectionEnabled": false
  }
}
```

O configurar límites más permisivos:

```json
{
  "ApiProtection": {
    "RateLimitEnabled": true,
    "RateLimitWindowSeconds": 60,
    "RateLimitMaxRequests": 500,
    "RateLimitBlockSeconds": 10,
    "InjectionDetectionEnabled": true
  }
}
```

---

## 10. Consideraciones

### Seguridad

- Los middlewares se ejecutan **antes** de la autenticación, por lo que protegen incluso endpoints públicos
- El rate limit funciona por IP, no por usuario (un usuario autenticado tiene el mismo límite que uno anónimo desde la misma IP)
- La detección de inyección cubre body, query params y headers (excepto Cookie)

### Rendimiento

- Los middlewares son ligeros: la detección usa regex pre-compilados
- El rate limit usa `ConcurrentDictionary` con locks por IP (no global)
- La limpieza automática de ventanas vacías previene memory leaks
- Los archivos estáticos se saltan el rate limit (no impacto en assets)

### En Producción (con proxy/load balancer)

> **ALERTA — Si tu proyecto usa Nginx, Azure App Service o AWS ALB, lee esto:**
>
> Si la API está detrás de un proxy, el middleware verá la IP del proxy, **no la del cliente**. Sin la configuración correcta, **todas las peticiones tendrán la misma IP** y el rate limit no funcionará (un solo contador para todos los usuarios).
>
> **¿Cómo se obtiene la IP?**
> - La IP del cliente viene del paquete TCP, **no la envía el frontend** (sería fácil de falsificar)
> - El middleware primero busca en `X-Forwarded-For` (header que pone el proxy)
> - Si no existe, usa `RemoteIpAddress` (IP directa de la conexión TCP)
>
> **¿Qué configurar según tu infraestructura?**
>
> | Infraestructura | Acción requerida |
> |-----------------|------------------|
> | **Sin proxy** (directo a .NET) | No hacer nada — funciona automáticamente |
> | **Nginx** | Agregar `proxy_set_header` (ver abajo) |
> | **Azure App Service** | Ya envía `X-Forwarded-For` automáticamente |
> | **AWS ALB** | Ya envía `X-Forwarded-For` automáticamente |
> | **Docker/Kubernetes** | Verificar que el Ingress envíe el header |
>
> **Configuración para Nginx:**
>
> ```nginx
> server {
>     listen 80;
>     server_name api.reservafast.com;
>
>     location / {
>         proxy_pass http://localhost:5000;
>         proxy_http_version 1.1;
>
>         # IMPORTANTE: Envía la IP real del cliente
>         proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
>         proxy_set_header X-Real-IP $remote_addr;
>         proxy_set_header Host $host;
>         proxy_set_header X-Forwarded-Proto $scheme;
>     }
> }
> ```
>
> **¿Cómo verificar que funciona?**
>
> ```bash
> # Desde tu máquina (IP pública: 190.25.x.x)
> curl -v https://api.reservafast.com/api/Cancha/selectcombo
>
> # En los logs de la API, la IP debe ser la tuya, no la de Nginx
> # Rate limit: X-RateLimit-Remaining va bajando con cada request
> ```

---

*Documento generado para ReservaCanchas API — Seguridad*
