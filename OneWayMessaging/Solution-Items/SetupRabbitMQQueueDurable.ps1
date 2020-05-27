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

Write-Host "Creating Queue"
$model.QueueDeclare("OneWayMessaging.Sample3", $true, $false, $false, $null)

Write-Host "Setup Complete"