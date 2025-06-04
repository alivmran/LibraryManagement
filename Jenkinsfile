pipeline {
    agent any

    tools {
        // Must match exactly the name you created in Manage Jenkins → Tools → NodeJS
        nodejs 'Node 18.x'
    }

    stages {
        /************************************************************************
         * 1) Checkout your Git repository                                        *
         ************************************************************************/
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        /************************************************************************
         * 2) Restore All Projects                                                *
         *    Note the double-nesting for UserService and FrontEnd.               *
         ************************************************************************/
        stage('Restore All Projects') {
            steps {
                // UserService: Services/UserServices/UserServices/UserServices.csproj
                bat 'dotnet restore Services/UserServices/UserServices/UserServices.csproj'

                // AuthorService: Services/AuthorService/AuthorService.csproj
                bat 'dotnet restore Services/AuthorService/AuthorService.csproj'

                // BookService: Services/BookService/BookService.csproj
                bat 'dotnet restore Services/BookService/BookService.csproj'

                // FrontEnd: FrontEnd/FrontEnd/FrontEnd.csproj
                bat 'dotnet restore FrontEnd/FrontEnd/FrontEnd.csproj'
            }
        }

        /************************************************************************
         * 3) Build All Projects                                                  *
         ************************************************************************/
        stage('Build All Projects') {
            steps {
                // Build UserService (nested)
                bat 'dotnet build Services/UserServices/UserServices/UserServices.csproj -c Release --no-restore'

                // Build AuthorService
                bat 'dotnet build Services/AuthorService/AuthorService.csproj -c Release --no-restore'

                // Build BookService
                bat 'dotnet build Services/BookService/BookService.csproj -c Release --no-restore'

                // Build FrontEnd (nested)
                bat 'dotnet build FrontEnd/FrontEnd/FrontEnd.csproj -c Release --no-restore'
            }
        }

        /************************************************************************
         * 4) Publish Services & FrontEnd                                          *
         ************************************************************************/
        stage('Publish Services & FrontEnd') {
            steps {
                // Ensure “publish” directory exists
                bat 'if not exist publish mkdir publish'

                // Publish UserService to publish/UserService
                bat 'dotnet publish Services/UserServices/UserServices/UserServices.csproj -c Release -o publish/UserService --no-build'

                // Publish AuthorService to publish/AuthorService
                bat 'dotnet publish Services/AuthorService/AuthorService.csproj -c Release -o publish/AuthorService --no-build'

                // Publish BookService to publish/BookService
                bat 'dotnet publish Services/BookService/BookService.csproj -c Release -o publish/BookService --no-build'

                // Publish FrontEnd to publish/FrontEnd
                bat 'dotnet publish FrontEnd/FrontEnd/FrontEnd.csproj -c Release -o publish/FrontEnd --no-build'
            }
        }

        /************************************************************************
         * 5) Start Microservices (Background)                                     *
         *    Uses “start /B” so Jenkins doesn’t wait and logs go to logs/*.log.   *
         ************************************************************************/
        stage('Start Microservices (Background)') {
            steps {
                // Start UserService (nested)
                bat 'start /B dotnet run --project Services/UserServices/UserServices/UserServices.csproj -c Release > "%WORKSPACE%\\logs\\UserService.log" 2>&1'

                // Start AuthorService
                bat 'start /B dotnet run --project Services/AuthorService/AuthorService.csproj -c Release > "%WORKSPACE%\\logs\\AuthorService.log" 2>&1'

                // Start BookService
                bat 'start /B dotnet run --project Services/BookService/BookService.csproj -c Release > "%WORKSPACE%\\logs\\BookService.log" 2>&1'

                // Start FrontEnd (nested)
                bat 'start /B dotnet run --project FrontEnd/FrontEnd/FrontEnd.csproj -c Release > "%WORKSPACE%\\logs\\FrontEnd.log" 2>&1'
            }
        }

        /************************************************************************
         * 6) Wait for Services to Spin Up                                         *
         ************************************************************************/
        stage('Wait for Services to Spin Up') {
            steps {
                // Wait ~10 seconds for all services to initialize
                bat 'timeout /t 10 /nobreak'
            }
        }

        /************************************************************************
         * 7) Run API Tests (Playwright)                                           *
         ************************************************************************/
        stage('Run API Tests (Playwright)') {
            steps {
                dir('Playwright') {
                    // Install Node dependencies (e.g., @playwright/test, browsers)
                    bat 'npm install'

                    // Execute your API tests
                    bat 'npm run test'
                }
            }
        }
    }

    post {
        always {
            /********************************************************************
             * Archive Publish Artifacts (compiled output of services & UI)        *
             ********************************************************************/
            archiveArtifacts artifacts: 'publish/**', fingerprint: true

            /********************************************************************
             * Archive Log files for each microservice (if something crashed)     *
             ********************************************************************/
            archiveArtifacts artifacts: 'logs/*.log', allowEmptyArchive: true

            /********************************************************************
             * (Optional) Archive Playwright’s HTML report (if you configured one) *
             ********************************************************************/
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
