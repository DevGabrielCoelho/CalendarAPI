# CallendarAPI

A calendar management API that allows users to create, update, and manage events and reminders with secure authentication.

## 🛠️ Features

- User authentication and authorization with JWT.
- Creation and management of events and reminders.
- Email notifications for scheduled events.
- API logging using Serilog.
- Docker support for easy deployment.

## 📋 Prerequisites

Ensure you have the following tools installed on your machine before starting:

- **[Docker](https://www.docker.com/products/docker-desktop/)** (recommended latest version)

## 🚀 Installation and Setup

1. **Clone the repository:**

   ```bash
   git clone https://github.com/DevGabrielCoelho/CalendarApi.git
   cd CallendarApi
   ```

2. **Configure the environment file:**  
    Create the `.env` file in the root of the project and add the following configurations (adjust as needed):

   ```.env
    Logging_DOCKER_LogLevel__Default=Information
    Logging_DOCKER_LogLevel__MicrosoftAspNetCore=Warning
    Serilog_DOCKER_MinimumLevel=Information
    Serilog_DOCKER_WriteTo__1__Args__rollingInterval=Day

    AllowedHostsDOCKER="*"

    ConnectionStrings_DOCKER_Npgsql='Host=localhost;Port=5432;Database=CalendarApi;Username=postgres;Password=mysecretpassword'

    JWT_DOCKER_Issuer='http://localhost:5236'
    JWT_DOCKER_Audience='http://localhost:5236'
    JWT_DOCKER_SigninKey='YourStrongSigninKey' #Sha512

    Email_DOCKER_SmtpServer='smtp.gmail.com'
    Email_DOCKER_SmtpPort=587
    Email_DOCKER_SmtpUser='your-email@gmail.com'
    Email_DOCKER_SmtpPass='your-email-password or app password' 

    PasswordHasherSettings_DOCKER_SaltSize=16
    PasswordHasherSettings_DOCKER_KeySize=32
    PasswordHasherSettings_DOCKER_MemorySize=15360
    PasswordHasherSettings_DOCKER_Iterations=2
    PasswordHasherSettings_DOCKER_DegreeOfParallelism=1
    PasswordHasherSettings_DOCKER_Delimiter=";"

    POSTGRES_DOCKER_PASSWORD=mysecretpassword
    POSTGRES_DOCKER_USER=postgres
    POSTGRES_DOCKER_DB=CalendarApi
    DOCKER_NPGSQL_PORT=5432:5432

    DOCKER_ASP_NET_CORE_PORT=5236:5236
    URLS=http://+:5236
   ```

3. **Start Docker build:**

   ```bash
   docker-compose up -d --build
   ```

4. Access the API at:
   ```
   http://localhost:5236 (or the port configured in .env)
   ```

## 🧪 API Endpoints

Here are the available routes and their main functionalities:

### 🛡️ Authentication
- **`POST /api/auth/register`**: Creates a new user.
- **`POST /api/auth/send-code`**: Send code to user email.
- **`POST /api/auth/validate-code`**: Validate user email.
- **`POST /api/auth/login`**: Performs user authentication.
### 📅 Events Management
- **`POST /api/events`**: Creates a new event.
- **`GET /api/events`**: Retrieves all events.
- **`GET /api/events/{id}`**: Retrieves events by id.
- **`PUT /api/events/{id}`**: Updates an existing event.
- **`DELETE /api/events/{id}`**: Deletes an event.
### ⏰ Reminders Management
- **`GET /api/reminders`**: Retrieves all reminders.
- **`POST /api/reminders`**: Creates a new reminder.
- **`PUT /api/reminders/{id}`**: Updates an existing reminder.
- **`DELETE /api/reminders/{id}`**: Deletes a reminder.

## 🛠️ Technologies Used

- **C#**: Programming language.
- **ASP.NET Core**: Framework for building RESTful APIs.
- **PostgreSQL**: Relational database.
- **JWT**: For secure authentication.
- **Serilog**: For logging.
- **Docker**: For containerization and deployment.

## 📂 Project Structure

```
src/
├── Controllers/        # Controllers for API routes
├── Data/               # Database context
├── Docker/             # Docker setup files
├── Dtos/               # Data transfer objects
├── Interfaces/         # Contracts for abstractions
├── Mappers/            # DTO to Model conversions and vice versa
├── Migrations/         # Database migrations
├── Models/             # Data models
├── Properties/         # Project configurations
├── Repository/         # Data access logic
└── Services/           # Business logic implementations
```

## 📞 Contact

For questions, suggestions, or collaborations, feel free to reach out:

- **Gabriel Coelho**
- GitHub: [DevGabrielCoelho](https://github.com/DevGabrielCoelho)
- Email: [gabriel.coelhosousasantos.pv@gmail.com](mailto:gabriel.coelhosousasantos.pv@gmail.com)