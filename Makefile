.DEFAULT_GOAL := api

-include .env

# -----------------------------------------------------------------------------
# Config
# -----------------------------------------------------------------------------

ASPNETCORE_ENVIRONMENT ?= Development
DB_PROVIDER ?= sqlite
BUILD_CONFIG ?= Debug

MSSQL_HOST ?= localhost
MSSQL_PORT ?= 1433
MSSQL_DB ?= BicycleShop
MSSQL_USER ?= sa
MSSQL_SA_PASSWORD ?= 2Secure@Password2
MSSQL_TRUST_SERVER_CERTIFICATE ?= True
MSSQL_ENCRYPT ?= False

SMTP_HOST ?= localhost
SMTP_PORT ?= 1025
SMTP_USER ?= "username"
SMTP_PASS ?= "password"
SMTP_FROM_EMAIL ?= "admin@localhost.com"
SMTP_FROM_NAME ?= "dfdd"

API_PROJECT := WebXeDap.WebAPI
INFRA_PROJECT := WebXeDap.Infrastructure
SEED_PROJECT := WebXeDap.Seeder
STATIC_PROJECT := WebXeDap.StaticWeb
ADMIN_PROJECT := WebXeDap.AdminPanel

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
	CONNECTION_STRING="$(CONNECTION_STRING)" \
	SMTP_HOST=$(SMTP_HOST) \
	SMTP_PORT=$(SMTP_PORT) \
	SMTP_USER=$(SMTP_USER) \
	SMTP_PASS=$(SMTP_PASS) \
	SMTP_FROM_EMAIL=$(SMTP_FROM_EMAIL) \
	SMTP_FROM_NAME=$(SMTP_FROM_NAME) \
	GOOGLE_CLIENT_ID=$(GOOGLE_CLIENT_ID) \
	GOOGLE_CLIENT_SECRET=$(GOOGLE_CLIENT_SECRET)
# -----------------------------------------------------------------------------
# SERVICES
# -----------------------------------------------------------------------------

.PHONY: api
api:
	@$(API_ENV) dotnet watch --project $(API_PROJECT)

.PHONY: static
static:
	@cd $(STATIC_PROJECT) && pnpm dev

.PHONY: admin
admin:
	@$(API_ENV) dotnet watch --project $(ADMIN_PROJECT)

# -----------------------------------------------------------------------------
# EXTERNAL SERVICES
# -----------------------------------------------------------------------------

.PHONY: db
db:
	@podman compose up -d mssql

.PHONY: db-stop
db-stop:
	@podman compose stop mssql

.PHONY: mail
mail:
	@podman compose up -d mailpit

.PHONY: mail-stop
mail-stop:
	@podman compose stop mailpit

# -----------------------------------------------------------------------------
# TESTING
# -----------------------------------------------------------------------------

.PHONY: test
test: test-unit test-integration

.PHONY: test-unit
test-unit:
	@dotnet test \
		--configuration $(BUILD_CONFIG) \
		--no-build \
		--filter "Category=Unit"

.PHONY: test-integration
test-integration:
	@$(API_ENV) dotnet test \
		--configuration $(BUILD_CONFIG) \
		--no-build \
		--filter "Category=Integration"


# -----------------------------------------------------------------------------
# MIGRATING
# -----------------------------------------------------------------------------

.PHONY: addmigration
addmigration: guard-name
	@$(API_ENV) dotnet ef migrations add $(name) --project $(INFRA_PROJECT)

.PHONY: migrate
migrate:
	@$(API_ENV) dotnet ef database update --project $(INFRA_PROJECT)

# -----------------------------------------------------------------------------
# MISC
# -----------------------------------------------------------------------------

.PHONY: createadmin
createadmin:
	@$(API_ENV) dotnet run --project $(SEED_PROJECT) -- createadmin

.PHONY: seeddb
seeddb:
	@$(API_ENV) dotnet run --project $(SEED_PROJECT) -- db

.PHONY: build
build:
	@dotnet build --configuration $(BUILD_CONFIG)

.PHONY: clean
clean:
	@dotnet clean

.PHONY: fmt
fmt:
	@dotnet csharpier format .
	@cd $(STATIC_PROJECT) && pnpm format

.PHONY: tool-restore
tool-restore:
	@dotnet tool restore
