// File: G:\LibraryManagement\Jenkinsfile

pipeline {
    agent any

    // Pin to your solution name & Release configuration
    environment {
        SOLUTION = 'LibraryManagement.sln'
        CONFIGURATION = 'Release'
    }

    stages {
        stage('Checkout') {
            steps {
                // Checkout from GitHub repository (main branch)
                checkout([$class: 'GitSCM',
                  branches: [[name: '*/main']],
                  userRemoteConfigs: [[
                    url: 'https://github.com/alivmran/LibraryManagement.git'
                  ]]
                ])
            }
        }

        stage('Restore') {
            steps {
                script {
                    // Ensure we are in the workspace root (where global.json lives)
                    bat "dotnet restore \"${env.SOLUTION}\""
                }
            }
        }

        stage('Build') {
            steps {
                script {
                    // Build solution in Release, no restore (already done)
                    bat "dotnet build \"${env.SOLUTION}\" -c ${env.CONFIGURATION} --no-restore"
                }
            }
        }

        stage('Test') {
            steps {
                script {
                    // If you have test projects, run them. Otherwise comment this out.
                    bat "dotnet test \"${env.SOLUTION}\" -c ${env.CONFIGURATION} --no-build --logger trx"
                }
            }
            post {
                always {
                    // Archive test results (if any .trx or XML files appear under TestResults)
                    junit '**/TestResults/*.trx'
                }
            }
        }

        stage('Publish Services & Frontend') {
            steps {
                script {
                    // Create a 'publish' directory
                    bat 'if not exist publish mkdir publish'

                    // Publish UserService
                    bat "dotnet publish \"Services\\UserServices\\UserServices.csproj\" -c ${env.CONFIGURATION} -o publish/UserService --no-build"

                    // Publish AuthorService
                    bat "dotnet publish \"Services\\AuthorService\\AuthorService.csproj\" -c ${env.CONFIGURATION} -o publish/AuthorService --no-build"

                    // Publish BookService
                    bat "dotnet publish \"Services\\BookService\\BookService.csproj\" -c ${env.CONFIGURATION} -o publish/BookService --no-build"

                    // Publish FrontEnd
                    bat "dotnet publish \"FrontEnd\\FrontEnd.csproj\" -c ${env.CONFIGURATION} -o publish/FrontEnd --no-build"
                }
            }
        }
    }

    post {
        always {
            // Archive everything under publish/** so it’s available as a build artifact
            archiveArtifacts artifacts: 'publish/**/*', fingerprint: true
        }
        success {
            echo 'Build succeeded 🎉'
        }
        unstable {
            echo 'Build succeeded but test failures (unstable) ⚠️'
        }
        failure {
            echo 'Build failed 💥'
        }
    }
}
