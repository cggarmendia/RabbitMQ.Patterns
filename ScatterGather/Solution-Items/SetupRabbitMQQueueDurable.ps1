param([String]$RabbitDllPath = "not specified")
Set-ExecutionPolicy Unrestricted

Write-Host "Rabbit DLL Path: "
Write-Host $RabbitDllPath -ForegroundColor Green
$absoluteRabbitDllPath = Resolve-Path $RabbitDllPath
Write-Host "Absolute Rabbit Dll Path"
Write-Host $absoluteRabbitDllPath -ForegroundColor Green
[Reflection.Assembly]::LoadFile($absoluteRabbitDllPath)

Write-Host "Setting up RabbitMQ Connection Factory"
$factory = New-Object RabbitMQ.Client.ConnectionFactory
$factory.HostName = "localhost"
$factory.UserName = "develop"
$factory.Password = "develop"

Write-Host "Setting up RabbitMQ Connection"
$connection = $factory.CreateConnection()

Write-Host "Setting up RabbitMQ Model"
$model = $connection.CreateModel()

Write-Host "Creating Exchange"
$exchangeType = [RabbitMQ.Client.ExchangeType]::Topic
$model.ExchangeDeclare("ScatterGather.Sample9.Exchange", $exchangeType, $true, $false, $null)

Write-Host "Creating Server 1 Queue" -ForegroundColor Green
$model.QueueDeclare("ScatterGather.Sample9.Queue1", $true, $false, $false, $null)
$model.QueueBind("ScatterGather.Sample9.Queue1", "ScatterGather.Sample9.Exchange", "1", $null)
$model.QueueBind("ScatterGather.Sample9.Queue1", "ScatterGather.Sample9.Exchange", "4", $null)

Write-Host "Creating Server 2 Queue" -ForegroundColor Green
$model.QueueDeclare("ScatterGather.Sample9.Queue2", $true, $false, $false, $null)
$model.QueueBind("ScatterGather.Sample9.Queue2", "ScatterGather.Sample9.Exchange", "2", $null)
$model.QueueBind("ScatterGather.Sample9.Queue2", "ScatterGather.Sample9.Exchange", "4", $null)
$model.QueueBind("ScatterGather.Sample9.Queue2", "ScatterGather.Sample9.Exchange", "6", $null)

Write-Host "Creating Server 3 Queue" -ForegroundColor Green
$model.QueueDeclare("ScatterGather.Sample9.Queue3", $true, $false, $false, $null)
$model.QueueBind("ScatterGather.Sample9.Queue3", "ScatterGather.Sample9.Exchange", "3", $null)
$model.QueueBind("ScatterGather.Sample9.Queue3", "ScatterGather.Sample9.Exchange", "4", $null)
$model.QueueBind("ScatterGather.Sample9.Queue3", "ScatterGather.Sample9.Exchange", "6", $null)

Write-Host "Setup Complete"