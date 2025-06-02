pipeline {
    agent any

    // Force each API to pick up appsettings.Development.json
    environment {
        ASPNETCORE_ENVIRONMENT = 'Development'
    }

    stages {
        stage('Checkout') {
            steps {
                // Clone your repo into the workspace
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
                bat 'dotnet build Services/UserServices/UserServices/UserServices.csproj -c Release --no-restore'
                bat 'dotnet build Services/AuthorService/AuthorService.csproj    -c Release --no-restore'
                bat 'dotnet build Services/BookService/BookService.csproj       -c Release --no-restore'
                bat 'dotnet build FrontEnd/FrontEnd/FrontEnd.csproj             -c Release --no-restore'
            }
        }

        stage('Publish Services & FrontEnd') {
            steps {
                // Create “publish” directory once
                bat 'if not exist publish mkdir publish'

                // Publish each service into its own subfolder under publish/
                bat 'dotnet publish Services/UserServices/UserServices/UserServices.csproj -c Release -o publish/UserService --no-build'
                bat 'dotnet publish Services/AuthorService/AuthorService.csproj    -c Release -o publish/AuthorService --no-build'
                bat 'dotnet publish Services/BookService/BookService.csproj       -c Release -o publish/BookService --no-build'

                // Publish the front-end (static files, etc.)
                bat 'dotnet publish FrontEnd/FrontEnd/FrontEnd.csproj             -c Release -o publish/FrontEnd --no-build'
            }
        }

        stage('Launch APIs') {
            steps {
                // Start UserService, pointing content root to its publish folder
                bat '''start /B cmd /C "dotnet "%WORKSPACE%\\publish\\UserService\\UserServices.dll" ^
                    --contentroot "%WORKSPACE%\\publish\\UserService" ^
                    --urls "https://localhost:7175""'''

                // Start AuthorService on port 7183
                bat '''start /B cmd /C "dotnet "%WORKSPACE%\\publish\\AuthorService\\AuthorService.dll" ^
                    --contentroot "%WORKSPACE%\\publish\\AuthorService" ^
                    --urls "https://localhost:7183""'''

                // Start BookService on port 7265
                bat '''start /B cmd /C "dotnet "%WORKSPACE%\\publish\\BookService\\BookService.dll" ^
                    --contentroot "%WORKSPACE%\\publish\\BookService" ^
                    --urls "https://localhost:7265""'''

                // Wait ~10 seconds for all three APIs to spin up
                // (On Windows agents, `timeout /t` may require user interaction, so we use ping hack.)
                bat 'ping 127.0.0.1 -n 11 > nul'
            }
        }

        stage('Postman API Tests (via Docker)') {
            steps {
                // Run Newman inside Docker; it will mount the workspace’s Tests/ folder
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
                // Always collect the Newman JUnit XML so Jenkins can show it
                always {
                    junit '**/Tests/newman-report.xml'
                }
            }
        }
    }

    post {
        always {
            // Archive everything under publish/ so you can download the built artifacts
            archiveArtifacts artifacts: 'publish/**/*.*', fingerprint: true

            // If anything failed, this message will appear
            echo '🚨 Build or API tests failed (see console output above).'
        }
    }
}
