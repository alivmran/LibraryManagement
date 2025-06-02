// File: G:\LibraryManagement\Jenkinsfile

pipeline {
    agent any

    environment {
        // We no longer have a single top‐level solution.
        // Instead, we will restore/build each service individually:
        USER_SOL = 'Services/UserServices/UserServices.sln'      // Adjust if your file is named differently
        AUTHOR_PROJ = 'Services/AuthorService/AuthorService.csproj'
        BOOK_PROJ = 'Services/BookService/BookService.csproj'
        FRONTEND_PROJ = 'FrontEnd/FrontEnd.csproj'
        CONFIGURATION = 'Release'
    }

    stages {
        stage('Checkout') {
            steps {
                // Pull from Git (main branch)
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
                script {
                    bat "dotnet restore \"${env.USER_SOL}\""
                }
            }
        }

        stage('Restore AuthorService') {
            steps {
                script {
                    // If AuthorService has a .sln, replace AUTHOR_PROJ with that .sln path:
                    // bat "dotnet restore \"Services/AuthorService/AuthorService.sln\"

                    // Otherwise, restore the .csproj directly:
                    bat "dotnet restore \"${env.AUTHOR_PROJ}\""
                }
            }
        }

        stage('Restore BookService') {
            steps {
                script {
                    // If BookService has a .sln, change BOOK_PROJ accordingly:
                    // bat "dotnet restore \"Services/BookService/BookService.sln\"

                    // Otherwise, restore its .csproj:
                    bat "dotnet restore \"${env.BOOK_PROJ}\""
                }
            }
        }

        stage('Restore FrontEnd') {
            steps {
                script {
                    bat "dotnet restore \"${env.FRONTEND_PROJ}\""
                }
            }
        }

        stage('Build UserService') {
            steps {
                script {
                    bat "dotnet build \"${env.USER_SOL}\" -c ${env.CONFIGURATION} --no-restore"
                }
            }
        }

        stage('Build AuthorService') {
            steps {
                script {
                    // If there’s an AuthorService.sln, use it. Otherwise:
                    bat "dotnet build \"${env.AUTHOR_PROJ}\" -c ${env.CONFIGURATION} --no-restore"
                }
            }
        }

        stage('Build BookService') {
            steps {
                script {
                    // If there’s a BookService.sln, use it. Otherwise:
                    bat "dotnet build \"${env.BOOK_PROJ}\" -c ${env.CONFIGURATION} --no-restore"
                }
            }
        }

        stage('Build FrontEnd') {
            steps {
                script {
                    bat "dotnet build \"${env.FRONTEND_PROJ}\" -c ${env.CONFIGATION} --no-restore"
                }
            }
        }

        stage('Test UserService') {
            steps {
                script {
                    // If your UserService solution contains tests, adjust accordingly.
                    // If not, skip this stage or mark it “unstable.”
                    bat "dotnet test \"${env.USER_SOL}\" -c ${env.CONFIGURATION} --no-build --logger trx"
                }
            }
            post {
                always {
                    junit 'Services/UserServices/TestResults/*.trx'
                }
            }
        }

        // You can add similar Test stages for AuthorService or BookService if they have tests.

        stage('Publish Services & FrontEnd') {
            steps {
                script {
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
            echo 'Build succeeded but some tests may have failed (unstable) ⚠️'
        }
        failure {
            echo 'Build failed 💥'
        }
    }
}
