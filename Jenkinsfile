// Jenkinsfile — gregCore CI pilot (Multibranch Pipeline).
//
// What it does:
//   1. Verifies game reference assemblies resolve (fail fast with a clear message)
//   2. Builds gregCore.csproj (Release, no shared compilation — same flags as local)
//   3. Runs the test suite (TRX report + OpenCover coverage via coverlet)
//   4. Runs SonarQube analysis (.NET scanner) + enforces the Quality Gate
//   5. Validates the wiki import (docs check, best-effort if node is present)
//   6. Publishes GitHub checks named exactly like the branch-protection
//      contexts: `tests`, `build-linux`, `docs`
//   7. Archives build artifacts (DLLs, TRX, coverage)
//   8. Files a Mantis issue when the build fails (needs MANTIS_* credentials)
//
// Required Jenkins plugins: workflow-aggregator (Pipeline), git,
// github-branch-source, sonar (SonarQube Scanner), mstest (TRX reports),
// github-checks (publishChecks step).
//
// Required Jenkins configuration (see docs/ci-jenkins-sonarqube-mantis.md):
//   - Agent label `greg-dotnet` (.NET SDK 8, JDK 17, git; node 22 optional)
//   - Game assemblies available so that references/*.dll resolve
//     (Steam install on the agent + tools/sync-melon-assemblies.sh equivalent,
//     or a private copy of the reference DLLs)
//   - SonarQube server named `sonarqube` + project `gregCore`
//   - Credentials: `sonar-token` (Secret text), `mantis-api-token` (Secret text)
//   - GitHub webhook http://<jenkins>/github-webhook/ (or GitHub App)

// Helper: publish the three branch-protection checks. Never fails the build
// (missing GitHub Checks plugin degrades to a log line, not a red build).
def notifyChecks(conclusion, summary) {
    def titles = ['tests': 'Tests', 'build-linux': 'Build (Linux)', 'docs': 'Docs (wiki import)']
    for (c in ['tests', 'build-linux', 'docs']) {
        try {
            publishChecks name: c, title: titles[c], summary: summary, conclusion: conclusion
        } catch (err) {
            echo "publishChecks unavailable for ${c}: ${err.getMessage()}"
        }
    }
}

// ── Site config: adjust these to your instance (defaults = Marvin's LAN) ──
// (Kept in `environment` so every stage/step can use them as $VARS.)
pipeline {
    agent { label 'greg-dotnet' }

    options {
        timeout(time: 30, unit: 'MINUTES')
        disableConcurrentBuilds()
    }

    environment {
        DOTNET_NOLOGO = '1'
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_ROLL_FORWARD = 'LatestMajor'
        SONAR_PROJECT_KEY = 'gregCore'
        MANTIS_URL = 'http://192.168.178.127'
        MANTIS_PROJECT_ID = '1'      // Mantis project id for CI failure tickets
        MANTIS_CATEGORY_ID = '1'     // Mantis category id for CI failure tickets
    }

    stages {
        stage('Verify game references') {
            steps {
                sh '''
                    missing=0
                    for f in references/*.dll; do
                        if [ ! -s "$f" ]; then echo "MISSING/EMPTY: $f"; missing=1; fi
                    done
                    if [ "$missing" -ne 0 ]; then
                        echo "Game reference assemblies do not resolve."
                        echo "See docs/ci-jenkins-sonarqube-mantis.md section 'Agent game references'."
                        exit 1
                    fi
                    echo "All reference assemblies resolve."
                '''
            }
        }

        stage('Restore') {
            steps {
                sh 'dotnet restore gregCore.csproj'
            }
        }

        stage('SonarQube begin') {
            steps {
                withSonarQubeEnv('sonarqube') {
                    withCredentials([string(credentialsId: 'sonar-token', variable: 'SONAR_TOKEN')]) {
                        sh '''
                            dotnet tool install --global dotnet-sonarscanner || true
                            export PATH="$PATH:$HOME/.dotnet/tools"
                            dotnet sonarscanner begin \
                                /k:"$SONAR_PROJECT_KEY" \
                                /v:"$BUILD_NUMBER" \
                                /d:sonar.host.url="$SONAR_HOST_URL" \
                                /d:sonar.token="$SONAR_TOKEN" \
                                /d:sonar.cs.opencover.reportsPaths="tests/**/coverage.opencover.xml"
                        '''
                    }
                }
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build gregCore.csproj -c Release --no-restore --nologo -v q /p:UseSharedCompilation=false'
            }
        }

        stage('Test') {
            steps {
                sh '''
                    DOTNET_ROLL_FORWARD=LatestMajor dotnet test tests/ --no-build -c Release --nologo -v q \
                        --logger "trx;LogFileName=results.trx" \
                        /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
                '''
            }
        }

        stage('SonarQube end') {
            steps {
                withSonarQubeEnv('sonarqube') {
                    withCredentials([string(credentialsId: 'sonar-token', variable: 'SONAR_TOKEN')]) {
                        sh '''
                            export PATH="$PATH:$HOME/.dotnet/tools"
                            dotnet sonarscanner end /d:sonar.token="$SONAR_TOKEN"
                        '''
                    }
                }
            }
        }

        stage('Quality Gate') {
            steps {
                waitForQualityGate abortPipeline: true
            }
        }

        stage('Docs (wiki import)') {
            steps {
                sh '''
                    if command -v node >/dev/null 2>&1; then
                        node wiki-site/scripts/import-wiki.mjs
                    else
                        echo "node not on agent - docs check skipped (neutral)."
                    fi
                '''
            }
        }
    }

    post {
        always {
            // TRX report (needs the MSTest plugin) + artifacts, whatever happened.
            script {
                try {
                    mstest testResultsFile: '**/results.trx', keepLongStdio: true
                } catch (err) {
                    echo "mstest publisher unavailable: ${err.getMessage()}"
                }
            }
            archiveArtifacts artifacts: '**/results.trx, **/coverage.opencover.xml, src/**/bin/Release/**/gregCore.dll', allowEmptyArchive: true
        }
        success {
            notifyChecks('SUCCESS', 'green')
        }
        failure {
            notifyChecks('FAILURE', "See ${env.BUILD_URL}")
            // File a Mantis ticket for the red build (never fails the build itself).
            script {
                try {
                    withCredentials([string(credentialsId: 'mantis-api-token', variable: 'MANTIS_TOKEN')]) {
                        sh '''
                            curl -s -o /dev/null -w "%{http_code}\\n" -X POST "$MANTIS_URL/api/rest/issues/" \
                                -H "Authorization: $MANTIS_TOKEN" \
                                -H "Content-Type: application/json" \
                                -d "{\\"summary\\": \\"[CI] $JOB_NAME #$BUILD_NUMBER failed ($BRANCH_NAME)\\", \\"description\\": \\"Branch: $BRANCH_NAME\\\\nBuild: $BUILD_URL\\\\nCommit: $GIT_COMMIT\\", \\"project\\": {\\"id\\": $MANTIS_PROJECT_ID}, \\"category\\": {\\"id\\": $MANTIS_CATEGORY_ID}}" || true
                        '''
                    }
                } catch (err) {
                    echo "Mantis reporting skipped: ${err.getMessage()}"
                }
            }
        }
    }
}
