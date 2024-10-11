# Blog C# <span style="color: #68217A;">ASP.NET Core</span>

## Overview

This is my first project using C# and ASP.NET Core. After recently exploring Go and building a project with it, I wanted to continue broadening my skill set by trying something new. My curiosity led me to C# and ASP.NET Core, and this project gave me the chance to dive into both and see how they compare.

Key features and technologies implemented are:

- `JWT Authentication`: Secure user authentication using JSON Web Tokens for session management.

- `Middlewares`: Developed custom middleware to verify JWTs, ensuring only authenticated users can access protected routes.

- `Password Hashing`: Leveraged bcrypt for secure password storage, ensuring user credentials are safely hashed.

- `OTP Verification`: Implemented a robust signup process with One-Time Password (OTP) verification, enhancing user registration security.

- `Cloudinary Integration`: Successfully integrated Cloudinary for image uploads, allowing users to manage their profile pictures seamlessly.

- `Input Validation`: Incorporated comprehensive input validation to ensure data integrity and prevent malicious input.

- `Bloom Filters`: Implemented Bloom filters to efficiently check for existing emails during user registration, optimizing performance for large databases.

- `CORS Management`: Configured Cross-Origin Resource Sharing (CORS) to enable secure interactions between the frontend and backend.

## Tech Used

- `C# ASP.NET Core`: Used as the main framework for building the backend of the application. It provides a powerful, cross-platform environment for building web APIs and services.

- `Entity Framework Core`: The Object-Relational Mapper (ORM) used to manage database operations. It allows developers to work with databases using .NET objects and eliminates the need for most of the data-access code typically required.

- `Cloudinary`: Integrated for image management and storage. It provides services such as image uploads, transformations, and optimized delivery, simplifying the handling of media files.

- `Docker`: Used to containerize the application, ensuring consistent environments for development, testing, and deployment. Docker simplifies dependency management and allows the app to run seamlessly across different machines.

## Set-Up the Project Locally

### .NET

1. Clone the repository to your local machine:

   ```bash
   git clone https://github.com/Sahil2k07/Blog-CSharp.git
   ```

2. Move to the project directory:

   ```bash
   cd Blog-CSharp
   ```

3. Set up all the required env variables by making a `.env` file. A `.env.example` file has been given for reference.

   ```dotenv
   APP_URL=http://localhost:6000

   ALLOWED_ORIGIN=*

   DATABASE_URL="Host=localhost;Port=5432;Database=database;Username=postgres;Password=password;"

   # Mailer Details.
   MAIL_HOST=smtp.gmail.com
   MAIL_USER=
   MAIL_PASS=

   # Cloudinary Details.
   CLOUD_NAME=
   API_KEY=
   API_SECRET=
   FOLDER_NAME=

   # Minimum 32 characters or will give error ;) Microsoft Guys want Applications to be Secured
   JWT_SECRET="Blog-App-C#-secret-for-authenticating-users"
   ```

4. Restore all the packages

   ```bash
   dotnet restore
   ```

5. Install the `ef` package globally to apply migrations

   ```bash
   dotnet tool install --global dotnet-ef
   ```

6. Apply the Migrations

   ```bash
   dotnet ef database update
   ```

7. Run the Application

   ```bash
   dotnet run
   ```

### Docker

1. Clone the repository to your local machine:

   ```bash
   git clone https://github.com/Sahil2k07/Blog-CSharp.git
   ```

2. Move to the project directory:

   ```bash
   cd Blog-CSharp
   ```

3. Set up these env variables by making a `.env` file. A `.env.example` file has been given for reference.

   ```dotenv
   # Mailer Details.
   MAIL_HOST=smtp.gmail.com
   MAIL_USER=
   MAIL_PASS=

   # Cloudinary Details.
   CLOUD_NAME=
   API_KEY=
   API_SECRET=
   FOLDER_NAME=
   ```

4. Run the command to start the Docker container:

   ```bash
   docker-compose up
   ```

   or run in background `(detached mode)`

   ```bash
   docker-compose up -d

   ```

5. If you have docker compose plugin, use this command instead:

   ```bash
   docker compose up
   ```

   or run in background `(detached mode)`

   ```bash
   docker compose up -d
   ```

6. Access the project on `http://localhost:5000` of your machine.
