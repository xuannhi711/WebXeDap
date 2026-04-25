include .env


.PHONY=db
db:
	@podman compose up -d mssql

.PHONY=stop-db
stop-db:
	@podman compose stop mssql

.PHONY=dev
dev: db
	echo "Starting development server..."
	@ASPNETCORE_ENVIRONMENT=Development \
	ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=BicycleShop;User Id=sa;Password=$(MSSQL_SA_PASSWORD);TrustServerCertificate=True;Encrypt=False" \
	dotnet watch --project WebXeDap run

.PHONY=migrate
migrate:
	echo "Applying database migrations..."
	@ASPNETCORE_ENVIRONMENT=Development \
	ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=BicycleShop;User Id=sa;Password=$(MSSQL_SA_PASSWORD);TrustServerCertificate=True;Encrypt=False" \
	dotnet ef database update --project WebXeDap