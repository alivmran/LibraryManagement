pipeline {
    agent any

    // Ensure ASPNETCORE_ENVIRONMENT=Development so that
    // appsettings.Development.json (with e.g. JWT keys, connection strings, etc.)
    // gets picked up. Otherwise AuthorService (and BookService) will throw at startup
    environment {
        ASPNETCORE_ENVIRONMENT = 'Development'
    }

    stages {
        stage('Checkout') {
            steps {
                // This checks out your GitHub repository
                checkout scm
            }
        }

        stage('Restore All Projects') {
            steps {
                bat 'dotnet restore Services/UserServices/UserServices/UserServices.csproj'
                bat 'dotnet restore Services/AuthorService/AuthorService.csproj'
                bat 'dotnet restore Services/BookService/BookService.csproj'
                bat 'dotnet restore FrontEnd/FrontEnd/FrontEnd.csproj'
            }
        }

        stage('Build All Projects') {
            steps {
                // Build each project in Release mode, skipping restore
                bat 'dotnet build Services/UserServices/UserServices/UserServices.csproj -c Release --no-restore'
                bat 'dotnet build Services/AuthorService/AuthorService.csproj    -c Release --no-restore'
                bat 'dotnet build Services/BookService/BookService.csproj       -c Release --no-restore'
                bat 'dotnet build FrontEnd/FrontEnd/FrontEnd.csproj             -c Release --no-restore'
            }
        }

        stage('Publish Services & FrontEnd') {
            steps {
                // Create a "publish" folder if it doesn’t exist
                bat 'if not exist publish mkdir publish'

                // Publish each service into its own folder under publish/
                bat 'dotnet publish Services/UserServices/UserServices/UserServices.csproj -c Release -o publish/UserService --no-build'
                bat 'dotnet publish Services/AuthorService/AuthorService.csproj    -c Release -o publish/AuthorService --no-build'
                bat 'dotnet publish Services/BookService/BookService.csproj       -c Release -o publish/BookService --no-build'

                // Publish FrontEnd as well
                bat 'dotnet publish FrontEnd/FrontEnd/FrontEnd.csproj             -c Release -o publish/FrontEnd --no-build'
            }
        }

        stage('Launch APIs') {
            steps {
                // Start UserService on https://localhost:7175 in the background
                bat 'start /B dotnet "%WORKSPACE%\\publish\\UserService\\UserServices.dll" --urls "https://localhost:7175"'

                // Start AuthorService on https://localhost:7183 in the background
                bat 'start /B dotnet "%WORKSPACE%\\publish\\AuthorService\\AuthorService.dll" --urls "https://localhost:7183"'

                // Start BookService on https://localhost:7265 in the background
                bat 'start /B dotnet "%WORKSPACE%\\publish\\BookService\\BookService.dll" --urls "https://localhost:7265"'

                // Give them a moment to come up before running tests
                bat 'timeout /t 10 /nobreak'
            }
        }

        stage('Postman API Tests (via Docker)') {
            steps {
                // Run Newman inside Docker: 
                //   * Mount the Jenkins workspace's Tests folder into /etc/newman
                //   * Execute your Postman collection file from there
                bat '''
                docker run --rm ^
                  -v "%WORKSPACE%\\Tests":/etc/newman ^
                  postman/newman:latest run ^
                    "/etc/newman/LibraryManagement APIs.postman_test_run.json" ^
                    --reporters cli,junit ^
                    --reporter-junit-export "/etc/newman/newman-report.xml"
                '''
            }
            post {
                // Always publish the JUnit XML, even if tests fail
                always {
                    junit '**/Tests/newman-report.xml'
                }
            }
        }
    }

    post {
        always {
            // Archive everything under publish/ so you can download the built artifacts later
            archiveArtifacts artifacts: 'publish/**/*.*', fingerprint: true

            // If any step in the pipeline failed, print a final warning
            echo '🚨 Build or API tests failed (if you see this message, check the previous logs).'
        }
    }
}
