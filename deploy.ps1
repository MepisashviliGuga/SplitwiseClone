$ErrorActionPreference = "Stop"

# ==========================
# Configuration
# ==========================
$acrName = "splitwisecloneacr"
$imageName = "splitwiseclone-api"
$resourceGroup = "splitwise-rg"
$containerApp = "splitwise-api"

$timestamp = Get-Date -Format "yyyyMMdd-HHmm"
$imageTag = "v$timestamp"
$fullImageName = "$acrName.azurecr.io/${imageName}:$imageTag"

Write-Host "Starting deployment..." -ForegroundColor Cyan

# ==========================
# Check Docker
# ==========================
Write-Host "Checking Docker..." -ForegroundColor Cyan
docker info > $null 2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Host "Docker is not running! Please start Docker Desktop." -ForegroundColor Red
    exit 1
}

# ==========================
# Check Azure login
# ==========================
Write-Host "Checking Azure login..." -ForegroundColor Cyan
az account show > $null 2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Host "Not logged in to Azure. Running az login..." -ForegroundColor Yellow
    az login
}

# ==========================
# Build Docker image (no cache)
# ==========================
Write-Host "Building Docker image..." -ForegroundColor Cyan
docker build --no-cache -t $imageName .

# ==========================
# Tag Docker image
# ==========================
Write-Host "Tagging image..." -ForegroundColor Cyan
docker tag $imageName $fullImageName

# ==========================
# Login to ACR
# ==========================
Write-Host "Logging into Azure Container Registry..." -ForegroundColor Cyan
az acr login --name $acrName

# ==========================
# Push image to ACR
# ==========================
Write-Host "Pushing image to registry..." -ForegroundColor Cyan
docker push $fullImageName

# ==========================
# Deploy to Container App
# ==========================
Write-Host "Updating Azure Container App..." -ForegroundColor Cyan
az containerapp update `
    --name $containerApp `
    --resource-group $resourceGroup `
    --image $fullImageName `
    --revision-suffix $imageTag

# ==========================
# Get Application URL
# ==========================
Write-Host "Fetching application URL..." -ForegroundColor Cyan

$apiUrl = az containerapp show `
    --name $containerApp `
    --resource-group $resourceGroup `
    --query properties.configuration.ingress.fqdn `
    --output tsv

Write-Host ""
Write-Host "Deployment successful!" -ForegroundColor Green
Write-Host "Revision: $imageTag"
Write-Host "API URL: https://$apiUrl"