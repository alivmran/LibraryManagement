pipeline {
  agent any

  environment {
    CONFIGURATION    = 'Release'

    // Paths to your .NET project files
    USER_PROJ        = 'Services/UserServices/UserServices/UserServices.csproj'
    AUTHOR_PROJ      = 'Services/AuthorService/AuthorService.csproj'
    BOOK_PROJ        = 'Services/BookService/BookService.csproj'
    FRONTEND_PROJ    = 'FrontEnd/FrontEnd/FrontEnd.csproj'

    // Postman collection (exported v2.1 JSON)
    POSTMAN_COLL     = 'Tests/LibraryManagement APIs.postman_test_run.json'
  }

  stages {
    stage('Checkout') {
      steps {
        // Pull your code from GitHub (adjust URL/branch to your own)
        checkout([
          $class: 'GitSCM',
          branches: [[name: '*/main']],
          userRemoteConfigs: [[url: 'https://github.com/your-org/LibraryManagement.git']]
        ])
      }
    }

    stage('Restore All Projects') {
      steps {
        // Restore NuGet packages for each csproj
        bat "dotnet restore \"${USER_PROJ}\""
        bat "dotnet restore \"${AUTHOR_PROJ}\""
        bat "dotnet restore \"${BOOK_PROJ}\""
        bat "dotnet restore \"${FRONTEND_PROJ}\""
      }
    }

    stage('Build All Projects') {
      steps {
        // Build each project in Release mode (no need to restore again)
        bat "dotnet build \"${USER_PROJ}\" -c Release --no-restore"
        bat "dotnet build \"${AUTHOR_PROJ}\" -c Release --no-restore"
        bat "dotnet build \"${BOOK_PROJ}\" -c Release --no-restore"
        bat "dotnet build \"${FRONTEND_PROJ}\" -c Release --no-restore"
      }
    }

    stage('Publish Services & FrontEnd') {
      steps {
        // Create a "publish" folder if it doesn't exist
        bat 'if not exist publish mkdir publish'

        // Publish each service into its own folder under "publish/"
        bat "dotnet publish \"${USER_PROJ}\"   -c Release -o publish/UserService   --no-build"
        bat "dotnet publish \"${AUTHOR_PROJ}\" -c Release -o publish/AuthorService --no-build"
        bat "dotnet publish \"${BOOK_PROJ}\"   -c Release -o publish/BookService   --no-build"
        bat "dotnet publish \"${FRONTEND_PROJ}\" -c Release -o publish/FrontEnd  --no-build"
      }
    }

    stage('Launch APIs') {
      steps {
        // Start UserService on https://localhost:7175
        bat '''
          start /B dotnet "%WORKSPACE%\\publish\\UserService\\UserServices.dll" ^
            --urls "https://localhost:7175"
        '''

        // Start AuthorService on https://localhost:7183
        bat '''
          start /B dotnet "%WORKSPACE%\\publish\\AuthorService\\AuthorService.dll" ^
            --urls "https://localhost:7183"
        '''

        // Start BookService on https://localhost:7265
        bat '''
          start /B dotnet "%WORKSPACE%\\publish\\BookService\\BookService.dll" ^
            --urls "https://localhost:7265"
        '''

        // Give each service ~10 seconds to finish booting up before running tests
        bat 'timeout /t 10 /nobreak'
      }
    }

    stage('Postman API Tests (via Docker)') {
      steps {
        script {
          /*
           When you run Newman inside Docker on Windows, "localhost" inside the container
           does NOT refer to your host machine. Instead, use "host.docker.internal" so that
           Newman inside the container can reach "https://localhost:7175", etc., on the host.

           We pass three --env-var flags so that Postman’s {{userBaseUrl}} etc. resolve correctly.
          */
          bat """
            docker run --rm ^
              -v "%WORKSPACE%:/etc/newman" ^
              postman/newman:latest run ^
              "/etc/newman/${POSTMAN_COLL}" ^
              --env-var "userBaseUrl=https://host.docker.internal:7175" ^
              --env-var "authorBaseUrl=https://host.docker.internal:7183" ^
              --env-var "bookBaseUrl=https://host.docker.internal:7265" ^
              --reporters cli,junit ^
              --reporter-junit-export "/etc/newman/newman-report.xml"
          """
        }
      }
    }

    stage('Publish Test Results') {
      steps {
        // Let Jenkins pick up the JUnit‐style XML that Newman generated
        junit 'newman-report.xml'
      }
    }
  }

  post {
    always {
      // Archive your compiled/published output so you can download artifacts if needed
      archiveArtifacts artifacts: 'publish/**', fingerprint: true
    }
    success {
      echo '✅ All builds, publishes, and API tests passed.'
    }
    failure {
      echo '🚨 Something failed (build, launch, or Postman tests).'
    }
  }
}
