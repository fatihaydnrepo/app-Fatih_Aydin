pipeline {
    agent {
        kubernetes {
            yaml """
apiVersion: v1
kind: Pod
spec:
  containers:
  - name: docker
    image: docker:20.10.24-dind
    securityContext:
      privileged: true
    args:
      - "--insecure-registry=20.71.249.196:8080"
    volumeMounts:
    - mountPath: /var/lib/docker
      name: dockersock
  volumes:
  - name: dockersock
    emptyDir: {}
"""
        }
    }

    environment {
        HARBOR_IP = '20.71.249.196:8080'
        PROJECT_NAME = 'fatihh-app'
        APP_NAME = 'app-v1'
        IMAGE_TAG = "${BUILD_NUMBER}"
    }

    stages {
        stage('Clone Repo') {
            steps {
                git branch: 'main', url: 'https://github.com/fatihaydnrepo/app-Fatih_Aydin.git'
            }
        }
        
        stage('Build and Push Docker Image') {
            steps {
                container('docker') {
                    withCredentials([usernamePassword(credentialsId: 'harbor-credentials', usernameVariable: 'HARBOR_USER', passwordVariable: 'HARBOR_PASS')]) {
                        sh '''
                            echo "$HARBOR_PASS" | docker login -u "$HARBOR_USER" --password-stdin $HARBOR_IP
                            docker build -t $HARBOR_IP/$PROJECT_NAME/$APP_NAME:$IMAGE_TAG .
                            docker push $HARBOR_IP/$PROJECT_NAME/$APP_NAME:$IMAGE_TAG
                            docker tag $HARBOR_IP/$PROJECT_NAME/$APP_NAME:$IMAGE_TAG $HARBOR_IP/$PROJECT_NAME/$APP_NAME:latest
                            docker push $HARBOR_IP/$PROJECT_NAME/$APP_NAME:latest
                        '''
                    }
                }
            }
        }

    }
}