pipeline {
    agent any

    tools {
        // Use exactly the same name you configured under Manage Jenkins → Tools → NodeJS
        nodejs 'Node 18.x'
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Restore All Projects') {
            steps {
                // UserService (nested two levels)
                bat 'dotnet restore Services/UserServices/UserServices/UserServices.csproj'
                // AuthorService (one level)
                bat 'dotnet restore Services/AuthorService/AuthorService.csproj'
                // BookService (one level)
                bat 'dotnet restore Services/BookService/BookService.csproj'
                // FrontEnd (nested two levels)
                bat 'dotnet restore FrontEnd/FrontEnd/FrontEnd.csproj'
            }
        }

        stage('Build All Projects') {
            steps {
                // Build UserService
                bat 'dotnet build Services/UserServices/UserServices/UserServices.csproj -c Release --no-restore'
                // Build AuthorService
                bat 'dotnet build Services/AuthorService/AuthorService.csproj -c Release --no-restore'
                // Build BookService
                bat 'dotnet build Services/BookService/BookService.csproj -c Release --no-restore'
                // Build FrontEnd
                bat 'dotnet build FrontEnd/FrontEnd/FrontEnd.csproj -c Release --no-restore'
            }
        }

        stage('Publish Services & FrontEnd') {
            steps {
                // Create a publish/ directory if it does not already exist
                bat 'if not exist publish mkdir publish'

                // Publish UserService
                bat 'dotnet publish Services/UserServices/UserServices/UserServices.csproj -c Release -o publish/UserService --no-build'
                // Publish AuthorService
                bat 'dotnet publish Services/AuthorService/AuthorService.csproj -c Release -o publish/AuthorService --no-build'
                // Publish BookService
                bat 'dotnet publish Services/BookService/BookService.csproj -c Release -o publish/BookService --no-build'
                // Publish FrontEnd
                bat 'dotnet publish FrontEnd/FrontEnd/FrontEnd.csproj -c Release -o publish/FrontEnd --no-build'
            }
        }

        stage('Start Microservices (Background)') {
            steps {
                // 1) Ensure logs/ folder exists (so redirection does not fail)
                bat 'if not exist logs mkdir logs'

                // 2) Start UserService (nested two levels)
                bat 'start /B dotnet run --project Services/UserServices/UserServices/UserServices.csproj -c Release > "%WORKSPACE%\\logs\\UserService.log" 2>&1'

                // 3) Start AuthorService (one level)
                bat 'start /B dotnet run --project Services/AuthorService/AuthorService.csproj -c Release > "%WORKSPACE%\\logs\\AuthorService.log" 2>&1'

                // 4) Start BookService (one level)
                bat 'start /B dotnet run --project Services/BookService/BookService.csproj -c Release > "%WORKSPACE%\\logs\\BookService.log" 2>&1'

                // 5) Start FrontEnd (nested two levels)
                bat 'start /B dotnet run --project FrontEnd/FrontEnd/FrontEnd.csproj -c Release > "%WORKSPACE%\\logs\\FrontEnd.log" 2>&1'
            }
        }

        stage('Wait for Services to Spin Up') {
            steps {
                // Give each service ~10 seconds to initialize fully
                bat 'timeout /t 10 /nobreak'
            }
        }

        stage('Run API Tests (Playwright)') {
            steps {
                dir('Playwright') {
                    // Install all Node modules (including @playwright/test and browsers)
                    bat 'npm install'
                    // Execute Playwright tests
                    bat 'npm run test'
                }
            }
        }
    }

    post {
        always {
            // Archive compiled outputs in publish/
            archiveArtifacts artifacts: 'publish/**', fingerprint: true
            // Archive any service log files
            archiveArtifacts artifacts: 'logs/*.log', allowEmptyArchive: true
            // If you have an HTML report under Playwright/playwright-report, archive it too:
            archiveArtifacts artifacts: 'Playwright/playwright-report/**/*.html', allowEmptyArchive: true
        }
        success {
            echo '✅ Build + Publish + API tests all succeeded!'
        }
        failure {
            echo '❌ One or more stages failed. Check console output & archived artifacts.'
        }
    }
}
