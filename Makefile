.DEFAULT_GOAL := dev

include .env

# Fallbacks when variables are omitted from .env or shell environment.
MSSQL_HOST ?= localhost
MSSQL_PORT ?= 1433
MSSQL_DB ?= BicycleShop
MSSQL_USER ?= sa

ifndef MSSQL_SA_PASSWORD
$(error MSSQL_SA_PASSWORD is not set. Please set it in the .env file)
endif

CONNECTION_STRING = Server=$(MSSQL_HOST),$(MSSQL_PORT);Database=$(MSSQL_DB);User Id=$(MSSQL_USER);Password=$(MSSQL_SA_PASSWORD);TrustServerCertificate=True;Encrypt=False


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
	ConnectionStrings__DefaultConnection="$(CONNECTION_STRING)" \
	dotnet watch --project WebXeDap run

.PHONY=migrate
migrate:
	echo "Applying database migrations..."
	@ASPNETCORE_ENVIRONMENT=Development \
	ConnectionStrings__DefaultConnection="$(CONNECTION_STRING)" \
	dotnet ef database update --project WebXeDap

.PHONY=seed
seed:
	@echo "Seeding admin user..."
	@CONNECTION_STRING="$(CONNECTION_STRING)" \
	dotnet run --project WebXeDap.Seeder