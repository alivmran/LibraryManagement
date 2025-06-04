pipeline {
    agent any

    /************************************************************************
     * 1) Tell Jenkins which NodeJS installation to use (must match “Name”   *
     *    you entered under Manage Jenkins → Tools → NodeJS).                *
     ************************************************************************/
    tools {
        nodejs 'Node 18.x'
    }

    environment {
        // Path to your published output. Change if your path differs.
        PUBLISH_DIR = "${env.WORKSPACE}\\publish"
    }

    stages {
        /************************************************************************
         * 2) Checkout stage: Jenkins automatically clones your Git repo.       *
         ************************************************************************/
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        /************************************************************************
         * 3) Dotnet Restore: restore NuGet packages for each project.          *
         ************************************************************************/
        stage('Restore All Projects') {
            steps {
                bat 'dotnet restore Services/UserServices/UserServices/UserServices.csproj'
                bat 'dotnet restore Services/AuthorService/AuthorService/AuthorService.csproj'
                bat 'dotnet restore Services/BookService/BookService/BookService.csproj'
                bat 'dotnet restore FrontEnd/FrontEnd/FrontEnd.csproj'
            }
        }

        /************************************************************************
         * 4) Dotnet Build: compile each project in Release mode (no-restore).  *
         ************************************************************************/
        stage('Build All Projects') {
            steps {
                bat 'dotnet build Services/UserServices/UserServices/UserServices.csproj -c Release --no-restore'
                bat 'dotnet build Services/AuthorService/AuthorService/AuthorService.csproj -c Release --no-restore'
                bat 'dotnet build Services/BookService/BookService/BookService.csproj -c Release --no-restore'
                bat 'dotnet build FrontEnd/FrontEnd/FrontEnd.csproj -c Release --no-restore'
            }
        }

        /************************************************************************
         * 5) Dotnet Publish: publish each service into a subfolder under “publish”. *
         ************************************************************************/
        stage('Publish Services & FrontEnd') {
            steps {
                // Ensure the “publish” directory exists
                bat 'if not exist publish mkdir publish'

                // Publish each project into a designated folder under publish/
                bat 'dotnet publish Services/UserServices/UserServices/UserServices.csproj -c Release -o "%PUBLISH_DIR%\\UserService" --no-build'
                bat 'dotnet publish Services/AuthorService/AuthorService/AuthorService.csproj -c Release -o "%PUBLISH_DIR%\\AuthorService" --no-build'
                bat 'dotnet publish Services/BookService/BookService/BookService.csproj -c Release -o "%PUBLISH_DIR%\\BookService" --no-build'
                bat 'dotnet publish FrontEnd/FrontEnd/FrontEnd.csproj -c Release -o "%PUBLISH_DIR%\\FrontEnd" --no-build'
            }
        }

        /************************************************************************
         * 6) Start all three services in the background so endpoints are live:  *
         *    - UserService at https://localhost:7175                          *
         *    - AuthorService at https://localhost:7183                        *
         *    - BookService at https://localhost:7265                          *
         *                                                                      *
         * Note: we use “start /B” on Windows to launch a process in background *
         * without waiting. We redirect logs to a .log file so Jenkins console *
         * doesn’t hang. You can adjust paths as needed.                       *
         ************************************************************************/
        stage('Start Microservices (Background)') {
            steps {
                script {
                    // Launch UserService
                    bat label: 'Start UserService',
                        script: 'start /B dotnet run --project Services/UserServices/UserServices/UserServices.csproj -c Release > "%WORKSPACE%\\logs\\UserService.log" 2>&1'
                    // Launch AuthorService
                    bat label: 'Start AuthorService',
                        script: 'start /B dotnet run --project Services/AuthorService/AuthorService/AuthorService.csproj -c Release > "%WORKSPACE%\\logs\\AuthorService.log" 2>&1'
                    // Launch BookService
                    bat label: 'Start BookService',
                        script: 'start /B dotnet run --project Services/BookService/BookService/BookService.csproj -c Release > "%WORKSPACE%\\logs\\BookService.log" 2>&1'
                }
            }
        }

        /************************************************************************
         * 7) Wait a few seconds to ensure all services are fully up            *
         ************************************************************************/
        stage('Wait for Services to Spin Up') {
            steps {
                // Wait 10 seconds for all services to finish booting
                bat 'timeout /t 10 /nobreak'
            }
        }

        /************************************************************************
         * 8) Run your Playwright API tests                                       *
         *    cd into the “Playwright” folder, npm install, then npm run test     *
         ************************************************************************/
        stage('Run API Tests (Playwright)') {
            steps {
                dir('Playwright') {
                    // Install all Node packages, including @playwright/test and browsers
                    bat 'npm install'

                    // Run your API tests. If you added an HTML reporter in package.json,
                    // you might do: npm run test -- --reporter=html
                    bat 'npm run test'
                }
            }
        }
    }

    post {
        always {
            /********************************************************************
             * 9) Archive everything under “publish/” so you can download the  *
             *    compiled .NET outputs, and archive any logs you want to keep.*
             ********************************************************************/
            archiveArtifacts artifacts: 'publish/**', fingerprint: true
            // Optionally archive your Microservice logs:
            archiveArtifacts artifacts: 'logs/*.log', allowEmptyArchive: true

            // If you generate an HTML test report under Playwright (e.g. at
            // Playwright/playwright-report/index.html), you could archive that too:
            archiveArtifacts artifacts: 'Playwright/playwright-report/**/*.html', allowEmptyArchive: true
        }
        success {
            echo '✅ Build + Publish + API tests all succeeded!'
        }
        failure {
            echo '❌ One or more stages failed. Check the console log & archived artifacts.'
        }
    }
}
