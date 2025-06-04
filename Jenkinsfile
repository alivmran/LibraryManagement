pipeline {
    agent any

    tools {
        nodejs 'Node 18.x'  // Use your configured NodeJS tool name
    }

    stages {
        stage('Checkout') {
            steps {
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
                bat 'dotnet build Services/AuthorService/AuthorService.csproj -c Release --no-restore'
                bat 'dotnet build Services/BookService/BookService.csproj -c Release --no-restore'
                bat 'dotnet build FrontEnd/FrontEnd/FrontEnd.csproj -c Release --no-restore'
            }
        }

        stage('Publish Services & FrontEnd') {
            steps {
                bat 'if not exist publish mkdir publish'
                bat 'dotnet publish Services/UserServices/UserServices/UserServices.csproj -c Release -o publish/UserService --no-build'
                bat 'dotnet publish Services/AuthorService/AuthorService.csproj -c Release -o publish/AuthorService --no-build'
                bat 'dotnet publish Services/BookService/BookService.csproj -c Release -o publish/BookService --no-build'
                bat 'dotnet publish FrontEnd/FrontEnd/FrontEnd.csproj -c Release -o publish/FrontEnd --no-build'
            }
        }

        stage('Start Microservices (Background)') {
            steps {
                // Ensure logs/ exists so the redirection won't fail:
                bat 'if not exist logs mkdir logs'

                bat 'start /B dotnet run --project Services/UserServices/UserServices/UserServices.csproj -c Release > "%WORKSPACE%\\logs\\UserService.log" 2>&1'
                bat 'start /B dotnet run --project Services/AuthorService/AuthorService.csproj -c Release > "%WORKSPACE%\\logs\\AuthorService.log" 2>&1'
                bat 'start /B dotnet run --project Services/BookService/BookService.csproj -c Release > "%WORKSPACE%\\logs\\BookService.log" 2>&1'
                bat 'start /B dotnet run --project FrontEnd/FrontEnd/FrontEnd.csproj -c Release > "%WORKSPACE%\\logs\\FrontEnd.log" 2>&1'
            }
        }

        stage('Wait for Services to Spin Up') {
            steps {
                // Option A: ping‐based delay of ~10 seconds
                bat 'ping 127.0.0.1 -n 11 >nul'
                
                // --- OR, if you prefer Option B (PowerShell) instead, comment out the ping line above and uncomment below:
                // bat 'powershell -NoProfile -Command "Start-Sleep -Seconds 10"'
            }
        }

        stage('Run API Tests (Playwright)') {
            steps {
                dir('Playwright') {
                    bat 'npm install'
                    bat 'npm run test'
                }
            }
        }
    }

    post {
        always {
            archiveArtifacts artifacts: 'publish/**', fingerprint: true
            archiveArtifacts artifacts: 'logs/*.log', allowEmptyArchive: true
            archiveArtifacts artifacts: 'Playwright/playwright-report/**/*.html', allowEmptyArchive: true
        }
        success {
            echo '✅ Build + Publish + API tests all succeeded!'
        }
        failure {
            echo '❌ One or more stages failed. Check console & archived artifacts.'
        }
    }
}
