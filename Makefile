.DEFAULT_GOAL := api

include .env

# -----------------------------------------------------------------------------
# Config
# -----------------------------------------------------------------------------

ASPNETCORE_ENVIRONMENT ?= Development
DB_PROVIDER ?= sqlite

MSSQL_HOST ?= localhost
MSSQL_PORT ?= 1433
MSSQL_DB ?= BicycleShop
MSSQL_USER ?= sa
MSSQL_SA_PASSWORD ?= 2Secure@Password2
MSSQL_TRUST_SERVER_CERTIFICATE ?= True
MSSQL_ENCRYPT ?= False

API_PROJECT := WebXeDap.WebAPI
INFRA_PROJECT := WebXeDap.Infrastructure
SEED_PROJECT := WebXeDap.Seeder
STATIC_PROJECT := WebXeDap.StaticWeb

# -----------------------------------------------------------------------------
# Helpers
# -----------------------------------------------------------------------------

guard-%:
	$(if $($*),,$(error Missing required variable: $*))

ifeq ($(DB_PROVIDER),sqlite)
CONNECTION_STRING := Data Source=$(CURDIR)/db.sqlite
else
CONNECTION_STRING := Server=$(MSSQL_HOST),$(MSSQL_PORT); \
	Database=$(MSSQL_DB); \
	User Id=$(MSSQL_USER); \
	Password=$(MSSQL_SA_PASSWORD); \
	TrustServerCertificate=$(MSSQL_TRUST_SERVER_CERTIFICATE); \
	Encrypt=$(MSSQL_ENCRYPT)
endif

API_ENV := \
	ASPNETCORE_ENVIRONMENT=$(ASPNETCORE_ENVIRONMENT) \
	DB_PROVIDER=$(DB_PROVIDER) \
	CONNECTION_STRING="$(CONNECTION_STRING)"


# -----------------------------------------------------------------------------
# Targets
# -----------------------------------------------------------------------------

.PHONY: db
db:
	@podman compose up -d mssql

.PHONY: db-stop
db-stop:
	@podman compose stop mssql

.PHONY: api
api:
	@$(API_ENV) dotnet watch --project $(API_PROJECT)

.PHONY: addmigration
addmigration: guard-name
	@$(API_ENV) dotnet ef migrations add $(name) --project $(INFRA_PROJECT)

.PHONY: migrate
migrate:
	@$(API_ENV) dotnet ef database update --project $(INFRA_PROJECT)

.PHONY: seed
seed:
	@$(API_ENV) dotnet run --project $(SEED_PROJECT)

.PHONY: test
test:
	@$(API_ENV) dotnet test

.PHONY: clean
clean:
	@dotnet clean

.PHONY: static
static:
	@cd $(STATIC_PROJECT) && pnpm dev

.PHONY: fmt
fmt:
	@dotnet csharpier format .
	@cd $(STATIC_PROJECT) && pnpm format