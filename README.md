# AI Agent Purchase Application

Una aplicación full-stack para la compra y gestión de suscripciones de agentes de IA con integración de WhatsApp. Construida con frontend Vue 3 y backend .NET 8, con integración de pagos Bold.co y automatización de flujos N8N.

## 🚀 Características

### Frontend (Vue 3)
- Diseño moderno y responsivo con Tailwind CSS
- Soporte multiidioma (Inglés/Español)
- Autenticación JWT
- Integración de pagos Bold.co
- Dashboard de usuario con gestión de suscripciones
- Actualizaciones de estado de pago en tiempo real

### Backend (.NET 8)
- API RESTful con documentación Swagger
- Base de datos PostgreSQL con Entity Framework Core
- Autenticación y autorización JWT
- Procesamiento de pagos Bold.co
- Notificaciones por email
- Integración con flujos N8N
- Gestión de suscripciones

### Capacidades Clave
- **Procesamiento de Pagos**: Pagos seguros vía Bold.co con tokenización
- **Gestión de Suscripciones**: Renovaciones automáticas, cancelaciones y notificaciones
- **Automatización de Email**: Emails de bienvenida, recordatorios de vencimiento y notificaciones
- **Integración WhatsApp**: Listo para automatización de flujos N8N
- **Multiidioma**: Soporte para inglés y español
- **Diseño Responsivo**: Funciona en todos los dispositivos

## 🛠️ Stack Tecnológico

**Frontend:**
- Vue 3 (Composition API)
- Vue Router 4
- Vue I18n (internacionalización)
- Tailwind CSS
- Heroicons
- Axios
- Vue Toastification

**Backend:**
- .NET 8
- Entity Framework Core
- PostgreSQL
- Autenticación JWT
- Swagger/OpenAPI
- MailKit (email)
- BCrypt (hash de contraseñas)

**Infraestructura:**
- Docker & Docker Compose
- N8N (automatización de flujos)
- Bold.co (procesamiento de pagos)

## 📋 Prerrequisitos

- Node.js 18+ y npm
- .NET 8 SDK
- PostgreSQL 15+
- Visual Studio 2022 o Visual Studio Code
- Docker & Docker Compose (opcional)

## 🚀 Inicio Rápido

### 1. Clonar e Instalar

```bash
# Clonar el repositorio
git clone <repository-url>
cd ai-agent-purchase-app

# Instalar dependencias del frontend
cd frontend
npm install
cd ..
```

### 2. Configuración de Variables de Entorno

#### Backend (.env en la raíz del proyecto)
```bash
# Copiar plantilla de variables de entorno
cp .env.example .env

# Editar .env con tu configuración
nano .env
```

**Variables de Entorno Requeridas para Backend:**

```env
# Base de Datos
DATABASE_URL=postgresql://postgres:postgres@localhost:5432/aiagent

# JWT
JWT_SECRET=your-super-secret-jwt-key-that-is-at-least-32-characters-long

# Bold.co Payment
BOLD_API_KEY=your-bold-api-key
BOLD_BASE_URL=https://api.bold.co

# Email (ejemplo Gmail)
EMAIL_HOST=smtp.gmail.com
EMAIL_PORT=587
EMAIL_USERNAME=your-email@gmail.com
EMAIL_PASSWORD=your-app-password

# N8N (opcional)
N8N_BASE_URL=http://localhost:5678
N8N_API_KEY=your-n8n-api-key

# URLs de la aplicación
APP_BASE_URL=https://localhost:58402
FRONTEND_URL=http://localhost:3000
```

#### Frontend (frontend/.env)
```bash
# Crear archivo de variables de entorno del frontend
cd frontend
cp .env.example .env
```

**Variables de Entorno del Frontend:**

```env
# Configuración de API
VITE_API_URL=https://localhost:58402

# Configuración de la aplicación
VITE_APP_NAME=AI Agent
VITE_APP_DESCRIPTION=Intelligent WhatsApp AI Agent

# Bold.co Payment (Frontend)
VITE_BOLD_PUBLIC_KEY=your-bold-public-key

# Flags de características
VITE_ENABLE_REGISTRATION=true
VITE_ENABLE_GUEST_CHECKOUT=true

# Configuración de desarrollo
VITE_DEV_MODE=true
VITE_LOG_LEVEL=debug
```

### 3. Configuración de Base de Datos

```bash
# Iniciar PostgreSQL (si usas Docker)
docker run --name postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:15

# Ejecutar migraciones de base de datos (desde Visual Studio o terminal)
cd backend
dotnet ef database update
```

### 4. Iniciar Desarrollo

#### Opción 1: Visual Studio + Terminal (Recomendado)

**Backend (Visual Studio):**
1. Abrir `backend/AiAgentApi.sln` en Visual Studio 2022
2. Configurar el proyecto como proyecto de inicio
3. Presionar F5 o hacer clic en "Iniciar"
4. El backend se ejecutará en: `https://localhost:58402`
5. Swagger UI estará disponible en: `https://localhost:58402/swagger`

**Frontend (Terminal):**
```bash
# En una nueva terminal
cd frontend
npm run dev
```

El frontend se ejecutará en: `http://localhost:3000`

#### Opción 2: Solo Terminal

```bash
# Backend
cd backend
dotnet run

# Frontend (en otra terminal)
cd frontend
npm run dev
```

### 5. Acceder a la Aplicación

- **Frontend**: http://localhost:3000
- **Backend API**: https://localhost:58402
- **Swagger UI**: https://localhost:58402/swagger

## 🔧 Configuración de Desarrollo

### Configuración de Visual Studio

1. **Configurar HTTPS**: El proyecto está configurado para usar HTTPS en el puerto 58402
2. **Variables de Entorno**: Configurar en `appsettings.Development.json` o usar User Secrets
3. **Base de Datos**: Asegúrate de que la cadena de conexión apunte a tu instancia de PostgreSQL

### Configuración del Frontend

El frontend está configurado para hacer proxy de las llamadas API al backend:

```javascript
// vite.config.js
server: {
  port: 3000,
  proxy: {
    '/api': {
      target: 'https://localhost:58402',
      changeOrigin: true,
      secure: false // Para certificados auto-firmados en desarrollo
    }
  }
}
```

## 🐳 Despliegue con Docker

### Desarrollo con Docker

```bash
# Iniciar todos los servicios
docker-compose up -d

# Ver logs
docker-compose logs -f

# Detener servicios
docker-compose down
```

### Build de Producción

```bash
# Construir imágenes de producción
npm run docker:build

# Desplegar a producción
npm run docker:up
```

## 📚 Documentación de API

Una vez que el backend esté ejecutándose, visita:
- **Swagger UI**: https://localhost:58402/swagger
- **Documentos API**: https://localhost:58402/swagger/v1/swagger.json

### Endpoints Principales

```
POST /api/auth/login          # Login de usuario
POST /api/auth/register       # Registro de usuario
GET  /api/user/dashboard      # Datos del dashboard
POST /api/payments/create-bold-payment  # Crear pago
POST /api/subscriptions/renew # Renovar suscripción
POST /api/subscriptions/cancel # Cancelar suscripción
```

## 🔧 Configuración

### Configuración de Bold.co

1. Crear una cuenta en Bold.co en https://bold.co
2. Obtener tu API key desde el dashboard
3. Configurar URL de webhook: `https://localhost:58402/api/payments/webhook`
4. Actualizar `.env` con tus credenciales de Bold.co

### Configuración de Email

Para Gmail:
1. Habilitar autenticación de 2 factores
2. Generar una contraseña de aplicación
3. Usar la contraseña de aplicación en `EMAIL_PASSWORD`

### Integración N8N

1. Iniciar N8N: `docker run -it --rm --name n8n -p 5678:5678 n8nio/n8n`
2. Acceder: http://localhost:5678
3. Crear flujos para:
   - Notificaciones de registro de usuarios
   - Cambios de suscripción
   - Automatización de mensajes WhatsApp

## 🏗️ Estructura del Proyecto

```
ai-agent-purchase-app/
├── frontend/                 # Frontend Vue 3
│   ├── src/
│   │   ├── components/      # Componentes Vue
│   │   ├── locales/         # Traducciones i18n
│   │   ├── utils/           # Utilidades (API, etc.)
│   │   └── main.js         # Punto de entrada de la app
│   ├── .env                # Variables de entorno del frontend
│   ├── package.json
│   └── vite.config.js
├── backend/                 # Backend .NET 8
│   ├── Controllers/         # Controladores API
│   ├── Services/           # Lógica de negocio
│   ├── Models/             # Modelos de datos
│   ├── DTOs/               # Objetos de transferencia de datos
│   ├── Data/               # Contexto de base de datos
│   ├── Migrations/         # Migraciones EF
│   └── Program.cs          # Punto de entrada de la app
├── docker-compose.yml      # Servicios Docker
├── .env                   # Variables de entorno del backend
├── package.json           # Package.json raíz
└── README.md
```

## 🔐 Características de Seguridad

- **Autenticación JWT**: Autenticación segura basada en tokens
- **Hash de Contraseñas**: BCrypt para almacenamiento seguro de contraseñas
- **Protección CORS**: Configurado para el dominio del frontend
- **Validación de Entrada**: Validación completa de requests
- **Protección contra Inyección SQL**: Consultas parametrizadas de Entity Framework
- **Protección XSS**: Headers de Content Security Policy

## 📱 Flujo de Pago

1. **Registro de Usuario**: Usuario crea cuenta o compra directamente
2. **Creación de Pago**: Se genera enlace de pago Bold.co
3. **Procesamiento de Pago**: Usuario completa pago en Bold.co
4. **Procesamiento de Webhook**: Se recibe confirmación de pago
5. **Creación de Cuenta**: Se crea cuenta de usuario con credenciales por email
6. **Activación de Suscripción**: Se habilita acceso al agente IA

## 🔄 Gestión de Suscripciones

- **Auto-renovación**: Pagos recurrentes vía Bold.co
- **Cancelación**: Los usuarios pueden cancelar en cualquier momento
- **Notificaciones de Vencimiento**: Recordatorios por email antes del vencimiento
- **Período de Gracia**: El servicio continúa hasta el final del período de facturación

## 🐛 Solución de Problemas

### Problemas Comunes

**Problemas de Conexión a Base de Datos:**
```bash
# Verificar que PostgreSQL esté ejecutándose
docker ps | grep postgres

# Reiniciar base de datos
cd backend
dotnet ef database drop
dotnet ef database update
```

**Problemas de Build del Frontend:**
```bash
# Limpiar node_modules
rm -rf frontend/node_modules
cd frontend && npm install
```

**Problemas de Webhook de Pagos:**
- Asegurar que la URL del webhook sea públicamente accesible
- Verificar configuración del webhook en Bold.co
- Verificar que las API keys sean correctas

### Logs

```bash
# Logs del backend (Visual Studio)
Ver ventana de Output en Visual Studio

# Logs del frontend
cd frontend && npm run dev

# Logs de Docker
docker-compose logs -f backend
docker-compose logs -f frontend
```

## 🚀 Despliegue

### Checklist de Producción

- [ ] Actualizar variables de entorno para producción
- [ ] Configurar base de datos de producción
- [ ] Configurar certificados SSL
- [ ] Configurar dominio y DNS
- [ ] Configurar monitoreo y logging
- [ ] Configurar estrategia de backup
- [ ] Probar webhooks de pago
- [ ] Verificar entrega de emails

### Variables de Entorno para Producción

```env
# Usar base de datos de producción
DATABASE_URL=postgresql://user:pass@prod-db:5432/aiagent

# Usar keys de producción de Bold.co
BOLD_API_KEY=prod_your-bold-api-key

# Usar servicio de email de producción
EMAIL_HOST=smtp.sendgrid.net
EMAIL_USERNAME=apikey
EMAIL_PASSWORD=your-sendgrid-api-key

# Usar URLs de producción
APP_BASE_URL=https://your-domain.com
FRONTEND_URL=https://your-domain.com
```

## 📄 Licencia

Este proyecto está licenciado bajo la Licencia MIT - ver el archivo LICENSE para detalles.

## 🤝 Contribuir

1. Fork el repositorio
2. Crear una rama de feature
3. Hacer tus cambios
4. Agregar tests si es aplicable
5. Enviar un pull request

## 📞 Soporte

Para soporte y preguntas:
- Email: support@aiagent.com
- Documentación: [Enlace a docs]
- Issues: [GitHub Issues]

---

**Construido con ❤️ para automatización inteligente de WhatsApp**