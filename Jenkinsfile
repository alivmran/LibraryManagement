// File: G:\LibraryManagement\Jenkinsfile

pipeline {
    agent any

    environment {
        // Build configuration
        CONFIGURATION = 'Release'

        // Paths to each microservice project (use .csproj for publishing)
        USER_PROJ      = 'Services/UserServices/UserServices/UserServices.csproj'
        AUTHOR_PROJ    = 'Services/AuthorService/AuthorService.csproj'
        BOOK_PROJ      = 'Services/BookService/BookService.csproj'
        FRONTEND_PROJ  = 'FrontEnd/FrontEnd/FrontEnd.csproj'

        // Path to your Postman collection (adjust if your path differs)
        POSTMAN_COLL   = 'Tests/LibraryManagement APIs.postman_test_run.json'
    }

    stages {
        stage('Checkout') {
            steps {
                checkout([$class: 'GitSCM',
                          branches: [[name: '*/main']],
                          userRemoteConfigs: [[
                              url: 'https://github.com/alivmran/LibraryManagement.git'
                          ]]
                ])
            }
        }

        stage('Restore All Projects') {
            steps {
                // Restore each .csproj individually
                bat "dotnet restore \"${env.USER_PROJ}\""
                bat "dotnet restore \"${env.AUTHOR_PROJ}\""
                bat "dotnet restore \"${env.BOOK_PROJ}\""
                bat "dotnet restore \"${env.FRONTEND_PROJ}\""
            }
        }

        stage('Build All Projects') {
            steps {
                // Build each project (no-restore since we already restored)
                bat "dotnet build \"${env.USER_PROJ}\" -c ${env.CONFIGURATION} --no-restore"
                bat "dotnet build \"${env.AUTHOR_PROJ}\" -c ${env.CONFIGURATION} --no-restore"
                bat "dotnet build \"${env.BOOK_PROJ}\" -c ${env.CONFIGURATION} --no-restore"
                bat "dotnet build \"${env.FRONTEND_PROJ}\" -c ${env.CONFIGURATION} --no-restore"
            }
        }

        stage('Publish Services & FrontEnd') {
            steps {
                // Make sure the publish folder exists
                bat 'if not exist publish mkdir publish'

                // Publish UserService into its own folder
                bat "dotnet publish \"${env.USER_PROJ}\" -c ${env.CONFIGURATION} -o publish/UserService --no-build"

                // Publish AuthorService into its own folder
                bat "dotnet publish \"${env.AUTHOR_PROJ}\" -c ${env.CONFIGURATION} -o publish/AuthorService --no-build"

                // Publish BookService into its own folder
                bat "dotnet publish \"${env.BOOK_PROJ}\" -c ${env.CONFIGURATION} -o publish/BookService --no-build"

                // Publish FrontEnd into its own folder
                bat "dotnet publish \"${env.FRONTEND_PROJ}\" -c ${env.CONFIGURATION} -o publish/FrontEnd --no-build"
            }
        }

        stage('Postman API Tests (via Docker)') {
            steps {
                script {
                    // If your microservices are not already running, you can start them here:
                    // (Uncomment and adjust ports/paths if needed)
                    //
                    // bat "start cmd /c dotnet run --project \"Services/UserServices/UserServices/UserServices.csproj\" --urls=https://localhost:7175"
                    // bat "start cmd /c dotnet run --project \"Services/AuthorService/AuthorService.csproj\" --urls=https://localhost:7183"
                    // bat "start cmd /c dotnet run --project \"Services/BookService/BookService.csproj\" --urls=https://localhost:7265"
                    // bat "timeout /t 5 /nobreak"

                    // Run the Postman collection in Dockerized Newman:
                    bat """
                      docker run --rm ^
                        -v \"%cd%:/etc/newman\" ^
                        postman/newman:latest run ^
                          \"/etc/newman/${POSTMAN_COLL}\" ^
                          --reporters cli,junit ^
                          --reporter-junit-export \"/etc/newman/newman-report.xml\"
                    """
                }
            }
            post {
                always {
                    // Publish the JUnit‐style test report
                    junit 'newman-report.xml'
                }
            }
        }
    }

    post {
        always {
            archiveArtifacts artifacts: 'publish/**/*', fingerprint: true
        }
        success {
            echo '✅ Build, Publish, and API tests all passed!'
        }
        unstable {
            echo '⚠️ Build succeeded but some API tests failed.'
        }
        failure {
            echo '🚨 Build or API tests failed.'
        }
    }
}
