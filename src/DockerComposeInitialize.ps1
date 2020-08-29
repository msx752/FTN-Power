class DkrConfig{
        [string] $ActionType
        [AllowNull()][string] $ContainerTag 
        [string] $DockerComposePath
        [string] $DockerComposeFile
        [string] $ErrorActionPreference
}
function ExecDockerCompose([string]$acType, [AllowNull()][string]$conTag){
    [DkrConfig]$DockerConf =  [DkrConfig]::new();
    $DockerConf.ErrorActionPreference = 'Stop';
    $DockerConf.ActionType = $acType;
    $DockerConf.ContainerTag = $conTag;
    $DockerConf.DockerComposePath = "";
    $DockerConf.DockerComposeFile = [IO.Path]::Combine($DockerConf.DockerComposePath, 'docker-compose.yml');
    $ErrorActionPreference = $DockerConf.ErrorActionPreference;
    
    Write-Host ("EXEC>> path:'{0}' action:'{1}' tag:'{2}'" -f $DockerConf.DockerComposeFile, $DockerConf.ActionType, $DockerConf.ContainerTag) -ForegroundColor Green;

    if([string]::IsNullOrEmpty($DockerConf.ContainerTag)){
        $DockerConf.ContainerTag = '';
    }
    if($DockerConf.ActionType -eq "build-push"){
         docker-compose --log-level 'WARNING' -f $DockerConf.DockerComposeFile build $DockerConf.ContainerTag;
         docker-compose --log-level 'WARNING' -f $DockerConf.DockerComposeFile push $DockerConf.ContainerTag;
    }else{
         docker-compose --log-level 'WARNING' -f $DockerConf.DockerComposeFile $DockerConf.ActionType $DockerConf.ContainerTag;
    }
}
Write-Host "Docker-Compose Module LOADED" -ForegroundColor Green;