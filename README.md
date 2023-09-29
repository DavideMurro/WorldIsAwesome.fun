# WorldIsAwesome.fun

World is Awesome .fun is a web application where to search places to visit, find experiences to do, make new friends and share profiles to other people.
It allows to:

- sign up and manage a personal profile.
- scroll a 3D world where can find all experiences to do in each place of earth.
- make new friends and send them emails.
- manage own experiences making it public, private or confidential.
- save your experiences in every kinds of formats: images, videos, audios

The application is written with MVC .Net Core, so it uses:

- Front end: HTML, CSS, Javascript
- Back end: C# .Net Core 3.1
- Database: SQL SERVER

## File structure

### Models

It is the application's data structure. It contains:

- **DBContext**
  Models of the DB. The application is Database first so it needs to be updated each time the DB changes
- **Exceptions**
  Personal extended classes for exceptions
- **View**
  Classes of the view used for the visualization of data recivied from the database
- _root_
  Other shared classes and enums

### Views

It contains all application pages, the cshtml files, divided in:

- **Error**
  The error page, in case of any problem like 404 not found
- **Home**
  Main pages like index, login and contact us page
- **Search**
  Pages focused only on search with text field and several filters
- **Shared**
  Shared pieces of html code used in multiple pages (like metadata for the html pages head)
- **Status**
  External pages reached from other platform (like the confirmation page for user activations)
- **View**
  Pages for single subject visualizations, such as specific user or experience

### Controllers

Accepts input and converts it to commands for the model or view. <br>
There is a file for each view component for its management, more the _ApiController_ that contains methods for Javascript calls (like login)

### Services

A service manages all functionalities of a specific feature.

- **DateTimeHelpers**
  Converts time to differents formats and parse milliseconds to datetime and viceversa
- **ErrorHandlingMiddleware**
  Manages a custom error handling
- **FileManager**
  Has methods for getting, creating and deleting files
- **MediaEditor**
  Using different libraries it edits, saves and deletes photos and videos. <br>
  It also crops, compresses media to saveserver space
- **SecurePasswordHaser**
  It encrypts and verify passwords
- **SessionAuthorizeAttribute**
  It is a middle class that controls the user access

### Custom_Lib

External libraries for .net core used by the applications. Other libraries are managed by NuGet

### [Custom_ExperiencesMedia]

Default custom folder contains all experience media uploaded by users. <br>
The application creates other folders:

- **Previews**: cut and compressed media for quick viewing
- **Temp**: temporary folder to facilitate media management

This folder is created automatically. <br>
Folder path can be changed in the _appsettings.json_ file

### [Custom_UsersPictures]

Default custom folder contains all user pictures. <br>
The application creates other folders:

- **Previews**: cut and compressed pictures for quick viewing
- **Temp**: temporary folder to facilitate pictures management

This folder is created automatically. <br>
Folder path can be changed in the _appsettings.json_ file

### [Custom_PlacesPictures]

Default custom folder contains all place pictures. <br>
The application creates other folders:

- **Previews**: cut and compressed pictures for quick viewing
- **Temp**: temporary folder to facilitate pictures management

This folder is created automatically. <br>
Folder path can be changed in the _appsettings.json_ file

### wwwroot

Contains html assets:

- **css**: all css files
- **images**: images, logos and icons of the applications
- **js**: all js files to manage the web side and call APIs
- **lib**: fonts and libraries for web side
- _root_: files for web indexing

## .Net core libraries

- **EntityFramework**
  Entity Framework (EF) object–relational mapping (ORM) framework. In this case it is used for managing operations with SQL SERVER
- **Newtonsoft.json**
  Json.NET is a popular high-performance JSON framework for .NET
- **NReco.VideoConverter**
  Video converter component for C#/.NET (wrapper for FFMpeg command line utility). Can be used for converting videos, transcoding live streams, extracting video thumbnails, applying watermarks etc. <br>
  A copy of _FFMpeg_ is saved into _Custom_Lib_ folder
- **SixLabors.ImageSharp**
  A new, fully featured, fully managed, cross-platform, 2D graphics API for .NET

## Javascript libraries and frameworks

- **hammerjs**
  A JavaScript library for detecting touch gestures. <br>
  It is used to open the menu
- **miniature.earth**
  It manages the entire 3D earth component.
- **mydropinput**
  Library for files drag and drop. <br>
  It is used to drop files into input file forms
- **SunEditor**
  The Suneditor is a lightweight, flexible, customizable WYSIWYG text editor for web applications. <br>
  It is used to write a very engaging description
- **zoom.js**
  Enables a minimal JS API for zooming in on specific points or DOM elements. <br>
  It is used to zoom images

## Database

The database was designed for SQL SERVER. <br>
It has these tables:

- **Experiences**
  All experiences saved
- **Places**
  All places where at least 1 experience is connected. So here there are all places the users visited
- **Users**
  Users registered on the application
- **Users_ExperiencesToDo**
  All experiences added by the users in their profile as 'experience to do'
- **Users_UsersFriends**
  Connections between users when they become friends
- **Files**
  Table contains files uploaded by users. It cointains only information, the real files are saved in the filesystem, into folder specified in the _appsettings.json_ file

It has these views:

- **View_ExperienceFiles**
  All experiences plus files and user connected informations
- **View_PlaceExperiences**
  All places plus experiences and user connected informations

## Configuration

Configuration is quite simple.

- Check that all libraries are updated in NuGet Package manager for .Net Core and in wwwroot > lib for javascript
- Check and, in case, edit _appsettings.json_ file

  ```json
  {
    "Logging": {
      "LogLevel": {
        "Default": "Information",
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information"
      }
    },
    "AllowedHosts": "*",
    "ConnectionStrings": {
      "WorldIsAwesomeContext": "Server=localhost\\SQLEXPRESS;Database=WorldIsAwesome;Trusted_Connection=True;" // Line to edit: DB connection string
    },
    "FolderManagement": {
      "ExperiencesMedia": "Custom_ExperiencesMedia", // Name of folder where experience media are saved
      "UsersPictures": "Custom_UsersPictures", // Name of folder where user pictures are saved
      "PlacesPictures": "Custom_PlacesPictures" // Name of folder where place pictures are saved
    },
    "SMTPMail": {
      // Paragraph to edit: SMTP Mail configuration
      "Server": "mail.worldisawesome.fun",
      "Port": 25,
      "Username": "info@worldisawesome.fun",
      "Password": "",
      "IsSSL": false
    }
  }
  ```

- Check and, in case, edit _launchSettings.json_ file
  ```json
  {
    "iisSettings": {
      "windowsAuthentication": false,
      "anonymousAuthentication": true,
      "iisExpress": {
        "applicationUrl": "http://localhost:49886",
        "sslPort": 44310
      }
    },
    "$schema": "http://json.schemastore.org/launchsettings.json",
    "profiles": {
      "IIS Express": {
        "commandName": "IISExpress",
        "launchBrowser": true,
        "launchUrl": "weatherforecast",
        "environmentVariables": {
          "ASPNETCORE_ENVIRONMENT": "Development"
        }
      },
      "www.worldisawesome.fun": {
        "commandName": "Project",
        "environmentVariables": {
          "ASPNETCORE_ENVIRONMENT": "Development"
        },
        "applicationUrl": "http://localhost:5000;https://localhost:5001" // Line to edit: these are urls where the application is hosted during the debug
      }
    }
  }
  ```
  - It is not necessary to use IIS Express to host the application, but kestrel webserver is enough
  - It is possible to install SSL certification to make https protocol working in development mode just executing this command:
  ```PowerShell
  dotnet dev-certs https --trust
  ```

## Screenshots
![tabletbig3](https://github.com/DavideMurro/WorldIsAwesome.fun/assets/118051417/c99d7c30-d0a6-45cf-b795-89dbb8e71e16)
![tabletbig2](https://github.com/DavideMurro/WorldIsAwesome.fun/assets/118051417/63726725-fd1b-4c4f-8426-53ae17003d2a)
![tabletbig4](https://github.com/DavideMurro/WorldIsAwesome.fun/assets/118051417/431e9e13-d1d9-401f-9f6e-01cfca613ff3)
![tablet4](https://github.com/DavideMurro/WorldIsAwesome.fun/assets/118051417/1e145609-2e97-4ea5-96c3-863858fcaa83)
![mobile2](https://github.com/DavideMurro/WorldIsAwesome.fun/assets/118051417/b9900a7d-6718-49c2-ab0d-00c9fff711c2)
![mobile3](https://github.com/DavideMurro/WorldIsAwesome.fun/assets/118051417/c21884f7-ec1f-416f-87ad-7bcb4bc3c38f)
![mobile4](https://github.com/DavideMurro/WorldIsAwesome.fun/assets/118051417/97249fb2-e4b8-4b25-bce1-fab7f2875829)


