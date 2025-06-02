pipeline {
    agent any

    environment {
        CONFIGURATION = 'Release'
        USER_PROJ     = 'Services/UserServices/UserServices/UserServices.csproj'
        AUTHOR_PROJ   = 'Services/AuthorService/AuthorService.csproj'
        BOOK_PROJ     = 'Services/BookService/BookService.csproj'
        FRONTEND_PROJ = 'FrontEnd/FrontEnd/FrontEnd.csproj'

        // We’ll correct this below. For now, keep as you know it:
        POSTMAN_COLL  = 'Tests/LibraryManagement APIs.postman_test_run.json'
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

        // ← Temporary: List every file Jenkins pulled down
        stage('Inspect Workspace') {
            steps {
                bat 'dir /s /b'
            }
        }

        stage('Restore All Projects') {
            steps {
                bat "dotnet restore \"${env.USER_PROJ}\""
                bat "dotnet restore \"${env.AUTHOR_PROJ}\""
                bat "dotnet restore \"${env.BOOK_PROJ}\""
                bat "dotnet restore \"${env.FRONTEND_PROJ}\""
            }
        }

        stage('Build All Projects') {
            steps {
                bat "dotnet build \"${env.USER_PROJ}\" -c ${env.CONFIGURATION} --no-restore"
                bat "dotnet build \"${env.AUTHOR_PROJ}\" -c ${env.CONFIGURATION} --no-restore"
                bat "dotnet build \"${env.BOOK_PROJ}\" -c ${env.CONFIGURATION} --no-restore"
                bat "dotnet build \"${env.FRONTEND_PROJ}\" -c ${env.CONFIGURATION} --no-restore"
            }
        }

        stage('Publish Services & FrontEnd') {
            steps {
                bat 'if not exist publish mkdir publish'
                bat "dotnet publish \"${env.USER_PROJ}\" -c ${env.CONFIGURATION} -o publish/UserService --no-build"
                bat "dotnet publish \"${env.AUTHOR_PROJ}\" -c ${env.CONFIGURATION} -o publish/AuthorService --no-build"
                bat "dotnet publish \"${env.BOOK_PROJ}\" -c ${env.CONFIGURATION} -o publish/BookService --no-build"
                bat "dotnet publish \"${env.FRONTEND_PROJ}\" -c ${env.CONFIGURATION} -o publish/FrontEnd --no-build"
            }
        }

        stage('Postman API Tests (via Docker)') {
            steps {
                script {
                    // If your services must be started, uncomment these:
                    // bat "start cmd /c dotnet run --project \"${env.USER_PROJ}\" --urls=https://localhost:7175"
                    // bat "start cmd /c dotnet run --project \"${env.AUTHOR_PROJ}\" --urls=https://localhost:7183"
                    // bat "start cmd /c dotnet run --project \"${env.BOOK_PROJ}\" --urls=https://localhost:7265"
                    // bat "timeout /t 5 /nobreak"

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
