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
$exchangeType = [RabbitMQ.Client.ExchangeType]::Fanout
$model.ExchangeDeclare("PublishSubscribe.Sample5.Exchange", $exchangeType, $true, $false, $null)

Write-Host "Creating Server 1 Queue"
$model.QueueDeclare("PublishSubscribe.Sample5.Queue1", $true, $false, $false, $null)
$model.QueueBind("PublishSubscribe.Sample5.Queue1", "PublishSubscribe.Sample5.Exchange", "", $null)

Write-Host "Creating Server 2 Queue"
$model.QueueDeclare("PublishSubscribe.Sample5.Queue2", $true, $false, $false, $null)
$model.QueueBind("PublishSubscribe.Sample5.Queue2", "PublishSubscribe.Sample5.Exchange", "", $null)

Write-Host "Setup Complete"