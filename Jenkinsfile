pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                // Pull from your GitHub repo
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
                // Create a folder named "publish" under the workspace (if it doesn't exist)
                bat 'if not exist publish mkdir publish'

                // Publish each .NET project into a subfolder of "publish"
                bat 'dotnet publish Services/UserServices/UserServices/UserServices.csproj -c Release -o publish/UserService --no-build'
                bat 'dotnet publish Services/AuthorService/AuthorService.csproj -c Release -o publish/AuthorService --no-build'
                bat 'dotnet publish Services/BookService/BookService.csproj -c Release -o publish/BookService --no-build'
                bat 'dotnet publish FrontEnd/FrontEnd/FrontEnd.csproj -c Release -o publish/FrontEnd --no-build'
            }
        }
    }

    post {
        always {
            // Archive everything under publish/ so you can download the compiled outputs later
            archiveArtifacts artifacts: 'publish/**', fingerprint: true
            echo '✅ Build and publish completed; no API tests were run.'
        }
    }
}
