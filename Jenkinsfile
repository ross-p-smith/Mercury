pipeline {
    agent any

    stages {
        stage ("Checkout") {
            steps {
                checkout scm
            }
        }

        stage ("Build")
        {
            steps {
                sh './build/scripts/dockerbuild.sh'
            }
        }

        stage("Cloud Builds") {
            steps {
                parallel(
                    azure: {
                        build job: 'precheckin_ci_build_azure', propagate: true, wait: true, parameters: [[$class: 'StringParameterValue', name: 'target_branch', value: "${target_branch}"]]
                    },
                    gcp: {
                        build job: 'precheckin_ci_build_gcp', propagate: true, wait: true, parameters: [[$class: 'StringParameterValue', name: 'target_branch', value: "${target_branch}"]]
                    },
                )
            }
        }

        stage("Docker Cleanup") {
            steps {
                sh './build/scripts/dockercleanup.sh'
            }
        }
    }

    post {
        always {
            sh 'docker system prune -a -f'
            cleanWs()
        }

    }
}
