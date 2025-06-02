// File: G:\LibraryManagement\Jenkinsfile

pipeline {
    agent any

    environment {
        // Paths to each microservice and frontend project/solution
        USER_SOL      = 'Services/UserServices/UserServices.sln'
        AUTHOR_PROJ   = 'Services/AuthorService/AuthorService.csproj'
        BOOK_PROJ     = 'Services/BookService/BookService.csproj'
        FRONTEND_PROJ = 'FrontEnd/FrontEnd/FrontEnd.csproj'
        CONFIGURATION = 'Release'

        // Path to your Postman collection (note the space in the filename)
        POSTMAN_COLL  = 'Tests/Postman/LibraryManagement APIs.postman_test_run.json'
    }

    stages {
        stage('Checkout') {
            steps {
                // Pull from GitHub (main branch)
                checkout([$class: 'GitSCM',
                          branches: [[name: '*/main']],
                          userRemoteConfigs: [[
                              url: 'https://github.com/alivmran/LibraryManagement.git'
                          ]]
                ])
            }
        }

        stage('Restore UserService') {
            steps {
                bat "dotnet restore \"${env.USER_SOL}\""
            }
        }

        stage('Restore AuthorService') {
            steps {
                bat "dotnet restore \"${env.AUTHOR_PROJ}\""
            }
        }

        stage('Restore BookService') {
            steps {
                bat "dotnet restore \"${env.BOOK_PROJ}\""
            }
        }

        stage('Restore FrontEnd') {
            steps {
                bat "dotnet restore \"${env.FRONTEND_PROJ}\""
            }
        }

        stage('Build UserService') {
            steps {
                bat "dotnet build \"${env.USER_SOL}\" -c ${env.CONFIGURATION} --no-restore"
            }
        }

        stage('Build AuthorService') {
            steps {
                bat "dotnet build \"${env.AUTHOR_PROJ}\" -c ${env.CONFIGURATION} --no-restore"
            }
        }

        stage('Build BookService') {
            steps {
                bat "dotnet build \"${env.BOOK_PROJ}\" -c ${env.CONFIGURATION} --no-restore"
            }
        }

        stage('Build FrontEnd') {
            steps {
                bat "dotnet build \"${env.FRONTEND_PROJ}\" -c ${env.CONFIGURATION} --no-restore"
            }
        }

        // (Optional) Insert any unit‐test stages here if you have them.

        stage('Publish Services & FrontEnd') {
            steps {
                // Ensure 'publish' directory exists
                bat 'if not exist publish mkdir publish'

                // Publish UserService
                bat "dotnet publish \"${env.USER_SOL}\" -c ${env.CONFIGURATION} -o publish/UserService --no-build"

                // Publish AuthorService
                bat "dotnet publish \"${env.AUTHOR_PROJ}\" -c ${env.CONFIGURATION} -o publish/AuthorService --no-build"

                // Publish BookService
                bat "dotnet publish \"${env.BOOK_PROJ}\" -c ${env.CONFIGURATION} -o publish/BookService --no-build"

                // Publish FrontEnd
                bat "dotnet publish \"${env.FRONTEND_PROJ}\" -c ${env.CONFIGURATION} -o publish/FrontEnd --no-build"
            }
        }

        stage('Postman API Tests (via Docker)') {
            steps {
                script {
                    // If your services are not already running on localhost, you can start them here:
                    //
                    // bat "start cmd /c dotnet run --project \"Services/UserServices/UserServices.csproj\" --urls=https://localhost:7175"
                    // bat "start cmd /c dotnet run --project \"Services/AuthorService/AuthorService.csproj\" --urls=https://localhost:7183"
                    // bat "start cmd /c dotnet run --project \"Services/BookService/BookService.csproj\" --urls=https://localhost:7265"
                    // 
                    // Wait a few seconds for services to start up:
                    // bat "timeout /t 5 /nobreak"

                    // Run Newman inside Docker, mounting the workspace to /etc/newman
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
                    // Publish the JUnit XML so Jenkins can display test results
                    junit 'newman-report.xml'
                }
            }
        }
    }

    post {
        always {
            // Archive the compiled/published outputs for download
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
