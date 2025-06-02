// File: G:\LibraryManagement\Jenkinsfile

pipeline {
    agent any

    environment {
        // Paths to each service’s solution or project
        USER_SOL    = 'Services/UserServices/UserServices.sln'
        AUTHOR_PROJ = 'Services/AuthorService/AuthorService.csproj'
        BOOK_PROJ   = 'Services/BookService/BookService.csproj'
        // FrontEnd is located under FrontEnd/FrontEnd, so we point there:
        FRONTEND_PROJ = 'FrontEnd/FrontEnd/FrontEnd.csproj'
        CONFIGURATION = 'Release'
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

        stage('Test UserService') {
            steps {
                bat "dotnet test \"${env.USER_SOL}\" -c ${env.CONFIGURATION} --no-build --logger trx"
            }
            post {
                always {
                    junit 'Services/UserServices/TestResults/*.trx'
                }
            }
        }

        stage('Publish Services & FrontEnd') {
            steps {
                bat 'if not exist publish mkdir publish'

                // Publish UserService
                bat "dotnet publish \"${env.USER_SOL}\" -c ${env.CONFIGURATION} -o publish/UserService --no-build"

                // Publish AuthorService
                bat "dotnet publish \"${env.AUTHOR_PROJ}\" -c ${env.CONFIGURATION} -o publish/AuthorService --no-build"

                // Publish BookService
                bat "dotnet publish \"${env.BOOK_PROJ}\" -c ${env.CONFIGURATION} -o publish/BookService --no-build"

                // Publish FrontEnd (note the extra /FrontEnd/ in the path)
                bat "dotnet publish \"${env.FRONTEND_PROJ}\" -c ${env.CONFIGURATION} -o publish/FrontEnd --no-build"
            }
        }
    }

    post {
        always {
            archiveArtifacts artifacts: 'publish/**/*', fingerprint: true
        }
        success {
            echo 'Build + Publish succeeded 🎉'
        }
        unstable {
            echo 'Build succeeded with test failures (unstable) ⚠️'
        }
        failure {
            echo 'Build failed 💥'
        }
    }
}
