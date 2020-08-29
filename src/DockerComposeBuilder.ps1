Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser -Force
cd "C:\Users\msali\Documents\gitRepos\FTN-Power\src"
Import-Module ".\DockerComposeInitialize.ps1";
#ExecDockerCompose 'build-push';

ExecDockerCompose 'build-push' 'fortnitepowerqueue';

ExecDockerCompose 'build-push' 'fortnitepowerbot';

ExecDockerCompose 'build-push' 'website';

ExecDockerCompose 'build-push' 'imageservice';

ExecDockerCompose 'build-push' 'redis';

ExecDockerCompose 'build-push' 'seq';

ExecDockerCompose 'build-push' 'ignite';

#ExecDockerCompose 'build-push' 'nginx';
